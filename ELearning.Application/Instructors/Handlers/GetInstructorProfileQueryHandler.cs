using AutoMapper;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
using ELearning.Application.Common.Resilience;
using ELearning.Application.Instructors.Abstractions.ReadModels;
using ELearning.Application.Instructors.Dtos;
using ELearning.Application.Instructors.Queries;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

namespace ELearning.Application.Instructors.Handlers;

/// <summary>
/// Handler for the GetInstructorProfileQuery
/// </summary>
public class GetInstructorProfileQueryHandler(
        IInstructorReadService instructorReadService,
        IInstructorRepository instructorRepository,
        IMapper mapper) : IRequestHandler<GetInstructorProfileQuery, Result<InstructorDto>>
{
    public async Task<Result<InstructorDto>> Handle(GetInstructorProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // First attempt to get data from Dapr read service (distributed cache)
            var instructorDto = await instructorReadService.GetInstructorByIdAsync(request.InstructorId, cancellationToken);
            return Result.Success(instructorDto);
        }
        catch (Exception ex) when (ReadModelFallbackPolicy.ShouldFallback(ex, cancellationToken))
        {
            // If Dapr service fails, fall back to direct repository access
            var instructor = await instructorRepository.GetByIdAsync(request.InstructorId, cancellationToken) ??
                throw new NotFoundException(nameof(Instructor), request.InstructorId);

            var instructorDto = mapper.Map<InstructorDto>(instructor);

            return Result.Success(instructorDto);
        }
    }
}
