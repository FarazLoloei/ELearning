using ELearning.Application.Common.Model;
using ELearning.Application.Students.Dtos;
using MediatR;

namespace ELearning.Application.Students.Queries;

public sealed record GetStudentProgressQuery : IRequest<Result<StudentProgressDto>>
{
    public Guid StudentId { get; init; }
}
