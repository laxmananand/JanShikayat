using JanShikayat.Api.Data;
using JanShikayat.Api.DTOs.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JanShikayat.Api.Controllers
{
    /// <summary>
    /// Read-only lookup data (branches, departments, competent-authority
    /// designations) used to populate dropdowns on the frontend forms.
    /// </summary>
    [ApiController]
    [Route("api")]
    [Authorize]
    public class LookupController : ControllerBase
    {
        private readonly AppDbContext _db;

        public LookupController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("branches")]
        public async Task<ActionResult<IEnumerable<BranchDto>>> GetBranches()
        {
            var branches = await _db.Branches
                .Where(b => b.IsActive)
                .OrderBy(b => b.Id)
                .Select(b => new BranchDto { Id = b.Id, Code = b.Code, Name = b.Name, NameHindi = b.NameHindi })
                .ToListAsync();
            return Ok(branches);
        }

        [HttpGet("departments")]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
        {
            var departments = await _db.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.Id)
                .Select(d => new DepartmentDto { Id = d.Id, Code = d.Code, Name = d.Name, NameHindi = d.NameHindi })
                .ToListAsync();
            return Ok(departments);
        }

        [HttpGet("competent-authorities")]
        public async Task<ActionResult<IEnumerable<CompetentAuthorityDesignationDto>>> GetAuthorities()
        {
            var authorities = await _db.CompetentAuthorityDesignations
                .Where(a => a.IsActive)
                .OrderBy(a => a.Id)
                .Select(a => new CompetentAuthorityDesignationDto { Id = a.Id, Code = a.Code, Title = a.Title })
                .ToListAsync();
            return Ok(authorities);
        }
    }
}
