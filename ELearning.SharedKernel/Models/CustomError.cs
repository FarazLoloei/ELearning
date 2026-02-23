namespace ELearning.SharedKernel.Models;
public record CustomError(string Code, string Message)
{
    public static readonly CustomError None = new(string.Empty, string.Empty);

    public override string ToString() => $"{Code}: {Message}";
}