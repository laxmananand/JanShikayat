namespace JanShikayat.Api.Services
{
    /// <summary>
    /// Stores uploaded PDFs (complaint attachments, signed enquiry reports)
    /// on local disk under wwwroot/uploads/{complaintId}/. Swap this out for
    /// S3/Azure Blob in production by implementing IFileStorageService.
    /// </summary>
    public class FileStorageService : IFileStorageService
    {
        private readonly string _webRoot;
        private readonly string _rootPath;

        public FileStorageService(IWebHostEnvironment env)
        {
            _webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
            _rootPath = Path.Combine(_webRoot, "uploads");
            Directory.CreateDirectory(_rootPath);
        }

        public async Task<(string storagePath, long sizeBytes)> SaveAsync(IFormFile file, int complaintId)
        {
            if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".pdf")
                throw new InvalidOperationException("Only PDF files are allowed.");

            var folder = Path.Combine(_rootPath, complaintId.ToString());
            Directory.CreateDirectory(folder);

            var safeFileName = $"{Guid.NewGuid():N}.pdf";
            var fullPath = Path.Combine(folder, safeFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relative = Path.Combine("uploads", complaintId.ToString(), safeFileName).Replace("\\", "/");
            return (relative, file.Length);
        }

        public string GetPhysicalPath(string storagePath)
        {
            return Path.Combine(_webRoot, storagePath);
        }
    }
}
