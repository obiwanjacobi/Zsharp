using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstVariableReference : AstVariable
    {
        private readonly Variable_refContext? _refCtx;
        private readonly Variable_assign_autoContext? _assignCtx;

        public AstVariableReference(Variable_refContext context)
        {
            _refCtx = context;
        }
        public AstVariableReference(Variable_assign_autoContext context)
        {
            _assignCtx = context;
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