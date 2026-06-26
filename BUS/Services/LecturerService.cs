using BUS.DTOs;
using BUS.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BUS.Services;

public class LecturerService : ILecturerService
{
    private readonly IUnitOfWork _unitOfWork;

    public LecturerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<LecturerAssignmentDto>> GetAssignmentsAsync(int lecturerId, string? semester = null, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<LecturerSubject>()
            .Query()
            .Include(item => item.Subject)
            .Include(item => item.Class)
            .Where(item => item.LecturerId == lecturerId);

        if (!string.IsNullOrWhiteSpace(semester))
        {
            var normalizedSemester = semester.Trim();
            query = query.Where(item => item.Semester == normalizedSemester);
        }

        return query
            .OrderBy(item => item.Semester)
            .ThenBy(item => item.Subject!.SubjectCode)
            .Select(item => new LecturerAssignmentDto
            {
                LecturerSubjectId = item.LecturerSubjectId,
                LecturerId = item.LecturerId,
                SubjectId = item.SubjectId,
                SubjectCode = item.Subject != null ? item.Subject.SubjectCode : string.Empty,
                SubjectName = item.Subject != null ? item.Subject.SubjectName : string.Empty,
                ClassId = item.ClassId,
                ClassCode = item.Class != null ? item.Class.ClassCode : null,
                Semester = item.Semester
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<GradeDto>> GetStudentGradesAsync(int lecturerSubjectId, CancellationToken cancellationToken = default)
    {
        var assignment = await _unitOfWork.Repository<LecturerSubject>()
            .Query()
            .FirstOrDefaultAsync(item => item.LecturerSubjectId == lecturerSubjectId, cancellationToken);

        if (assignment is null)
        {
            return [];
        }

        var query = _unitOfWork.Repository<Enrollment>()
            .Query()
            .Include(enrollment => enrollment.Student)
            .Include(enrollment => enrollment.Subject)
            .Include(enrollment => enrollment.Grades)
            .Where(enrollment => enrollment.SubjectId == assignment.SubjectId && enrollment.Semester == assignment.Semester);

        if (assignment.ClassId is not null)
        {
            query = query.Where(enrollment => enrollment.Student != null && enrollment.Student.ClassId == assignment.ClassId);
        }

        return await query
            .OrderBy(enrollment => enrollment.Student!.StudentCode)
            .Select(enrollment => new GradeDto
            {
                GradeId = enrollment.Grades.Select(grade => grade.GradeId).FirstOrDefault(),
                EnrollmentId = enrollment.EnrollmentId,
                StudentCode = enrollment.Student != null ? enrollment.Student.StudentCode : string.Empty,
                StudentName = enrollment.Student != null ? enrollment.Student.FullName : string.Empty,
                SubjectCode = enrollment.Subject != null ? enrollment.Subject.SubjectCode : string.Empty,
                SubjectName = enrollment.Subject != null ? enrollment.Subject.SubjectName : string.Empty,
                Semester = enrollment.Semester,
                AssignmentScore = enrollment.Grades.Select(grade => grade.AssignmentScore).FirstOrDefault(),
                FinalScore = enrollment.Grades.Select(grade => grade.FinalScore).FirstOrDefault(),
                GPA = enrollment.Grades.Select(grade => grade.GPA).FirstOrDefault()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<OperationResultDto> UpdateGradeAsync(GradeUpdateDto grade, CancellationToken cancellationToken = default)
    {
        if (ServiceValidation.RequireScore(grade.AssignmentScore, "Assignment score") is { } assignmentError)
        {
            return assignmentError;
        }

        if (ServiceValidation.RequireScore(grade.FinalScore, "Final score") is { } finalError)
        {
            return finalError;
        }

        var enrollmentExists = await _unitOfWork.Repository<Enrollment>()
            .Query()
            .AnyAsync(enrollment => enrollment.EnrollmentId == grade.EnrollmentId, cancellationToken);

        if (!enrollmentExists)
        {
            return OperationResultDto.Fail("Enrollment was not found.");
        }

        var gradeEntity = await _unitOfWork.Repository<Grade>()
            .Query()
            .FirstOrDefaultAsync(item => item.EnrollmentId == grade.EnrollmentId, cancellationToken);

        if (gradeEntity is null)
        {
            gradeEntity = new Grade
            {
                EnrollmentId = grade.EnrollmentId
            };

            await _unitOfWork.Repository<Grade>().AddAsync(gradeEntity, cancellationToken);
        }

        gradeEntity.AssignmentScore = grade.AssignmentScore;
        gradeEntity.FinalScore = grade.FinalScore;
        gradeEntity.GPA = ServiceValidation.CalculateGpa(grade.AssignmentScore, grade.FinalScore);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await RefreshStudentGpaAsync(grade.EnrollmentId, cancellationToken);

        return OperationResultDto.Success("Grade saved successfully.");
    }

    public async Task<ClassStatisticsDto> GetClassStatisticsAsync(int lecturerSubjectId, CancellationToken cancellationToken = default)
    {
        var grades = await GetStudentGradesAsync(lecturerSubjectId, cancellationToken);
        var completedGrades = grades
            .Where(grade => grade.AssignmentScore is not null && grade.FinalScore is not null)
            .Select(grade => new
            {
                AverageScore = (grade.AssignmentScore!.Value * 0.4m) + (grade.FinalScore!.Value * 0.6m),
                grade.GPA
            })
            .ToList();

        var passedCount = completedGrades.Count(item => item.AverageScore >= 5m);
        var failedCount = completedGrades.Count - passedCount;

        return new ClassStatisticsDto
        {
            StudentCount = grades.Count,
            PassedCount = passedCount,
            FailedCount = failedCount,
            PassRate = completedGrades.Count > 0 ? Math.Round(passedCount * 100m / completedGrades.Count, 2) : 0,
            AverageScore = completedGrades.Count > 0 ? Math.Round(completedGrades.Average(item => item.AverageScore), 2) : null,
            AverageGPA = completedGrades.Where(item => item.GPA is not null).Any()
                ? Math.Round(completedGrades.Where(item => item.GPA is not null).Average(item => item.GPA!.Value), 2)
                : null
        };
    }

    private async Task RefreshStudentGpaAsync(int enrollmentId, CancellationToken cancellationToken)
    {
        var enrollment = await _unitOfWork.Repository<Enrollment>()
            .Query()
            .FirstOrDefaultAsync(item => item.EnrollmentId == enrollmentId, cancellationToken);

        if (enrollment is null)
        {
            return;
        }

        var gradeItems = await _unitOfWork.Repository<Grade>()
            .Query()
            .Include(grade => grade.Enrollment)
                .ThenInclude(item => item!.Subject)
            .Where(grade => grade.Enrollment != null && grade.Enrollment.StudentId == enrollment.StudentId && grade.GPA != null)
            .Select(grade => new
            {
                Gpa = grade.GPA!.Value,
                Credit = grade.Enrollment != null && grade.Enrollment.Subject != null ? grade.Enrollment.Subject.Credit : 0
            })
            .ToListAsync(cancellationToken);

        var student = await _unitOfWork.Repository<Student>().GetByIdAsync(enrollment.StudentId, cancellationToken);

        if (student is null)
        {
            return;
        }

        if (gradeItems.Count == 0)
        {
            student.GPA = null;
        }
        else
        {
            var totalCredits = gradeItems.Sum(item => item.Credit);
            student.GPA = totalCredits > 0
                ? Math.Round(gradeItems.Sum(item => item.Gpa * item.Credit) / totalCredits, 2)
                : Math.Round(gradeItems.Average(item => item.Gpa), 2);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
