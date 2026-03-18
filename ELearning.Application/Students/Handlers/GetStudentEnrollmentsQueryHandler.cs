using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Common.Security;
using ELearning.Application.Enrollments.Abstractions;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Application.Students.Queries;
using ELearning.SharedKernel;
using MediatR;

namespace ELearning.Application.Students.Handlers;

public class GetStudentEnrollmentsQueryHandler(
        IEnrollmentReadRepository enrollmentReadRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<GetStudentEnrollmentsQuery, Result<PaginatedList<EnrollmentDto>>>
{
    public async Task<Result<PaginatedList<EnrollmentDto>>> Handle(GetStudentEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        CurrentUserAuthorizationGuard.EnsureStudentSelfOrAdmin(currentUserService, request.StudentId);

        var paginatedList = await enrollmentReadRepository.GetStudentEnrollmentsAsync(
            request.StudentId,
            new SharedKernel.Models.PaginationParameters(request.PageNumber, request.PageSize),
            cancellationToken);

        var dtoItems = paginatedList.Items
            .Select(e => new EnrollmentDto(
                e.Id,
                e.StudentId,
                e.StudentName,
                e.CourseId,
                e.CourseTitle,
                e.Status,
                e.EnrollmentDate,
                e.CompletedDate,
                e.CompletionPercentage))
            .ToList();

        return Result.Success(new PaginatedList<EnrollmentDto>(
            dtoItems,
            paginatedList.TotalCount,
            request.PageNumber,
            request.PageSize));
    }
}
