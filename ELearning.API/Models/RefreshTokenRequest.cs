using System.ComponentModel.DataAnnotations;

namespace ELearning.API.Models;

public sealed class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
