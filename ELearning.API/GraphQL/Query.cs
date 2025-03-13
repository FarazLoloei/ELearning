using ELearning.Application.Courses.Dtos;
using ELearning.Application.Courses.Queries;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Application.Instructors.Dtos;
using ELearning.Application.Students;
using HotChocolate;
using HotChocolate.Types;
using MediatR;

namespace ELearning.API.GraphQL;

public class Query
{
    // Course queries
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<CourseListDto>> GetCourses(
        [Service] IMediator mediator,
        string? searchTerm = null,
        int? categoryId = null,
        int? levelId = null,
        bool? isFeatured = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = new GetCoursesListQuery
        {
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            LevelId = levelId,
            IsFeatured = isFeatured,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value.Items : new List<CourseListDto>();
    }

    public async Task<CourseDetailDto?> GetCourse(
        [Service] IMediator mediator,
        Guid id)
    {
        var query = new GetCourseDetailQuery { CourseId = id };
        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value : null;
    }

    public async Task<List<CourseListDto>> GetFeaturedCourses(
        [Service] IMediator mediator,
        int count = 5)
    {
        // You would have a specific query for featured courses
        var query = new GetCoursesListQuery
        {
            IsFeatured = true,
            PageSize = count
        };

        var result = await mediator.Send(query);
        return result.IsSuccess ? new List<CourseListDto>(result.Value.Items) : new List<CourseListDto>();
    }

    // Student queries
    public async Task<StudentDto> GetStudent(
        [Service] IMediator mediator,
        Guid id)
    {
        var query = new GetStudentProfileQuery { StudentId = id };
        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value : null;
    }

    public async Task<StudentProgressDto> GetStudentProgress(
        [Service] IMediator mediator,
        Guid studentId)
    {
        var query = new GetStudentProgressQuery { StudentId = studentId };
        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value : null;
    }

    // Instructor queries
    public async Task<InstructorDto> GetInstructor(
        [Service] IMediator mediator,
        Guid id)
    {
        var query = new GetInstructorProfileQuery { InstructorId = id };
        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value : null;
    }

    // Enrollment queries
    public async Task<EnrollmentDetailDto> GetEnrollment(
        [Service] IMediator mediator,
        Guid id)
    {
        var query = new GetEnrollmentDetailQuery { EnrollmentId = id };
        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value : null;
    }

    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<EnrollmentDto>> GetStudentEnrollments(
        [Service] IMediator mediator,
        Guid studentId,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = new GetStudentEnrollmentsQuery
        {
            StudentId = studentId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value.Items : new List<EnrollmentDto>();
    }
}