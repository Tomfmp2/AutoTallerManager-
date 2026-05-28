using System.Numerics;

namespace Application.Common;

public class Result
{
    public bool IsSuccess { get; set; }
    public bool IsFailure { get; set; }
    public string Error { get; set; }

    protected Result(bool isSuccess, string error)
    {
        if (IsSuccess && error != string.Empty)
        {
            throw new InvalidOperationException("Un resultado exitoso no puede contener un mensaje de error.");
        }
        if (!IsFailure && error == string.Empty)
        {
            throw new InvalidOperationException("Un resultado fallido debe contener un mensaje de error.");
        }
        IsSuccess = isSuccess;
        Error = error;

    }
    public static Result Success() => new Result(true, string.Empty);
    public static Result Failure(string error) => new Result(false, error);
    public static Result<TValue> Success<TValue>(TValue value) => new Result<TValue>(value, true, string.Empty);
    public static Result<TValue> Failure<TValue>(string error) => new Result<TValue>(default, false, error);
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
    public static implicit operator Result<TValue>(TValue value) => Success(value);
}