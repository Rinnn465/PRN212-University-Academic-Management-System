using BUS.DTOs;

namespace BUS.Interfaces;

public interface IStudentService
{
    Task<StudentDto?> GetProfileAsync(int studentId, CancellationToken cancellationToken = default);
    Task<OperationResultDto> UpdateProfileAsync(StudentDto student, CancellationToken cancellationToken = default);
    Task<List<SubjectDto>> GetAvailableSubjectsAsync(int studentId, string semester, CancellationToken cancellationToken = default);
    Task<List<EnrollmentDto>> GetRegisteredSubjectsAsync(int studentId, CancellationToken cancellationToken = default);
    Task<OperationResultDto> EnrollSubjectAsync(int studentId, int subjectId, string semester, CancellationToken cancellationToken = default);
    Task<OperationResultDto> CancelEnrollmentAsync(int studentId, int enrollmentId, CancellationToken cancellationToken = default);
    Task<List<GradeDto>> GetGradesAsync(int studentId, CancellationToken cancellationToken = default);
    Task<decimal?> GetGpaAsync(int studentId, CancellationToken cancellationToken = default);
}
