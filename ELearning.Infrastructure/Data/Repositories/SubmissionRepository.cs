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

    public async Task<Submission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Submissions
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Submission>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Submissions
            .Where(s => s.AssignmentId == assignmentId)
            .OrderByDescending(s => s.SubmittedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Submission>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _context.Submissions
            .Join(_context.Enrollments,
                s => s.EnrollmentId,
                e => e.Id,
                (s, e) => new { Submission = s, Enrollment = e })
            .Where(x => x.Enrollment.StudentId == studentId)
            .Select(x => x.Submission)
            .OrderByDescending(s => s.SubmittedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Submission?> GetByStudentAndAssignmentIdAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Submissions
            .Join(_context.Enrollments,
                s => s.EnrollmentId,
                e => e.Id,
                (s, e) => new { Submission = s, Enrollment = e })
            .Where(x => x.Enrollment.StudentId == studentId && x.Submission.AssignmentId == assignmentId)
            .Select(x => x.Submission)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Submission>> GetUngradedSubmissionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Submissions
            .Where(s => !s.IsGraded)
            .OrderBy(s => s.SubmittedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        await _context.Submissions.AddAsync(submission, cancellationToken);
    }

    public Task UpdateAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        _context.Entry(submission).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}
