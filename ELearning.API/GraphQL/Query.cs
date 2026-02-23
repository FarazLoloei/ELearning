using ELearning.Application.Courses.Dtos;
using ELearning.Application.Courses.Queries;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Application.Enrollments.Queries;
using ELearning.Application.Instructors.Dtos;
using ELearning.Application.Instructors.Queries;
using ELearning.Application.Students.Dtos;
using ELearning.Application.Students.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace ELearning.API.GraphQL;

/// <summary>
/// GraphQL query root type
/// </summary>
[GraphQLDescription("The query root type for the E-Learning API")]
public class Query
{
    /// <summary>
    /// Get a paginated list of courses with optional filtering
    /// </summary>
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    [GraphQLDescription("Get a paginated list of courses")]
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

    /// <summary>
    /// Get course details by ID
    /// </summary>
    [GraphQLDescription("Get course details by ID")]
    public async Task<CourseDto?> GetCourse(
        [Service] IMediator mediator,
        Guid id)
    {
        var query = new GetCourseDetailQuery { CourseId = id };
        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value : null;
    }

    /// <summary>
    /// Get featured courses
    /// </summary>
    [GraphQLDescription("Get featured courses")]
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

    /// <summary>
    /// Get student profile by ID
    /// </summary>
    [GraphQLDescription("Get student profile by ID")]
    public async Task<StudentDto?> GetStudent(
        [Service] IMediator mediator,
        Guid id)
    {
        var query = new GetStudentProfileQuery { StudentId = id };
        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value : null;
    }

    /// <summary>
    /// Get student progress by ID
    /// </summary>
    [GraphQLDescription("Get student progress by student ID")]
    [Authorize]
    public async Task<StudentProgressDto?> GetStudentProgress(
        [Service] IMediator mediator,
        Guid studentId)
    {
        var query = new GetStudentProgressQuery { StudentId = studentId };
        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value : null;
    }

    /// <summary>
    /// Get instructor profile by ID
    /// </summary>
    [GraphQLDescription("Get instructor profile by ID")]
    public async Task<InstructorDto?> GetInstructor(
        [Service] IMediator mediator,
        Guid id)
    {
        var query = new GetInstructorProfileQuery { InstructorId = id };
        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value : null;
    }

    /// <summary>
    /// Get enrollment details by ID
    /// </summary>
    [GraphQLDescription("Get enrollment details by ID")]
    [Authorize]
    public async Task<EnrollmentDetailDto?> GetEnrollment(
        [Service] IMediator mediator,
        Guid id)
    {
        var query = new GetEnrollmentDetailQuery { EnrollmentId = id };
        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value : null;
    }

    /// <summary>
    /// Get a student's enrollments
    /// </summary>
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    [GraphQLDescription("Get a student's enrollments")]
    [Authorize]
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
