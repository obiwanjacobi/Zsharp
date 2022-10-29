using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrStatement : IrNode
{
    protected IrStatement(StatementSyntax syntax)
        : base(syntax)
    { }

    public new StatementSyntax Syntax
        => (StatementSyntax)base.Syntax;
}

internal sealed class IrStatementLoop : IrStatement
{
    public IrStatementLoop(StatementSyntax syntax)
        : base(syntax)
    { }
}
