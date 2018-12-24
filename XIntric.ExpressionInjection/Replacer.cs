using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace XIntric.ExpressionInjection
{
    internal class DelegatedReplacer : ExpressionVisitor
    {
        public Func<MemberExpression, Expression> ReplaceMember { get; }

        public DelegatedReplacer(
            Func<MemberExpression, Expression> replacemember = null)
        {
            ReplaceMember = replacemember;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var ret = ReplaceMember(node);
            return ret ?? base.VisitMember(node);
        }



    }
}
