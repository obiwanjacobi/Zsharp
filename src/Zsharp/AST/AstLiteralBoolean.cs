using Antlr4.Runtime;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstLiteralBoolean : AstNode
    {
        internal AstLiteralBoolean(bool value)
            : base(AstNodeKind.Literal)
        {
            Value = value;
        }

        public AstLiteralBoolean(Literal_boolContext context)
        : base(AstNodeKind.Literal)
        {
            Context = context;
            Value = context.TRUE() is not null;
        }

        public ParserRuleContext? Context { get; }

        public bool Value { get; }

        public override void Accept(AstVisitor visitor) => visitor.VisitLiteralBoolean(this);
    }
}
