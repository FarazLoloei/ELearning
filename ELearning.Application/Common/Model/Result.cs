namespace ELearning.Application.Common.Model;

public sealed class Result<T>
{
    private readonly T? _value;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    public T Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot access Value on a failed result.");

    private Result(bool isSuccess, T? value, string error)
    {
        if (isSuccess)
        {
            if (!string.IsNullOrEmpty(error))
                throw new ArgumentException("Successful result cannot contain an error.", nameof(error));
            if (value is null)
                throw new ArgumentNullException(nameof(value), "Successful result must contain a value.");
        }
        else
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(error);
        }

        IsSuccess = isSuccess;
        _value = value;
        Error = error;
    }

    public static Result<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Result<T>(true, value, string.Empty);
    }

    public static Result<T> Failure(string error) => new Result<T>(false, default, error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return IsSuccess ? onSuccess(Value) : onFailure(Error);
    }
}

public sealed class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    private Result(bool isSuccess, string error)
    {
        if (isSuccess)
        {
            if (!string.IsNullOrEmpty(error))
                throw new ArgumentException("Successful result cannot contain an error.", nameof(error));
        }
        else
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(error);
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new Result(true, string.Empty);

    public static Result Failure(string error) => new Result(false, error);

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<string, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return IsSuccess ? onSuccess() : onFailure(Error);
    }
}
