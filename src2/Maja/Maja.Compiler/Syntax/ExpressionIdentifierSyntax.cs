using System.Linq;

namespace Maja.Compiler.Syntax
{
    public sealed class ExpressionIdentifierSyntax : ExpressionSyntax
    {
        public ExpressionIdentifierSyntax(string text)
            : base(text)
        { }

        public NameSyntax Name
            => ChildNodes.OfType<NameSyntax>().Single();

        public override SyntaxKind SyntaxKind
            => SyntaxKind.IdentifierExpression;

        public override R Accept<R>(ISyntaxVisitor<R> visitor)
            => visitor.OnExpressionIdentifier(this);
    }
}