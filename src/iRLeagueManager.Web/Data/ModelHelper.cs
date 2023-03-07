using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace iRLeagueManager.Web.Data;

internal static class ModelHelper
{
    public static T? CopyModel<T>(T? model) where T : class
    {
        if (model is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(model))
            ?? throw new InvalidOperationException("Could not copy model");
    }
}
