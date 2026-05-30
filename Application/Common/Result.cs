namespace Application.Common;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; protected set; } = string.Empty;

    protected Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value)
        => new(value, true, string.Empty);

    public static Result<TValue> Failure<TValue>(string error)
        => new(default, false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("No se puede acceder al valor de un resultado fallido.");

    protected internal Result(TValue? value, bool isSuccess, string error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>Convenience factory so handlers can write Result&lt;T&gt;.Success(...)</summary>
    public static Result<TValue> Success(TValue value) => new(value, true, string.Empty);

    /// <summary>Convenience factory so handlers can write Result&lt;T&gt;.Failure(...)</summary>
    public new static Result<TValue> Failure(string error) => new(default, false, error);

    public static implicit operator Result<TValue>(TValue value) => Success(value);
}