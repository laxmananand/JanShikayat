namespace JanShikayat.Api.Models.Enums
{
    /// <summary>
    /// Static role names used throughout the system. These are seeded into
    /// AspNetCore Identity roles at startup (see DbSeeder).
    /// Maps to the multi-level hierarchy described in the Jan Shikayat spec:
    /// Branch Officer (42 branches) -> Department Head -> Field/Competent Authority -> Super Admin.
    /// </summary>
    public static class AppRoles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string BranchOfficer = "BranchOfficer";
        public const string DepartmentHead = "DepartmentHead";
        public const string CompetentAuthority = "CompetentAuthority";

        public static readonly string[] All =
        {
            SuperAdmin, BranchOfficer, DepartmentHead, CompetentAuthority
        };
    }
}
