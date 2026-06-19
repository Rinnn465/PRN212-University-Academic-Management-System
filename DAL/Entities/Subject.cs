namespace DAL.Entities;

public class Subject
{
    public int SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int Credit { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<LecturerSubject> LecturerSubjects { get; set; } = new List<LecturerSubject>();
}
