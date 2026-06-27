using BUS.DTOs;

namespace BUS.Interfaces;

public interface IAuthService
{
    Task<AuthResultDto> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
}

