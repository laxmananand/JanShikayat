using Microsoft.AspNetCore.Identity;

namespace JanShikayat.Api.Models
{
    /// <summary>
    /// Extends ASP.NET Identity's user with the fields needed for the grievance
    /// portal hierarchy: which branch/department a user belongs to, and their
    /// designation (e.g. "SDO", "SP", "Commissioner") for display purposes.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;

        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
