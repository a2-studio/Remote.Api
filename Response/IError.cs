namespace Remote.Api.Response;

public interface IError
{
    string Message { get; }

    string ToString();
}
