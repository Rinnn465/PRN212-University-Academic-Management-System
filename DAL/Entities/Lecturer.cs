namespace DAL.Entities;

public class Lecturer
{
    public int LecturerId { get; set; }
    public string LecturerCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public ICollection<User> UserAccounts { get; set; } = new List<User>();
    public ICollection<LecturerSubject> LecturerSubjects { get; set; } = new List<LecturerSubject>();
}
