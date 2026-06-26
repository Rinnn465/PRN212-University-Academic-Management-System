namespace BUS.DTOs;

public class DashboardSummaryDto
{
    public int TotalStudents { get; set; }
    public int TotalLecturers { get; set; }
    public int TotalClasses { get; set; }
    public int TotalSubjects { get; set; }
    public decimal? AverageGPA { get; set; }
}
