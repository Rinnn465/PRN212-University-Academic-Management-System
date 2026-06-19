namespace BUS.DTOs;

public class AuthenticatedUserDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? StudentId { get; set; }
    public int? LecturerId { get; set; }
}

