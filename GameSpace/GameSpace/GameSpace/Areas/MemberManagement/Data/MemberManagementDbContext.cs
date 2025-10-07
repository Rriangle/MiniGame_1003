using GameSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Areas.MemberManagement.Data
{
    public class MemberManagementDbContext : DbContext
    {
        public MemberManagementDbContext(DbContextOptions<MemberManagementDbContext> options)
            : base(options) { }

        public DbSet<ManagerAccountRole> ManagerAccountRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ManagerAccountRole>(entity =>
            {
                entity.HasKey(e => new { e.ManagerId, e.ManagerRoleId });
                entity.ToTable("ManagerRole");
                entity.Property(e => e.ManagerId).HasColumnName("Manager_Id");
                entity.Property(e => e.ManagerRoleId).HasColumnName("ManagerRole_Id");
            });
        }
    }
}
