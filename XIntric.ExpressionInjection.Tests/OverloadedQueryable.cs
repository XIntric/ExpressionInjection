using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace XIntric.ExpressionInjection.Tests
{
    public class OverloadedQueryable
    {

        class MyQueryable<T> : Queryable<T>
        {
            public MyQueryable(IQueryProvider p, Expression expr)
                : base(p, expr) { }
        }

        static IQueryable<T> WithMyQueryable<T>(IQueryable<T> q)
            => q.Provider.WithFactory((p, t, e) =>
            (IQueryable<T>)Activator.CreateInstance(
                    typeof(MyQueryable<>).MakeGenericType(t),
                    p, e)).CreateQuery<T>(q.Expression);

        [Fact]
        public void Query_WhenOverloadedWithOwnFactory_ShouldReturnMyQueryable()
        {
            var q = Enumerable.Empty<int>()
                .AsQueryable()
                .EnableInjection();
            q = WithMyQueryable(q);
            Assert.True(q is MyQueryable<int>);
        }

    }
}
