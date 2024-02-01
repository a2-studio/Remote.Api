using Remote.Api.Response;

namespace Remote.Api.Request;

public static class RequestListUtils
{
    public static async Task<IResponse<ICollection<TElement>, TError>> ExecuteListAsync<TList, TElement, TError>(this IApiClient<TError> client,
        IRequestOffsetList<TList, TElement> request,
        CancellationToken cancellationToken)
        where TList : IElementOffsetList<TElement>
        where TError : IError
    {
        LinkedList<TElement> elements = [];

        int? total = null;
        int offset = 0;

        request.Limit = 500;
        
        do
        {
            request.Offset = offset;

            IResponse<TList, TError> response = await client.ExecuteAsync(request, cancellationToken);
            if (response is ErrorResponse<TList, TError> error)
                return error.Cast<ICollection<TElement>>();
            else if (response is NoContentResponse<TList, TError> noContent)
                return noContent.Cast<ICollection<TElement>>();
            else if (response is not SuccessResponse<TList, TError> success)
                return response.Cast<ICollection<TElement>>(v => []);
            else
            {
                int newTotal = success.Data.GetTotal();                
                if (total is not null && total != newTotal) 
                {
                    total = null;
                    offset = 0;
                    elements.Clear();
                    continue;
                }
                total = newTotal;
                int count = 0;
                foreach (TElement element in success.Data.GetElements())
                {
                    count++;
                    elements.AddLast(element);
                }
                if (count == 0) 
                {
                    total = null;
                    offset = 0;
                    elements.Clear();
                    continue;
                }
                offset += count;
            }
        }
        while (total > offset);

        return IResponse<ICollection<TElement>, TError>.CreateSuccess(elements, System.Net.HttpStatusCode.OK);
    }
    public static async Task<IResponse<ICollection<TElement>, TError>> ExecuteListAsync<TList, TElement, TError>(this IApiClient<TError> client,
        IRequestPaginationList<TList, TElement> request,
        CancellationToken cancellationToken)
        where TList : IElementPaginationList<TElement>
        where TError : IError
    {
        LinkedList<TElement> elements = [];

        long? startingAfter = null;


        do
        {
            request.StartingAfter = startingAfter;
        
            IResponse<TList, TError> response = await client.ExecuteAsync(request, cancellationToken);
            if (response is ErrorResponse<TList, TError> error)
                return error.Cast<ICollection<TElement>>();
            else if (response is NoContentResponse<TList, TError> noContent)
                return noContent.Cast<ICollection<TElement>>();
            else if (response is not SuccessResponse<TList, TError> success)
                return response.Cast<ICollection<TElement>>(v => []);
            else
            {
                TList data = success.Data;
                foreach (TElement element in data.GetElements())
                    elements.AddLast(element);
                startingAfter = data.GetNext();
            }
        }
        while (startingAfter is not null);

        return IResponse<ICollection<TElement>, TError>.CreateSuccess(elements, System.Net.HttpStatusCode.OK);
    }

    public static async Task<IResponse<ICollection<TElement>>> ExecuteListAsync<TList, TElement>(this IApiClient<IError> client,
        IRequestOffsetList<TList, TElement> request,
        CancellationToken cancellationToken)
        where TList : IElementOffsetList<TElement>
        => await client.ExecuteListAsync<TList, TElement, IError>(request, cancellationToken);
    public static async Task<IResponse<ICollection<TElement>>> ExecuteListAsync<TList, TElement>(this IApiClient<IError> client,
        IRequestPaginationList<TList, TElement> request,
        CancellationToken cancellationToken)
        where TList : IElementPaginationList<TElement>
        => await client.ExecuteListAsync<TList, TElement, IError>(request, cancellationToken);
}