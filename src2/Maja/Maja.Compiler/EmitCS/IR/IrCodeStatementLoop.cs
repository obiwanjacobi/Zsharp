using Maja.Compiler.IR;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.EmitCS.IR;

internal enum CodeLoopKind
{
    None,
    For,
    While,
    ForEach
}

internal abstract class IrCodeStatementLoop : IrStatementLoop
{
    protected IrCodeStatementLoop(StatementLoopSyntax syntax, IrExpression? expression, IrCodeBlock codeBlock)
        : base(syntax, expression, codeBlock)
    {}

    public virtual CodeLoopKind LoopKind { get; }
}

internal sealed class IrCodeStatementWhileLoop : IrCodeStatementLoop
{
    public IrCodeStatementWhileLoop(StatementLoopSyntax syntax, IrExpression expression, IrCodeBlock codeBlock)
        : base(syntax, expression, codeBlock)
    {}

    public override CodeLoopKind LoopKind => CodeLoopKind.While;
}

internal sealed class IrCodeStatementForLoop : IrCodeStatementLoop
{
    public IrCodeStatementForLoop(StatementLoopSyntax syntax, IrDeclarationVariable initializer, IrExpression condition, IrStatementAssignment step, IrCodeBlock codeBlock)
        : base(syntax, condition, codeBlock)
    {
        Initializer = initializer;
        Step = step;
    }

    public IrDeclarationVariable Initializer { get; }
    public IrExpression Condition => base.Expression!;
    public IrStatementAssignment Step { get; }

    public override CodeLoopKind LoopKind => CodeLoopKind.For;
}