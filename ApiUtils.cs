using Microsoft.Extensions.Primitives;
using Remote.Api.Additional;
using Remote.Api.Response;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.CovariantTasks;

namespace Remote.Api;

public static partial class ApiUtils
{
    private static readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    [GeneratedRegex("{(?'key'\\w+)}")] private static partial Regex FormattableRegex();

    private static JsonNode? ExtractJson(object? query)
        => query is null ? null : JsonSerializer.SerializeToNode(query, options);
    private static IEnumerable<KeyValuePair<string, StringValues>> ExtractQuery(object? query)
        => ExtractJson(query)?
            .AsObject()
            .Select(kv =>
            {
                StringValues values;
                if (kv.Value is JsonArray array) values = array.Select(v => v!.ToString()).ToArray();
                else values = kv.Value!.ToString();
                return KeyValuePair.Create(kv.Key, values);
            })
        ?? Enumerable.Empty<KeyValuePair<string, StringValues>>();
    
    private static async Task<IResponse<TResponse, TError>> SendAsync<TResponse, TError>(this IApiClient<TError> client,
        HttpMethod method,
        bool authorize,
        IEnumerable<KeyValuePair<BaseAdditionalAttribute, object?>> additional,
        string path,
        IEnumerable<KeyValuePair<string, StringValues>> query,
        JsonNode? body,
        CancellationToken cancellationToken)
        where TError : IError
        where TResponse : notnull
    {
        Regex regex = FormattableRegex();
        Dictionary<string, StringValues> map = new Dictionary<string, StringValues>(query);
        HashSet<string> skip = [];
        path = regex.Replace(path, v =>
        {
            if (!v.Groups.TryGetValue("key", out Group? group)) return v.Value;
            string key = group.Value;
            if (body?[key] is JsonValue jsonValue) return jsonValue.ToString();
            if (!map.TryGetValue(key, out StringValues value)) return v.Value;
            skip.Add(key);
            return value!;
        });
        foreach (string key in skip)
            map.Remove(key);

        IResponse<JsonNode, TError> response = await client.SendAsync(method, authorize, additional, path, map, body, cancellationToken).AsTask();
        return response.Cast(v =>
        {
            foreach (FromRequestAttribute attribute in additional.GetTyped<FromRequestAttribute>())
            {
                v[attribute.Key] = regex.Replace(attribute.Format, v =>
                {
                    if (!v.Groups.TryGetValue("key", out Group? group)) return v.Value;
                    string key = group.Value;
                    if (!map.TryGetValue(key, out StringValues value)) return v.Value;
                    return value!;
                });
            }
            return v.Deserialize<TResponse>(options)!;
        });
    }

    public static async Task<IResponse<TResponse, TError>> GetAsync<TQuery, TResponse, TError>(this IApiClient<TError> client,
        bool authorize,
        string path,
        TQuery? query,
        CancellationToken cancellationToken)
        where TError : IError
        where TResponse : notnull
        => await client.SendAsync<TResponse, TError>(HttpMethod.Get, authorize, AdditionalUtils.ExtractAdditionals(query), path, ExtractQuery(query), null, cancellationToken);
    public static async Task<IResponse<TResponse, TError>> PostAsync<TQuery, TBody, TResponse, TError>(this IApiClient<TError> client,
        bool authorize,
        string path,
        TQuery? query,
        TBody? body,
        CancellationToken cancellationToken)
        where TError : IError
        where TResponse : notnull
        => await client.SendAsync<TResponse, TError>(HttpMethod.Post, authorize, AdditionalUtils.ExtractAdditionals(query, body), path, ExtractQuery(query), ExtractJson(body), cancellationToken);
    public static async Task<IResponse<TResponse, TError>> DeleteAsync<TQuery, TBody, TResponse, TError>(this IApiClient<TError> client,
        bool authorize,
        string path,
        TQuery? query,
        TBody? body,
        CancellationToken cancellationToken)
        where TError : IError
        where TResponse : notnull
        => await client.SendAsync<TResponse, TError>(HttpMethod.Delete, authorize, AdditionalUtils.ExtractAdditionals(query, body), path, ExtractQuery(query), ExtractJson(body), cancellationToken);


    public static async Task<IResponse<TResponse>> GetAsync<TQuery, TResponse>(this IApiClient<IError> client,
        bool authorize,
        string path,
        TQuery? query,
        CancellationToken cancellationToken)
        where TResponse : notnull
        => await client.GetAsync<TQuery, TResponse, IError>(authorize, path, query, cancellationToken);
    public static async Task<IResponse<TResponse>> PostAsync<TQuery, TBody, TResponse>(this IApiClient<IError> client,
        bool authorize,
        string path,
        TQuery? query,
        TBody? body,
        CancellationToken cancellationToken)
        where TResponse : notnull
        => await client.PostAsync<TQuery, TBody, TResponse, IError>(authorize, path, query, body, cancellationToken);
    public static async Task<IResponse<TResponse>> DeleteAsync<TQuery, TBody, TResponse>(this IApiClient<IError> client,
        bool authorize,
        string path,
        TQuery? query,
        TBody? body,
        CancellationToken cancellationToken)
        where TResponse : notnull
        => await client.DeleteAsync<TQuery, TBody, TResponse, IError>(authorize, path, query, body, cancellationToken);
}
