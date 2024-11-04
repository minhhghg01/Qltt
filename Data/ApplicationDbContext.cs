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

            //Cấu hình quan hệ giữa Teacher và User
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teachers");
                entity.HasOne(d => d.User)
                    .WithOne(u => u.Teacher)
                    .HasForeignKey<Teacher>(t => t.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            //Cấu hình quan hệ giữa Student và User
           modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Students");
                entity.HasOne(s => s.User)
                    .WithOne(u => u.Student)
                    .HasForeignKey<Student>(s => s.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });


            // Cấu hình quan hệ một-một giữa Teacher và Class với TeacherId là khóa ngoại trong bảng Classes
           modelBuilder.Entity<Class>()
            .HasOne(c => c.Teacher)
            .WithMany(t => t.Classes)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.SetNull);

            // Cấu hình các quan hệ khác
            modelBuilder.Entity<StudentTest>(entity =>
            {
                entity.HasKey(e => e.StudentTestId);
                entity.HasOne(st => st.Student)
                    .WithMany(s => s.StudentTests)
                    .HasForeignKey(st => st.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(st => st.Test)
                    .WithMany(t => t.StudentTests)
                    .HasForeignKey(st => st.TestId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.Score)
                    .HasColumnType("decimal(5,2)")
                    .IsRequired();
            });

            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasOne(a => a.Student)
                    .WithMany(s => s.Attendances)
                    .HasForeignKey(a => a.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.Class)
                    .WithMany(c => c.Attendances)
                    .HasForeignKey(a => a.ClassId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.HasIndex(e => new { e.StudentId, e.ClassId, e.Date }).IsUnique();
            });

            // Cấu hình thêm cho User, Class, Test
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

            // Cấu hình quan hệ giữa Student và Class
            modelBuilder.Entity<Class>()
        .HasMany(c => c.Students)
        .WithOne(s => s.Class)
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // Vô hiệu hóa cascade delete mặc định
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }
    }
}
