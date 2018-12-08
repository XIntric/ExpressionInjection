using System;
using System.Collections.Generic;
using System.Text;

namespace XIntric.ExpressionInjection
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InjectableAttribute : Attribute
    {
    }
}
