using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstLiteralBoolean : AstNode
    {
        internal AstLiteralBoolean(bool value)
            : base(AstNodeKind.Literal)
        {
            Value = value;
        }

        internal AstLiteralBoolean(ParserRuleContext context, bool value)
            : base(AstNodeKind.Literal)
        {
            Context = context;
            Value = value;
        }

        public ParserRuleContext? Context { get; }

        public bool Value { get; }

        public override void Accept(AstVisitor visitor) => visitor.VisitLiteralBoolean(this);
    }
}
