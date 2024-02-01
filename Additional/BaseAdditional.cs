namespace Remote.Api.Additional;

[AttributeUsage(AttributeTargets.Property)]
public abstract class BaseAdditionalAttribute : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class BaseAdditionalValueAttribute : BaseAdditionalAttribute
{
    protected abstract object? Value { get; }

    public KeyValuePair<BaseAdditionalAttribute, object?> Create()
        => KeyValuePair.Create<BaseAdditionalAttribute, object?>(this, Value);
}