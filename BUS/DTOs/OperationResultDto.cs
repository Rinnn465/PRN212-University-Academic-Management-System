namespace BUS.DTOs;

public class OperationResultDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;

    public static OperationResultDto Success(string message)
    {
        return new OperationResultDto
        {
            IsSuccess = true,
            Message = message
        };
    }

    public static OperationResultDto Fail(string message)
    {
        return new OperationResultDto
        {
            IsSuccess = false,
            Message = message
        };
    }
}
