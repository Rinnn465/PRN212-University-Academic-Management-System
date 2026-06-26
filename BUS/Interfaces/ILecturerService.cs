using BUS.DTOs;

namespace BUS.Interfaces;

public interface ILecturerService
{
    Task<List<LecturerAssignmentDto>> GetAssignmentsAsync(int lecturerId, string? semester = null, CancellationToken cancellationToken = default);
    Task<List<GradeDto>> GetStudentGradesAsync(int lecturerSubjectId, CancellationToken cancellationToken = default);
    Task<OperationResultDto> UpdateGradeAsync(GradeUpdateDto grade, CancellationToken cancellationToken = default);
    Task<ClassStatisticsDto> GetClassStatisticsAsync(int lecturerSubjectId, CancellationToken cancellationToken = default);
}
