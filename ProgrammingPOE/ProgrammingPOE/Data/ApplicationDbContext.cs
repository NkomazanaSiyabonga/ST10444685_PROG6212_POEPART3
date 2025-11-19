using Microsoft.EntityFrameworkCore;
using ProgrammingPOE.Models;
using ProgrammingPOE.Services;

namespace ProgrammingPOE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<SupportingDocument> SupportingDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Role).IsRequired().HasMaxLength(50);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
                entity.HasIndex(u => u.Email).IsUnique();
            });

            // Configure Claim entity - FIXED: Make approval fields nullable
            modelBuilder.Entity<Claim>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.UserId).IsRequired().HasMaxLength(450);
                entity.Property(c => c.LecturerName).IsRequired().HasMaxLength(200);
                entity.Property(c => c.HoursWorked).HasColumnType("decimal(18,2)");
                entity.Property(c => c.HourlyRate).HasColumnType("decimal(18,2)");
                entity.Property(c => c.TotalAmount)
                    .HasColumnType("decimal(18,2)")
                    .HasComputedColumnSql("[HoursWorked] * [HourlyRate]");
                entity.Property(c => c.AdditionalNotes).HasMaxLength(500).IsRequired(false);

                // MAKE THESE FIELDS NULLABLE
                entity.Property(c => c.RejectionNotes).HasMaxLength(1000).IsRequired(false);
                entity.Property(c => c.RejectedBy).HasMaxLength(200).IsRequired(false);
                entity.Property(c => c.VerifiedBy).HasMaxLength(200).IsRequired(false);
                entity.Property(c => c.ApprovedBy).HasMaxLength(200).IsRequired(false);

                entity.Property(c => c.VerificationDate).IsRequired(false);
                entity.Property(c => c.ApprovalDate).IsRequired(false);

                // Relationships
                entity.HasMany(c => c.SupportingDocuments)
                      .WithOne()
                      .HasForeignKey(d => d.ClaimId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure SupportingDocument entity
            modelBuilder.Entity<SupportingDocument>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.FileName).IsRequired().HasMaxLength(255);
                entity.Property(d => d.FilePath).IsRequired().HasMaxLength(500);
                entity.Property(d => d.ContentType).HasMaxLength(100);
            });
        }
    }
}