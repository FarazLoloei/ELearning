// <copyright file="ApplicationDbContext.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data;

using System.Text.Json;
using ELearning.Domain.Entities.CertificateAggregate;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Events;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Events;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Infrastructure.Data.Models;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; } = null!;

    public DbSet<Module> Modules { get; set; } = null!;

    public DbSet<Lesson> Lessons { get; set; } = null!;

    public DbSet<Assignment> Assignments { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Student> Students { get; set; } = null!;

    public DbSet<Instructor> Instructors { get; set; } = null!;

    public DbSet<Enrollment> Enrollments { get; set; } = null!;

    public DbSet<Progress> Progresses { get; set; } = null!;

    public DbSet<Submission> Submissions { get; set; } = null!;

    public DbSet<Certificate> Certificates { get; set; } = null!;

    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    public DbSet<SecurityAuditEvent> SecurityAuditEvents { get; set; } = null!;

    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in this.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    break;

                case EntityState.Modified:
                    break;
            }
        }

        this.EnqueueDomainEventsToOutbox();

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    private void EnqueueDomainEventsToOutbox()
    {
        var domainEntities = this.ChangeTracker
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
            this.OutboxMessages.Add(CreateOutboxMessage(domainEvent));
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
                e.OccurredOnUTC,
            },
            CourseCompletedEvent e => new
            {
                StudentId = e.Student.Id,
                CourseId = e.Course.Id,
                EnrollmentId = e.Enrollment.Id,
                e.OccurredOnUTC,
            },
            EnrollmentCreatedEvent e => new
            {
                StudentId = e.Student.Id,
                CourseId = e.Course.Id,
                EnrollmentId = e.Enrollment.Id,
                e.OccurredOnUTC,
            },
            SubmissionGradedEvent e => new { SubmissionId = e.Submission.Id, e.OccurredOnUTC },
            _ => new { domainEvent.OccurredOnUTC },
        };

        return new OutboxMessage
        {
            OccurredOnUtc = domainEvent.OccurredOnUTC,
            Type = domainEvent.GetType().FullName ?? domainEvent.GetType().Name,
            Payload = JsonSerializer.Serialize(payload),
        };
    }
}
