using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using XIntric.ExpressionInjection;

namespace System.Linq
{
    public static class ExpressionInjectionExtensions
    {

        public static IQueryable<T> EnableInjection<T>(this IQueryable<T> q)
        {
            return new XIntric.ExpressionInjection.Queryable<T>(new XIntric.ExpressionInjection.Provider(q.Provider), q.Expression);
        }

        public static Expression GetExpandedExpression<T>(this IQueryable<T> q)
        {
            var eq = q as Queryable<T>;
            return eq?.GetExpandedExpression() ?? q.Expression;
        }

    }

}
