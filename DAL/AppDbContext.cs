using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Lecturer> Lecturers => Set<Lecturer>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<LecturerSubject> LecturerSubjects => Set<LecturerSubject>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Grade> Grades => Set<Grade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("Class");
            entity.HasKey(e => e.ClassId);
            entity.Property(e => e.ClassCode).HasMaxLength(20).IsRequired();
            entity.Property(e => e.ClassName).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Student");
            entity.HasKey(e => e.StudentId);
            entity.Property(e => e.StudentCode).HasMaxLength(20).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.GPA).HasPrecision(3, 2);

            entity.HasOne(e => e.Class)
                .WithMany(e => e.Students)
                .HasForeignKey(e => e.ClassId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Lecturer>(entity =>
        {
            entity.ToTable("Lecturer");
            entity.HasKey(e => e.LecturerId);
            entity.Property(e => e.LecturerCode).HasMaxLength(20).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Password).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();

            entity.HasOne(e => e.Student)
                .WithMany(e => e.UserAccounts)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Lecturer)
                .WithMany(e => e.UserAccounts)
                .HasForeignKey(e => e.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subject");
            entity.HasKey(e => e.SubjectId);
            entity.Property(e => e.SubjectCode).HasMaxLength(20).IsRequired();
            entity.Property(e => e.SubjectName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Credit).IsRequired();
        });

        modelBuilder.Entity<LecturerSubject>(entity =>
        {
            entity.ToTable("LecturerSubject");
            entity.HasKey(e => e.LecturerSubjectId);
            entity.Property(e => e.Semester).HasMaxLength(20).IsRequired();

            entity.HasOne(e => e.Lecturer)
                .WithMany(e => e.LecturerSubjects)
                .HasForeignKey(e => e.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Subject)
                .WithMany(e => e.LecturerSubjects)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Class)
                .WithMany(e => e.LecturerSubjects)
                .HasForeignKey(e => e.ClassId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("Enrollment");
            entity.HasKey(e => e.EnrollmentId);
            entity.Property(e => e.Semester).HasMaxLength(20).IsRequired();

            entity.HasOne(e => e.Student)
                .WithMany(e => e.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Subject)
                .WithMany(e => e.Enrollments)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.ToTable("Grade");
            entity.HasKey(e => e.GradeId);
            entity.Property(e => e.AssignmentScore).HasPrecision(4, 2);
            entity.Property(e => e.FinalScore).HasPrecision(4, 2);
            entity.Property(e => e.GPA).HasPrecision(3, 2);

            entity.HasOne(e => e.Enrollment)
                .WithMany(e => e.Grades)
                .HasForeignKey(e => e.EnrollmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
