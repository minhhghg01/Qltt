using Microsoft.EntityFrameworkCore;
using Qltt.Models;

namespace Qltt.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<StudentTest> StudentTests { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.ToTable("Users");
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                    
                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admins");
                entity.HasOne(d => d.User)
                    .WithOne()
                    .HasForeignKey<Admin>(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teachers");
                entity.HasOne(d => d.User)
                    .WithOne()
                    .HasForeignKey<Teacher>(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Cấu hình quan hệ Teacher - Class
            modelBuilder.Entity<Class>()
                .HasOne(c => c.Teacher)
                .WithOne(t => t.Class)
                .HasForeignKey<Class>(c => c.TeacherId);

            // Cấu hình quan hệ Student - StudentTest
            modelBuilder.Entity<StudentTest>(entity =>
            {
                // Khóa chính
                entity.HasKey(e => e.StudentTestId);

                // Quan hệ với Student
                entity.HasOne(st => st.Student)
                    .WithMany(s => s.StudentTests)
                    .HasForeignKey(st => st.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Quan hệ với Test
                entity.HasOne(st => st.Test)
                    .WithMany(t => t.StudentTests)
                    .HasForeignKey(st => st.TestId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Cấu hình Score
                entity.Property(e => e.Score)
                    .HasColumnType("decimal(5,2)")
                    .IsRequired();
            });

            // Cấu hình quan hệ Test - StudentTest
            modelBuilder.Entity<StudentTest>()
                .HasOne(st => st.Test)
                .WithMany(t => t.StudentTests)
                .HasForeignKey(st => st.TestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ Student - Attendance
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ Class - Attendance
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Class)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // Các cấu hình khác
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Password).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.Property(e => e.ClassName).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Test>(entity =>
            {
                entity.Property(e => e.TestName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Date).IsRequired();
            });

            modelBuilder.Entity<StudentTest>(entity =>
            {
                entity.Property(e => e.Score).HasColumnType("decimal(5,2)");
            });

            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.HasIndex(e => new { e.StudentId, e.ClassId, e.Date }).IsUnique();
            });

            // Vô hiệu hóa cascade delete mặc định
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}