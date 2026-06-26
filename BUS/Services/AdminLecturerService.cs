using BUS.DTOs;
using BUS.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BUS.Services;

public class AdminLecturerService : IAdminLecturerService
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminLecturerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<LecturerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Repository<Lecturer>()
            .Query()
            .OrderBy(lecturer => lecturer.LecturerCode)
            .Select(lecturer => ToDto(lecturer))
            .ToListAsync(cancellationToken);
    }

    public Task<LecturerDto?> GetByIdAsync(int lecturerId, CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Repository<Lecturer>()
            .Query()
            .Where(lecturer => lecturer.LecturerId == lecturerId)
            .Select(lecturer => ToDto(lecturer))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<OperationResultDto> CreateAsync(LecturerDto lecturer, CancellationToken cancellationToken = default)
    {
        if (ServiceValidation.Require(lecturer.LecturerCode, "Lecturer code") is { } codeError)
        {
            return codeError;
        }

        if (ServiceValidation.Require(lecturer.FullName, "Full name") is { } nameError)
        {
            return nameError;
        }

        var lecturerCode = lecturer.LecturerCode.Trim();
        var isDuplicated = await _unitOfWork.Repository<Lecturer>()
            .Query()
            .AnyAsync(item => item.LecturerCode == lecturerCode, cancellationToken);

        if (isDuplicated)
        {
            return OperationResultDto.Fail("Lecturer code already exists.");
        }

        await _unitOfWork.Repository<Lecturer>().AddAsync(new Lecturer
        {
            LecturerCode = lecturerCode,
            FullName = lecturer.FullName.Trim(),
            Email = lecturer.Email?.Trim(),
            Phone = lecturer.Phone?.Trim()
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Lecturer created successfully.");
    }

    public async Task<OperationResultDto> UpdateAsync(LecturerDto lecturer, CancellationToken cancellationToken = default)
    {
        if (ServiceValidation.Require(lecturer.LecturerCode, "Lecturer code") is { } codeError)
        {
            return codeError;
        }

        if (ServiceValidation.Require(lecturer.FullName, "Full name") is { } nameError)
        {
            return nameError;
        }

        var entity = await _unitOfWork.Repository<Lecturer>().GetByIdAsync(lecturer.LecturerId, cancellationToken);

        if (entity is null)
        {
            return OperationResultDto.Fail("Lecturer was not found.");
        }

        var lecturerCode = lecturer.LecturerCode.Trim();
        var isDuplicated = await _unitOfWork.Repository<Lecturer>()
            .Query()
            .AnyAsync(item => item.LecturerId != lecturer.LecturerId && item.LecturerCode == lecturerCode, cancellationToken);

        if (isDuplicated)
        {
            return OperationResultDto.Fail("Lecturer code already exists.");
        }

        entity.LecturerCode = lecturerCode;
        entity.FullName = lecturer.FullName.Trim();
        entity.Email = lecturer.Email?.Trim();
        entity.Phone = lecturer.Phone?.Trim();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Lecturer updated successfully.");
    }

    public async Task<OperationResultDto> DeleteAsync(int lecturerId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Lecturer>().GetByIdAsync(lecturerId, cancellationToken);

        if (entity is null)
        {
            return OperationResultDto.Fail("Lecturer was not found.");
        }

        var hasUsers = await _unitOfWork.Repository<User>().Query().AnyAsync(user => user.LecturerId == lecturerId, cancellationToken);
        var hasAssignments = await _unitOfWork.Repository<LecturerSubject>().Query().AnyAsync(item => item.LecturerId == lecturerId, cancellationToken);

        if (hasUsers || hasAssignments)
        {
            return OperationResultDto.Fail("Cannot delete this lecturer because related accounts or assignments exist.");
        }

        _unitOfWork.Repository<Lecturer>().Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Lecturer deleted successfully.");
    }

    private static LecturerDto ToDto(Lecturer lecturer)
    {
        return new LecturerDto
        {
            LecturerId = lecturer.LecturerId,
            LecturerCode = lecturer.LecturerCode,
            FullName = lecturer.FullName,
            Email = lecturer.Email,
            Phone = lecturer.Phone
        };
    }
}
