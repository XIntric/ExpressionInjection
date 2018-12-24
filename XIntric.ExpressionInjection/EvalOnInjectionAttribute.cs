using System;
using System.Collections.Generic;
using System.Text;

namespace XIntric.ExpressionInjection
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class EvalOnInjectionAttribute : Attribute
    {
    }
}
