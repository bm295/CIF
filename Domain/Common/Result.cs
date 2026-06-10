namespace CIF.Domain.Common;

public sealed record Error(string Code, string Field, string Message);

public sealed class Result
{
    private Result(bool isSuccess, IReadOnlyCollection<Error> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyCollection<Error> Errors { get; }

    public static Result Success() => new(true, []);
    public static Result Failure(params Error[] errors) => new(false, errors);
}

public sealed class Result<T>
{
    private Result(T? value, bool isSuccess, IReadOnlyCollection<Error> errors)
    {
        Value = value;
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public T? Value { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyCollection<Error> Errors { get; }

    public static Result<T> Success(T value) => new(value, true, []);
    public static Result<T> Failure(params Error[] errors) => new(default, false, errors);
}
