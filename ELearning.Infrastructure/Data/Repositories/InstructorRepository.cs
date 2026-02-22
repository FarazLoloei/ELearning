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

    public async Task<Instructor?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Instructors
            .Include(i => i.Courses)
            .SingleOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Instructor>> ListAllAsync(CancellationToken cancellationToken) => await _context.Instructors.ToListAsync(cancellationToken);

    public async Task AddAsync(Instructor entity, CancellationToken cancellationToken)
    {
        await _context.Instructors.AddAsync(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Instructor entity, CancellationToken cancellationToken)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Instructor entity, CancellationToken cancellationToken)
    {
        _context.Instructors.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Instructor>> GetTopInstructorsAsync(int count, CancellationToken cancellationToken) =>
        await _context.Instructors
            .AsNoTracking()
            .OrderByDescending(i => i.Courses.Count)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task<int> GetTotalStudentCountAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        var coursesIds = await _context.Courses
            .AsNoTracking()
            .Where(c => c.InstructorId == instructorId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        return await _context.Enrollments
            .AsNoTracking()
            .Where(e => coursesIds.Contains(e.CourseId))
            .Select(e => e.StudentId)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    public async Task<decimal> GetAverageRatingAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        var ratings = await _context.Courses
            .AsNoTracking()
            .Where(c => c.InstructorId == instructorId)
            .Select(c => new
            {
                Rating = c.AverageRating.Value,
                Count = c.AverageRating.NumberOfRatings
            })
            .ToListAsync(cancellationToken);

        if (!ratings.Any() || ratings.Sum(r => r.Count) == 0)
            return 0;

        return ratings.Sum(r => r.Rating * r.Count) / ratings.Sum(r => r.Count);
    }

    public async Task<Instructor?> GetInstructorWithCoursesAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        return await _context.Instructors
            .Include(i => i.Courses)
            .SingleOrDefaultAsync(i => i.Id == instructorId, cancellationToken);
    }
}
