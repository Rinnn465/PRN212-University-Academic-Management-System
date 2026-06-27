namespace DAL.Entities;

public class Student
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public decimal? GPA { get; set; }
    public int? ClassId { get; set; }
    public Class? Class { get; set; }
    public ICollection<User> UserAccounts { get; set; } = new List<User>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
