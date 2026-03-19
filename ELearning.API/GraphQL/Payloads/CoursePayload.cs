// <copyright file="CoursePayload.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.Payloads;

using ELearning.Application.Courses.Dtos;

[GraphQLDescription("Payload for course operations")]
public class CoursePayload : PayloadBase
{
    [GraphQLDescription("The course affected by the operation")]
    public CourseDto? Course { get; }

    public CoursePayload()
    {
    }

    public CoursePayload(CourseDto course) => this.Course = course;

    public CoursePayload(string error)
        : base(new Error("COURSE_ERROR", error))
    {
    }

    public CoursePayload(Error error)
        : base(error)
    {
    }

    public CoursePayload(IEnumerable<Error> errors)
        : base(errors)
    {
    }
}