using System.Reflection;

namespace Remote.Api.Additional;

public static class AdditionalUtils
{
    public static IEnumerable<KeyValuePair<BaseAdditionalAttribute, object?>> ExtractAdditionals(params object?[] datas)
    {
        foreach (object? data in datas)
        {
            if (data?.GetType() is not Type dataType) continue;
            foreach (BaseAdditionalValueAttribute valueAdditionalAttribute in dataType.GetCustomAttributes<BaseAdditionalValueAttribute>())
            {
                yield return valueAdditionalAttribute.Create();
            }
            foreach (PropertyInfo propertyInfo in dataType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (propertyInfo.GetCustomAttribute<BaseAdditionalAttribute>() is not BaseAdditionalAttribute additionalAttribute) continue;
                if (propertyInfo.GetGetMethod(true) is not MethodInfo getter) continue;
                yield return KeyValuePair.Create(additionalAttribute, getter.Invoke(getter.IsStatic ? null : data, []));
            }
        }
    }
    public static IEnumerable<KeyValuePair<BaseAdditionalAttribute, object?>> ExtractTypeAdditionals(Type dataType)
    {
        foreach (BaseAdditionalValueAttribute valueAdditionalAttribute in dataType.GetCustomAttributes<BaseAdditionalValueAttribute>())
        {
            yield return valueAdditionalAttribute.Create();
        }
    }

    public static bool TryGetValue<TAdditional, TValue>(
        this IEnumerable<KeyValuePair<BaseAdditionalAttribute, object?>> values,
        out TValue? value)
        where TAdditional : BaseAdditionalAttribute
    {
        foreach ((BaseAdditionalAttribute additional, object? element) in values)
        {
            if (additional is not TAdditional) continue;
            if (element is not TValue tValue) throw new Exception($"Value '{element}' is not type '{typeof(TValue)}'");
            value = tValue;
            return true;
        }
        value = default;
        return false;
    }
    public static TValue GetRequiredValue<TAdditional, TValue>(
        this IEnumerable<KeyValuePair<BaseAdditionalAttribute, object?>> values)
        where TAdditional : BaseAdditionalAttribute
    {
        foreach ((BaseAdditionalAttribute additional, object? element) in values)
        {
            if (additional is not TAdditional) continue;
            if (element is not TValue value) throw new ArgumentException($"Value '{element}' is not type '{typeof(TValue)}'");
            return value;
        }
        throw new ArgumentException($"Value of type '{typeof(TValue)}' is not found");
    }
    public static TValue GetValue<TAdditional, TValue>(
        this IEnumerable<KeyValuePair<BaseAdditionalAttribute, object?>> values,
        TValue defaultValue)
        where TAdditional : BaseAdditionalAttribute
    {
        foreach ((BaseAdditionalAttribute additional, object? element) in values)
        {
            if (additional is not TAdditional) continue;
            if (element is not TValue value) throw new ArgumentException($"Value '{element}' is not type '{typeof(TValue)}'");
            return value;
        }
        return defaultValue;
    }

    public static IEnumerable<KeyValuePair<TAdditional, TValue>> GetValues<TAdditional, TValue>(
        this IEnumerable<KeyValuePair<BaseAdditionalAttribute, object?>> values)
        where TAdditional : BaseAdditionalAttribute
    {
        foreach ((BaseAdditionalAttribute baseAdditional, object? element) in values)
        {
            if (baseAdditional is not TAdditional additional) continue;
            if (element is not TValue value) throw new ArgumentException($"Value '{element}' is not type '{typeof(TValue)}'");
            yield return KeyValuePair.Create(additional, value);
        }
    }
    public static IEnumerable<TAdditional> GetTyped<TAdditional>(
        this IEnumerable<KeyValuePair<BaseAdditionalAttribute, object?>> values)
        where TAdditional : BaseAdditionalAttribute
    {
        foreach ((BaseAdditionalAttribute baseAdditional, _) in values)
        {
            if (baseAdditional is not TAdditional additional) continue;
            yield return additional;
        }
    }
}