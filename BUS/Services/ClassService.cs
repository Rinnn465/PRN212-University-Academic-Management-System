using BUS.DTOs;
using BUS.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using AcademicClass = DAL.Entities.Class;

namespace BUS.Services;

public class ClassService : IClassService
{
    private readonly IUnitOfWork _unitOfWork;

    public ClassService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<ClassDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Repository<AcademicClass>()
            .Query()
            .OrderBy(classEntity => classEntity.ClassCode)
            .Select(classEntity => new ClassDto
            {
                ClassId = classEntity.ClassId,
                ClassCode = classEntity.ClassCode,
                ClassName = classEntity.ClassName
            })
            .ToListAsync(cancellationToken);
    }

    public Task<ClassDto?> GetByIdAsync(int classId, CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Repository<AcademicClass>()
            .Query()
            .Where(classEntity => classEntity.ClassId == classId)
            .Select(classEntity => new ClassDto
            {
                ClassId = classEntity.ClassId,
                ClassCode = classEntity.ClassCode,
                ClassName = classEntity.ClassName
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<OperationResultDto> CreateAsync(ClassDto classDto, CancellationToken cancellationToken = default)
    {
        if (ServiceValidation.Require(classDto.ClassCode, "Class code") is { } codeError)
        {
            return codeError;
        }

        if (ServiceValidation.Require(classDto.ClassName, "Class name") is { } nameError)
        {
            return nameError;
        }

        var classCode = classDto.ClassCode.Trim();
        var isDuplicated = await _unitOfWork.Repository<AcademicClass>()
            .Query()
            .AnyAsync(item => item.ClassCode == classCode, cancellationToken);

        if (isDuplicated)
        {
            return OperationResultDto.Fail("Class code already exists.");
        }

        await _unitOfWork.Repository<AcademicClass>().AddAsync(new AcademicClass
        {
            ClassCode = classCode,
            ClassName = classDto.ClassName.Trim()
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Class created successfully.");
    }

    public async Task<OperationResultDto> UpdateAsync(ClassDto classDto, CancellationToken cancellationToken = default)
    {
        if (ServiceValidation.Require(classDto.ClassCode, "Class code") is { } codeError)
        {
            return codeError;
        }

        if (ServiceValidation.Require(classDto.ClassName, "Class name") is { } nameError)
        {
            return nameError;
        }

        var entity = await _unitOfWork.Repository<AcademicClass>().GetByIdAsync(classDto.ClassId, cancellationToken);

        if (entity is null)
        {
            return OperationResultDto.Fail("Class was not found.");
        }

        var classCode = classDto.ClassCode.Trim();
        var isDuplicated = await _unitOfWork.Repository<AcademicClass>()
            .Query()
            .AnyAsync(item => item.ClassId != classDto.ClassId && item.ClassCode == classCode, cancellationToken);

        if (isDuplicated)
        {
            return OperationResultDto.Fail("Class code already exists.");
        }

        entity.ClassCode = classCode;
        entity.ClassName = classDto.ClassName.Trim();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Class updated successfully.");
    }

    public async Task<OperationResultDto> DeleteAsync(int classId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<AcademicClass>().GetByIdAsync(classId, cancellationToken);

        if (entity is null)
        {
            return OperationResultDto.Fail("Class was not found.");
        }

        var hasStudents = await _unitOfWork.Repository<Student>().Query().AnyAsync(student => student.ClassId == classId, cancellationToken);
        var hasAssignments = await _unitOfWork.Repository<LecturerSubject>().Query().AnyAsync(item => item.ClassId == classId, cancellationToken);

        if (hasStudents || hasAssignments)
        {
            return OperationResultDto.Fail("Cannot delete this class because students or lecturer assignments exist.");
        }

        _unitOfWork.Repository<AcademicClass>().Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Class deleted successfully.");
    }
}
