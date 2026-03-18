using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
using ELearning.Application.Students.Abstractions;
using ELearning.Application.Students.Dtos;
using ELearning.Application.Students.Queries;
using MediatR;

namespace ELearning.Application.Students.Handlers;

public class GetStudentProfileQueryHandler(
        IStudentReadRepository studentReadRepository
        ) : IRequestHandler<GetStudentProfileQuery, Result<StudentDto>>
{
    public async Task<Result<StudentDto>> Handle(GetStudentProfileQuery request, CancellationToken cancellationToken)
    {
        var student = await studentReadRepository.GetByIdAsync(request.StudentId, cancellationToken)
            ?? throw new NotFoundException("Student", request.StudentId);

        var studentDto = new StudentDto(
            student.Id,
            student.FullName,
            student.Email,
            student.ProfilePictureUrl ?? string.Empty,
            student.LastLoginDate);

        return Result.Success(studentDto);
    }
}
