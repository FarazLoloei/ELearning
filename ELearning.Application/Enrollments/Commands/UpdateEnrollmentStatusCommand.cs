using ELearning.Application.Common.Model;
using MediatR;

namespace ELearning.Application.Enrollments.Commands;

/// <summary>
/// Command to update the status of an enrollment
/// </summary>
public class UpdateEnrollmentStatusCommand : IRequest<Result>
{
    public Guid EnrollmentId { get; set; }

    public string Status { get; set; }
}