using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrExport : IrNode
{
    public IrExport(QualifiedNameSyntax syntax, SymbolName name)
        : base(syntax)
    {
        Name = name;
    }

    public SymbolName Name { get; }

    public new QualifiedNameSyntax Syntax
        => (QualifiedNameSyntax)base.Syntax;
}