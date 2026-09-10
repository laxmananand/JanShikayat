using JanShikayat.Api.DTOs.Auth;
using JanShikayat.Api.Models;
using JanShikayat.Api.Models.Enums;
using JanShikayat.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JanShikayat.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !user.IsActive)
                return Unauthorized(new { message = "Invalid credentials." });

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!valid)
                return Unauthorized(new { message = "Invalid credentials." });

            var roles = await _userManager.GetRolesAsync(user);
            var (token, expiresAt) = _jwtTokenService.GenerateToken(user, roles);

            return Ok(new LoginResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                Role = roles.FirstOrDefault() ?? string.Empty,
                BranchId = user.BranchId,
                DepartmentId = user.DepartmentId
            });
        }

        /// <summary>
        /// Creates a new user account. Only a SuperAdmin can create accounts for
        /// the 42 branch officers, department heads, and competent authorities.
        /// </summary>
        [HttpPost("register")]
        [Authorize(Roles = AppRoles.SuperAdmin)]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            if (!AppRoles.All.Contains(dto.Role))
                return BadRequest(new { message = "Invalid role." });

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true,
                FullName = dto.FullName,
                Designation = dto.Designation,
                BranchId = dto.BranchId,
                DepartmentId = dto.DepartmentId
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            await _userManager.AddToRoleAsync(user, dto.Role);
            return Ok(new { message = "User created." });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (email == null) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new
            {
                user.Email,
                user.FullName,
                user.Designation,
                user.BranchId,
                user.DepartmentId,
                Roles = roles
            });
        }
    }
}
