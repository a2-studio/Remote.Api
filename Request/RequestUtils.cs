using Remote.Api.Response;

namespace Remote.Api.Request;

public static class RequestUtils
{
    public static async Task<IResponse<TResponse, TError>> ExecuteAsync<TQuery, TResponse, TError>(this IApiClient<TError> client,
        IRequestPost<TQuery, TResponse> request,
        TQuery query,
        CancellationToken cancellationToken)
        where TError : IError
        where TResponse : notnull
        => await client.PostAsync<TQuery, IRequestPost<TQuery, TResponse>, TResponse, TError>(
            request.Authorize,
            request.Path,
            query,
            request,
            cancellationToken);
    public static async Task<IResponse<TResponse, TError>> ExecuteAsync<TResponse, TError>(this IApiClient<TError> client,
        IRequestPost<TResponse> request,
        CancellationToken cancellationToken)
        where TError : IError
        where TResponse : notnull
        => await client.PostAsync<object?, IRequestPost<TResponse>, TResponse, TError>(
            request.Authorize,
            request.Path,
            null,
            request,
            cancellationToken);
    public static async Task<IResponse<TResponse, TError>> ExecuteAsync<TQuery, TResponse, TError>(this IApiClient<TError> client,
        IRequestDelete<TQuery, TResponse> request,
        TQuery query,
        CancellationToken cancellationToken)
        where TError : IError
        where TResponse : notnull
        => await client.DeleteAsync<TQuery, IRequestDelete<TQuery, TResponse>, TResponse, TError>(
            request.Authorize,
            request.Path,
            query,
            request,
            cancellationToken);
    public static async Task<IResponse<TResponse, TError>> ExecuteAsync<TResponse, TError>(this IApiClient<TError> client,
        IRequestDelete<TResponse> request,
        CancellationToken cancellationToken)
        where TError : IError
        where TResponse : notnull
        => request.IsQuery
        ? await client.DeleteAsync<IRequestDelete<TResponse>, object?, TResponse, TError>(
            request.Authorize,
            request.Path,
            request,
            null,
            cancellationToken)
        : await client.DeleteAsync<object?, IRequestDelete<TResponse>, TResponse, TError>(
            request.Authorize,
            request.Path,
            null,
            request,
            cancellationToken);
    public static async Task<IResponse<TResponse, TError>> ExecuteAsync<TResponse, TError>(this IApiClient<TError> client,
        IRequestGet<TResponse> request,
        CancellationToken cancellationToken)
        where TError : IError
        where TResponse : notnull
        => await client.GetAsync<IRequestGet<TResponse>, TResponse, TError>(
            request.Authorize,
            request.Path,
            request,
            cancellationToken);



    public static async Task<IResponse<TResponse>> ExecuteAsync<TQuery, TResponse>(this IApiClient<IError> client,
        IRequestPost<TQuery, TResponse> request,
        TQuery query,
        CancellationToken cancellationToken)
        where TResponse : notnull
        => await client.ExecuteAsync<TQuery, TResponse, IError>(request, query, cancellationToken);
    public static async Task<IResponse<TResponse>> ExecuteAsync<TResponse>(this IApiClient<IError> client,
        IRequestPost<TResponse> request,
        CancellationToken cancellationToken)
        where TResponse : notnull
        => await client.ExecuteAsync<TResponse, IError>(request, cancellationToken);
    public static async Task<IResponse<TResponse>> ExecuteAsync<TQuery, TResponse>(this IApiClient<IError> client,
        IRequestDelete<TQuery, TResponse> request,
        TQuery query,
        CancellationToken cancellationToken)
        where TResponse : notnull
        => await client.ExecuteAsync<TQuery, TResponse, IError>(request, query, cancellationToken);
    public static async Task<IResponse<TResponse>> ExecuteAsync<TResponse>(this IApiClient<IError> client,
        IRequestDelete<TResponse> request,
        CancellationToken cancellationToken)
        where TResponse : notnull
        => await client.ExecuteAsync<TResponse, IError>(request, cancellationToken);
    public static async Task<IResponse<TResponse>> ExecuteAsync<TResponse>(this IApiClient<IError> client,
        IRequestGet<TResponse> request,
        CancellationToken cancellationToken)
        where TResponse : notnull
        => await client.ExecuteAsync<TResponse, IError>(request, cancellationToken);
}
