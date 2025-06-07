using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrStatementIf : IrStatement, IrContainer
{
    public IrStatementIf(StatementIfSyntax syntax, IrExpression condition, IrCodeBlock codeBlock, IrElseClause? elseClause, IrElseIfClause? elifClause)
        : base(syntax, IrLocality.None)
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

    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
    {
        var exprTypes = Condition.GetDescendantsOfType<T>();
        var codeBlockTypes = CodeBlock.GetDescendantsOfType<T>();
        var elseTypes = ElseClause is not null ? ElseClause.GetDescendantsOfType<T>() : Enumerable.Empty<T>();
        var elseIfTypes = ElseIfClause is not null ? ElseIfClause.GetDescendantsOfType<T>() : Enumerable.Empty<T>();

        return exprTypes.Concat(codeBlockTypes).Concat(elseIfTypes).Concat(elseTypes);
    }
}

internal sealed class IrElseClause : IrNode, IrContainer
{
    public IrElseClause(StatementElseSyntax syntax, IrCodeBlock codeBlock)
        : base(syntax)
    {
        CodeBlock = codeBlock;
    }

    public IrCodeBlock CodeBlock { get; }

    public new StatementElseSyntax Syntax
        => (StatementElseSyntax)base.Syntax;

    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
        => CodeBlock.GetDescendantsOfType<T>();
}

internal sealed class IrElseIfClause : IrNode, IrContainer
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

    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
    {
        var exprTypes = Condition.GetDescendantsOfType<T>();
        var codeBlockTypes = CodeBlock.GetDescendantsOfType<T>();
        var elseTypes = ElseClause is not null ? ElseClause.GetDescendantsOfType<T>() : Enumerable.Empty<T>();
        var elseIfTypes = ElseIfClause is not null ? ElseIfClause.GetDescendantsOfType<T>() : Enumerable.Empty<T>();

        return exprTypes.Concat(codeBlockTypes).Concat(elseIfTypes).Concat(elseTypes);
    }
}