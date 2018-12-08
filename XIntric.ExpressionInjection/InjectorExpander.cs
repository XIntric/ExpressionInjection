using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace XIntric.ExpressionInjection
{
    public class InjectorExpander : ExpressionVisitor, Injector.ITrapper
    {

        public InjectorExpander()
        {

        }

        public object Register(LambdaExpression expr, params object[] args)
        {
            ReceivedExpression = expr;
            ReceivedArguments = args;
            return GetDefault(expr.ReturnType);
        }

        LambdaExpression ReceivedExpression;
        object[] ReceivedArguments;

        public override Expression Visit(Expression node)
        {
            Injector.ITrapper prevtrap = Injector.Trap.Value;
            Injector.Trap.Value = this;
            try
            {
                return base.Visit(node);
            }
            finally
            {
                Injector.Trap.Value = prevtrap;
            }

        }


        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!node.Method.CustomAttributes.Any(x => x.AttributeType == typeof(InjectableAttribute))) return base.VisitMethodCall(node);
            try
            {
                if (ReceivedExpression != null) throw new InvalidOperationException("Detected trap collision.");
                node.Method.Invoke(null, node.Method.GetParameters().Select(p => GetDefault(p.ParameterType)).ToArray());

                var expr = ReceivedExpression.Body;
                var paramreplacer = new ParameterReplacer(ReceivedExpression.Parameters, node.Arguments);
                expr = paramreplacer.Visit(expr);

                ReceivedExpression = null;
                ReceivedArguments = null;

                return base.Visit(expr);
            }
            finally
            {
                ReceivedExpression = null;
                ReceivedArguments = null;
            }


        }

        public static object GetDefault(Type type)
        {
            if (type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
