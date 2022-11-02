using System.Linq;

namespace Maja.Compiler.Syntax
{
    public sealed class ModuleSyntax : SyntaxNode
    {
        public ModuleSyntax(string text)
            : base(text)
        { }

        public QualifiedNameSyntax Identifier
            => ChildNodes.OfType<QualifiedNameSyntax>().Single();

        public override SyntaxKind SyntaxKind
            => SyntaxKind.ModuleDirective;

        public override R Accept<R>(ISyntaxVisitor<R> visitor)
            => visitor.OnModule(this);
    }
}