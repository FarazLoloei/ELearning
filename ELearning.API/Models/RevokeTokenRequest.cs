// <copyright file="RevokeTokenRequest.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Models;

using System.ComponentModel.DataAnnotations;

public sealed class RevokeTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
