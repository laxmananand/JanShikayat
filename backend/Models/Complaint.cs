using System.ComponentModel.DataAnnotations;
using JanShikayat.Api.Models.Enums;

namespace JanShikayat.Api.Models
{
    /// <summary>
    /// Core grievance record. Field names follow the "Applicant Details" +
    /// "Complaint Details" sections of the spec (applicant name, guardian name,
    /// mobile, address, district/block/area, subject, category, description,
    /// related department, plus review fields used on the official screens:
    /// district / block / police station / opposite party / status).
    /// </summary>
    public class Complaint
    {
        public int Id { get; set; }

        [MaxLength(40)]
        public string ComplaintNumber { get; set; } = string.Empty; // e.g. JS-2026-0001

        // ---- Applicant details ----
        [MaxLength(200)]
        public string ApplicantName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string GuardianName { get; set; } = string.Empty; // father/husband/guardian

        [MaxLength(20)]
        public string MobileNumber { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(100)]
        public string District { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Block { get; set; } = string.Empty; // प्रखंड

        [MaxLength(100)]
        public string? PoliceStation { get; set; }

        [MaxLength(100)]
        public string? Area { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(200)]
        public string? OppositeParty { get; set; } // प्रतिपक्षी

        // ---- Complaint details ----
        [MaxLength(300)]
        public string Subject { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Category { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? SubCategory { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime ComplaintDate { get; set; } = DateTime.UtcNow;

        public ComplaintSource Source { get; set; } = ComplaintSource.Offline;

        // ---- Routing ----
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? CompetentAuthorityDesignationId { get; set; }
        public CompetentAuthorityDesignation? CompetentAuthorityDesignation { get; set; }

        public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;

        // ---- Audit ----
        public string CreatedByUserId { get; set; } = string.Empty;
        public ApplicationUser? CreatedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAt { get; set; }

        [MaxLength(1000)]
        public string? ClosureRemarks { get; set; }

        public ICollection<ComplaintDocument> Documents { get; set; } = new List<ComplaintDocument>();
        public ICollection<ComplaintRemark> Remarks { get; set; } = new List<ComplaintRemark>();
        public ICollection<ComplaintHistory> HistoryEntries { get; set; } = new List<ComplaintHistory>();
    }
}
