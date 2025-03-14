﻿using ELearning.Application.Common.Model;
using ELearning.Application.Enrollments.Dtos;
using MediatR;

namespace ELearning.Application.Enrollments.Queries;

public class GetEnrollmentDetailQuery : IRequest<Result<EnrollmentDetailDto>>
{
    public Guid EnrollmentId { get; set; }
}