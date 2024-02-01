using System.Text.Json.Serialization;

namespace Remote.Api.Request;

public interface IRequest
{
    [JsonIgnore] bool Authorize { get; }
    [JsonIgnore] string Path { get; }
}

public interface IRequestPost<TQuery, TResponse> : IRequest;
public interface IRequestPost<TResponse> : IRequest;

public interface IRequestDelete<TQuery, TResponse> : IRequest;
public interface IRequestDelete<TResponse> : IRequest
{
    [JsonIgnore] bool IsQuery { get; }
}

public interface IRequestGet<TResponse> : IRequest;