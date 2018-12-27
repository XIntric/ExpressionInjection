using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace XIntric.ExpressionInjection.Linq
{
    public interface IQueryProvider : System.Linq.IQueryProvider
    {
        IQueryProvider WithFactory(Func<IQueryProvider, Type, Expression, IQueryable> factory);

        System.Linq.IQueryProvider SourceProvider { get; }

        new IQueryable CreateQuery(Expression expression);
        new IQueryable<T> CreateQuery<T>(Expression expression);
    }

    internal class QueryProvider : IQueryProvider
    {

        public QueryProvider(System.Linq.IQueryProvider sourceprovider)
        {
            SourceProvider = sourceprovider;
            Factory = (provider, type, expr) =>
                (IQueryable)Activator.CreateInstance(
                    typeof(Queryable<>).MakeGenericType(type),
                    this, expr);
        }

        public QueryProvider(System.Linq.IQueryProvider sourceprovider, Func<IQueryProvider, Type, Expression, IQueryable> factory)
        {
            SourceProvider = sourceprovider;
            Factory = factory;
        }

        public System.Linq.IQueryProvider SourceProvider { get; }

        public virtual IQueryable CreateQuery(Expression expression)
            => (IQueryable)Factory.Invoke(this, typeof(object), expression);

        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => (IQueryable<TElement>)Factory.Invoke(this, typeof(TElement), expression);


        public object Execute(Expression expression)
        {
            var expander = new InjectorTrapper();
            var modified = expander.Visit(expression);
            return SourceProvider.Execute(modified);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var expander = new InjectorTrapper();
            var modified = expander.Visit(expression);
            return SourceProvider.Execute<TResult>(modified);
        }

        System.Linq.IQueryable System.Linq.IQueryProvider.CreateQuery(Expression expression) => CreateQuery(expression);

        System.Linq.IQueryable<TElement> System.Linq.IQueryProvider.CreateQuery<TElement>(Expression expression) => CreateQuery<TElement>(expression);

        public IQueryProvider WithFactory(Func<IQueryProvider, Type, Expression, IQueryable> factory)
            => new QueryProvider(SourceProvider, factory);

        private Func<IQueryProvider, Type, Expression, IQueryable> Factory;

    }
}
