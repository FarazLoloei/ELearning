using ELearning.Application.Common.Model;
using ELearning.Application.Enrollments.Dtos;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;
using MediatR;

namespace ELearning.Application.Students.Queries;

public sealed record GetStudentEnrollmentsQuery : IRequest<Result<PaginatedList<EnrollmentDto>>>, IPaginatable
{
    public Guid StudentId { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}
