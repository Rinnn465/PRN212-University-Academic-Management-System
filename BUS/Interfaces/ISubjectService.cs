using BUS.DTOs;

namespace BUS.Interfaces;

public interface ISubjectService
{
    Task<List<SubjectDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SubjectDto?> GetByIdAsync(int subjectId, CancellationToken cancellationToken = default);
    Task<OperationResultDto> CreateAsync(SubjectDto subject, CancellationToken cancellationToken = default);
    Task<OperationResultDto> UpdateAsync(SubjectDto subject, CancellationToken cancellationToken = default);
    Task<OperationResultDto> DeleteAsync(int subjectId, CancellationToken cancellationToken = default);
}
