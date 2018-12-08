using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XIntric.ExpressionInjection.UnitTest
{
    public class InjectedMethodBehaviour
    {

        [Injectable]
        public static bool IsEven(int value)
            => Injector.Inject(value, v => (v % 2) == 0);

        [Injectable]
        public static bool IsHigherThanAndEven(int value, int threshold)
            => Injector.Inject(value, threshold, (v, t) => IsEven(v) && v > t);


        List<int> IntValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

        [Fact]
        public void QueryWithEnabledInjection_UseInjectedMethod_ShouldExpandExpression()
        {

            var q = IntValues
                .AsQueryable()
                .EnableInjection()
                .Where(x => IsEven(x));

            Assert.True(q.All(x => (x % 2) == 0));
            Assert.DoesNotContain("IsEven", q.GetExpandedExpression().ToString());

        }

        [Fact]
        public void QueryWithoutEnabledInjection_UseInjectedMethod_ShouldNotExpandExpressionButStillWork()
        {

            var q = IntValues
                .AsQueryable()
                .Where(x => IsEven(x));

            Assert.True(q.All(x => (x % 2) == 0));
            Assert.Contains("IsEven", q.GetExpandedExpression().ToString());

        }

        [Fact]
        public void QueryWithEnabledInjection_UseNestedInjectedMethod_ShouldExpandExpression()
        {

            var q = IntValues
                .AsQueryable()
                .EnableInjection()
                .Where(x => IsHigherThanAndEven(x, 5));

            Assert.DoesNotContain("IsEven", q.GetExpandedExpression().ToString());

        }

        [Fact]
        public void InjectableMethod_UsedDirectly_ShouldReturnExpectedValue()
        {
            Assert.True(IsEven(2));
            Assert.False(IsEven(1));
        }

        [Fact]
        public void NestedInjectableMethod_UsedDirectly_ShouldReturnExpectedValue()
        {
            Assert.True(IsHigherThanAndEven(12, 10));
            Assert.False(IsHigherThanAndEven(13, 10));
            Assert.False(IsHigherThanAndEven(12, 15));
            Assert.False(IsHigherThanAndEven(13, 15));
        }
    }
}
