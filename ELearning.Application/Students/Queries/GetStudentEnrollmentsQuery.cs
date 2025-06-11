using ELearning.Application.Common.Model;
using ELearning.Application.Enrollments.Dtos;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;
using MediatR;

namespace ELearning.Application.Students.Queries;

public class GetStudentEnrollmentsQuery : IRequest<Result<PaginatedList<EnrollmentDto>>>, IPaginatable
{
    public Guid StudentId { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}