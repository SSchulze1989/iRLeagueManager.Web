﻿using iRLeagueApiCore.Common.Models;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace iRLeagueManager.Web.Data
{
    public class SamePropertyEqualityComparer<T> : IEqualityComparer<Expression<Func<T, IComparable>>>
    {
        public bool Equals(Expression<Func<T, IComparable>>? x, Expression<Func<T, IComparable>>? y)
        {
            var xMemberExpression = x?.Body as MemberExpression;
            var yMemberExpression = y?.Body as MemberExpression;
            if (xMemberExpression == null || yMemberExpression == null)
            {
                return false;
            }
            return xMemberExpression.Member == yMemberExpression.Member;
        }

        public int GetHashCode([DisallowNull] Expression<Func<T, IComparable>> obj)
        {
            var memberExpression = obj.Body as MemberExpression;
            //var memberName = memberExpression.Member.Name;
            return memberExpression?.Member.GetHashCode() ?? 0;
        }
    }
}