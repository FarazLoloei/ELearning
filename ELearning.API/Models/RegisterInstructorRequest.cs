// <copyright file="RegisterInstructorRequest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.API.Models;

public class RegisterInstructorRequest : RegisterStudentRequest
{
    public string Bio { get; set; } = string.Empty;

    public string Expertise { get; set; } = string.Empty;
}