using BUS.DTOs;
using BUS.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BUS.Services;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<StudentDto?> GetProfileAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Repository<Student>()
            .Query()
            .Include(student => student.Class)
            .Where(student => student.StudentId == studentId)
            .Select(student => new StudentDto
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
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<OperationResultDto> UpdateProfileAsync(StudentDto student, CancellationToken cancellationToken = default)
    {
        if (ServiceValidation.Require(student.FullName, "Full name") is { } nameError)
        {
            return nameError;
        }

        var entity = await _unitOfWork.Repository<Student>().GetByIdAsync(student.StudentId, cancellationToken);

        if (entity is null)
        {
            return OperationResultDto.Fail("Student was not found.");
        }

        entity.FullName = student.FullName.Trim();
        entity.Gender = student.Gender.Trim();
        entity.DateOfBirth = student.DateOfBirth;
        entity.Email = student.Email?.Trim();
        entity.Phone = student.Phone?.Trim();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Profile updated successfully.");
    }

    public async Task<List<SubjectDto>> GetAvailableSubjectsAsync(int studentId, string semester, CancellationToken cancellationToken = default)
    {
        var registeredSubjectIds = _unitOfWork.Repository<Enrollment>()
            .Query()
            .Where(enrollment => enrollment.StudentId == studentId && enrollment.Semester == semester)
            .Select(enrollment => enrollment.SubjectId);

        return await _unitOfWork.Repository<Subject>()
            .Query()
            .Where(subject => !registeredSubjectIds.Contains(subject.SubjectId))
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

    public Task<List<EnrollmentDto>> GetRegisteredSubjectsAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Repository<Enrollment>()
            .Query()
            .Include(enrollment => enrollment.Subject)
            .Where(enrollment => enrollment.StudentId == studentId)
            .OrderBy(enrollment => enrollment.Semester)
            .ThenBy(enrollment => enrollment.Subject!.SubjectCode)
            .Select(enrollment => new EnrollmentDto
            {
                EnrollmentId = enrollment.EnrollmentId,
                StudentId = enrollment.StudentId,
                SubjectId = enrollment.SubjectId,
                SubjectCode = enrollment.Subject != null ? enrollment.Subject.SubjectCode : string.Empty,
                SubjectName = enrollment.Subject != null ? enrollment.Subject.SubjectName : string.Empty,
                Credit = enrollment.Subject != null ? enrollment.Subject.Credit : 0,
                Semester = enrollment.Semester
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<OperationResultDto> EnrollSubjectAsync(int studentId, int subjectId, string semester, CancellationToken cancellationToken = default)
    {
        var normalizedSemester = semester.Trim();

        if (ServiceValidation.Require(normalizedSemester, "Semester") is { } semesterError)
        {
            return semesterError;
        }

        var studentExists = await _unitOfWork.Repository<Student>()
            .Query()
            .AnyAsync(student => student.StudentId == studentId, cancellationToken);

        if (!studentExists)
        {
            return OperationResultDto.Fail("Student was not found.");
        }

        var subjectExists = await _unitOfWork.Repository<Subject>()
            .Query()
            .AnyAsync(subject => subject.SubjectId == subjectId, cancellationToken);

        if (!subjectExists)
        {
            return OperationResultDto.Fail("Subject was not found.");
        }

        var isDuplicated = await _unitOfWork.Repository<Enrollment>()
            .Query()
            .AnyAsync(
                enrollment => enrollment.StudentId == studentId
                    && enrollment.SubjectId == subjectId
                    && enrollment.Semester == normalizedSemester,
                cancellationToken);

        if (isDuplicated)
        {
            return OperationResultDto.Fail("Student already registered this subject in the selected semester.");
        }

        await _unitOfWork.Repository<Enrollment>().AddAsync(new Enrollment
        {
            StudentId = studentId,
            SubjectId = subjectId,
            Semester = normalizedSemester
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Subject registered successfully.");
    }

    public async Task<OperationResultDto> CancelEnrollmentAsync(int studentId, int enrollmentId, CancellationToken cancellationToken = default)
    {
        var enrollment = await _unitOfWork.Repository<Enrollment>()
            .Query()
            .Include(item => item.Grades)
            .FirstOrDefaultAsync(item => item.EnrollmentId == enrollmentId && item.StudentId == studentId, cancellationToken);

        if (enrollment is null)
        {
            return OperationResultDto.Fail("Enrollment was not found.");
        }

        if (enrollment.Grades.Any(grade => grade.AssignmentScore is not null || grade.FinalScore is not null))
        {
            return OperationResultDto.Fail("Cannot cancel a subject after grades have been entered.");
        }

        foreach (var grade in enrollment.Grades)
        {
            _unitOfWork.Repository<Grade>().Delete(grade);
        }

        _unitOfWork.Repository<Enrollment>().Delete(enrollment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResultDto.Success("Subject registration cancelled successfully.");
    }

    public Task<List<GradeDto>> GetGradesAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Repository<Grade>()
            .Query()
            .Include(grade => grade.Enrollment)
                .ThenInclude(enrollment => enrollment!.Student)
            .Include(grade => grade.Enrollment)
                .ThenInclude(enrollment => enrollment!.Subject)
            .Where(grade => grade.Enrollment != null && grade.Enrollment.StudentId == studentId)
            .OrderBy(grade => grade.Enrollment!.Semester)
            .ThenBy(grade => grade.Enrollment!.Subject!.SubjectCode)
            .Select(grade => new GradeDto
            {
                GradeId = grade.GradeId,
                EnrollmentId = grade.EnrollmentId,
                StudentCode = grade.Enrollment != null && grade.Enrollment.Student != null ? grade.Enrollment.Student.StudentCode : string.Empty,
                StudentName = grade.Enrollment != null && grade.Enrollment.Student != null ? grade.Enrollment.Student.FullName : string.Empty,
                SubjectCode = grade.Enrollment != null && grade.Enrollment.Subject != null ? grade.Enrollment.Subject.SubjectCode : string.Empty,
                SubjectName = grade.Enrollment != null && grade.Enrollment.Subject != null ? grade.Enrollment.Subject.SubjectName : string.Empty,
                Semester = grade.Enrollment != null ? grade.Enrollment.Semester : string.Empty,
                AssignmentScore = grade.AssignmentScore,
                FinalScore = grade.FinalScore,
                GPA = grade.GPA
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal?> GetGpaAsync(int studentId, CancellationToken cancellationToken = default)
    {
        var gradeItems = await _unitOfWork.Repository<Grade>()
            .Query()
            .Include(grade => grade.Enrollment)
                .ThenInclude(enrollment => enrollment!.Subject)
            .Where(grade => grade.Enrollment != null && grade.Enrollment.StudentId == studentId && grade.GPA != null)
            .Select(grade => new
            {
                Gpa = grade.GPA!.Value,
                Credit = grade.Enrollment != null && grade.Enrollment.Subject != null ? grade.Enrollment.Subject.Credit : 0
            })
            .ToListAsync(cancellationToken);

        if (gradeItems.Count == 0)
        {
            return null;
        }

        var totalCredits = gradeItems.Sum(item => item.Credit);
        var gpa = totalCredits > 0
            ? gradeItems.Sum(item => item.Gpa * item.Credit) / totalCredits
            : gradeItems.Average(item => item.Gpa);

        return Math.Round(gpa, 2);
    }
}
