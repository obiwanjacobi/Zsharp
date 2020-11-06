using Antlr4.Runtime;
using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstLiteralBoolean : AstNode
    {
        public AstLiteralBoolean(Literal_boolContext context)
            : base(AstNodeType.Literal)
        {
            Context = context;
            Value = context.TRUE() != null;
        }

        public ParserRuleContext Context { get; }

        public Boolean Value { get; }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitLiteralBoolean(this);
        }
    }
}
