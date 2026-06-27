using BUS.DTOs;
using BUS.Interfaces;
using DAL.Entities;
using DAL.Enums;
using DAL.Interfaces;

namespace BUS.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResultDto> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var normalizedUsername = username.Trim();

        if (string.IsNullOrWhiteSpace(normalizedUsername) || string.IsNullOrWhiteSpace(password))
        {
            return AuthResultDto.Fail("Username and password are required.");
        }

        var users = await _unitOfWork.Repository<User>().FindAsync(
            user => user.Username == normalizedUsername && user.Password == password,
            cancellationToken);

        var user = users.FirstOrDefault();

        if (user is null)
        {
            return AuthResultDto.Fail("Invalid username or password.");
        }

        if (user.Status != UserStatus.Active.ToString())
        {
            return AuthResultDto.Fail("This account is locked.");
        }

        return AuthResultDto.Success(new AuthenticatedUserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            Status = user.Status,
            StudentId = user.StudentId,
            LecturerId = user.LecturerId
        });
    }
}

