using Microsoft.Extensions.Primitives;
using Remote.Api.Additional;
using Remote.Api.Response;
using System.Text.Json.Nodes;
using System.Threading.CovariantTasks;

namespace Remote.Api;

public interface IApiClient<out TError> : IAsyncDisposable
    where TError : IError
{
    ICovariantTask<IResponse<JsonNode, TError>> SendAsync(
        HttpMethod method,
        bool authorize,
        IEnumerable<KeyValuePair<BaseAdditionalAttribute, object?>> additional,
        string path,
        IEnumerable<KeyValuePair<string, StringValues>> query,
        JsonNode? body,
        CancellationToken cancellationToken);
}