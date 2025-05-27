using ELearning.Application.Common.Model;
using ELearning.Application.Instructors.Dtos;
using MediatR;

namespace ELearning.Application.Instructors.Queries;

/// <summary>
/// Query to retrieve the profile information of an instructor
/// </summary>
public record GetInstructorProfileQuery : IRequest<Result<InstructorDto>>
{
    /// <summary>
    /// ID of the instructor to retrieve
    /// </summary>
    public Guid InstructorId { get; set; }
}