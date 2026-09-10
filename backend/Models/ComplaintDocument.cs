using System.ComponentModel.DataAnnotations;

namespace JanShikayat.Api.Models
{
    /// <summary>
    /// A PDF attached to a complaint: either the original citizen-submitted
    /// complaint PDF, or a signed enquiry / field-verification report uploaded
    /// later by the reviewing officer.
    /// </summary>
    public class ComplaintDocument
    {
        public int Id { get; set; }

        public int ComplaintId { get; set; }
        public Complaint? Complaint { get; set; }

        [MaxLength(300)]
        public string FileName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string StoragePath { get; set; } = string.Empty;

        [MaxLength(50)]
        public string DocumentType { get; set; } = "Complaint"; // Complaint | EnquiryReport

        public long FileSizeBytes { get; set; }

        public string UploadedByUserId { get; set; } = string.Empty;
        public ApplicationUser? UploadedByUser { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
