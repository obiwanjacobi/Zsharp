using Maja.Compiler.Diagnostics;
using Maja.Compiler.IR;
using Maja.Compiler.IR.Lower;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.Eval;

internal sealed class EvalWalker : IrWalker<object?>
{
    private readonly EvaluatorState _state;

    public EvalWalker(EvaluatorState state)
        => _state = state;

    public override object? OnExpressionBinary(IrExpressionBinary expression)
    {
        var left = (IrConstant)OnExpression(expression.Left)!;
        var right = (IrConstant)OnExpression(expression.Right)!;

        var result = IR.Lower.Evaluator.Evaluate(
            expression.Left.TypeSymbol, left,
            expression.Operator,
            expression.Right.TypeSymbol, right);

        return result;
    }

    public override object? OnExpressionIdentifier(IrExpressionIdentifier identifier)
    {
        if (identifier.ConstantValue is not null)
            return identifier.ConstantValue;

        if (_state.TryLookupVariable(identifier.Symbol.Name.FullName, out var value))
            return new IrConstant(value);

        return null;
    }

    public override object? OnExpressionLiteral(IrExpressionLiteral expression)
    {
        return expression.ConstantValue;
    }

    public override object? OnVariableDeclaration(IrVariableDeclaration variable)
    {
        var symbol = variable.Symbol;
        var type = variable.TypeSymbol;
        var name = symbol.Name.FullName;

        var value = variable.Initializer is null
            ? IrConstant.Zero
            : (IrConstant?)OnExpression(variable.Initializer!) ?? IrConstant.Zero;

        if (TypeSymbol.IsBoolean(type))
            SetVariable(name, value.ToBool());

        else if (TypeSymbol.IsI8(type))
            SetVariable(name, value.ToI8());
        else if (TypeSymbol.IsU8(type))
            SetVariable(name, value.ToU8());
        else if (TypeSymbol.IsI16(type))
            SetVariable(name, value.ToI16());
        else if (TypeSymbol.IsU16(type))
            SetVariable(name, value.ToU16());
        else if (TypeSymbol.IsI32(type))
            SetVariable(name, value.ToI32());
        else if (TypeSymbol.IsU32(type))
            SetVariable(name, value.ToU32());
        else if (TypeSymbol.IsI64(type))
            SetVariable(name, value.ToI64());
        else if (TypeSymbol.IsU64(type))
            SetVariable(name, value.ToU64());

        else if (TypeSymbol.IsF16(type))
            SetVariable(name, value.ToF16());
        else if (TypeSymbol.IsF32(type))
            SetVariable(name, value.ToF32());
        else if (TypeSymbol.IsF64(type))
            SetVariable(name, value.ToF64());
        else if (TypeSymbol.IsF96(type))
            SetVariable(name, value.ToF96());

        else if (TypeSymbol.IsC16(type))
            SetVariable(name, value.ToC16());
        else if (TypeSymbol.IsStr(type))
            SetVariable(name, value.ToStr());

        return null;

        void SetVariable<T>(string name, T value)
        {
            if (!_state.TrySetVariable(name, value!))
            {
                _state.Diagnostics.Add(DiagnosticMessageKind.Error, variable.Syntax.Location,
                    $"Setting variable '{name}' to '{value}' failed.");
            }
        }
    }

    public override object? OnStatementAssignment(IrStatementAssignment statement)
    {
        var value = (IrConstant)OnExpression(statement.Expression)!;
        var name = statement.Symbol.Name.FullName;

        if (!_state.TrySetVariable(name, value.Value))
        {
            _state.Diagnostics.VariableNotFound(statement.Syntax.Location, name);
        }
        return value;
    }

    public override object? OnStatementIf(IrStatementIf statement)
    {
        var condition = (IrConstant)OnExpression(statement.Condition)!;
        if (condition.ToBool())
        {
            return OnCodeBlock(statement.CodeBlock);
        }
        else if (statement.ElseIfClause is not null)
        {
            return OnElseIfClause(statement.ElseIfClause);
        }
        else if (statement.ElseClause is not null)
        {
            return OnCodeBlock(statement.ElseClause.CodeBlock);
        }
        return null;
    }

    private object? OnElseIfClause(IrElseIfClause elseIfClause)
    {
        var condition = (IrConstant)OnExpression(elseIfClause.Condition)!;
        if (condition.ToBool())
        {
            return OnCodeBlock(elseIfClause.CodeBlock);
        }
        if (elseIfClause.ElseIfClause is not null)
        {
            return OnElseIfClause(elseIfClause.ElseIfClause);
        }
        if (elseIfClause.ElseClause is not null)
        {
            return OnCodeBlock(elseIfClause.ElseClause.CodeBlock);
        }
        return null;
    }
}