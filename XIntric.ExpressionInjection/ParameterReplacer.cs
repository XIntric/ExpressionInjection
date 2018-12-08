using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace XIntric.ExpressionInjection
{
    internal class ParameterReplacer : ExpressionVisitor
    {
        public ParameterReplacer(IEnumerable<ParameterExpression> sources, IEnumerable<Expression> replacements)
        {
            Replacements = sources.Zip(replacements, (s, r) => new KeyValuePair<ParameterExpression, Expression>(s, r)).ToDictionary(x => x.Key, x => x.Value);
        }
        Dictionary<ParameterExpression, Expression> Replacements;

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (Replacements.TryGetValue(node, out var replacement))
                return replacement;
            return base.VisitParameter(node);
        }
    }
}
