namespace ELearning.API.Contracts;

public sealed class ApiResponse<T>
{
    public bool Succeeded { get; init; }

    public T? Data { get; init; }

    public ApiError? Error { get; init; }

    public static ApiResponse<T> Success(T? data) =>
        new()
        {
            Succeeded = true,
            Data = data
        };

    public static ApiResponse<T> Failure(string code, string message) =>
        new()
        {
            Succeeded = false,
            Error = new ApiError(code, message)
        };
}
