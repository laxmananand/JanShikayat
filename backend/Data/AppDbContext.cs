using JanShikayat.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JanShikayat.Api.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<CompetentAuthorityDesignation> CompetentAuthorityDesignations => Set<CompetentAuthorityDesignation>();
        public DbSet<Complaint> Complaints => Set<Complaint>();
        public DbSet<ComplaintDocument> ComplaintDocuments => Set<ComplaintDocument>();
        public DbSet<ComplaintRemark> ComplaintRemarks => Set<ComplaintRemark>();
        public DbSet<ComplaintHistory> ComplaintHistories => Set<ComplaintHistory>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Branch>().HasIndex(b => b.Code).IsUnique();
            builder.Entity<Department>().HasIndex(d => d.Code).IsUnique();
            builder.Entity<Complaint>().HasIndex(c => c.ComplaintNumber).IsUnique();

            builder.Entity<Complaint>()
                .HasOne(c => c.Branch)
                .WithMany(b => b.Complaints)
                .HasForeignKey(c => c.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Complaint>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Complaints)
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Complaint>()
                .HasOne(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Branch)
                .WithMany(b => b.Users)
                .HasForeignKey(u => u.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ComplaintDocument>()
                .HasOne(d => d.Complaint)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ComplaintRemark>()
                .HasOne(r => r.Complaint)
                .WithMany(c => c.Remarks)
                .HasForeignKey(r => r.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ComplaintHistory>()
                .HasOne(h => h.Complaint)
                .WithMany(c => c.HistoryEntries)
                .HasForeignKey(h => h.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Complaint>().Property(c => c.Status).HasConversion<string>().HasMaxLength(40);
            builder.Entity<Complaint>().Property(c => c.Source).HasConversion<string>().HasMaxLength(40);
            builder.Entity<ComplaintHistory>().Property(h => h.FromStatus).HasConversion<string>().HasMaxLength(40);
            builder.Entity<ComplaintHistory>().Property(h => h.ToStatus).HasConversion<string>().HasMaxLength(40);
        }
    }
}
