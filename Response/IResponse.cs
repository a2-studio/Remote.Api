using System.Net;

namespace Remote.Api.Response;

public interface IResponse
{
    HttpStatusCode Status { get; }

    string ToString();
}
public interface IResponse<out T> :
    IResponse
    where T : notnull
{
    T? Data { get; }
    IError? Error { get; }

    IResponse<J> Cast<J>(Func<T, J> func) where J : notnull;
}
public interface IResponse<out T, out TError> :
    IResponse<T>
    where TError : IError
    where T : notnull
{
    new TError? Error { get; }
    IError? IResponse<T>.Error => Error;

    new IResponse<J, TError> Cast<J>(Func<T, J> func) where J : notnull;
    IResponse<J> IResponse<T>.Cast<J>(Func<T, J> func) => Cast(func);

    static ErrorResponse<T, TError> CreateError(TError error, HttpStatusCode statusCode)
        => new ErrorResponse<T, TError>() { Error = error, Status = statusCode };
    static SuccessResponse<T, TError> CreateSuccess(T data, HttpStatusCode statusCode)
        => new SuccessResponse<T, TError>() { Data = data, Status = statusCode };
    static NoContentResponse<T, TError> CreateNoContent(HttpStatusCode statusCode)
        => new NoContentResponse<T, TError>() { Status = statusCode };
}
