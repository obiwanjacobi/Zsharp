using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrImport : IrNode
{
    public IrImport(UseImportSyntax syntax)
        : base(syntax)
    { }
}