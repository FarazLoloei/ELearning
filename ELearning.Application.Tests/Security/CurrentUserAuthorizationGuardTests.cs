using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Security;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel.Models;
using FluentAssertions;

namespace ELearning.Application.Tests.Security;

public sealed class CurrentUserAuthorizationGuardTests
{
    [Fact]
    public void EnsureAuthenticated_WhenUserIsNotAuthenticated_ThrowsForbiddenAccessException()
    {
        var currentUser = new FakeCurrentUserService(null, isAuthenticated: false);

        var action = () => CurrentUserAuthorizationGuard.EnsureAuthenticated(currentUser);

        action.Should().Throw<ForbiddenAccessException>();
    }

    [Fact]
    public void EnsureStudentSelfOrAdmin_WhenCurrentUserIsRequestedStudent_DoesNotThrow()
    {
        var studentId = Guid.NewGuid();
        var currentUser = new FakeCurrentUserService(studentId, isAuthenticated: true);

        var action = () => CurrentUserAuthorizationGuard.EnsureStudentSelfOrAdmin(currentUser, studentId);

        action.Should().NotThrow();
    }

    [Fact]
    public void EnsureStudentSelfOrAdmin_WhenCurrentUserIsAdmin_DoesNotThrow()
    {
        var currentUser = new FakeCurrentUserService(Guid.NewGuid(), isAuthenticated: true, roles: ["Admin"]);

        var action = () => CurrentUserAuthorizationGuard.EnsureStudentSelfOrAdmin(currentUser, Guid.NewGuid());

        action.Should().NotThrow();
    }

    [Fact]
    public void EnsureStudentSelfOrAdmin_WhenCurrentUserIsAnotherStudent_ThrowsForbiddenAccessException()
    {
        var currentUser = new FakeCurrentUserService(Guid.NewGuid(), isAuthenticated: true);

        var action = () => CurrentUserAuthorizationGuard.EnsureStudentSelfOrAdmin(currentUser, Guid.NewGuid());

        action.Should().Throw<ForbiddenAccessException>();
    }

    [Fact]
    public async Task EnsureEnrollmentReadAccessAsync_WhenCurrentUserIsEnrollmentOwner_DoesNotThrow()
    {
        var studentId = Guid.NewGuid();
        var currentUser = new FakeCurrentUserService(studentId, isAuthenticated: true);
        var repository = new FakeCourseRepository(course: null);

        var action = async () => await CurrentUserAuthorizationGuard.EnsureEnrollmentReadAccessAsync(
            currentUser,
            studentId,
            Guid.NewGuid(),
            repository,
            CancellationToken.None);

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureEnrollmentReadAccessAsync_WhenCurrentUserIsAdmin_DoesNotThrow()
    {
        var currentUser = new FakeCurrentUserService(Guid.NewGuid(), isAuthenticated: true, roles: ["Admin"]);
        var repository = new FakeCourseRepository(course: null);

        var action = async () => await CurrentUserAuthorizationGuard.EnsureEnrollmentReadAccessAsync(
            currentUser,
            Guid.NewGuid(),
            Guid.NewGuid(),
            repository,
            CancellationToken.None);

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureEnrollmentReadAccessAsync_WhenCurrentUserIsCourseInstructor_DoesNotThrow()
    {
        var instructorId = Guid.NewGuid();
        var currentUser = new FakeCurrentUserService(instructorId, isAuthenticated: true);
        var course = CreateCourse(instructorId);
        var repository = new FakeCourseRepository(course);

        var action = async () => await CurrentUserAuthorizationGuard.EnsureEnrollmentReadAccessAsync(
            currentUser,
            Guid.NewGuid(),
            course.Id,
            repository,
            CancellationToken.None);

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureEnrollmentReadAccessAsync_WhenCurrentUserIsNotOwnerAdminOrInstructor_ThrowsForbiddenAccessException()
    {
        var currentUser = new FakeCurrentUserService(Guid.NewGuid(), isAuthenticated: true);
        var course = CreateCourse(Guid.NewGuid());
        var repository = new FakeCourseRepository(course);

        var action = async () => await CurrentUserAuthorizationGuard.EnsureEnrollmentReadAccessAsync(
            currentUser,
            Guid.NewGuid(),
            course.Id,
            repository,
            CancellationToken.None);

        await action.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task EnsureEnrollmentReadAccessAsync_WhenCourseDoesNotExist_ThrowsNotFoundException()
    {
        var currentUser = new FakeCurrentUserService(Guid.NewGuid(), isAuthenticated: true);
        var repository = new FakeCourseRepository(course: null);

        var action = async () => await CurrentUserAuthorizationGuard.EnsureEnrollmentReadAccessAsync(
            currentUser,
            Guid.NewGuid(),
            Guid.NewGuid(),
            repository,
            CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    private static Course CreateCourse(Guid instructorId)
    {
        return new Course(
            "Security Test Course",
            "Course used for guard tests.",
            instructorId,
            CourseCategory.Programming,
            CourseLevel.Beginner,
            Duration.Create(1, 0),
            0m);
    }

    private sealed class FakeCurrentUserService(Guid? userId, bool isAuthenticated, IEnumerable<string>? roles = null)
        : ICurrentUserService
    {
        private readonly HashSet<string> _roles = new(roles ?? [], StringComparer.OrdinalIgnoreCase);

        public Guid? UserId { get; } = userId;

        public string? UserEmail => "security-tests@local";

        public bool IsAuthenticated { get; } = isAuthenticated;

        public bool IsInRole(string role) => _roles.Contains(role);
    }

    private sealed class FakeCourseRepository(Course? course) : ICourseRepository
    {
        public Task<Course?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (course is null || course.Id != id)
                return Task.FromResult<Course?>(null);

            return Task.FromResult<Course?>(course);
        }

        public Task AddAsync(Course entity, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task UpdateAsync(Course entity, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task DeleteAsync(Course entity, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<Course>> GetByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<Course>> GetByCategoryAsync(CourseCategory category, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<Course>> GetFeaturedCoursesAsync(int count, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<Course>> SearchCoursesAsync(string? searchTerm, PaginationParameters pagination, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<Course>> GetRecentCoursesAsync(int count, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<Course>> GetByLevelAsync(CourseLevel level, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<int> GetCoursesCountAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}
