namespace JanShikayat.Api.Services
{
    public interface IFileStorageService
    {
        Task<(string storagePath, long sizeBytes)> SaveAsync(IFormFile file, int complaintId);
        string GetPhysicalPath(string storagePath);
    }
}
