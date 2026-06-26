using BUS.DTOs;
using BUS.Interfaces;
using DAL.Entities;
using DAL.Enums;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BUS.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Repository<User>()
            .Query()
            .OrderBy(user => user.Role)
            .ThenBy(user => user.Username)
            .Select(user => ToDto(user))
            .ToListAsync(cancellationToken);
    }

    public Task<UserDto?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Repository<User>()
            .Query()
            .Where(user => user.UserId == userId)
            .Select(user => ToDto(user))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<OperationResultDto> CreateAsync(UserDto user, CancellationToken cancellationToken = default)
    {
        if (ServiceValidation.Require(user.Username, "Username") is { } usernameError)
        {
            return usernameError;
        }

        if (ServiceValidation.Require(user.Password, "Password") is { } passwordError)
        {
            return passwordError;
        }

        if (ServiceValidation.Require(user.FullName, "Full name") is { } nameError)
        {
            return nameError;
        }

        if (!IsValidRole(user.Role))
        {
            return OperationResultDto.Fail("Role must be Admin, Lecturer, or Student.");
        }

        if (!IsValidStatus(user.Status))
        {
            return OperationResultDto.Fail("Status must be Active or Locked.");
        }

        var username = user.Username.Trim();
        var isDuplicated = await _unitOfWork.Repository<User>()
            .Query()
            .AnyAsync(item => item.Username == username, cancellationToken);

        if (isDuplicated)
        {
            return OperationResultDto.Fail("Username already exists.");
        }

        if (await ValidateOwnerAsync(user, cancellationToken) is { } ownerError)
        {
            return ownerError;
        }

        await _unitOfWork.Repository<User>().AddAsync(new User
        {
            Username = username,
            Password = user.Password,
            FullName = user.FullName.Trim(),
            Role = user.Role.Trim(),
            Status = user.Status.Trim(),
            StudentId = user.StudentId,
            LecturerId = user.LecturerId
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("User created successfully.");
    }

    public async Task<OperationResultDto> UpdateAsync(UserDto user, CancellationToken cancellationToken = default)
    {
        if (ServiceValidation.Require(user.Username, "Username") is { } usernameError)
        {
            return usernameError;
        }

        if (ServiceValidation.Require(user.FullName, "Full name") is { } nameError)
        {
            return nameError;
        }

        if (!IsValidRole(user.Role))
        {
            return OperationResultDto.Fail("Role must be Admin, Lecturer, or Student.");
        }

        if (!IsValidStatus(user.Status))
        {
            return OperationResultDto.Fail("Status must be Active or Locked.");
        }

        var entity = await _unitOfWork.Repository<User>().GetByIdAsync(user.UserId, cancellationToken);

        if (entity is null)
        {
            return OperationResultDto.Fail("User was not found.");
        }

        var username = user.Username.Trim();
        var isDuplicated = await _unitOfWork.Repository<User>()
            .Query()
            .AnyAsync(item => item.UserId != user.UserId && item.Username == username, cancellationToken);

        if (isDuplicated)
        {
            return OperationResultDto.Fail("Username already exists.");
        }

        if (await ValidateOwnerAsync(user, cancellationToken) is { } ownerError)
        {
            return ownerError;
        }

        entity.Username = username;
        entity.FullName = user.FullName.Trim();
        entity.Role = user.Role.Trim();
        entity.Status = user.Status.Trim();
        entity.StudentId = user.StudentId;
        entity.LecturerId = user.LecturerId;

        if (!string.IsNullOrWhiteSpace(user.Password))
        {
            entity.Password = user.Password;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("User updated successfully.");
    }

    public Task<OperationResultDto> LockAsync(int userId, CancellationToken cancellationToken = default)
    {
        return ChangeStatusAsync(userId, UserStatus.Locked.ToString(), "User locked successfully.", cancellationToken);
    }

    public Task<OperationResultDto> UnlockAsync(int userId, CancellationToken cancellationToken = default)
    {
        return ChangeStatusAsync(userId, UserStatus.Active.ToString(), "User unlocked successfully.", cancellationToken);
    }

    public async Task<OperationResultDto> DeleteAsync(int userId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<User>().GetByIdAsync(userId, cancellationToken);

        if (entity is null)
        {
            return OperationResultDto.Fail("User was not found.");
        }

        _unitOfWork.Repository<User>().Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("User deleted successfully.");
    }

    private async Task<OperationResultDto> ChangeStatusAsync(int userId, string status, string successMessage, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Repository<User>().GetByIdAsync(userId, cancellationToken);

        if (entity is null)
        {
            return OperationResultDto.Fail("User was not found.");
        }

        entity.Status = status;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success(successMessage);
    }

    private async Task<OperationResultDto?> ValidateOwnerAsync(UserDto user, CancellationToken cancellationToken)
    {
        var role = user.Role.Trim();

        if (role == UserRole.Student.ToString())
        {
            if (user.StudentId is null)
            {
                return OperationResultDto.Fail("Student user must be assigned to a student profile.");
            }

            var studentExists = await _unitOfWork.Repository<Student>()
                .Query()
                .AnyAsync(student => student.StudentId == user.StudentId, cancellationToken);

            return studentExists ? null : OperationResultDto.Fail("Assigned student profile does not exist.");
        }

        if (role == UserRole.Lecturer.ToString())
        {
            if (user.LecturerId is null)
            {
                return OperationResultDto.Fail("Lecturer user must be assigned to a lecturer profile.");
            }

            var lecturerExists = await _unitOfWork.Repository<Lecturer>()
                .Query()
                .AnyAsync(lecturer => lecturer.LecturerId == user.LecturerId, cancellationToken);

            return lecturerExists ? null : OperationResultDto.Fail("Assigned lecturer profile does not exist.");
        }

        return null;
    }

    private static bool IsValidRole(string role)
    {
        return Enum.TryParse<UserRole>(role, ignoreCase: false, out _);
    }

    private static bool IsValidStatus(string status)
    {
        return Enum.TryParse<UserStatus>(status, ignoreCase: false, out _);
    }

    private static UserDto ToDto(User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Password = string.Empty,
            FullName = user.FullName,
            Role = user.Role,
            Status = user.Status,
            StudentId = user.StudentId,
            LecturerId = user.LecturerId
        };
    }
}
