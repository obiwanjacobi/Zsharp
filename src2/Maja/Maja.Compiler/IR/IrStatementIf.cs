using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrStatementIf : IrStatement
{
    public IrStatementIf(StatementIfSyntax syntax, IrExpression condition, IrCodeBlock codeBlock, IrElseClause? elseClause, IrElseIfClause? elifClause)
        : base(syntax)
    {
        Condition = condition;
        CodeBlock = codeBlock;
        ElseClause = elseClause;
        ElseIfClause = elifClause;
    }

    public IrExpression Condition { get; }
    public IrCodeBlock CodeBlock { get; }
    public IrElseClause? ElseClause { get; }
    public IrElseIfClause? ElseIfClause { get; }

    public new StatementIfSyntax Syntax
        => (StatementIfSyntax)base.Syntax;
}

internal sealed class IrElseClause : IrNode
{
    public IrElseClause(StatementElseSyntax syntax, IrCodeBlock codeBlock)
        : base(syntax)
    {
        CodeBlock = codeBlock;
    }

    public IrCodeBlock CodeBlock { get; }

    public new StatementElseSyntax Syntax
        => (StatementElseSyntax)base.Syntax;
}

internal sealed class IrElseIfClause : IrNode
{
    public IrElseIfClause(StatementElseIfSyntax syntax, IrExpression condition, IrCodeBlock codeBlock, IrElseClause? elseClause, IrElseIfClause? elifClause)
        : base(syntax)
    {
        Condition = condition;
        CodeBlock = codeBlock;
        ElseClause = elseClause;
        ElseIfClause = elifClause;
    }

    public IrExpression Condition { get; }
    public IrCodeBlock CodeBlock { get; }
    public IrElseClause? ElseClause { get; }
    public IrElseIfClause? ElseIfClause { get; }

    public new StatementElseIfSyntax Syntax
        => (StatementElseIfSyntax)base.Syntax;
}