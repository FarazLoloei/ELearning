// <copyright file="GetInstructorProfileQuery.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Instructors.Queries;

using ELearning.Application.Common.Model;
using ELearning.Application.Instructors.Dtos;
using MediatR;

/// <summary>
/// Query to retrieve the profile information of an instructor.
/// </summary>
public record GetInstructorProfileQuery : IRequest<Result<InstructorDto>>
{
    /// <summary>
    /// Gets or sets iD of the instructor to retrieve.
    /// </summary>
    public Guid InstructorId { get; set; }
}