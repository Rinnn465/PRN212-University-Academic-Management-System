namespace BUS.DTOs;

public class StudentDto
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
    public string? ClassCode { get; set; }
}
