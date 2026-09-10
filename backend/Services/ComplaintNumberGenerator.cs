using JanShikayat.Api.Data;

namespace JanShikayat.Api.Services
{
    /// <summary>
    /// Generates unique complaint numbers in the "JS-{year}-{sequence}" format
    /// shown in the sample UI (e.g. JS-2026-0001), scoped per calendar year.
    /// </summary>
    public class ComplaintNumberGenerator : IComplaintNumberGenerator
    {
        private readonly AppDbContext _db;

        public ComplaintNumberGenerator(AppDbContext db)
        {
            _db = db;
        }

        public async Task<string> GenerateAsync()
        {
            var year = DateTime.UtcNow.Year;
            var prefix = $"JS-{year}-";

            var countThisYear = await Task.Run(() =>
                _db.Complaints.Count(c => c.ComplaintNumber.StartsWith(prefix)));

            var next = countThisYear + 1;
            return $"{prefix}{next:D4}";
        }
    }
}
