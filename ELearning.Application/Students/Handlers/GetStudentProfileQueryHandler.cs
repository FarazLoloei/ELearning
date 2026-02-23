using AutoMapper;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
using ELearning.Application.Common.Resilience;
using ELearning.Application.Students.Abstractions.ReadModels;
using ELearning.Application.Students.Dtos;
using ELearning.Application.Students.Queries;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

namespace ELearning.Application.Students.Handlers;

public class GetStudentProfileQueryHandler(
        IStudentReadService studentReadService,
        IStudentRepository studentRepository,
        IMapper mapper
        ) : IRequestHandler<GetStudentProfileQuery, Result<StudentDto>>
{
    public async Task<Result<StudentDto>> Handle(GetStudentProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // First try to get from Dapr read service
            var studentDto = await studentReadService.GetStudentByIdAsync(request.StudentId);
            return Result.Success(studentDto);
        }
        catch (Exception ex) when (ReadModelFallbackPolicy.ShouldFallback(ex, cancellationToken))
        {
            // If not found in Dapr, fall back to repository
            var student = await studentRepository.GetByIdAsync(request.StudentId);

            if (student == null)
            {
                throw new NotFoundException(nameof(Student), request.StudentId);
            }

            var dto = mapper.Map<StudentDto>(student);
            return Result.Success(dto);
        }
    }
}
