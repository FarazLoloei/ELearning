using AutoMapper;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Courses.Handlers;
using ELearning.Application.Courses.Queries;
using ELearning.Application.Enrollments.Abstractions.ReadModels;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Application.Instructors.Dtos;
using ELearning.Application.Students.Abstractions.ReadModels;
using ELearning.Application.Students.Dtos;
using ELearning.Application.Students.Handlers;
using ELearning.Application.Students.Queries;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace ELearning.Application.Tests.Handlers;

public sealed class ReadModelFallbackHandlerTests
{
    [Fact]
    public async Task GetStudentProgressQueryHandler_WhenReadServiceFails_UsesEnrollmentRepositoryFallback()
    {
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var enrollmentActive = new Enrollment(studentId, courseId, EnrollmentStatus.Active);
        var enrollmentCompleted = new Enrollment(studentId, courseId, EnrollmentStatus.Completed);

        var course = BuildCourseWithOneLessonAndAssignment(courseId, Guid.NewGuid());
        var student = new Student("John", "Doe", Email.Create("john.doe@example.com"), "hash");
        SetEntityId(student, studentId);

        var handler = new GetStudentProgressQueryHandler(
            studentReadService: new ThrowingStudentReadService(),
            studentRepository: new FakeStudentRepository(student),
            enrollmentRepository: new FakeEnrollmentRepository([enrollmentActive, enrollmentCompleted]),
            progressRepository: new FakeProgressReadRepository(
                percentagesByEnrollmentId: new Dictionary<Guid, double>
                {
                    [enrollmentActive.Id] = 50,
                    [enrollmentCompleted.Id] = 100,
                }),
            courseRepository: new FakeCourseRepository(course),
            currentUserService: new FakeCurrentUserService(studentId, isAuthenticated: true));

        var result = await handler.Handle(new GetStudentProgressQuery { StudentId = studentId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.CompletedCourses.Should().Be(1);
        result.Value.InProgressCourses.Should().Be(1);
        result.Value.Enrollments.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetStudentEnrollmentsQueryHandler_WhenReadServiceFails_AppliesDeterministicPaginationOverFallbackData()
    {
        var studentId = Guid.NewGuid();
        var courseA = BuildCourseWithOneLessonAndAssignment(Guid.NewGuid(), Guid.NewGuid(), title: "Course A");
        var courseB = BuildCourseWithOneLessonAndAssignment(Guid.NewGuid(), Guid.NewGuid(), title: "Course B");
        var courseC = BuildCourseWithOneLessonAndAssignment(Guid.NewGuid(), Guid.NewGuid(), title: "Course C");

        var e1 = new Enrollment(studentId, courseA.Id, EnrollmentStatus.Active);
        var e2 = new Enrollment(studentId, courseB.Id, EnrollmentStatus.Active);
        var e3 = new Enrollment(studentId, courseC.Id, EnrollmentStatus.Active);

        // Ensure sort order is predictable in test.
        SetCreatedAt(e1, new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc));
        SetCreatedAt(e2, new DateTime(2024, 01, 03, 0, 0, 0, DateTimeKind.Utc));
        SetCreatedAt(e3, new DateTime(2024, 01, 02, 0, 0, 0, DateTimeKind.Utc));

        var courseRepository = new FakeCourseRepository(courseA, courseB, courseC);

        var handler = new GetStudentEnrollmentsQueryHandler(
            enrollmentReadService: new ThrowingEnrollmentReadService(),
            enrollmentRepository: new FakeEnrollmentRepository([e1, e2, e3]),
            courseRepository: courseRepository,
            studentReadService: new FakeStudentReadService("John Doe"),
            progressRepository: new FakeProgressReadRepository(),
            currentUserService: new FakeCurrentUserService(studentId, isAuthenticated: true));

        var result = await handler.Handle(
            new GetStudentEnrollmentsQuery
            {
                StudentId = studentId,
                PageNumber = 2,
                PageSize = 1,
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(3);
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items[0].Id.Should().Be(e3.Id); // e2 (newest) on page 1, e3 on page 2.
    }

    [Fact]
    public async Task GetInstructorCoursesQueryHandler_WhenFallbackIsUsed_LoadsEnrollmentCountsViaReadService()
    {
        var instructorId = Guid.NewGuid();
        var instructor = new Instructor("Jane", "Doe", Email.Create("jane.doe@example.com"), "hash", "bio", "expertise");
        SetEntityId(instructor, instructorId);

        var firstCourse = BuildCourseWithOneLessonAndAssignment(Guid.NewGuid(), instructorId, title: "Course 1");
        var secondCourse = BuildCourseWithOneLessonAndAssignment(Guid.NewGuid(), instructorId, title: "Course 2");
        instructor.AddCourse(firstCourse);
        instructor.AddCourse(secondCourse);

        var mapper = BuildInstructorMapper();
        var enrollmentReadService = new FakeEnrollmentReadService(
            courseEnrollmentCounts: new Dictionary<Guid, int>
            {
                [firstCourse.Id] = 7,
                [secondCourse.Id] = 3,
            });

        var handler = new GetInstructorCoursesQueryHandler(
            instructorRepository: new ThrowingThenFallbackInstructorRepository(instructor),
            enrollmentReadService: enrollmentReadService,
            mapper: mapper);

        var result = await handler.Handle(new GetInstructorCoursesQuery { InstructorId = instructorId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Courses.Should().HaveCount(2);
        result.Value.Courses.Should().Contain(c => c.Id == firstCourse.Id && c.EnrollmentsCount == 7);
        result.Value.Courses.Should().Contain(c => c.Id == secondCourse.Id && c.EnrollmentsCount == 3);
    }

    private static IMapper BuildInstructorMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Instructor, InstructorCoursesDto>()
                .ConstructUsing(src => new InstructorCoursesDto(
                    src.Id,
                    src.FullName,
                    src.Email.Value,
                    src.Bio,
                    src.Expertise,
                    src.ProfilePictureUrl ?? string.Empty,
                    AverageRating: 0m,
                    TotalStudents: 0,
                    TotalCourses: src.Courses.Count,
                    Courses: new List<InstructorCourseDto>()));

            cfg.CreateMap<Course, InstructorCourseDto>()
                .ConstructUsing(src => new InstructorCourseDto(
                    src.Id,
                    src.Title,
                    src.Category.Name,
                    EnrollmentsCount: src.Enrollments.Count,
                    src.Status.Name,
                    src.CreatedAt(),
                    src.PublishedDate));
        },
            NullLoggerFactory.Instance);

        return config.CreateMapper();
    }

    private static Course BuildCourseWithOneLessonAndAssignment(Guid courseId, Guid instructorId, string title = "Course")
    {
        var course = new Course(
            title,
            "desc",
            instructorId,
            CourseCategory.Programming,
            CourseLevel.Beginner,
            Duration.FromMinutes(60),
            10);

        SetEntityId(course, courseId);

        var module = new Module("Module 1", "desc", 1, courseId);
        var lesson = new Lesson("Lesson 1", "content", LessonType.Text, 1, module.Id);
        var assignment = new Assignment("Assignment 1", "desc", AssignmentType.Quiz, 100, module.Id);
        module.AddLesson(lesson);
        module.AddAssignment(assignment);
        course.AddModule(module);
        return course;
    }

    private static void SetCreatedAt(BaseEntity entity, DateTime utcDateTime)
    {
        var method = typeof(BaseEntity).GetMethod(
            "CreatedAt",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
            binder: null,
            types: [typeof(DateTime)],
            modifiers: null)!;

        method.Invoke(entity, [utcDateTime]);
    }

    private static void SetEntityId(BaseEntity entity, Guid id)
    {
        var idProperty = typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))!;
        idProperty.SetValue(entity, id);
    }

    private sealed class FakeCurrentUserService(Guid? userId, bool isAuthenticated) : ICurrentUserService
    {
        public Guid? UserId { get; } = userId;
        public string? UserEmail => "tests@local";
        public bool IsAuthenticated { get; } = isAuthenticated;
        public bool IsInRole(string role) => false;
    }

    private sealed class ThrowingStudentReadService : IStudentReadService
    {
        public Task<StudentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            throw new HttpRequestException("Dapr unavailable");

        public Task<PaginatedList<StudentDto>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken = default) =>
            throw new HttpRequestException("Dapr unavailable");

        public Task<StudentDto> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken = default) =>
            throw new HttpRequestException("Dapr unavailable");

        public Task<StudentProgressDto> GetStudentProgressAsync(Guid studentId, CancellationToken cancellationToken = default) =>
            throw new HttpRequestException("Dapr unavailable");
    }

    private sealed class FakeStudentReadService(string fullName) : IStudentReadService
    {
        public Task<StudentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(new StudentDto(id, fullName, "student@example.com", string.Empty, null));

        public Task<PaginatedList<StudentDto>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<StudentDto> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken = default) =>
            Task.FromResult(new StudentDto(studentId, fullName, "student@example.com", string.Empty, null));

        public Task<StudentProgressDto> GetStudentProgressAsync(Guid studentId, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }

    private sealed class ThrowingEnrollmentReadService : IEnrollmentReadService
    {
        public Task<EnrollmentDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            throw new HttpRequestException("Dapr unavailable");

        public Task<PaginatedList<EnrollmentDetailDto>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken = default) =>
            throw new HttpRequestException("Dapr unavailable");

        public Task<PaginatedList<EnrollmentDto>> GetStudentEnrollmentsAsync(Guid studentId, PaginationParameters pagination, CancellationToken cancellationToken = default) =>
            throw new HttpRequestException("Dapr unavailable");

        public Task<PaginatedList<EnrollmentDto>> GetCourseEnrollmentsAsync(Guid courseId, PaginationParameters pagination, CancellationToken cancellationToken = default) =>
            throw new HttpRequestException("Dapr unavailable");

        public Task<EnrollmentDetailDto> GetByStudentAndCourseIdAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default) =>
            throw new HttpRequestException("Dapr unavailable");
    }

    private sealed class FakeEnrollmentReadService(Dictionary<Guid, int> courseEnrollmentCounts) : IEnrollmentReadService
    {
        public Task<EnrollmentDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<PaginatedList<EnrollmentDetailDto>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<PaginatedList<EnrollmentDto>> GetStudentEnrollmentsAsync(Guid studentId, PaginationParameters pagination, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<PaginatedList<EnrollmentDto>> GetCourseEnrollmentsAsync(Guid courseId, PaginationParameters pagination, CancellationToken cancellationToken = default)
        {
            var count = courseEnrollmentCounts.TryGetValue(courseId, out var value) ? value : 0;
            return Task.FromResult(new PaginatedList<EnrollmentDto>(new List<EnrollmentDto>(), count, 1, 1));
        }

        public Task<EnrollmentDetailDto> GetByStudentAndCourseIdAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }

    private sealed class FakeStudentRepository(Student? student) : IStudentRepository
    {
        public Task<Student?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(student?.Id == id ? student : null);

        public Task AddAsync(Student entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task UpdateAsync(Student entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task DeleteAsync(Student entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IReadOnlyList<Student>> GetStudentsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<IReadOnlyList<Course>> GetCoursesByStudentIdAsync(Guid studentId, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<int> GetEnrolledStudentCountAsync(Guid courseId, CancellationToken cancellationToken) => throw new NotImplementedException();
    }

    private sealed class FakeEnrollmentRepository(IReadOnlyList<Enrollment> enrollments) : IEnrollmentRepository
    {
        public Task<Enrollment?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(enrollments.SingleOrDefault(x => x.Id == id));

        public Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken) =>
            Task.FromResult((IReadOnlyList<Enrollment>)enrollments.Where(x => x.StudentId == studentId).ToList());

        public Task<Enrollment?> GetByStudentAndCourseIdAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken) =>
            Task.FromResult(enrollments.SingleOrDefault(x => x.StudentId == studentId && x.CourseId == courseId));

        public Task<Enrollment?> GetBySubmissionIdAsync(Guid submissionId, CancellationToken cancellationToken) =>
            Task.FromResult(enrollments.SingleOrDefault(x => x.Submissions.Any(s => s.Id == submissionId)));

        public Task<bool> HasAnyForCourseAsync(Guid courseId, CancellationToken cancellationToken) =>
            Task.FromResult(enrollments.Any(x => x.CourseId == courseId));

        public Task AddAsync(Enrollment entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task UpdateAsync(Enrollment entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task DeleteAsync(Enrollment entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }

    private sealed class FakeProgressReadRepository(Dictionary<Guid, double>? percentagesByEnrollmentId = null) : IProgressReadRepository
    {
        private readonly Dictionary<Guid, double> _percentagesByEnrollmentId = percentagesByEnrollmentId ?? new Dictionary<Guid, double>();

        public Task<Progress?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Progress?>(null);

        public Task<IReadOnlyList<Progress>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
        {
            var progress = new Progress(enrollmentId, Guid.NewGuid(), 60);
            progress.MarkAsCompleted();
            return Task.FromResult((IReadOnlyList<Progress>)new List<Progress> { progress });
        }

        public Task<Progress?> GetByEnrollmentAndLessonIdAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default) =>
            Task.FromResult<Progress?>(null);

        public Task<double> GetCourseProgressPercentageAsync(Guid enrollmentId, CancellationToken cancellationToken = default) =>
            Task.FromResult(_percentagesByEnrollmentId.TryGetValue(enrollmentId, out var value) ? value : 0);
    }

    private sealed class FakeCourseRepository(params Course[] courses) : ICourseRepository
    {
        public Task<Course?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(courses.SingleOrDefault(x => x.Id == id));

        public Task AddAsync(Course entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task UpdateAsync(Course entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task DeleteAsync(Course entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IReadOnlyList<Course>> GetByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<IReadOnlyList<Course>> GetByCategoryAsync(CourseCategory category, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<IReadOnlyList<Course>> GetFeaturedCoursesAsync(int count, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<IReadOnlyList<Course>> SearchCoursesAsync(string? searchTerm, PaginationParameters pagination, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<IReadOnlyList<Course>> GetRecentCoursesAsync(int count, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<IReadOnlyList<Course>> GetByLevelAsync(CourseLevel level, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<int> GetCoursesCountAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
    }

    private sealed class ThrowingThenFallbackInstructorRepository(Instructor fallbackInstructor) : IInstructorRepository
    {
        public Task<Instructor?> GetInstructorWithCoursesAsync(Guid instructorId, CancellationToken cancellationToken) =>
            throw new HttpRequestException("Dapr unavailable");

        public Task<Instructor?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(fallbackInstructor.Id == id ? fallbackInstructor : null);

        public Task AddAsync(Instructor entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task UpdateAsync(Instructor entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task DeleteAsync(Instructor entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IReadOnlyList<Instructor>> GetTopInstructorsAsync(int count, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<int> GetTotalStudentCountAsync(Guid instructorId, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<decimal> GetAverageRatingAsync(Guid instructorId, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}
