namespace BUS.DTOs;

public class ClassStatisticsDto
{
    public int StudentCount { get; set; }
    public int PassedCount { get; set; }
    public int FailedCount { get; set; }
    public decimal PassRate { get; set; }
    public decimal? AverageScore { get; set; }
    public decimal? AverageGPA { get; set; }
}
