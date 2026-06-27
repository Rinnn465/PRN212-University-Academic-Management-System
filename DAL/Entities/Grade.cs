namespace DAL.Entities;

public class Grade
{
    public int GradeId { get; set; }
    public int EnrollmentId { get; set; }
    public decimal? AssignmentScore { get; set; }
    public decimal? FinalScore { get; set; }
    public decimal? GPA { get; set; }
    public Enrollment? Enrollment { get; set; }
}

