using System.ComponentModel.DataAnnotations;

namespace JanShikayat.Api.Models
{
    /// <summary>
    /// Field / competent authority designations a complaint can be escalated to for
    /// enquiry (Commissioner, IG, DIG, CM, SSP, SP, SDO/Anchal Adhikari, CO, etc.).
    /// Kept as a lookup table (rather than a hard-coded enum) so new designations
    /// can be added by a SuperAdmin without a code change.
    /// </summary>
    public class CompetentAuthorityDesignation
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Code { get; set; } = string.Empty; // e.g. "SDO"

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty; // e.g. "Sub-Divisional Officer"

        public bool IsActive { get; set; } = true;
    }
}
