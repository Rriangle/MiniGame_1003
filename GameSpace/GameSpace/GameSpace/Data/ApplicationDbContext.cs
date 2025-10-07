using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<IdentityUserSignInStat> UserSignInStats { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.IsActive)
                    .HasColumnName("IsActive")
                    .HasDefaultValue(true);

                entity.Property(u => u.LastLoginAt)
                    .HasColumnName("LastLoginAt");

                entity.Property(u => u.UserStatus)
                    .HasColumnName("UserStatus")
                    .HasMaxLength(50);

                entity.Property(u => u.User_Address)
                    .HasColumnName("User_Address")
                    .HasMaxLength(255);

                entity.Property(u => u.User_birthdate)
                    .HasColumnName("User_birthdate");

                entity.Property(u => u.User_email)
                    .HasColumnName("User_email")
                    .HasMaxLength(255);

                entity.Property(u => u.User_phone)
                    .HasColumnName("User_phone")
                    .HasMaxLength(50);

                entity.Property(u => u.User_registration_date)
                    .HasColumnName("User_registration_date");

                entity.Property(u => u.User_CreatedAt)
                    .HasColumnName("User_CreatedAt");
            });

            builder.Entity<IdentityUserSignInStat>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.Property(e => e.LogId)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CouponGained)
                    .HasMaxLength(200);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
