using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionInjection
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InjectableAttribute : Attribute
    {
    }
}
