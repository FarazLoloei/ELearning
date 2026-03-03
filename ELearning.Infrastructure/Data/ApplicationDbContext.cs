using System.Text.Json;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Events;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Events;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Infrastructure.Data.Models;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    //private readonly ICurrentUserService _currentUserService;
    //private readonly IDateTime _dateTime;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options//,
                                                  //ICurrentUserService currentUserService,
                                                  //IDateTime dateTime
        ) : base(options)
    {
        //_currentUserService = currentUserService;
        //_dateTime = dateTime;
    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Progress> Progresses { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<SecurityAuditEvent> SecurityAuditEvents { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    //entry.Entity.CreatedAt = _dateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    //entry.Entity.UpdatedAt = _dateTime.UtcNow;
                    break;
            }
        }

        EnqueueDomainEventsToOutbox();

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    private void EnqueueDomainEventsToOutbox()
    {
        var domainEntities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        if (domainEvents.Count == 0)
        {
            return;
        }

        foreach (var domainEvent in domainEvents)
        {
            OutboxMessages.Add(CreateOutboxMessage(domainEvent));
        }

        domainEntities.ForEach(entity => entity.ClearDomainEvents());
    }

    private static OutboxMessage CreateOutboxMessage(IDomainEvent domainEvent)
    {
        object payload = domainEvent switch
        {
            CourseCreatedEvent e => new { CourseId = e.Course.Id, e.OccurredOnUTC },
            CoursePublishedEvent e => new { CourseId = e.Course.Id, e.OccurredOnUTC },
            CourseRatedEvent e => new
            {
                StudentId = e.Student.Id,
                CourseId = e.Course.Id,
                EnrollmentId = e.Enrollment.Id,
                RatingValue = e.Rating.Value,
                e.OccurredOnUTC
            },
            CourseCompletedEvent e => new
            {
                StudentId = e.Student.Id,
                CourseId = e.Course.Id,
                EnrollmentId = e.Enrollment.Id,
                e.OccurredOnUTC
            },
            EnrollmentCreatedEvent e => new
            {
                StudentId = e.Student.Id,
                CourseId = e.Course.Id,
                EnrollmentId = e.Enrollment.Id,
                e.OccurredOnUTC
            },
            SubmissionGradedEvent e => new { SubmissionId = e.Submission.Id, e.OccurredOnUTC },
            _ => new { domainEvent.OccurredOnUTC }
        };

        return new OutboxMessage
        {
            OccurredOnUtc = domainEvent.OccurredOnUTC,
            Type = domainEvent.GetType().FullName ?? domainEvent.GetType().Name,
            Payload = JsonSerializer.Serialize(payload)
        };
    }
}
