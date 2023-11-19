using System;
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

        if (_state.TryLookupValue(identifier.Symbol.Name.FullName, out var value))
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

        if (TypeSymbol.IsBoolean(type))
            return _state.TryAdd(name, false) ? false : null;

        if (TypeSymbol.IsI8(type))
            return AddVariable(name, (sbyte)0);
        if (TypeSymbol.IsU8(type))
            return AddVariable(name, (byte)0);
        if (TypeSymbol.IsI16(type))
            return AddVariable(name, (short)0);
        if (TypeSymbol.IsU16(type))
            return AddVariable(name, (ushort)0);
        if (TypeSymbol.IsI32(type))
            return AddVariable(name, 0);    // by default an int
        if (TypeSymbol.IsU32(type))
            return AddVariable(name, (uint)0);
        if (TypeSymbol.IsI64(type))
            return AddVariable(name, (long)0);
        if (TypeSymbol.IsU64(type))
            return AddVariable(name, (ulong)0);

        if (TypeSymbol.IsF16(type))
            return AddVariable(name, 0.0f);
        if (TypeSymbol.IsF32(type))
            return AddVariable(name, 0.0f);
        if (TypeSymbol.IsF64(type))
            return AddVariable(name, 0.0d);
        if (TypeSymbol.IsF96(type))
            return AddVariable(name, 0.0m);

        if (TypeSymbol.IsC16(type))
            return AddVariable(name, '\0');
        if (TypeSymbol.IsStr(type))
            return AddVariable(name, String.Empty);

        return null;

        object? AddVariable<T>(string name, T value)
            => _state.TryAdd(name, value!) ? value : null;
    }

    public override object? OnStatementAssignment(IrStatementAssignment statement)
    {
        var value = (IrConstant)OnExpression(statement.Expression)!;
        var name = statement.Symbol.Name.FullName;

        if (!_state.TrySetValue(name, value.Value))
        {
            // Diagnostics
        }
        return value;
    }

    public override object? OnStatementIf(IrStatementIf statement)
    {
        return null;
    }

    public override object? OnStatementLoop(IrStatementLoop statement)
    {
        return null;
    }
}