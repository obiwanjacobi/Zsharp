﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Maja.Compiler.EmitCS.CSharp;
using Maja.Compiler.External;
using Maja.Compiler.IR;

namespace Maja.Compiler.EmitCS;

/// <summary>
/// Generates code for all the Ir objects
/// </summary>
internal class CodeBuilder : IrWalker<object?>
{
    private readonly CSharpWriter _writer = new();
    private readonly Stack<Scope> _scopes = new();

    private bool IsNamespaceScope
        => _scopes.Count == 1;

    private bool IsModuleClassScope
        => _scopes.Count == 2 && _scopes.Peek().Type is not null;
    private CSharp.Type CurrentType
        => _scopes.Peek().Type ?? throw new InvalidOperationException("Not a Type.");

    private bool IsFunctionScope
        => _scopes.Peek().Method is not null;

    public override object? OnProgram(IrProgram program)
    {
        base.OnProgram(program);
        _writer.CloseScope();   // namespace
        _scopes.Pop();

        Debug.Assert(_scopes.Count == 0);
        return null;
    }

    public override object? OnModule(IrModule module)
    {
        var ns = CSharpFactory.CreateNamespace(module);
        var mc = CSharpFactory.CreateModuleClass(module);
        ns.AddType(mc);

        _writer.StartNamespace(ns.Name);
        _scopes.Push(new Scope(ns));
        return null;
    }

    public override object? OnCompilation(IrCompilation compilation)
    {
        OnImports(compilation.Imports);
        OnExports(compilation.Exports);

        var ns = _scopes.Peek().Namespace!;
        var mc = ns.GetModuleClass();
        if (compilation.Exports.Any())
            mc.AccessModifiers = AccessModifiers.Public;

        _writer.StartType(mc);
        _scopes.Push(new Scope(mc));
        var mi = CSharpFactory.CreateModuleInitializer(mc.Name);
        mc.AddMethod(mi);
        
        _writer.StartMethod(mi);
        _writer.OpenMethodBody();
        _scopes.Push(new Scope(mi));
        OnStatements(compilation.Statements);
        _scopes.Pop();
        _writer.CloseScope();

        OnDeclarations(compilation.Declarations);
        _scopes.Pop();
        _writer.CloseScope();
        return null;
    }

    public override object? OnImport(IrImport import)
    {
        _writer.Using(import.SymbolName.FullName);
        return null;
    }

    public override object OnVariableDeclaration(IrVariableDeclaration variable)
    {
        var netType = MajaTypeMapper.MapToDotNetType(variable.TypeSymbol);
        
        if (IsModuleClassScope)
        {
            var field = CSharpFactory.CreateField(variable.Symbol.Name.Value, netType);
            CurrentType.AddField(field);
        }

        _writer.WriteVariable(netType, variable.Symbol.Name.Value);
        var init = variable.Initializer;
        if (init != null)
        {
            _writer.Assignment();
            OnExpression(init);
        }
        _writer.Semicolon();

        return null;
    }

    public override object? OnOperatorBinary(IrBinaryOperator op)
    {
        var netOperator = op.Kind switch
        {
            IrBinaryOperatorKind.Add => "+",
            IrBinaryOperatorKind.And => "and",
            IrBinaryOperatorKind.BitwiseAnd => "&&",
            IrBinaryOperatorKind.BitwiseOr => "||",
            IrBinaryOperatorKind.BitwiseShiftLeft => "<<",
            IrBinaryOperatorKind.BitwiseShiftRight => ">>",
            IrBinaryOperatorKind.BitwiseXor => "^",
            IrBinaryOperatorKind.Divide => "/",
            IrBinaryOperatorKind.Equals => "==",
            IrBinaryOperatorKind.Greater => ">",
            IrBinaryOperatorKind.GreaterOrEquals => ">=",
            IrBinaryOperatorKind.Lesser => "<",
            IrBinaryOperatorKind.LesserOrEquals => "<=",
            IrBinaryOperatorKind.Modulo => "%",
            IrBinaryOperatorKind.Multiply => "*",
            IrBinaryOperatorKind.NotEquals => "!=",
            IrBinaryOperatorKind.Or => "or",
            IrBinaryOperatorKind.Subtract => "-",
            _ => throw new NotSupportedException($"Binary Operator '{op.Kind}' is not supported.")
        };
        _writer.Write(netOperator);
        return null;
    }

    public override object? OnExpressionLiteral(IrExpressionLiteral expression)
    {
        _writer.Write(expression.ConstantValue!.Value?.ToString());
        return null;
    }

    public override string ToString()
        => _writer.ToString();
}
