using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

// Student Repository Implementation
public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student> GetByIdAsync(Guid id)
    {
        return await _context.Students
            .Include(s => s.Enrollments)
            .SingleOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IReadOnlyList<Student>> ListAllAsync()
    {
        return await _context.Students.ToListAsync();
    }

    public async Task AddAsync(Student entity)
    {
        await _context.Students.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Student entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Student entity)
    {
        _context.Students.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Student>> GetByEnrolledCourseIdAsync(Guid courseId)
    {
        return await _context.Students
            .Include(s => s.Enrollments)
            .Where(s => s.Enrollments.Any(e => e.CourseId == courseId))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Course>> GetEnrolledCoursesAsync(Guid studentId)
    {
        var enrollments = await _context.Enrollments
            .Where(e => e.StudentId == studentId)
            .Select(e => e.CourseId)
            .ToListAsync();

        return await _context.Courses
            .Where(c => enrollments.Contains(c.Id))
            .ToListAsync();
    }

    public async Task<int> GetEnrolledStudentsCountByCourseIdAsync(Guid courseId)
    {
        return await _context.Enrollments
            .Where(e => e.CourseId == courseId)
            .Select(e => e.StudentId)
            .Distinct()
            .CountAsync();
    }
}