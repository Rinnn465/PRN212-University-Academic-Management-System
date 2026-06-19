namespace DAL.Entities;

public class LecturerSubject
{
    public int LecturerSubjectId { get; set; }
    public int LecturerId { get; set; }
    public int SubjectId { get; set; }
    public int? ClassId { get; set; }
    public string Semester { get; set; } = string.Empty;
    public Lecturer? Lecturer { get; set; }
    public Subject? Subject { get; set; }
    public Class? Class { get; set; }
}
