using System.Diagnostics.CodeAnalysis;

namespace dotFitness.SharedKernel.Results;

public class Result
{
    protected Result(bool isSuccess, string? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public string? Error { get; }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsFailure => !IsSuccess;

    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true);
    public static Result<TValue> Failure<TValue>(string error) => new(default, false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    internal Result(TValue? value, bool isSuccess, string? error = null)
        : base(isSuccess, error)
    {
        _value = value;
    }

    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue => IsSuccess && _value != null;

    public TValue? Value => IsSuccess ? _value : default;

    public static implicit operator Result<TValue>(TValue value) => Success(value);
}
