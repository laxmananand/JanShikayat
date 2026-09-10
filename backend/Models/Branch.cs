using System.ComponentModel.DataAnnotations;

namespace JanShikayat.Api.Models
{
    /// <summary>
    /// One of the 42 originating branches ("प्रषाखा") that register grievances.
    /// </summary>
    public class Branch
    {
        public int Id { get; set; }

        [MaxLength(20)]
        public string Code { get; set; } = string.Empty; // e.g. "B01"

        [MaxLength(200)]
        public string Name { get; set; } = string.Empty; // e.g. "Branch 1"

        [MaxLength(200)]
        public string? NameHindi { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
