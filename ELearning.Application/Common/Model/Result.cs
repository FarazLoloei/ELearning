namespace ELearning.Application.Common.Model;

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }

    private Result(bool isSuccess, T value, string error)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new ArgumentException("Successful result cannot contain an error.", nameof(error));
        if (!isSuccess && string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Failed result must contain an error.", nameof(error));

        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        return new Result<T>(true, value, string.Empty);
    }

    public static Result<T> Failure(string error)
        => new Result<T>(false, default!, error);
}

public sealed class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }

    private Result(bool isSuccess, string error)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new ArgumentException("Successful result cannot contain an error.", nameof(error));
        if (!isSuccess && string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Failed result must contain an error.", nameof(error));

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new Result(true, string.Empty);

    public static Result Failure(string error) => new Result(false, error);

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);
}
