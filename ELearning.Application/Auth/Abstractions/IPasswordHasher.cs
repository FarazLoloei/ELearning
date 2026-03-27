// <copyright file="IPasswordHasher.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Auth.Abstractions;

public interface IPasswordHasher
{
    string HashPassword(string password);

    bool VerifyPassword(string hashedPassword, string providedPassword);
}
