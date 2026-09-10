namespace JanShikayat.Api.Services
{
    public interface IComplaintNumberGenerator
    {
        Task<string> GenerateAsync();
    }
}
