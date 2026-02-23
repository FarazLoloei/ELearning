using ELearning.Application.Common.Model;
using MediatR;

namespace ELearning.Application.Enrollments.Commands;

public sealed record CreateEnrollmentCommand : IRequest<Result>
{
    public Guid CourseId { get; init; }
}
