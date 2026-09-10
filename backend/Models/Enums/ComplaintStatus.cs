namespace JanShikayat.Api.Models.Enums
{
    /// <summary>
    /// Lifecycle states of a complaint, mirroring the "case lifecycle" workflow in the spec:
    /// registration -> departmental review -> forwarding to competent authority for field
    /// enquiry -> action taken -> final closure.
    /// </summary>
    public enum ComplaintStatus
    {
        Pending = 0,
        UnderReview = 1,
        ForwardedToDepartment = 2,
        ForwardedForFieldEnquiry = 3,
        ActionTaken = 4,
        Disposed = 5
    }

    public enum ComplaintSource
    {
        Offline = 0,
        CPGRAMS = 1,
        CMJaibodha = 2,
        PMOGrievancePortal = 3,
        Other = 4
    }
}
