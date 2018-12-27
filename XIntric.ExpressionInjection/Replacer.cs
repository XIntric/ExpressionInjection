using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace XIntric.ExpressionInjection
{
    internal class DelegatedReplacer : ExpressionVisitor
    {
        public Func<MemberExpression, Expression> ReplaceMemberFunc { get; private set; }
        public Func<ParameterExpression, Expression> ReplaceParameterFunc { get; private set; }

        public DelegatedReplacer() { }

        public DelegatedReplacer ReplaceMember(Func<MemberExpression, Expression> replacemember)
        {
            ReplaceMemberFunc = replacemember;
            return this;
        }

        public DelegatedReplacer ReplaceParameter(Func<ParameterExpression, Expression> replaceparameter)
        {
            ReplaceParameterFunc = replaceparameter;
            return this;
        }



        protected override Expression VisitMember(MemberExpression node)
        {
            var ret = ReplaceMemberFunc?.Invoke(node);
            return ret ?? base.VisitMember(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var ret = ReplaceParameterFunc?.Invoke(node);
            return ret ?? base.VisitParameter(node);
        }


    }
}
