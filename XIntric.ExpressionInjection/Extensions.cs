using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.Linq
{
    public static class ExpressionInjectionExtensions
    {

        public static XIntric.ExpressionInjection.Linq.IQueryable<T> EnableInjection<T>(this IQueryable<T> q)
        {
            var provider = new XIntric.ExpressionInjection.Linq.QueryProvider(q.Provider);
            return provider.CreateQuery<T>(q.Expression);
        }

        public static Expression GetInjectedExpression<T>(this IQueryable<T> q)
        {
            var eq = q as XIntric.ExpressionInjection.Linq.Queryable<T>;
            return eq?.GetExpandedExpression() ?? q.Expression;
        }

        public static IQueryable<T> Inject<T>(this IQueryable<T> q)
        {
            if (!(q is XIntric.ExpressionInjection.Linq.Queryable<T> eq)) return q;
            return eq.Provider.CreateQuery<T>(q.GetInjectedExpression());
        }

    }

}
