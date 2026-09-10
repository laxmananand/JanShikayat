using System.ComponentModel.DataAnnotations;
using JanShikayat.Api.Models.Enums;

namespace JanShikayat.Api.Models
{
    /// <summary>
    /// Immutable audit trail of every status change / forward / closure action
    /// taken on a complaint ("Processing history" in the spec).
    /// </summary>
    public class ComplaintHistory
    {
        public int Id { get; set; }

        public int ComplaintId { get; set; }
        public Complaint? Complaint { get; set; }

        public ComplaintStatus FromStatus { get; set; }
        public ComplaintStatus ToStatus { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        public string ActionByUserId { get; set; } = string.Empty;
        public ApplicationUser? ActionByUser { get; set; }

        public DateTime ActionAt { get; set; } = DateTime.UtcNow;
    }
}
