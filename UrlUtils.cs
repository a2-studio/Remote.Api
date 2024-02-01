using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Remote.Api;

public static class UrlUtils
{
    public static string ChangeUrl(
        string url,
        string? prefixDomain,
        string path,
        IEnumerable<KeyValuePair<string, StringValues>> query)
    {
        UriBuilder builder = new UriBuilder(url) { Path = path };
        if (prefixDomain is not null)
            builder.Host = prefixDomain + "." + builder.Host;
        return QueryHelpers.AddQueryString(builder.ToString(), query);
    }
}