using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Course> GetByIdAsync(Guid id)
    {
        return await _context.Courses
            .Include(c => c.Modules)
            .ThenInclude(m => m.Lessons)
            .Include(c => c.Modules)
            .ThenInclude(m => m.Assignments)
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IReadOnlyList<Course>> ListAllAsync()
    {
        return await _context.Courses.ToListAsync();
    }

    public async Task AddAsync(Course entity)
    {
        await _context.Courses.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Course entity)
    {
        _context.Courses.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Course>> GetByInstructorIdAsync(Guid instructorId)
    {
        return await _context.Courses
            .Where(c => c.InstructorId == instructorId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Course>> GetByCategoryAsync(CourseCategory category)
    {
        return await _context.Courses
            .Where(c => c.Category == category)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Course>> GetFeaturedCoursesAsync(int count)
    {
        return await _context.Courses
            .Where(c => c.IsFeatured && c.Status == CourseStatus.Published)
            .OrderByDescending(c => c.AverageRating.Value)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Course>> SearchCoursesAsync(string searchTerm, int pageNumber, int pageSize)
    {
        var query = _context.Courses.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim().ToLower();
            query = query.Where(c =>
                c.Title.ToLower().Contains(searchTerm) ||
                c.Description.ToLower().Contains(searchTerm));
        }

        query = query.OrderByDescending(c => c.CreatedAt);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Course>> GetRecentCoursesAsync(int count)
    {
        return await _context.Courses
            .Where(c => c.Status == CourseStatus.Published)
            .OrderByDescending(c => c.PublishedDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Course>> GetByLevelAsync(CourseLevel level)
    {
        return await _context.Courses
            .Where(c => c.Level == level)
            .ToListAsync();
    }

    public async Task<int> GetCoursesCountAsync()
    {
        return await _context.Courses.CountAsync();
    }
}