using JanShikayat.Api.Models;
using JanShikayat.Api.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JanShikayat.Api.Data
{
    /// <summary>
    /// Seeds Identity roles, the 42 originating branches, a starter set of
    /// departments and competent-authority designations, and one demo login
    /// per role so the app is usable immediately after first run.
    /// </summary>
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var db = services.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in AppRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            if (!db.Branches.Any())
            {
                for (int i = 1; i <= 42; i++)
                {
                    db.Branches.Add(new Branch
                    {
                        Code = $"B{i:D2}",
                        Name = $"Branch {i}",
                        NameHindi = $"प्रषाखा {i}"
                    });
                }
                await db.SaveChangesAsync();
            }

            if (!db.Departments.Any())
            {
                var departments = new[]
                {
                    ("HOME", "Home Department", "गृह विभाग"),
                    ("RLR", "Revenue & Land Reforms Department", "राजस्व एवं भूमि सुधार विभाग"),
                    ("FIN", "Finance Department", "वित्त विभाग"),
                    ("TRN", "Transport Department", "परिवहन विभाग"),
                    ("GEN", "General Administration Department", "सामान्य प्रशासन विभाग"),
                };
                foreach (var (code, name, hindi) in departments)
                {
                    db.Departments.Add(new Department { Code = code, Name = name, NameHindi = hindi });
                }
                await db.SaveChangesAsync();
            }

            if (!db.CompetentAuthorityDesignations.Any())
            {
                var designations = new[]
                {
                    ("COMM", "Commissioner"),
                    ("IG", "Inspector General of Police"),
                    ("DIG", "Deputy Inspector General of Police"),
                    ("CM_HELPLINE", "CM Helpline / CM Jaibodha Cell"),
                    ("SSP", "Senior Superintendent of Police"),
                    ("SP", "Superintendent of Police"),
                    ("SDO", "Sub-Divisional Officer (Anchal Adhikari)"),
                    ("CO", "Circle Officer"),
                };
                foreach (var (code, title) in designations)
                {
                    db.CompetentAuthorityDesignations.Add(new CompetentAuthorityDesignation { Code = code, Title = title });
                }
                await db.SaveChangesAsync();
            }

            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            var branch1 = db.Branches.OrderBy(b => b.Id).First();
            var homeDept = db.Departments.First(d => d.Code == "HOME");

            await EnsureUserAsync(userManager, db, "admin@janshikayat.gov.in", "Admin@12345",
                "Super Admin", "System Administrator", AppRoles.SuperAdmin, null, null);

            await EnsureUserAsync(userManager, db, "branchofficer1@janshikayat.gov.in", "Branch@12345",
                "Branch Officer 1", "Branch Officer", AppRoles.BranchOfficer, branch1.Id, null);

            await EnsureUserAsync(userManager, db, "homedept.head@janshikayat.gov.in", "Dept@12345",
                "Home Department Head", "Department Head", AppRoles.DepartmentHead, null, homeDept.Id);

            await EnsureUserAsync(userManager, db, "sdo1@janshikayat.gov.in", "Authority@12345",
                "SDO Patna Sadar", "Sub-Divisional Officer", AppRoles.CompetentAuthority, null, null);
        }

        private static async Task EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            AppDbContext db,
            string email,
            string password,
            string fullName,
            string designation,
            string role,
            int? branchId,
            int? departmentId)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing != null) return;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                Designation = designation,
                BranchId = branchId,
                DepartmentId = departmentId
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
