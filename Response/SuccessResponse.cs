using System.Net;
using System.Text.Json;

namespace Remote.Api.Response;

public interface ISuccessResponse<T> :
    IResponse<T>
    where T : notnull
{
    new T Data { get; }
    T? IResponse<T>.Data => Data;
}
public interface ISuccessResponse<T, TError> :
    ISuccessResponse<T>,
    IResponse<T, TError>
    where TError : IError
    where T : notnull;

public class SuccessResponse<T, TError> :
    ISuccessResponse<T, TError>
    where TError : IError
    where T : notnull
{
    public required T Data { get; set; }
    public TError? Error => default;
    public required HttpStatusCode Status { get; set; }

    IResponse<J, TError> IResponse<T, TError>.Cast<J>(Func<T, J> func) => Cast(func);
    public SuccessResponse<J, TError> Cast<J>(Func<T, J> func) 
        where J : notnull
        => new ()
        {
            Data = func.Invoke(Data),
            Status = Status,
        };

    public override string ToString()
        => $"[Success:{Status}({(int)Status})] {JsonSerializer.SerializeToNode(Data)?.ToJsonString()}";
}