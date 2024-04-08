namespace iRLeagueApiCore.Server.Filters;

/// <summary>
/// Ignore parameter when generating Swagger open API documentation
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class ParameterIgnoreAttribute : System.Attribute
{
}

public sealed class OpenApiParameterIgnoreFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
{
    public void Apply(Microsoft.OpenApi.Models.OpenApiOperation operation, Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext context)
    {
        if (operation == null || context == null || context.ApiDescription?.ParameterDescriptions == null)
            return;

        var parametersToHide = context.ApiDescription.ParameterDescriptions
            .Where(parameterDescription => ParameterHasIgnoreAttribute(parameterDescription))
            .ToList();

        if (parametersToHide.Count == 0)
            return;

        foreach (var parameterToHide in parametersToHide)
        {
            var parameter = operation.Parameters.FirstOrDefault(parameter => string.Equals(parameter.Name, parameterToHide.Name, System.StringComparison.Ordinal));
            if (parameter != null)
                operation.Parameters.Remove(parameter);
        }
    }

    private static bool ParameterHasIgnoreAttribute(Microsoft.AspNetCore.Mvc.ApiExplorer.ApiParameterDescription parameterDescription)
    {
        if (parameterDescription.ModelMetadata is Microsoft.AspNetCore.Mvc.ModelBinding.Metadata.DefaultModelMetadata metadata)
        {
            return metadata.Attributes.ParameterAttributes?.Any(attribute => typeof(ParameterIgnoreAttribute).IsAssignableFrom(attribute.GetType())) ?? false;
        }

        return false;
    }
}
