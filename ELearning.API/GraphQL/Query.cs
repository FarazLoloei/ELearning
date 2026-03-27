// <copyright file="Query.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL;

using ELearning.Application.Certificates.Dtos;
using ELearning.Application.Certificates.Queries;
using ELearning.Application.Courses.Dtos;
using ELearning.Application.Courses.Queries;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Application.Enrollments.Queries;
using ELearning.Application.Instructors.Dtos;
using ELearning.Application.Instructors.Queries;
using ELearning.Application.Students.Dtos;
using ELearning.Application.Students.Queries;
using ELearning.Application.Submissions.Dtos;
using ELearning.Application.Submissions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// GraphQL query root type.
/// </summary>
[GraphQLDescription("The query root type for the E-Learning API")]
public class Query
{
    /// <summary>
    /// Get a paginated list of courses with optional filtering.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
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
            PageSize = pageSize,
        };

        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value.Items : new List<CourseListDto>();
    }

    /// <summary>
    /// Get course details by ID.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    [GraphQLDescription("Get course details by ID")]
    public async Task<CourseDto?> GetCourse(
        [Service] IMediator mediator,
        Guid id)
    {
        var query = new GetCourseDetailQuery { CourseId = id };
        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value : null;
    }

    [GraphQLDescription("Get public reviews for a course")]
    public async Task<IEnumerable<ReviewDto>> GetCourseReviews(
        [Service] IMediator mediator,
        Guid courseId)
    {
        var result = await mediator.Send(new GetCourseReviewsQuery(courseId));
        return result.IsSuccess ? result.Value : [];
    }

    /// <summary>
    /// Get featured courses.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    [GraphQLDescription("Get featured courses")]
    public async Task<List<CourseListDto>> GetFeaturedCourses(
        [Service] IMediator mediator,
        int count = 5)
    {
        // You would have a specific query for featured courses
        var query = new GetCoursesListQuery
        {
            IsFeatured = true,
            PageSize = count,
        };

        var result = await mediator.Send(query);
        return result.IsSuccess ? new List<CourseListDto>(result.Value.Items) : new List<CourseListDto>();
    }

    /// <summary>
    /// Get courses by category.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    [GraphQLDescription("Get courses by category ID")]
    public async Task<List<CourseListDto>> GetCoursesByCategory(
        [Service] IMediator mediator,
        int categoryId)
    {
        var query = new GetCoursesListQuery
        {
            CategoryId = categoryId,
        };

        var result = await mediator.Send(query);
        return result.IsSuccess ? new List<CourseListDto>(result.Value.Items) : new List<CourseListDto>();
    }

    /// <summary>
    /// Get student profile by ID.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
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
    /// Get student progress by ID.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
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
    /// Get instructor profile by ID.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
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
    /// Get instructor profile with courses by ID.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    [GraphQLDescription("Get instructor with courses by ID")]
    public async Task<InstructorCoursesDto?> GetInstructorWithCourses(
        [Service] IMediator mediator,
        Guid id)
    {
        var query = new GetInstructorCoursesQuery { InstructorId = id };
        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value : null;
    }

    /// <summary>
    /// Get pending submissions for instructor.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    [GraphQLDescription("Get pending submissions for an instructor")]
    [Authorize(Roles = "Instructor")]
    public async Task<IEnumerable<SubmissionDto>> GetPendingSubmissions(
        [Service] IMediator mediator,
        Guid instructorId,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = new GetPendingSubmissionsQuery
        {
            InstructorId = instructorId,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value.Items : new List<SubmissionDto>();
    }

    /// <summary>
    /// Get enrollment details by ID.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
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

    [GraphQLDescription("Get the certificate issued for an enrollment")]
    [Authorize]
    public async Task<CertificateDto?> GetEnrollmentCertificate(
        [Service] IMediator mediator,
        Guid enrollmentId)
    {
        var result = await mediator.Send(new GetEnrollmentCertificateQuery(enrollmentId));
        return result.IsSuccess ? result.Value : null;
    }

    [GraphQLDescription("Verify a certificate by public certificate code")]
    public async Task<CertificateDto?> VerifyCertificate(
        [Service] IMediator mediator,
        string certificateCode)
    {
        var result = await mediator.Send(new VerifyCertificateQuery(certificateCode));
        return result.IsSuccess ? result.Value : null;
    }

    /// <summary>
    /// Get submission details by ID.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    [GraphQLDescription("Get submission details by ID")]
    [Authorize]
    public async Task<SubmissionDetailDto?> GetSubmission(
        [Service] IMediator mediator,
        Guid id)
    {
        var query = new GetSubmissionDetailQuery { SubmissionId = id };
        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value : null;
    }

    /// <summary>
    /// Get a student's enrollments.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
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
            PageSize = pageSize,
        };

        var result = await mediator.Send(query);
        return result.IsSuccess ? result.Value.Items : new List<EnrollmentDto>();
    }
}
