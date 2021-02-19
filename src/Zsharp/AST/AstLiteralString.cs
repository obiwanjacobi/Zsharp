using Antlr4.Runtime;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstLiteralString : AstNode
    {
        public AstLiteralString(StringContext context)
            : base(AstNodeType.Literal)
        {
            Context = context;
            Value = Parse(context.STRING().GetText());
        }

        public ParserRuleContext Context { get; }

        public string Value { get; }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitLiteralString(this);

        private static string Parse(string withQuotes)
        {
            return withQuotes[1..^1];
        }
    }
}
