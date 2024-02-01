namespace Remote.Api.Request;

public interface IElementOffsetList<TElement>
{
    IEnumerable<TElement> GetElements();
    int GetTotal();
}
public interface IElementPaginationList<TElement>
{
    IEnumerable<TElement> GetElements();
    long? GetNext();
}