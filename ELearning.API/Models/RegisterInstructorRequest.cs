// <copyright file="RegisterInstructorRequest.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Models;

public class RegisterInstructorRequest : RegisterStudentRequest
{
    public string Bio { get; set; } = string.Empty;

    public string Expertise { get; set; } = string.Empty;
}