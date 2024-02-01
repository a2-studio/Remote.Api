using System.Net;

namespace Remote.Api.Response;

public interface IErrorResponse :
    IResponse
{
    IError Error { get; }
}
public interface IErrorResponse<TError> :
    IErrorResponse
    where TError : IError
{
    new TError Error { get; }
    IError IErrorResponse.Error => Error;
}

public class ErrorResponse<T, TError> :
    IErrorResponse<TError>,
    IResponse<T, TError>
    where TError : IError
    where T : notnull
{
    public T? Data => default;
    public required TError Error { get; set; }
    public required HttpStatusCode Status { get; set; }

    IResponse<J, TError> IResponse<T, TError>.Cast<J>(Func<T, J> func) 
        => Cast<J>();
    public ErrorResponse<J, TError> Cast<J>()
        where J : notnull
        => new ()
        {
            Error = Error,
            Status = Status
        };

    public override string ToString()
        => $"[Error:{Status}({(int)Status})] {Error}";
}
