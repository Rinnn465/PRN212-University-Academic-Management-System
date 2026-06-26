using BUS.DTOs;

namespace BUS.Interfaces;

public interface IAdminLecturerService
{
    Task<List<LecturerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LecturerDto?> GetByIdAsync(int lecturerId, CancellationToken cancellationToken = default);
    Task<OperationResultDto> CreateAsync(LecturerDto lecturer, CancellationToken cancellationToken = default);
    Task<OperationResultDto> UpdateAsync(LecturerDto lecturer, CancellationToken cancellationToken = default);
    Task<OperationResultDto> DeleteAsync(int lecturerId, CancellationToken cancellationToken = default);
}
