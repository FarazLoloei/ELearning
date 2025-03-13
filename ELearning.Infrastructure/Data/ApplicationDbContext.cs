using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.SharedKernel;
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

        var result = await base.SaveChangesAsync(cancellationToken);

        // Dispatch domain events after saving changes
        await DispatchDomainEvents();

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    private async Task DispatchDomainEvents()
    {
        var domainEntities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            // In a real application, you would dispatch these events
            // using a mediator or an event bus
            // await _mediator.Publish(domainEvent);
        }
    }
}