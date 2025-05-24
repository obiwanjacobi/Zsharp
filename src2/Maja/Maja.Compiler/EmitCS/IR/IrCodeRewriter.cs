using System.Collections.Generic;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.EmitCS.IR;

/// <summary>
/// Rewrites the Ir model to have a better/closer fit to emitting csharp.
/// </summary>
internal sealed class IrCodeRewriter : IrRewriter
{
    public IEnumerable<IrProgram> CodeRewrite(IrProgram program)
        => RewriteProgram(program);

    protected override IEnumerable<IrDeclarationType> RewriteDeclarationType(IrDeclarationType type)
    {
        return type.IsTemplate ? [] : base.RewriteDeclarationType(type);
    }

    protected override IEnumerable<IrDeclarationFunction> RewriteDeclarationFunction(IrDeclarationFunction function)
    {
        return function.IsTemplate ? [] : base.RewriteDeclarationFunction(function);
    }

    protected override IEnumerable<IrStatement> RewriteStatementLoop(IrStatementLoop statement)
    {
        if (statement.Expression is IrExpressionRange rangeExpr)
        {
            return [CreateForLoop("i", rangeExpr.TypeSymbol, rangeExpr.Start!, rangeExpr.End!, statement.CodeBlock, statement.Syntax)];
        }
        else if (statement.Expression is IrExpressionIdentifier identExpr)
        {
            var initExpr = new IrExpressionLiteral(identExpr.TypeSymbol, 0);
            return [CreateForLoop(identExpr.Symbol.Name.Value, identExpr.TypeSymbol,
                initExpr, identExpr, statement.CodeBlock, statement.Syntax)];
        }
        else if (statement.Expression is IrExpressionLiteral litExpr)
        {
            var type = litExpr.TypeSymbol;

            if (litExpr.TypeInferredSymbol is not null &&
                !litExpr.TypeInferredSymbol.TrySelectInferredType(TypeSymbol.I32, out type))
                throw new MajaException("No Type For Literal Expression");

            var initExpr = new IrExpressionLiteral(type!, 0);
            return [CreateForLoop("i", type!,
                initExpr, litExpr, statement.CodeBlock, statement.Syntax)];
        }

        var expression = statement.Expression
            ?? new IrExpressionLiteral(TypeSymbol.Bool, true);

        return [new IrCodeStatementWhileLoop(statement.Syntax, expression, statement.CodeBlock)];
    }

    private IrCodeStatementForLoop CreateForLoop(string varName, TypeSymbol type, IrExpression initExpr, IrExpression endValExpr,
        IrCodeBlock codeBlock, StatementLoopSyntax syntax)
    {
        var symbol = new DeclaredVariableSymbol(SymbolName.InternalName($"__{varName}"), type);
        var initializer = new IrDeclarationVariable(symbol, type, initExpr);

        var condition = new IrExpressionBinary(
            new IrExpressionIdentifier(symbol, type),
            new IrBinaryOperator(IrBinaryOperatorKind.Lesser, type),
            endValExpr
            );

        var addOne = new IrExpressionBinary(
            new IrExpressionIdentifier(symbol, type),
            new IrBinaryOperator(IrBinaryOperatorKind.Add, type),
            new IrExpressionLiteral(type, 1)
            );
        var step = new IrStatementAssignment(symbol, addOne, IrLocality.None);

        return new IrCodeStatementForLoop(syntax, initializer, condition, step, codeBlock);
    }
}
