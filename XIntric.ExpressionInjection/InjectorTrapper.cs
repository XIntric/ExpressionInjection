using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace XIntric.ExpressionInjection
{
    public class InjectorTrapper : ExpressionVisitor, Injector.ITrapper
    {

        public InjectorTrapper()
        {

        }

        public object Register(LambdaExpression expr)
        {
            ReplacementExpression = expr;
            return GetDefault(expr.ReturnType);
        }

        LambdaExpression ReplacementExpression;

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
                if (ReplacementExpression != null) throw new InvalidOperationException("Detected trap collision.");



                var arguments = node.Arguments.ToList();


                var parameterinfo = node.Method.GetParameters()
                    .Select((p, i) =>
                    {
                        var evaloninject = p.GetCustomAttribute(typeof(EvalOnInjectionAttribute)) != null;
                        return new
                        {
                            Parameter = p,
                            Index = i,
                            EvalOnInject = evaloninject,
                            EvaluationArgument = evaloninject
                                ? Evaluate(arguments[i])
                                : GetDefault(p.ParameterType),
                            Argument = arguments[i],
                        };
                    }).ToList();

                node.Method.Invoke(null, parameterinfo.Select(x => x.EvaluationArgument).ToArray());

                var replacementbody = ReplacementExpression.Body;

                ReplacementExpression = null;

                var replacer = new DelegatedReplacer(membernode =>
                {
                    var candidate = parameterinfo
                        .Where(x => x.Parameter.Name == membernode.Member.Name)
                        .FirstOrDefault();
                    if (candidate == null) return null;

                    if (candidate.EvalOnInject)
                    {
                        return Expression.Constant(candidate.EvaluationArgument, candidate.Parameter.ParameterType);
                    }
                    else
                    {
                        return candidate.Argument;
                    }
                });

                replacementbody = replacer.Visit(replacementbody);



                return base.Visit(replacementbody);

            }
            finally
            {
                ReplacementExpression = null;
            }


        }





        public static object Evaluate(Expression e)
        {
            if (e is ConstantExpression constante)
            {
                return constante.Value;
            }
            if (e is MemberExpression membere)
            {
                var obj = Evaluate(membere.Expression);
                if (membere.Member is PropertyInfo prop)
                {
                    return prop.GetValue(obj);
                }
                if (membere.Member is FieldInfo field)
                {
                    return field.GetValue(obj);
                }
            }
            throw new NotImplementedException($"Unable to figure out how to evaluate expression of type '{e.GetType().FullName}'.");
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
