using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

// Submission Repository Implementation
public class SubmissionRepository : ISubmissionRepository
{
    private readonly ApplicationDbContext _context;

    public SubmissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Submission?> GetByIdAsync(Guid id)
    {
        return await _context.Submissions
            .SingleOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IReadOnlyList<Submission>> GetByAssignmentIdAsync(Guid assignmentId)
    {
        return await _context.Submissions
            .Where(s => s.AssignmentId == assignmentId)
            .OrderByDescending(s => s.SubmittedDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Submission>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Submissions
            .Join(_context.Enrollments,
                s => s.EnrollmentId,
                e => e.Id,
                (s, e) => new { Submission = s, Enrollment = e })
            .Where(x => x.Enrollment.StudentId == studentId)
            .Select(x => x.Submission)
            .OrderByDescending(s => s.SubmittedDate)
            .ToListAsync();
    }

    public async Task<Submission?> GetByStudentAndAssignmentIdAsync(Guid studentId, Guid assignmentId)
    {
        return await _context.Submissions
            .Join(_context.Enrollments,
                s => s.EnrollmentId,
                e => e.Id,
                (s, e) => new { Submission = s, Enrollment = e })
            .Where(x => x.Enrollment.StudentId == studentId && x.Submission.AssignmentId == assignmentId)
            .Select(x => x.Submission)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Submission>> GetUngradedSubmissionsAsync()
    {
        return await _context.Submissions
            .Where(s => !s.IsGraded)
            .OrderBy(s => s.SubmittedDate)
            .ToListAsync();
    }

    public async Task AddAsync(Submission submission)
    {
        await _context.Submissions.AddAsync(submission);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Submission submission)
    {
        _context.Entry(submission).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}
