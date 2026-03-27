// <copyright file="EnrollmentPayload.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.Payloads;

public class EnrollmentPayload : PayloadBase
{
    public Guid? EnrollmentId { get; }

    public EnrollmentPayload() // (Guid enrollmentId)
    {
        // EnrollmentId = enrollmentId;
    }

    public EnrollmentPayload(string error)
        : base(new Error("ENROLLMENT_ERROR", error))
    {
    }
}
