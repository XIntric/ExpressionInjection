using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace XIntric.ExpressionInjection
{
    internal class Provider : IQueryProvider
    {

        public Provider(IQueryProvider sourceprovider)
        {
            SourceProvider = sourceprovider;
        }

        public IQueryProvider SourceProvider { get; }

        public IQueryable CreateQuery(Expression expression)
            => new Queryable<object>(this, expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new Queryable<TElement>(this, expression);

        public object Execute(Expression expression)
        {
            var modified = expression;
            return SourceProvider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var modified = expression;
            return SourceProvider.Execute<TResult>(expression);
        }
    }
}
