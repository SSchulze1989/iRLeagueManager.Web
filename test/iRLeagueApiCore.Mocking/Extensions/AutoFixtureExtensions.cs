using AutoFixture.Dsl;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Mocking.Extensions;

public static class AutoFixtureExtensions
{
    public static IPostprocessComposer<T> WithSequence<T, TProperty>(this IPostprocessComposer<T> composer, Expression<Func<T, TProperty>> propertyPicker, IEnumerable<TProperty> sequence)
    {
        if (sequence.Any() == false)
        {
            throw new ArgumentException("Sequence must containe at least one entry", nameof(sequence));
        }
        var enumerator = sequence.GetEnumerator();

        return composer.With(propertyPicker, () =>
        {
            if (enumerator.MoveNext() == false)
            {
                enumerator.Reset();
                enumerator.MoveNext();
            }
            return enumerator.Current;
        });
    }
}
