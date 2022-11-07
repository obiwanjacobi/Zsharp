using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrExport : IrNode
{
    public IrExport(QualifiedNameSyntax syntax)
        : base(syntax)
    { }

    public new QualifiedNameSyntax Syntax
        => (QualifiedNameSyntax)base.Syntax;
}