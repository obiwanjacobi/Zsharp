using static ZsharpParser;

namespace Zsharp.AST
{
    public class AstVariableReference : AstVariable
    {
        private readonly Variable_refContext? _refCtx;
        private readonly Variable_assign_autoContext? _assignCtx;

        public AstVariableReference(Variable_refContext ctx)
        {
            _refCtx = ctx;
        }
        public AstVariableReference(Variable_assign_autoContext ctx)
        {
            _assignCtx = ctx;
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitVariableReference(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            Identifier?.Accept(visitor);
        }
    }
}