using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace XIntric.ExpressionInjection
{

    internal class Queryable<T> : IOrderedQueryable<T>
    {

        public Queryable(Provider p, Expression e)
        {
            Provider = p;
            Expression = e;
        }

        
        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public Provider Provider { get; }

        IQueryProvider IQueryable.Provider => Provider;

        public IEnumerator<T> GetEnumerator()
        {
            Expression modified = GetExpandedExpression();
            var sq = Provider.SourceProvider.CreateQuery<T>(modified);
            return sq.GetEnumerator();
        }

        public Expression GetExpandedExpression()
        {
            var expander = new InjectorExpander();
            return expander.Visit(Expression);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
