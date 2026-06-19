namespace DAL.Entities;

public class Enrollment
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public string Semester { get; set; } = string.Empty;
    public Student? Student { get; set; }
    public Subject? Subject { get; set; }
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
