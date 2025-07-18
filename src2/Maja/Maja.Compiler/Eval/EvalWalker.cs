﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.Eval;

internal sealed class EvalWalker : IrWalker<object?>
{
    private EvaluatorState _state;

    public EvalWalker(EvaluatorState state)
        => _state = state;

    private IDisposable NewScope(IrFunctionScope scope)
    {
        _state = new EvaluatorState(_state, scope);
        return new EvaluationStatePopper(this);
    }

    public override object? OnDeclarations(IEnumerable<IrDeclaration> declarations)
    {
        var decls = new List<IrDeclaration>();
        decls.AddRange(declarations.OfType<IrDeclarationType>());
        decls.AddRange(declarations.OfType<IrDeclarationFunction>());
        decls.AddRange(declarations.OfType<IrDeclarationVariable>());

        return base.OnDeclarations(decls);
    }

    public override object? OnDeclarationVariable(IrDeclarationVariable variable)
    {
        var symbol = variable.Symbol;
        var type = variable.TypeSymbol;
        var name = symbol.Name.FullName;

        if (variable.Initializer is not null)
        {
            var exprResult = OnExpression(variable.Initializer);
            if (exprResult is IrConstant value)
            {
                if (TypeSymbol.IsBoolean(type))
                    _state.SetVariable(name, value.ToBool());

                else if (TypeSymbol.IsI8(type))
                    _state.SetVariable(name, value.ToI8());
                else if (TypeSymbol.IsU8(type))
                    _state.SetVariable(name, value.ToU8());
                else if (TypeSymbol.IsI16(type))
                    _state.SetVariable(name, value.ToI16());
                else if (TypeSymbol.IsU16(type))
                    _state.SetVariable(name, value.ToU16());
                else if (TypeSymbol.IsI32(type))
                    _state.SetVariable(name, value.ToI32());
                else if (TypeSymbol.IsU32(type))
                    _state.SetVariable(name, value.ToU32());
                else if (TypeSymbol.IsI64(type))
                    _state.SetVariable(name, value.ToI64());
                else if (TypeSymbol.IsU64(type))
                    _state.SetVariable(name, value.ToU64());

                else if (TypeSymbol.IsF16(type))
                    _state.SetVariable(name, value.ToF16());
                else if (TypeSymbol.IsF32(type))
                    _state.SetVariable(name, value.ToF32());
                else if (TypeSymbol.IsF64(type))
                    _state.SetVariable(name, value.ToF64());
                else if (TypeSymbol.IsF96(type))
                    _state.SetVariable(name, value.ToF96());

                else if (TypeSymbol.IsC16(type))
                    _state.SetVariable(name, value.ToC16());
                else if (TypeSymbol.IsStr(type))
                    _state.SetVariable(name, value.ToStr());
            }
            else if (exprResult is not null)
            {
                _state.SetVariable(name, exprResult);
            }
        }

        return null;
    }

    public override object? OnDeclarationType(IrDeclarationType type)
    {
        _state.DeclareType(type);
        return null;
    }

    public override object? OnDeclarationFunction(IrDeclarationFunction function)
    {
        // functions are evaluated when called
        _state.DeclareFunction(function);
        return null;
    }

    public override object? OnExpressionBinary(IrExpressionBinary expression)
    {
        var left = (IrConstant)OnExpression(expression.Left)!;
        var right = (IrConstant)OnExpression(expression.Right)!;

        var result = EvaluateConstant.Evaluate(
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

    public override object? OnExpressionInvocation(IrExpressionInvocation invocation)
    {
        if (_state.TryLookupFunction(invocation.Symbol.Name.FullName, out var function))
        {
            using var scope = NewScope(function.Scope);

            var args = invocation.Arguments
                .Select(a => (IrConstant?)OnExpression(a.Expression))
                .ToArray();

            // TODO: use default parameter values.
            // register parameter values as local vars
            for (var i = 0; i < Math.Min(args.Length, function.Parameters.Length); i++)
            {
                Debug.Assert(args[i] is not null, $"Argument at index {i} is null.");
                _state.SetVariable(function.Parameters[i].Symbol.Name.FullName, args[i]!.Value);
            }

            var result = OnCodeBlock(function.Body);
            // function.ReturnType?
            return result;
        }

        _state.Diagnostics.FunctionNotFound(invocation.Syntax.Location, invocation.Symbol.Name.FullOriginalName);
        return null;
    }

    public override object? OnExpressionTypeInitializer(IrExpressionTypeInitializer expression)
    {
        if (!_state.TryLookupType(expression.TypeSymbol.Name.FullName, out var typeDecl))
        {
            _state.Diagnostics.TypeNotFound(expression.Syntax.Location, expression.TypeSymbol.Name.FullOriginalName);
            return null;
        }

        var fields = new Dictionary<string, object>();
        foreach (var fld in expression.Fields)
        {
            var val = OnTypeInitializerField(fld);
            fields[fld.Field.Name.FullName] = val!;
        }

        var instance = new EvalTypeInstance(typeDecl, fields);

        return instance;
    }

    public override object? OnExpressionMemberAccess(IrExpressionMemberAccess memberAccess)
    {
        IrConstant? value;
        if (memberAccess.Identifier is not null)
        {
            value = (IrConstant?)OnExpressionIdentifier(memberAccess.Identifier);
        }
        else if (memberAccess.Invocation is not null)
        {
            value = (IrConstant?)OnExpressionInvocation(memberAccess.Invocation);
        }
        else
        {
            return IrConstant.Zero;
        }

        if (value is null) return IrConstant.Zero;

        var type = (EvalTypeInstance)value.Value;
        foreach (var fld in memberAccess.Members.Select(fld => fld.Name.FullName))
        {
            if (!type.Fields.TryGetValue(fld, out var val))
            {
                _state.Diagnostics.FieldNotFoundOnType(memberAccess.Syntax.Location,
                    type.TypeDeclaration.Symbol.Name.FullOriginalName, fld);
                return IrConstant.Zero;
            }

            if (val is EvalTypeInstance instance)
            {
                type = instance;
            }
            else
            {
                return (IrConstant)val;
            }
        }

        return new IrConstant(type);
    }

    public override object? OnStatementAssignment(IrStatementAssignment statement)
    {
        var value = (IrConstant)OnExpression(statement.Expression)!;

        if (!_state.TrySetVariable(statement.Symbol.Name.FullName, value.Value))
        {
            _state.Diagnostics.VariableNotFound(statement.Syntax.Location, statement.Symbol.Name.FullOriginalName);
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

    public override object? OnStatementLoop(IrStatementLoop statement)
    {
        if (statement.Expression is not null)
        {
            if (statement.Expression is IrExpressionRange rangeExpr)
            {
                var start = (IrConstant)OnExpression(rangeExpr.Start!)!;
                var end = (IrConstant)OnExpression(rangeExpr.End!)!;
                for (var i = start.ToI32(); i < end.ToI32(); i++)
                {
                    _ = OnCodeBlock(statement.CodeBlock);
                }
            }
            else if (statement.Expression is IrExpressionIdentifier identExpr)
            {
                var value = (IrConstant)OnExpression(identExpr)!;
                for (var i = 0; i < value.ToI32(); i++)
                {
                    _ = OnCodeBlock(statement.CodeBlock);
                }
            }
            else if (statement.Expression is IrExpressionLiteral litExpr)
            {
                var value = (IrConstant)OnExpression(litExpr)!;
                for (var i = 0; i < value.ToI32(); i++)
                {
                    _ = OnCodeBlock(statement.CodeBlock);
                }
            }
            else
            {
                while (true)
                {
                    // re-evaluate expression on each iteration
                    var value = (IrConstant)OnExpression(statement.Expression)!;
                    if (value.ToBool())
                        _ = OnCodeBlock(statement.CodeBlock);
                    else
                        break;
                }
            }
        }
        else
            _state.Diagnostics.Add(DiagnosticMessageKind.Error,
                statement.Syntax.Location, "Repl does not do endless loops.");

        return null;
    }

    private sealed class EvaluationStatePopper(EvalWalker walker) : IDisposable
    {
        public void Dispose()
        {
            var state = walker._state;
            if (state.Parent is null)
                throw new MajaException("Evaluation Stack inbalance.");

            walker._state = state.Parent;
        }
    }
}
