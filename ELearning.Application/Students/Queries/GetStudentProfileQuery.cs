using ELearning.Application.Common.Model;
using ELearning.Application.Students.Dtos;
using MediatR;

namespace ELearning.Application.Students.Queries;

public sealed record GetStudentProfileQuery : IRequest<Result<StudentDto>>
{
    public Guid StudentId { get; init; }
}
