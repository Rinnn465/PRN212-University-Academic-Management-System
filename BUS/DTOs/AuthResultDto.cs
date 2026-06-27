namespace BUS.DTOs;

public class AuthResultDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public AuthenticatedUserDto? User { get; set; }

    public static AuthResultDto Success(AuthenticatedUserDto user)
    {
        return new AuthResultDto
        {
            IsSuccess = true,
            Message = "Login successful.",
            User = user
        };
    }

    public static AuthResultDto Fail(string message)
    {
        return new AuthResultDto
        {
            IsSuccess = false,
            Message = message
        };
    }
}

