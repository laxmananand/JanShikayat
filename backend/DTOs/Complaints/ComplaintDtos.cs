using System.ComponentModel.DataAnnotations;
using JanShikayat.Api.Models.Enums;

namespace JanShikayat.Api.DTOs.Complaints
{
    public class CreateComplaintDto
    {
        [Required, MaxLength(200)]
        public string ApplicantName { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string GuardianName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string MobileNumber { get; set; } = string.Empty;

        [Required, MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string District { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Block { get; set; } = string.Empty;

        public string? PoliceStation { get; set; }
        public string? Area { get; set; }
        public string? Email { get; set; }
        public string? OppositeParty { get; set; }

        [Required, MaxLength(300)]
        public string Subject { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Category { get; set; } = string.Empty;

        public string? SubCategory { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public DateTime? ComplaintDate { get; set; }

        public ComplaintSource Source { get; set; } = ComplaintSource.Offline;

        [Required]
        public int BranchId { get; set; }

        public int? DepartmentId { get; set; }
    }

    public class ComplaintListItemDto
    {
        public int Id { get; set; }
        public string ComplaintNumber { get; set; } = string.Empty;
        public string ApplicantName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public DateTime ComplaintDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
    }

    public class ComplaintDetailDto : ComplaintListItemDto
    {
        public string GuardianName { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Block { get; set; } = string.Empty;
        public string? PoliceStation { get; set; }
        public string? OppositeParty { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? SubCategory { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string? ClosureRemarks { get; set; }
        public DateTime? ClosedAt { get; set; }

        public List<ComplaintDocumentDto> Documents { get; set; } = new();
        public List<ComplaintRemarkDto> Remarks { get; set; } = new();
        public List<ComplaintHistoryDto> History { get; set; } = new();
    }

    public class ComplaintDocumentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public string DownloadUrl { get; set; } = string.Empty;
    }

    public class ComplaintRemarkDto
    {
        public int Id { get; set; }
        public string RemarkType { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class AddRemarkDto
    {
        [Required]
        public string RemarkType { get; set; } = "Enquiry"; // Enquiry | Official

        [Required]
        [MaxLength(3000, ErrorMessage = "Remark must not exceed 500 words (~3000 characters).")]
        public string Text { get; set; } = string.Empty;
    }

    public class ComplaintHistoryDto
    {
        public string FromStatus { get; set; } = string.Empty;
        public string ToStatus { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string ActionByName { get; set; } = string.Empty;
        public DateTime ActionAt { get; set; }
    }

    public class ForwardComplaintDto
    {
        public int? DepartmentId { get; set; }
        public int? CompetentAuthorityDesignationId { get; set; }

        [Required, MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
    }

    public class CloseComplaintDto
    {
        [Required, MaxLength(1000)]
        public string ClosureRemarks { get; set; } = string.Empty;
    }
}
