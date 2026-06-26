using BUS.DTOs;

namespace BUS.Services;

internal static class ServiceValidation
{
    public static OperationResultDto? Require(string value, string fieldName)
    {
        return string.IsNullOrWhiteSpace(value)
            ? OperationResultDto.Fail($"{fieldName} is required.")
            : null;
    }

    public static OperationResultDto? RequireScore(decimal? score, string fieldName)
    {
        if (score is null)
        {
            return null;
        }

        return score < 0 || score > 10
            ? OperationResultDto.Fail($"{fieldName} must be between 0 and 10.")
            : null;
    }

    public static decimal? CalculateGpa(decimal? assignmentScore, decimal? finalScore)
    {
        if (assignmentScore is null || finalScore is null)
        {
            return null;
        }

        var average = (assignmentScore.Value * 0.4m) + (finalScore.Value * 0.6m);

        return average switch
        {
            >= 8.5m => 4.0m,
            >= 8.0m => 3.5m,
            >= 7.0m => 3.0m,
            >= 6.5m => 2.5m,
            >= 5.5m => 2.0m,
            >= 5.0m => 1.5m,
            >= 4.0m => 1.0m,
            _ => 0m
        };
    }
}
