namespace BUS.DTOs;

public class LecturerAssignmentDto
{
    public int LecturerSubjectId { get; set; }
    public int LecturerId { get; set; }
    public int SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int? ClassId { get; set; }
    public string? ClassCode { get; set; }
    public string Semester { get; set; } = string.Empty;
}
