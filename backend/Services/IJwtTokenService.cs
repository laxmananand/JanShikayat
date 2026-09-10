using JanShikayat.Api.Models;

namespace JanShikayat.Api.Services
{
    public interface IJwtTokenService
    {
        (string token, DateTime expiresAt) GenerateToken(ApplicationUser user, IList<string> roles);
    }
}
