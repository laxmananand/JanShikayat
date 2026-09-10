namespace JanShikayat.Api.DTOs.Admin
{
    public class BranchDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? NameHindi { get; set; }
    }

    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? NameHindi { get; set; }
    }

    public class CompetentAuthorityDesignationDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }

    public class DashboardSummaryDto
    {
        public int TotalComplaints { get; set; }
        public int Pending { get; set; }
        public int UnderReview { get; set; }
        public int Forwarded { get; set; }
        public int UnderFieldEnquiry { get; set; }
        public int Disposed { get; set; }
    }
}
