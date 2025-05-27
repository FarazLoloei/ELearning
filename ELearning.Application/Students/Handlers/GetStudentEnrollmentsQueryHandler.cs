using AutoMapper;
using ELearning.Application.Common.Model;
using ELearning.Application.Enrollments.Abstractions.ReadModels;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Application.Students.Abstractions.ReadModels;
using ELearning.Application.Students.Queries;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.SharedKernel;
using MediatR;

namespace ELearning.Application.Students.Handlers;

public class GetStudentEnrollmentsQueryHandler(
        IEnrollmentReadService enrollmentReadService,
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IStudentReadService studentReadService,
        IProgressRepository progressRepository,
        IMapper mapper)
    : IRequestHandler<GetStudentEnrollmentsQuery, Result<PaginatedList<EnrollmentDto>>>
{
    public async Task<Result<PaginatedList<EnrollmentDto>>> Handle(GetStudentEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Try Dapr read service first
            var paginatedList = await enrollmentReadService.GetStudentEnrollmentsAsync(
                request.StudentId,
                request.PageNumber,
                request.PageSize);

            return Result.Success(paginatedList);
        }
        catch (Exception)
        {
            // Fall back to repository
            var enrollments = await enrollmentRepository.GetByStudentIdAsync(request.StudentId);

            // Manual pagination
            var totalCount = enrollments.Count;
            var items = enrollments
        .Skip((request.PageNumber - 1) * request.PageSize)
        .Take(request.PageSize)
        .ToList();

            var enrollmentDtos = new List<EnrollmentDto>();
            foreach (var enrollment in items)
            {
                var course = await courseRepository.GetByIdAsync(enrollment.CourseId);
                var student = await studentReadService.GetByIdAsync(enrollment.StudentId);
                var completionPercentage = await progressRepository.GetCourseProgressPercentageAsync(enrollment.Id);

                var enrollmentDto = new EnrollmentDto
                {
                    Id = enrollment.Id,
                    StudentId = enrollment.StudentId,
                    StudentName = student.FullName,
                    CourseId = enrollment.CourseId,
                    CourseTitle = course.Title,
                    Status = enrollment.Status.Name,
                    EnrollmentDate = enrollment.CreatedAt(),
                    CompletedDate = enrollment.CompletedDateUTC,
                    CompletionPercentage = completionPercentage
                };

                enrollmentDtos.Add(enrollmentDto);
            }

            var paginatedList = new PaginatedList<EnrollmentDto>(
                enrollmentDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result.Success(paginatedList);
        }
    }
}