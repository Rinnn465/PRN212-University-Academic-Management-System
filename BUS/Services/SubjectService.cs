using BUS.DTOs;
using BUS.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BUS.Services;

public class SubjectService : ISubjectService
{
    private readonly IUnitOfWork _unitOfWork;

    public SubjectService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<SubjectDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Repository<Subject>()
            .Query()
            .OrderBy(subject => subject.SubjectCode)
            .Select(subject => new SubjectDto
            {
                SubjectId = subject.SubjectId,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Credit = subject.Credit
            })
            .ToListAsync(cancellationToken);
    }

    public Task<SubjectDto?> GetByIdAsync(int subjectId, CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Repository<Subject>()
            .Query()
            .Where(subject => subject.SubjectId == subjectId)
            .Select(subject => new SubjectDto
            {
                SubjectId = subject.SubjectId,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Credit = subject.Credit
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<OperationResultDto> CreateAsync(SubjectDto subject, CancellationToken cancellationToken = default)
    {
        if (ServiceValidation.Require(subject.SubjectCode, "Subject code") is { } codeError)
        {
            return codeError;
        }

        if (ServiceValidation.Require(subject.SubjectName, "Subject name") is { } nameError)
        {
            return nameError;
        }

        if (subject.Credit <= 0)
        {
            return OperationResultDto.Fail("Credit must be greater than 0.");
        }

        var subjectCode = subject.SubjectCode.Trim();
        var isDuplicated = await _unitOfWork.Repository<Subject>()
            .Query()
            .AnyAsync(item => item.SubjectCode == subjectCode, cancellationToken);

        if (isDuplicated)
        {
            return OperationResultDto.Fail("Subject code already exists.");
        }

        await _unitOfWork.Repository<Subject>().AddAsync(new Subject
        {
            SubjectCode = subjectCode,
            SubjectName = subject.SubjectName.Trim(),
            Credit = subject.Credit
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Subject created successfully.");
    }

    public async Task<OperationResultDto> UpdateAsync(SubjectDto subject, CancellationToken cancellationToken = default)
    {
        if (ServiceValidation.Require(subject.SubjectCode, "Subject code") is { } codeError)
        {
            return codeError;
        }

        if (ServiceValidation.Require(subject.SubjectName, "Subject name") is { } nameError)
        {
            return nameError;
        }

        if (subject.Credit <= 0)
        {
            return OperationResultDto.Fail("Credit must be greater than 0.");
        }

        var entity = await _unitOfWork.Repository<Subject>().GetByIdAsync(subject.SubjectId, cancellationToken);

        if (entity is null)
        {
            return OperationResultDto.Fail("Subject was not found.");
        }

        var subjectCode = subject.SubjectCode.Trim();
        var isDuplicated = await _unitOfWork.Repository<Subject>()
            .Query()
            .AnyAsync(item => item.SubjectId != subject.SubjectId && item.SubjectCode == subjectCode, cancellationToken);

        if (isDuplicated)
        {
            return OperationResultDto.Fail("Subject code already exists.");
        }

        entity.SubjectCode = subjectCode;
        entity.SubjectName = subject.SubjectName.Trim();
        entity.Credit = subject.Credit;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Subject updated successfully.");
    }

    public async Task<OperationResultDto> DeleteAsync(int subjectId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Subject>().GetByIdAsync(subjectId, cancellationToken);

        if (entity is null)
        {
            return OperationResultDto.Fail("Subject was not found.");
        }

        var hasEnrollments = await _unitOfWork.Repository<Enrollment>().Query().AnyAsync(enrollment => enrollment.SubjectId == subjectId, cancellationToken);
        var hasAssignments = await _unitOfWork.Repository<LecturerSubject>().Query().AnyAsync(item => item.SubjectId == subjectId, cancellationToken);

        if (hasEnrollments || hasAssignments)
        {
            return OperationResultDto.Fail("Cannot delete this subject because enrollments or lecturer assignments exist.");
        }

        _unitOfWork.Repository<Subject>().Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Subject deleted successfully.");
    }
}
