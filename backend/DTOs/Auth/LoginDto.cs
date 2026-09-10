using System.ComponentModel.DataAnnotations;

namespace JanShikayat.Api.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public int? DepartmentId { get; set; }
    }

    public class RegisterUserDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string Designation { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        public int? BranchId { get; set; }
        public int? DepartmentId { get; set; }
    }
}
