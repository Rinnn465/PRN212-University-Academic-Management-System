namespace BUS.DTOs;

public class LecturerDto
{
    public int LecturerId { get; set; }
    public string LecturerCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
