using System.Security.Claims;
using JanShikayat.Api.Data;
using JanShikayat.Api.DTOs.Complaints;
using JanShikayat.Api.Models;
using JanShikayat.Api.Models.Enums;
using JanShikayat.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JanShikayat.Api.Controllers
{
    [ApiController]
    [Route("api/complaints")]
    [Authorize]
    public class ComplaintsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IComplaintNumberGenerator _numberGenerator;
        private readonly IFileStorageService _fileStorage;

        public ComplaintsController(
            AppDbContext db,
            UserManager<ApplicationUser> userManager,
            IComplaintNumberGenerator numberGenerator,
            IFileStorageService fileStorage)
        {
            _db = db;
            _userManager = userManager;
            _numberGenerator = numberGenerator;
            _fileStorage = fileStorage;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        private bool IsInRole(string role) => User.IsInRole(role);

        /// <summary>
        /// Scopes the complaint list to what the caller's role is allowed to see:
        /// branch officers see only their branch's complaints, department heads
        /// see complaints forwarded to their department, competent authorities
        /// see complaints forwarded to them for field enquiry, and SuperAdmin sees all.
        /// </summary>
        private async Task<IQueryable<Complaint>> ScopedQueryAsync()
        {
            var query = _db.Complaints
                .Include(c => c.Branch)
                .Include(c => c.Department)
                .AsQueryable();

            if (IsInRole(AppRoles.SuperAdmin))
                return query;

            var user = await _userManager.FindByIdAsync(CurrentUserId);
            if (user == null) return query.Where(c => false);

            if (IsInRole(AppRoles.BranchOfficer) && user.BranchId.HasValue)
                return query.Where(c => c.BranchId == user.BranchId.Value);

            if (IsInRole(AppRoles.DepartmentHead) && user.DepartmentId.HasValue)
                return query.Where(c => c.DepartmentId == user.DepartmentId.Value);

            if (IsInRole(AppRoles.CompetentAuthority))
                return query.Where(c => c.Status == ComplaintStatus.ForwardedForFieldEnquiry
                                         || c.Status == ComplaintStatus.ActionTaken
                                         || c.Status == ComplaintStatus.Disposed);

            return query.Where(c => false);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComplaintListItemDto>>> GetAll(
            [FromQuery] string? status, [FromQuery] string? search)
        {
            var query = await ScopedQueryAsync();

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ComplaintStatus>(status, true, out var parsed))
                query = query.Where(c => c.Status == parsed);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c =>
                    c.ComplaintNumber.Contains(search) ||
                    c.ApplicantName.Contains(search) ||
                    c.Subject.Contains(search));

            var results = await query
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ComplaintListItemDto
                {
                    Id = c.Id,
                    ComplaintNumber = c.ComplaintNumber,
                    ApplicantName = c.ApplicantName,
                    Subject = c.Subject,
                    ComplaintDate = c.ComplaintDate,
                    Status = c.Status.ToString(),
                    BranchName = c.Branch != null ? c.Branch.Name : string.Empty,
                    DepartmentName = c.Department != null ? c.Department.Name : null
                })
                .ToListAsync();

            return Ok(results);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ComplaintDetailDto>> GetById(int id)
        {
            var scoped = await ScopedQueryAsync();
            var complaint = await scoped
                .Include(c => c.Documents)
                .Include(c => c.Remarks).ThenInclude(r => r.CreatedByUser)
                .Include(c => c.HistoryEntries).ThenInclude(h => h.ActionByUser)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null) return NotFound();

            return Ok(MapToDetailDto(complaint));
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.BranchOfficer + "," + AppRoles.SuperAdmin)]
        public async Task<ActionResult<ComplaintDetailDto>> Create(CreateComplaintDto dto)
        {
            var branch = await _db.Branches.FindAsync(dto.BranchId);
            if (branch == null) return BadRequest(new { message = "Invalid branch." });

            var complaint = new Complaint
            {
                ComplaintNumber = await _numberGenerator.GenerateAsync(),
                ApplicantName = dto.ApplicantName,
                GuardianName = dto.GuardianName,
                MobileNumber = dto.MobileNumber,
                Address = dto.Address,
                District = dto.District,
                Block = dto.Block,
                PoliceStation = dto.PoliceStation,
                Area = dto.Area,
                Email = dto.Email,
                OppositeParty = dto.OppositeParty,
                Subject = dto.Subject,
                Category = dto.Category,
                SubCategory = dto.SubCategory,
                Description = dto.Description,
                ComplaintDate = dto.ComplaintDate ?? DateTime.UtcNow,
                Source = dto.Source,
                BranchId = dto.BranchId,
                DepartmentId = dto.DepartmentId,
                Status = ComplaintStatus.Pending,
                CreatedByUserId = CurrentUserId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Complaints.Add(complaint);
            await _db.SaveChangesAsync();

            _db.ComplaintHistories.Add(new ComplaintHistory
            {
                ComplaintId = complaint.Id,
                FromStatus = ComplaintStatus.Pending,
                ToStatus = ComplaintStatus.Pending,
                Reason = "Complaint registered.",
                ActionByUserId = CurrentUserId
            });
            await _db.SaveChangesAsync();

            var created = await _db.Complaints
                .Include(c => c.Branch).Include(c => c.Department)
                .Include(c => c.Documents).Include(c => c.Remarks)
                .Include(c => c.HistoryEntries).ThenInclude(h => h.ActionByUser)
                .FirstAsync(c => c.Id == complaint.Id);

            return CreatedAtAction(nameof(GetById), new { id = complaint.Id }, MapToDetailDto(created));
        }

        [HttpPost("{id:int}/documents")]
        [RequestSizeLimit(20_000_000)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDocument(int id, [FromForm] UploadDocumentRequest request)
        {
            var complaint = await _db.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            if (request.File == null || request.File.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            try
            {
                var (storagePath, size) = await _fileStorage.SaveAsync(request.File, id);
                var doc = new ComplaintDocument
                {
                    ComplaintId = id,
                    FileName = request.File.FileName,
                    StoragePath = storagePath,
                    DocumentType = request.DocumentType ?? "Complaint",
                    FileSizeBytes = size,
                    UploadedByUserId = CurrentUserId
                };
                _db.ComplaintDocuments.Add(doc);
                await _db.SaveChangesAsync();

                return Ok(new ComplaintDocumentDto
                {
                    Id = doc.Id,
                    FileName = doc.FileName,
                    DocumentType = doc.DocumentType,
                    UploadedAt = doc.UploadedAt,
                    DownloadUrl = $"/api/complaints/{id}/documents/{doc.Id}/download"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}/documents/{documentId:int}/download")]
        public async Task<IActionResult> DownloadDocument(int id, int documentId)
        {
            var doc = await _db.ComplaintDocuments.FirstOrDefaultAsync(d => d.Id == documentId && d.ComplaintId == id);
            if (doc == null) return NotFound();

            var physicalPath = _fileStorage.GetPhysicalPath(doc.StoragePath);
            if (!System.IO.File.Exists(physicalPath)) return NotFound();

            var stream = System.IO.File.OpenRead(physicalPath);
            return File(stream, "application/pdf", doc.FileName);
        }

        [HttpPost("{id:int}/remarks")]
        [Authorize(Roles = AppRoles.DepartmentHead + "," + AppRoles.CompetentAuthority + "," + AppRoles.SuperAdmin)]
        public async Task<ActionResult<ComplaintRemarkDto>> AddRemark(int id, AddRemarkDto dto)
        {
            var complaint = await _db.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            var remark = new ComplaintRemark
            {
                ComplaintId = id,
                RemarkType = dto.RemarkType,
                Text = dto.Text,
                CreatedByUserId = CurrentUserId
            };
            _db.ComplaintRemarks.Add(remark);

            if (complaint.Status == ComplaintStatus.Pending)
                complaint.Status = ComplaintStatus.UnderReview;

            await _db.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(CurrentUserId);
            return Ok(new ComplaintRemarkDto
            {
                Id = remark.Id,
                RemarkType = remark.RemarkType,
                Text = remark.Text,
                CreatedByName = user?.FullName ?? string.Empty,
                CreatedAt = remark.CreatedAt
            });
        }

        [HttpPost("{id:int}/forward")]
        [Authorize(Roles = AppRoles.DepartmentHead + "," + AppRoles.SuperAdmin)]
        public async Task<IActionResult> Forward(int id, ForwardComplaintDto dto)
        {
            var complaint = await _db.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            var fromStatus = complaint.Status;

            if (dto.DepartmentId.HasValue)
            {
                complaint.DepartmentId = dto.DepartmentId;
                complaint.Status = ComplaintStatus.ForwardedToDepartment;
            }
            else if (dto.CompetentAuthorityDesignationId.HasValue)
            {
                complaint.CompetentAuthorityDesignationId = dto.CompetentAuthorityDesignationId;
                complaint.Status = ComplaintStatus.ForwardedForFieldEnquiry;
            }
            else
            {
                return BadRequest(new { message = "Specify either a department or a competent authority to forward to." });
            }

            _db.ComplaintHistories.Add(new ComplaintHistory
            {
                ComplaintId = id,
                FromStatus = fromStatus,
                ToStatus = complaint.Status,
                Reason = dto.Reason,
                ActionByUserId = CurrentUserId
            });

            await _db.SaveChangesAsync();
            return Ok(new { message = "Complaint forwarded.", status = complaint.Status.ToString() });
        }

        [HttpPost("{id:int}/action-taken")]
        [Authorize(Roles = AppRoles.CompetentAuthority + "," + AppRoles.DepartmentHead + "," + AppRoles.SuperAdmin)]
        public async Task<IActionResult> MarkActionTaken(int id, [FromBody] AddRemarkDto dto)
        {
            var complaint = await _db.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            var fromStatus = complaint.Status;
            complaint.Status = ComplaintStatus.ActionTaken;

            _db.ComplaintRemarks.Add(new ComplaintRemark
            {
                ComplaintId = id,
                RemarkType = "Official",
                Text = dto.Text,
                CreatedByUserId = CurrentUserId
            });

            _db.ComplaintHistories.Add(new ComplaintHistory
            {
                ComplaintId = id,
                FromStatus = fromStatus,
                ToStatus = complaint.Status,
                Reason = "Action taken; report submitted.",
                ActionByUserId = CurrentUserId
            });

            await _db.SaveChangesAsync();
            return Ok(new { message = "Action recorded.", status = complaint.Status.ToString() });
        }

        [HttpPost("{id:int}/close")]
        [Authorize(Roles = AppRoles.DepartmentHead + "," + AppRoles.CompetentAuthority + "," + AppRoles.SuperAdmin)]
        public async Task<IActionResult> Close(int id, CloseComplaintDto dto)
        {
            var complaint = await _db.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            var fromStatus = complaint.Status;
            complaint.Status = ComplaintStatus.Disposed;
            complaint.ClosureRemarks = dto.ClosureRemarks;
            complaint.ClosedAt = DateTime.UtcNow;

            _db.ComplaintHistories.Add(new ComplaintHistory
            {
                ComplaintId = id,
                FromStatus = fromStatus,
                ToStatus = complaint.Status,
                Reason = dto.ClosureRemarks,
                ActionByUserId = CurrentUserId
            });

            await _db.SaveChangesAsync();
            return Ok(new { message = "Case closed.", status = complaint.Status.ToString() });
        }

        private static ComplaintDetailDto MapToDetailDto(Complaint c) => new()
        {
            Id = c.Id,
            ComplaintNumber = c.ComplaintNumber,
            ApplicantName = c.ApplicantName,
            Subject = c.Subject,
            ComplaintDate = c.ComplaintDate,
            Status = c.Status.ToString(),
            BranchName = c.Branch?.Name ?? string.Empty,
            DepartmentName = c.Department?.Name,
            GuardianName = c.GuardianName,
            MobileNumber = c.MobileNumber,
            Address = c.Address,
            District = c.District,
            Block = c.Block,
            PoliceStation = c.PoliceStation,
            OppositeParty = c.OppositeParty,
            Category = c.Category,
            SubCategory = c.SubCategory,
            Description = c.Description,
            Source = c.Source.ToString(),
            ClosureRemarks = c.ClosureRemarks,
            ClosedAt = c.ClosedAt,
            Documents = c.Documents.Select(d => new ComplaintDocumentDto
            {
                Id = d.Id,
                FileName = d.FileName,
                DocumentType = d.DocumentType,
                UploadedAt = d.UploadedAt,
                DownloadUrl = $"/api/complaints/{c.Id}/documents/{d.Id}/download"
            }).ToList(),
            Remarks = c.Remarks.Select(r => new ComplaintRemarkDto
            {
                Id = r.Id,
                RemarkType = r.RemarkType,
                Text = r.Text,
                CreatedByName = r.CreatedByUser?.FullName ?? string.Empty,
                CreatedAt = r.CreatedAt
            }).OrderBy(r => r.CreatedAt).ToList(),
            History = c.HistoryEntries.Select(h => new ComplaintHistoryDto
            {
                FromStatus = h.FromStatus.ToString(),
                ToStatus = h.ToStatus.ToString(),
                Reason = h.Reason,
                ActionByName = h.ActionByUser?.FullName ?? string.Empty,
                ActionAt = h.ActionAt
            }).OrderBy(h => h.ActionAt).ToList()
        };
    }
}
