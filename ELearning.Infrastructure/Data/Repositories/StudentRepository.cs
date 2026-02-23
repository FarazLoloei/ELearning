using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await _context.Students
            .AsNoTracking()
            .Include(s => s.Enrollments)
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Student>> ListAllAsync(CancellationToken cancellationToken) =>
        await _context.Students
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    public async Task AddAsync(Student entity, CancellationToken cancellationToken)
    {
        await _context.Students.AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(Student entity, CancellationToken cancellationToken)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Student entity, CancellationToken cancellationToken)
    {
        _context.Students.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Student>> GetStudentsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken)
    => await _context.Students
        .AsNoTracking()
        .Include(s => s.Enrollments)
        .Where(s => s.Enrollments.Any(e => e.CourseId == courseId))
        .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Course>> GetCoursesByStudentIdAsync(Guid studentId, CancellationToken cancellationToken)
    {
        var enrollments = await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == studentId)
            .Select(e => e.CourseId)
            .ToListAsync(cancellationToken);

        return await _context.Courses
            .Where(c => enrollments.Contains(c.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetEnrolledStudentCountAsync(Guid courseId, CancellationToken cancellationToken) => await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.CourseId == courseId)
            .Select(e => e.StudentId)
            .Distinct()
            .CountAsync(cancellationToken);
}
