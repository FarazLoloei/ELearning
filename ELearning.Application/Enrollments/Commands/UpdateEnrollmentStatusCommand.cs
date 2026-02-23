using ELearning.Application.Common.Model;
using MediatR;

namespace ELearning.Application.Enrollments.Commands;

/// <summary>
/// Command to update the status of an enrollment
/// </summary>
public sealed record UpdateEnrollmentStatusCommand : IRequest<Result>
{
    public Guid EnrollmentId { get; init; }

    public string Status { get; init; } = string.Empty;
}
