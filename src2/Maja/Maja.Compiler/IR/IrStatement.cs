using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrStatement : IrNode
{
    protected IrStatement(SyntaxNode syntax)
        : base(syntax)
    { }
}

internal sealed class IrStatementLoop : IrStatement
{
    public IrStatementLoop(SyntaxNode syntax)
        : base(syntax)
    { }
}
