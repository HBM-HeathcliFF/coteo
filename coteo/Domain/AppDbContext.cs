using coteo.Areas.Identity.Data;
using coteo.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace coteo.Domain
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasOne(c => c.Creator)
                .WithMany(o => o.MyOrders)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Order>()
                .HasOne(c => c.Executor)
                .WithMany(o => o.IssuedToMeOrders)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder
                .Entity<Organization>()
                .HasOne(e => e.Creator)
                .WithOne(e => e.Organization)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder
                .Entity<Department>()
                .HasOne(e => e.Leader)
                .WithOne(e => e.Department)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder
                .Entity<Department>()
                .HasOne(e => e.Organization)
                .WithMany(e => e.Departments)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}