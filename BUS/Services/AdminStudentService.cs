using BUS.DTOs;
using BUS.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BUS.Services;

public class AdminStudentService : IAdminStudentService
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminStudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<StudentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Repository<Student>()
            .Query()
            .Include(student => student.Class)
            .OrderBy(student => student.StudentCode)
            .Select(student => ToDto(student))
            .ToListAsync(cancellationToken);
    }

    public Task<StudentDto?> GetByIdAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Repository<Student>()
            .Query()
            .Include(student => student.Class)
            .Where(student => student.StudentId == studentId)
            .Select(student => ToDto(student))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<OperationResultDto> CreateAsync(StudentDto student, CancellationToken cancellationToken = default)
    {
        if (ServiceValidation.Require(student.StudentCode, "Student code") is { } codeError)
        {
            return codeError;
        }

        if (ServiceValidation.Require(student.FullName, "Full name") is { } nameError)
        {
            return nameError;
        }

        var studentCode = student.StudentCode.Trim();
        var isDuplicated = await _unitOfWork.Repository<Student>()
            .Query()
            .AnyAsync(item => item.StudentCode == studentCode, cancellationToken);

        if (isDuplicated)
        {
            return OperationResultDto.Fail("Student code already exists.");
        }

        if (student.ClassId is not null && !await ClassExistsAsync(student.ClassId.Value, cancellationToken))
        {
            return OperationResultDto.Fail("Selected class does not exist.");
        }

        await _unitOfWork.Repository<Student>().AddAsync(new Student
        {
            StudentCode = studentCode,
            FullName = student.FullName.Trim(),
            Gender = student.Gender.Trim(),
            DateOfBirth = student.DateOfBirth,
            Email = student.Email?.Trim(),
            Phone = student.Phone?.Trim(),
            GPA = student.GPA,
            ClassId = student.ClassId
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Student created successfully.");
    }

    public async Task<OperationResultDto> UpdateAsync(StudentDto student, CancellationToken cancellationToken = default)
    {
        if (ServiceValidation.Require(student.StudentCode, "Student code") is { } codeError)
        {
            return codeError;
        }

        if (ServiceValidation.Require(student.FullName, "Full name") is { } nameError)
        {
            return nameError;
        }

        var entity = await _unitOfWork.Repository<Student>().GetByIdAsync(student.StudentId, cancellationToken);

        if (entity is null)
        {
            return OperationResultDto.Fail("Student was not found.");
        }

        var studentCode = student.StudentCode.Trim();
        var isDuplicated = await _unitOfWork.Repository<Student>()
            .Query()
            .AnyAsync(item => item.StudentId != student.StudentId && item.StudentCode == studentCode, cancellationToken);

        if (isDuplicated)
        {
            return OperationResultDto.Fail("Student code already exists.");
        }

        if (student.ClassId is not null && !await ClassExistsAsync(student.ClassId.Value, cancellationToken))
        {
            return OperationResultDto.Fail("Selected class does not exist.");
        }

        entity.StudentCode = studentCode;
        entity.FullName = student.FullName.Trim();
        entity.Gender = student.Gender.Trim();
        entity.DateOfBirth = student.DateOfBirth;
        entity.Email = student.Email?.Trim();
        entity.Phone = student.Phone?.Trim();
        entity.GPA = student.GPA;
        entity.ClassId = student.ClassId;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Student updated successfully.");
    }

    public async Task<OperationResultDto> DeleteAsync(int studentId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Student>().GetByIdAsync(studentId, cancellationToken);

        if (entity is null)
        {
            return OperationResultDto.Fail("Student was not found.");
        }

        var hasUsers = await _unitOfWork.Repository<User>().Query().AnyAsync(user => user.StudentId == studentId, cancellationToken);
        var hasEnrollments = await _unitOfWork.Repository<Enrollment>().Query().AnyAsync(enrollment => enrollment.StudentId == studentId, cancellationToken);

        if (hasUsers || hasEnrollments)
        {
            return OperationResultDto.Fail("Cannot delete this student because related accounts or enrollments exist.");
        }

        _unitOfWork.Repository<Student>().Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Student deleted successfully.");
    }

    private Task<bool> ClassExistsAsync(int classId, CancellationToken cancellationToken)
    {
        return _unitOfWork.Repository<Class>().Query().AnyAsync(item => item.ClassId == classId, cancellationToken);
    }

    private static StudentDto ToDto(Student student)
    {
        return new StudentDto
        {
            StudentId = student.StudentId,
            StudentCode = student.StudentCode,
            FullName = student.FullName,
            Gender = student.Gender,
            DateOfBirth = student.DateOfBirth,
            Email = student.Email,
            Phone = student.Phone,
            GPA = student.GPA,
            ClassId = student.ClassId,
            ClassCode = student.Class != null ? student.Class.ClassCode : null
        };
    }
}
