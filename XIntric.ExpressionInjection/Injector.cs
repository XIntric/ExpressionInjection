using System;
using System.Linq.Expressions;
using System.Threading;

namespace XIntric.ExpressionInjection
{
    public static class Injector
    {


        public static TRet Inject<TRet>(Expression<Func<TRet>> expr)
            => (TRet)Trap.Value.Register(expr);


        public static TRet Inject<TRet>(LambdaExpression expr, params object[] arguments)
            => (TRet)Trap.Value.Register(expr, arguments);


        internal static ThreadLocal<ITrapper> Trap = new ThreadLocal<ITrapper>(() => new DirectRunner());

        internal interface ITrapper
        {
            object Register(LambdaExpression expr, object[] arguments);

            object Register<TRet>(Expression<Func<TRet>> expr);
        }

        class DirectRunner : ITrapper
        {
            public object Register(LambdaExpression expr, object[] arguments)
            {
                var func = expr.Compile();
                return func.DynamicInvoke(arguments);
            }

            public object Register<TRet>(Expression<Func<TRet>> expr)
            {
                var func = expr.Compile();
                return func.DynamicInvoke(Empty);
            }

            static object[] Empty = new object[0];
        }
    }
}
