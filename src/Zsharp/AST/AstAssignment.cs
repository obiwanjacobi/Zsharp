using static ZsharpParser;

namespace Zsharp.AST
{
    public class AstAssignment : AstCodeBlockItem, IAstExpressionSite
    {
        public AstAssignment(Variable_assign_autoContext ctx)
            : base(AstNodeType.Assignment)
        { }
        public AstAssignment(Variable_def_typed_initContext ctx)
            : base(AstNodeType.Assignment)
        { }

        private AstExpression? _expression;
        public AstExpression? Expression => _expression;

        public bool SetExpression(AstExpression expr)
        {
            return this.SafeSetParent(ref _expression, expr);
        }

        private AstVariable? _variable;
        public AstVariable? Variable => _variable;

        public bool SetVariable(AstVariable variable)
        {
            return this.SafeSetParent(ref _variable, variable);
        }

        public override void Accept(AstVisitor visitor)
        {
            base.Accept(visitor);
            visitor.VisitAssignment(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            Variable?.Accept(visitor);
            Expression?.Accept(visitor);
        }
    }
}