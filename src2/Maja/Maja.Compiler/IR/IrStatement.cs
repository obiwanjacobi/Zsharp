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
    public IrStatementLoop(StatementLoopSyntax syntax, IrExpression? expression, IrCodeBlock codeBlock)
        : base(syntax)
    {
        Expression = expression;
        CodeBlock = codeBlock;
    }

    public new StatementLoopSyntax Syntax
        => (StatementLoopSyntax)base.Syntax;

    public IrExpression? Expression { get; }
    public IrCodeBlock CodeBlock { get; }
}
