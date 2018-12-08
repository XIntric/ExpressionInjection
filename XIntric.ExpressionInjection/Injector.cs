using System;
using System.Linq.Expressions;
using System.Threading;

namespace ExpressionInjection
{
    public static class Injector
    {


        public static TRet Inject<TRet>(Expression<Func<TRet>> expr)
            => (TRet)Trap.Value.Register(expr);

        public static TRet Inject<T1, TRet>(T1 p1, Expression<Func<T1, TRet>> expr)
            => (TRet)Trap.Value.Register(expr, p1);

        public static TRet Inject<T1, T2, TRet>(T1 p1, T2 p2, Expression<Func<T1, T2, TRet>> expr)
            => (TRet)Trap.Value.Register(expr, p1, p2);

        public static TRet Inject<T1, T2, T3, TRet>(T1 p1, T2 p2, T3 p3, Expression<Func<T1, T2, T3, TRet>> expr)
            => (TRet)Trap.Value.Register(expr, p1, p2, p3);

        public static TRet Inject<T1, T2, T3, T4, TRet>(T1 p1, T2 p2, T3 p3, T4 p4, Expression<Func<T1, T2, T3, T4, TRet>> expr)
            => (TRet)Trap.Value.Register(expr, p1, p2, p3, p4);



        internal static ThreadLocal<ITrapper> Trap = new ThreadLocal<ITrapper>(() => new DirectRunner());

        internal interface ITrapper
        {
            object Register(LambdaExpression expr, params object[] args);
        }

        class DirectRunner : ITrapper
        {
            public object Register(LambdaExpression expr, params object[] args)
            {                
                var func = expr.Compile();
                return func.DynamicInvoke(args);
            }
        }
    }
}
