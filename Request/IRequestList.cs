namespace Remote.Api.Request;

public interface IRequestOffsetList<TList, TElement> :
    IRequestGet<TList> 
    where TList : IElementOffsetList<TElement>
{
    int Offset { set; }
    int Limit { set; }
}
public interface IRequestPaginationList<TList, TElement> :
    IRequestPost<TList>
    where TList : IElementPaginationList<TElement>
{
    long? StartingAfter { set; }
}
