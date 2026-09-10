using System.ComponentModel.DataAnnotations;

namespace JanShikayat.Api.Models
{
    /// <summary>
    /// A government department a complaint can be forwarded to
    /// (Home Dept, Revenue & Land Reforms Dept, Finance Dept, Transport Dept, etc.).
    /// </summary>
    public class Department
    {
        public int Id { get; set; }

        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? NameHindi { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
