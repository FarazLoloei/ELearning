using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

// Instructor Repository Implementation
public class InstructorRepository : IInstructorRepository
{
    private readonly ApplicationDbContext _context;

    public InstructorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Instructor> GetByIdAsync(Guid id)
    {
        return await _context.Instructors
            .Include(i => i.Courses)
            .SingleOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IReadOnlyList<Instructor>> ListAllAsync()
    {
        return await _context.Instructors.ToListAsync();
    }

    public async Task AddAsync(Instructor entity)
    {
        await _context.Instructors.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Instructor entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Instructor entity)
    {
        _context.Instructors.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Instructor>> GetTopInstructorsAsync(int count)
    {
        return await _context.Instructors
            .OrderByDescending(i => i.Courses.Count)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetTotalStudentsCountByInstructorIdAsync(Guid instructorId)
    {
        var coursesIds = await _context.Courses
            .Where(c => c.InstructorId == instructorId)
            .Select(c => c.Id)
            .ToListAsync();

        return await _context.Enrollments
            .Where(e => coursesIds.Contains(e.CourseId))
            .Select(e => e.StudentId)
            .Distinct()
            .CountAsync();
    }

    public async Task<decimal> GetAverageRatingByInstructorIdAsync(Guid instructorId)
    {
        var ratings = await _context.Courses
            .Where(c => c.InstructorId == instructorId)
            .Select(c => new
            {
                Rating = c.AverageRating.Value,
                Count = c.AverageRating.NumberOfRatings
            })
            .ToListAsync();

        if (!ratings.Any() || ratings.Sum(r => r.Count) == 0)
        {
            return 0;
        }

        return ratings.Sum(r => r.Rating * r.Count) / ratings.Sum(r => r.Count);
    }

    public Task<Instructor> GetInstructorWithCoursesAsync(Guid instructorId)
    {
        throw new NotImplementedException();
    }
}