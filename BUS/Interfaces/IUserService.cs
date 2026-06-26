using BUS.DTOs;

namespace BUS.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<OperationResultDto> CreateAsync(UserDto user, CancellationToken cancellationToken = default);
    Task<OperationResultDto> UpdateAsync(UserDto user, CancellationToken cancellationToken = default);
    Task<OperationResultDto> LockAsync(int userId, CancellationToken cancellationToken = default);
    Task<OperationResultDto> UnlockAsync(int userId, CancellationToken cancellationToken = default);
    Task<OperationResultDto> DeleteAsync(int userId, CancellationToken cancellationToken = default);
}
