using BUS.DTOs;
using BUS.Interfaces;
using DAL.Entities;
using DAL.Enums;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

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

    public async Task<OperationResultDto> RegisterStudentAsync(StudentRegistrationDto registration, CancellationToken cancellationToken = default)
    {
        var studentCode = registration.StudentCode.Trim();
        var fullName = registration.FullName.Trim();

        if (ServiceValidation.Require(studentCode, "Student code") is { } codeError)
        {
            return codeError;
        }

        if (ServiceValidation.Require(fullName, "Full name") is { } nameError)
        {
            return nameError;
        }

        if (ServiceValidation.Require(registration.Password, "Password") is { } passwordError)
        {
            return passwordError;
        }

        var usernameExists = await _unitOfWork.Repository<User>()
            .Query()
            .AnyAsync(user => user.Username == studentCode, cancellationToken);

        if (usernameExists)
        {
            return OperationResultDto.Fail("Student code is already used as a username.");
        }

        var studentExists = await _unitOfWork.Repository<Student>()
            .Query()
            .AnyAsync(student => student.StudentCode == studentCode, cancellationToken);

        if (studentExists)
        {
            return OperationResultDto.Fail("Student code already exists.");
        }

        if (registration.ClassId is not null)
        {
            var classExists = await _unitOfWork.Repository<Class>()
                .Query()
                .AnyAsync(classEntity => classEntity.ClassId == registration.ClassId, cancellationToken);

            if (!classExists)
            {
                return OperationResultDto.Fail("Selected class does not exist.");
            }
        }

        var student = new Student
        {
            StudentCode = studentCode,
            FullName = fullName,
            Gender = registration.Gender.Trim(),
            DateOfBirth = registration.DateOfBirth,
            Email = registration.Email?.Trim(),
            Phone = registration.Phone?.Trim(),
            ClassId = registration.ClassId
        };

        await _unitOfWork.Repository<Student>().AddAsync(student, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var user = new User
        {
            Username = studentCode,
            Password = registration.Password,
            FullName = fullName,
            Role = UserRole.Student.ToString(),
            Status = UserStatus.Active.ToString(),
            StudentId = student.StudentId
        };

        await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Student account registered successfully.");
    }
}

