namespace DAL.Entities;

public class Class
{
    public int ClassId { get; set; }
    public string ClassCode { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<LecturerSubject> LecturerSubjects { get; set; } = new List<LecturerSubject>();
}
