using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrImport : IrNode
{
    public IrImport(QualifiedNameSyntax syntax)
        : base(syntax)
    { }

    public new QualifiedNameSyntax Syntax
        => (QualifiedNameSyntax)base.Syntax;
}