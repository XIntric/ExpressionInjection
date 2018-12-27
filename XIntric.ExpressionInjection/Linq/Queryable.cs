using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace XIntric.ExpressionInjection.Linq
{
    public interface IQueryable : System.Linq.IQueryable
    {
        new IQueryProvider Provider { get; }
    }

    public interface IOrderedQueryable : IQueryable, System.Linq.IOrderedQueryable
    {
    }


    public interface IQueryable<T> : IQueryable, System.Linq.IQueryable<T>
    {
    }

    public interface IOrderedQueryable<T> : IQueryable<T>, IOrderedQueryable, System.Linq.IOrderedQueryable<T>
    {

    }

    public class Queryable<T> : IOrderedQueryable<T>
    {

        public Queryable(IQueryProvider p, Expression e)
        {
            Provider = p;
            Expression = e;
        }

        
        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }

        System.Linq.IQueryProvider System.Linq.IQueryable.Provider => Provider;

        public IEnumerator<T> GetEnumerator()
        {
            Expression modified = GetExpandedExpression();
            var sq = Provider.SourceProvider.CreateQuery<T>(modified);
            return sq.GetEnumerator();
        }

        public Expression GetExpandedExpression()
        {
            var expander = new InjectorTrapper();
            return expander.Visit(Expression);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
