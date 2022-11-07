using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrImport : IrNode
{
    public IrImport(QualifiedNameSyntax syntax)
        : base(syntax)
    {
        SymbolName = syntax.ToSymbolName();
    }

    public new QualifiedNameSyntax Syntax
        => (QualifiedNameSyntax)base.Syntax;

    public SymbolName SymbolName { get; }
}