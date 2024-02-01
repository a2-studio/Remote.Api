using System.Net;

namespace Remote.Api.Response;

public interface INoContentResponse : IResponse;

public class NoContentResponse<T, TError> :
    INoContentResponse,
    IResponse<T, TError>
    where TError : IError
    where T : notnull
{
    public T? Data => default;
    public TError? Error => default;

    public required HttpStatusCode Status { get; set; }

    IResponse<J, TError> IResponse<T, TError>.Cast<J>(Func<T, J> func) 
        => Cast<J>();
    public NoContentResponse<J, TError> Cast<J>() 
        where J : notnull 
        => new()
        {
            Status = Status
        };

    public override string ToString()
        => $"[NoContent:{Status}({(int)Status})] NoContent";
}
