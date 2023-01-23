using Blazored.Modal;
using System.Linq.Expressions;

namespace iRLeagueManager.Web.Shared;

internal sealed class ModalParameters<T> : ModalParameters
{
    public ModalParameters<T> Add<TValue>(Expression<Func<T, TValue?>> expression, TValue value) where TValue : notnull
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            Add(memberExpression.Member.Name, value);
            return this;
        }

        throw new ArgumentException("Argument must be a MemberExpression", nameof(expression));
    }
}
