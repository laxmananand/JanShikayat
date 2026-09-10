using System.ComponentModel.DataAnnotations;

namespace JanShikayat.Api.Models
{
    /// <summary>
    /// Enquiry remarks or official/action remarks (up to 500 words) entered
    /// against a complaint during review, per the "Official Review Screen".
    /// </summary>
    public class ComplaintRemark
    {
        public int Id { get; set; }

        public int ComplaintId { get; set; }
        public Complaint? Complaint { get; set; }

        [MaxLength(30)]
        public string RemarkType { get; set; } = "Enquiry"; // Enquiry | Official

        [MaxLength(3000)]
        public string Text { get; set; } = string.Empty; // enforced <= 500 words in DTO validation

        public string CreatedByUserId { get; set; } = string.Empty;
        public ApplicationUser? CreatedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
