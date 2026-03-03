using System.ComponentModel.DataAnnotations;

namespace ELearning.API.Models;

public sealed class RevokeTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
