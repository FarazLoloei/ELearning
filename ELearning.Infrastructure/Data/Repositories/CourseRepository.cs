using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Courses
            .Include(c => c.Modules)
            .ThenInclude(m => m.Lessons)
            .Include(c => c.Modules)
            .ThenInclude(m => m.Assignments)
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> ListAllAsync(CancellationToken cancellationToken) => await _context.Courses
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    public async Task AddAsync(Course entity, CancellationToken cancellationToken)
    {
        await _context.Courses.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Course entity, CancellationToken cancellationToken)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Course entity, CancellationToken cancellationToken)
    {
        _context.Courses.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken) =>
        await _context.Courses
            .AsNoTracking()
            .Where(c => c.InstructorId == instructorId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Course>> GetByCategoryAsync(CourseCategory category, CancellationToken cancellationToken) =>
        await _context.Courses
            .AsNoTracking()
            .Where(c => c.Category == category)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Course>> GetFeaturedCoursesAsync(int count, CancellationToken cancellationToken) =>
        await _context.Courses
            .AsNoTracking()
            .Where(c => c.IsFeatured && c.Status == CourseStatus.Published)
            .OrderByDescending(c => c.AverageRating.Value)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Course>> SearchCoursesAsync(string searchTerm, PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var query = _context.Courses
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim().ToLower();
            query = query.Where(c =>
                c.Title.ToLower().Contains(searchTerm) ||
                c.Description.ToLower().Contains(searchTerm));
        }

        return await query
            .OrderByDescending(c => c.CreatedAt())
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetRecentCoursesAsync(int count, CancellationToken cancellationToken) =>
        await _context.Courses
            .AsNoTracking()
            .Where(c => c.Status == CourseStatus.Published)
            .OrderByDescending(c => c.PublishedDate)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Course>> GetByLevelAsync(CourseLevel level, CancellationToken cancellationToken) =>
        await _context.Courses
            .AsNoTracking()
            .Where(c => c.Level == level)
            .ToListAsync(cancellationToken);

    public async Task<int> GetCoursesCountAsync(CancellationToken cancellationToken) =>
        await _context.Courses
            .AsNoTracking()
            .CountAsync(cancellationToken);
}