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

        return (T?)JsonSerializer.Deserialize(JsonSerializer.Serialize(model), typeof(T))
            ?? throw new InvalidOperationException("Could not copy model");
    }
}
