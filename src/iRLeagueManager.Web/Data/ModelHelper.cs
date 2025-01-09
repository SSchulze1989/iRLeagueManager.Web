using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace iRLeagueManager.Web.Data;

internal static class ModelHelper
{
    [return: NotNullIfNotNull(nameof(model))]
    public static T? CopyModel<T>(T? model)
    {
        if (model is null)
        {
            return default;
        }
        if (model.GetType().IsClass == false)
        {
            return model;
        }

        return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(model))
            ?? throw new InvalidOperationException("Could not copy model");
    }

    public static T CopyModelProperties<T>(T source, T target)
    {
        var modelType = typeof(T);
        var properties = modelType.GetProperties()
            .Where(x => x.SetMethod != null);
        foreach (var property in properties)
        {
            var value = property.GetValue(source);
            property.SetValue(target, value);
        }
        return target;
    }
}
