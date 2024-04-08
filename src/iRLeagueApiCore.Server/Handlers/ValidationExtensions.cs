using FluentValidation.Results;

namespace iRLeagueApiCore.Server.Handlers;

public static class ValidationExtensions
{
    /// <summary>
    /// Run <see cref="IValidator{T}.ValidateAsync(T, CancellationToken)"/> for all validators
    /// and throw exception on validation errors
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="validators">Multiple validators to validate the same instance</param>
    /// <param name="instance">Instance to validate</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task ValidateAllAndThrowAsync<T>(this IEnumerable<IValidator<T>> validators, T instance, CancellationToken cancellationToken)
    {
        var results = new List<ValidationResult>();
        foreach (var validator in validators)
        {
            cancellationToken.ThrowIfCancellationRequested();
            results.Add(await validator.ValidateAsync(instance, cancellationToken));
        }
        if (results.Any(x => x.IsValid == false))
        {
            var errors = results.SelectMany(x => x.Errors);
            throw new ValidationException(errors);
        }
    }
}
