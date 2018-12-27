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

        class MyQueryable<T> : Linq.Queryable<T>
        {
            public MyQueryable(Linq.IQueryProvider p, Expression expr)
                : base(p, expr) { }
        }

        static Linq.IQueryable<T> WithMyQueryable<T>(Linq.IQueryable<T> q)
            => q.Provider.WithFactory((p, t, e) =>
            (Linq.IQueryable<T>)Activator.CreateInstance(
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
