using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace XIntric.ExpressionInjection.Tests
{
    public class InjectionByExpression
    {

        [Injectable]
        static bool GreaterThan(int value, int threshold)
            => Injector.Inject<bool>(BuildEvenExpression());

        static LambdaExpression BuildEvenExpression()
        {
            var p0 = Expression.Parameter(typeof(int), "value");
            var p1 = Expression.Parameter(typeof(int), "threshold");
            return Expression.Lambda(Expression.GreaterThan(p0, p1), p0, p1);
        }

        [Fact]
        public void CanBindParametersCorrectly()
        {
            var q = Enumerable.Range(0, 10)
                .AsQueryable()
                .EnableInjection()
                .Where(x => GreaterThan(x, 4));

            Assert.All(q, v => Assert.True(v > 4));

        }

    }
}
