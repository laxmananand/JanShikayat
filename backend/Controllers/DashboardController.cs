using JanShikayat.Api.Data;
using JanShikayat.Api.DTOs.Admin;
using JanShikayat.Api.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JanShikayat.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
        {
            var complaints = _db.Complaints.AsQueryable();

            var summary = new DashboardSummaryDto
            {
                TotalComplaints = await complaints.CountAsync(),
                Pending = await complaints.CountAsync(c => c.Status == ComplaintStatus.Pending),
                UnderReview = await complaints.CountAsync(c => c.Status == ComplaintStatus.UnderReview),
                Forwarded = await complaints.CountAsync(c => c.Status == ComplaintStatus.ForwardedToDepartment),
                UnderFieldEnquiry = await complaints.CountAsync(c => c.Status == ComplaintStatus.ForwardedForFieldEnquiry
                                                                       || c.Status == ComplaintStatus.ActionTaken),
                Disposed = await complaints.CountAsync(c => c.Status == ComplaintStatus.Disposed)
            };

            return Ok(summary);
        }
    }
}
