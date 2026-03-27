// <copyright file="GetInstructorProfileQueryHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Instructors.Handlers;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
using ELearning.Application.Instructors.Dtos;
using ELearning.Application.Instructors.Queries;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

/// <summary>
/// Handler for the GetInstructorProfileQuery.
/// </summary>
public class GetInstructorProfileQueryHandler(
        IInstructorReadRepository instructorReadRepository) : IRequestHandler<GetInstructorProfileQuery, Result<InstructorDto>>
{
    public async Task<Result<InstructorDto>> Handle(GetInstructorProfileQuery request, CancellationToken cancellationToken)
    {
        var instructor = await instructorReadRepository.GetByIdAsync(request.InstructorId, cancellationToken)
            ?? throw new NotFoundException("Instructor", request.InstructorId);

        var instructorDto = new InstructorDto(
            instructor.Id,
            instructor.FullName,
            instructor.Email,
            instructor.Bio,
            instructor.Expertise,
            instructor.ProfilePictureUrl ?? string.Empty,
            instructor.AverageRating,
            instructor.TotalStudents,
            instructor.TotalCourses);

        return Result.Success(instructorDto);
    }
}
