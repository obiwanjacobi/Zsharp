using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrStatement : IrNode
{
    protected IrStatement(IrLocality locality)
    {
        Locality = locality;
    }
    protected IrStatement(StatementSyntax syntax, IrLocality locality)
        : base(syntax)
    {
        Locality = locality;
    }

    public IrLocality Locality { get; }

    public new StatementSyntax Syntax
        => (StatementSyntax)base.Syntax;
}

internal class IrStatementLoop : IrStatement, IrContainer
{
    public IrStatementLoop(StatementLoopSyntax syntax, IrExpression? expression, IrCodeBlock codeBlock)
        : base(syntax, IrLocality.None)
    {
        Expression = expression;
        CodeBlock = codeBlock;
    }

    public new StatementLoopSyntax Syntax
        => (StatementLoopSyntax)base.Syntax;

    public IrExpression? Expression { get; }
    public IrCodeBlock CodeBlock { get; }

    public IEnumerable<T> GetDescendentsOfType<T>() where T : IrNode
        => Expression.GetDescendentsOfType<T>().Concat(CodeBlock.GetDescendentsOfType<T>());
}
