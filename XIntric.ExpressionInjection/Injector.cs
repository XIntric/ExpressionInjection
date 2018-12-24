using System;
using System.Linq.Expressions;
using System.Threading;

namespace XIntric.ExpressionInjection
{
    public static class Injector
    {


        public static TRet Inject<TRet>(Expression<Func<TRet>> expr)
            => (TRet)Trap.Value.Register(expr);


        public static TRet Inject<TRet>(LambdaExpression expr)
            => (TRet)Trap.Value.Register(expr);


        internal static ThreadLocal<ITrapper> Trap = new ThreadLocal<ITrapper>(() => new DirectRunner());

        internal interface ITrapper
        {
            object Register(LambdaExpression expr);
        }

        class DirectRunner : ITrapper
        {
            public object Register(LambdaExpression expr)
            {                
                var func = expr.Compile();
                return func.DynamicInvoke(Empty);
            }
            static object[] Empty = new object[0];
        }
    }
}
