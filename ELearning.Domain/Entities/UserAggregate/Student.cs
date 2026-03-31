// <copyright file="Student.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.UserAggregate;

using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.Domain.ValueObjects;

public class Student : User
{
    private Student()
        : base()
    {
    }

    public Student(string firstName, string lastName, Email email, string passwordHash)
        : base(firstName, lastName, email, passwordHash, UserRole.Student)
    {
    }
}
