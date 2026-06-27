using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BUS.Services
{
    public class StudentService
    {
        // KHÔNG tự tạo Context mới ở đây nữa
        private readonly UnitOfWork _unitOfWork;

        // Nhận UnitOfWork hợp lệ từ bên ngoài truyền vào thông qua Constructor
        public StudentService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Student?> GetStudentById(int studentId)
        {
            return await _unitOfWork.Repository<Student>().GetByIdAsync(studentId);
        }

        public async Task<List<Subject>> GetAvailableSubjects()
        {
            return await _unitOfWork.Repository<Subject>().GetAllAsync();
        }

        public async Task<string> RegisterSubject(int studentId, int subjectId)
        {
            var existingEnrollment = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.StudentId == studentId && e.SubjectId == subjectId);

            if (existingEnrollment.Any())
            {
                return "Bạn đã đăng ký môn học này rồi!";
            }

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                SubjectId = subjectId,
                Semester = "Spring 2026"
            };

            await _unitOfWork.Repository<Enrollment>().AddAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return "Đăng ký thành công!";
        }

        public async Task<List<Enrollment>> GetStudentGrades(int studentId)
        {
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .Query()
                .Include(e => e.Subject)
                .Include(e => e.Grades)
                .Where(e => e.StudentId == studentId)
                .ToListAsync();

            return enrollments;
        }
    }
}