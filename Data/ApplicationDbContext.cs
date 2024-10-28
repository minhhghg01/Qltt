using Microsoft.EntityFrameworkCore;
using Qltt.Models;

namespace Qltt.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Class> Classes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .UseTptMappingStrategy();  // Sử dụng Table-Per-Type
            
        modelBuilder.Entity<User>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETDATE()");
            
        modelBuilder.Entity<User>()
            .Property(u => u.UpdatedAt)
            .HasDefaultValueSql("GETDATE()");
    }
    }

}