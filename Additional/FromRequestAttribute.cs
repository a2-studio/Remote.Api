namespace Remote.Api.Additional;

[AttributeUsage(AttributeTargets.Class)]
public class FromRequestAttribute(string key, string format) : BaseAdditionalValueAttribute
{
    public string Key => key;
    public string Format => format;

    protected override object? Value => (key, format);
}