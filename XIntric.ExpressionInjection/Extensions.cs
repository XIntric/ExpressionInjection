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

        public static XIntric.ExpressionInjection.IQueryable<T> EnableInjection<T>(this IQueryable<T> q)
        {
            var provider = new XIntric.ExpressionInjection.QueryProvider(q.Provider);
            return provider.CreateQuery<T>(q.Expression);
        }

        public static Expression GetInjectedExpression<T>(this IQueryable<T> q)
        {
            var eq = q as Queryable<T>;
            return eq?.GetExpandedExpression() ?? q.Expression;
        }

        public static IQueryable<T> Inject<T>(this IQueryable<T> q)
        {
            if (!(q is Queryable<T> eq)) return q;
            return eq.Provider.CreateQuery<T>(q.GetInjectedExpression());
        }

    }

}
