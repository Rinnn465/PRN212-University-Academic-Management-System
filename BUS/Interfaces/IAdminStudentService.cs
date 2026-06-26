using BUS.DTOs;

namespace BUS.Interfaces;

public interface IAdminStudentService
{
    Task<List<StudentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StudentDto?> GetByIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<OperationResultDto> CreateAsync(StudentDto student, CancellationToken cancellationToken = default);
    Task<OperationResultDto> UpdateAsync(StudentDto student, CancellationToken cancellationToken = default);
    Task<OperationResultDto> DeleteAsync(int studentId, CancellationToken cancellationToken = default);
}
