using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XIntric.ExpressionInjection.Tests
{
    public class OnEval
    {

        [Injectable]
        static bool InRangeDynamic(int value, int start, int count)
            => Injector.Inject(() => Enumerable.Range(start, count).Contains(value));

        [Injectable]
        static bool InRangeOnEval(int value, [EvalOnInjection] int start, [EvalOnInjection] int count)
        {
            return Injector.Inject(() => Enumerable.Range(start, count).Contains(value));
        }


        [Fact]
        public void OnEvalArgumentIsEvaluatedOnInjection()
        {
            var ints = new int[] { 1, 5, 10, 15 };

            int rangestart = 5;
            int rangecount = 6;

            var inrangeonevalq = ints
                .AsQueryable()
                .EnableInjection()
                .Where(i => InRangeOnEval(i, rangestart, rangecount));


            inrangeonevalq = inrangeonevalq.Inject();


            rangestart = 0;
            rangecount = 0;

            Assert.Collection(
                inrangeonevalq,
                x => Assert.Equal(5, x),
                x => Assert.Equal(10, x));
        }

        [Fact]
        public void DynamicArgumentIsEvaluatedOnEnumeration()
        {
            var ints = new int[] { 0, 5, 10, 15 };

            int rangestart = 5;
            int rangecount = 6;

            var inrangedynamicq = ints
                .AsQueryable()
                .EnableInjection()
                .Where(i => InRangeDynamic(i, rangestart, rangecount));

            rangestart = 0;
            rangecount = 0;

            Assert.Empty(inrangedynamicq);
        }

    }
}
