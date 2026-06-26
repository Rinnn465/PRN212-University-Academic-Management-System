namespace BUS.DTOs;

public class GradeDto
{
    public int GradeId { get; set; }
    public int EnrollmentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public decimal? AssignmentScore { get; set; }
    public decimal? FinalScore { get; set; }
    public decimal? GPA { get; set; }
}
