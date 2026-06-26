using BUS.DTOs;
using BUS.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BUS.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var studentCount = await _unitOfWork.Repository<Student>().Query().CountAsync(cancellationToken);
        var lecturerCount = await _unitOfWork.Repository<Lecturer>().Query().CountAsync(cancellationToken);
        var classCount = await _unitOfWork.Repository<Class>().Query().CountAsync(cancellationToken);
        var subjectCount = await _unitOfWork.Repository<Subject>().Query().CountAsync(cancellationToken);
        var gpas = await _unitOfWork.Repository<Student>()
            .Query()
            .Where(student => student.GPA != null)
            .Select(student => student.GPA!.Value)
            .ToListAsync(cancellationToken);

        return new DashboardSummaryDto
        {
            TotalStudents = studentCount,
            TotalLecturers = lecturerCount,
            TotalClasses = classCount,
            TotalSubjects = subjectCount,
            AverageGPA = gpas.Count > 0 ? Math.Round(gpas.Average(), 2) : null
        };
    }
}
