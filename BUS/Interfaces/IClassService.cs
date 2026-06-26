using BUS.DTOs;

namespace BUS.Interfaces;

public interface IClassService
{
    Task<List<ClassDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ClassDto?> GetByIdAsync(int classId, CancellationToken cancellationToken = default);
    Task<OperationResultDto> CreateAsync(ClassDto classDto, CancellationToken cancellationToken = default);
    Task<OperationResultDto> UpdateAsync(ClassDto classDto, CancellationToken cancellationToken = default);
    Task<OperationResultDto> DeleteAsync(int classId, CancellationToken cancellationToken = default);
}
