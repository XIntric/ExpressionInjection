using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace XIntric.ExpressionInjection
{
    public static class ExpressionInjectionExtensions
    {

        public static IQueryable<T> EnableInjection<T>(this IQueryable<T> q)
        {
            return new ExpressionInjection.Queryable<T>(new ExpressionInjection.Provider(q.Provider), q.Expression);
        }

        public static Expression GetExpandedExpression<T>(this IQueryable<T> q)
        {
            var eq = q as Queryable<T>;
            return eq?.GetExpandedExpression() ?? q.Expression;
        }

    }

}
