using ELearning.Application.Common.Model;
using MediatR;

namespace ELearning.Application.Enrollments.Commands;

public class CreateEnrollmentCommand : IRequest<Result>
{
    public Guid CourseId { get; set; }
}