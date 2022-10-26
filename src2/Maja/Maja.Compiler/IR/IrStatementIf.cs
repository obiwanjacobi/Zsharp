using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrStatementIf : IrStatement
{
    public IrStatementIf(SyntaxNode syntax, IrExpression condition, IrScope codeBlock, IrElseClause? elseClause, IrElseIfClause? elifClause)
        : base(syntax)
    {
        Condition = condition;
        CodeBlock = codeBlock;
        ElseClause = elseClause;
        ElseIfClause = elifClause;
    }

    public IrExpression Condition { get; }
    public IrScope CodeBlock { get; }
    public IrElseClause? ElseClause { get; }
    public IrElseIfClause? ElseIfClause { get; }
}

internal class IrElseClause : IrNode
{
    public IrElseClause(SyntaxNode syntax, IrScope codeBlock)
        : base(syntax)
    {
        CodeBlock = codeBlock;
    }

    public IrScope CodeBlock { get; }
}

internal class IrElseIfClause : IrNode
{
    public IrElseIfClause(SyntaxNode syntax, IrExpression condition, IrScope codeBlock)
        : base(syntax)
    {
        Condition = condition;
        CodeBlock = codeBlock;
    }

    public IrExpression Condition { get; }
    public IrScope CodeBlock { get; }
}