﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Maja.Compiler.EmitCS.CSharp;
using Maja.Compiler.EmitCS.IR;
using Maja.Compiler.External;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.EmitCS;

/// <summary>
/// Generates code for all the Ir objects
/// </summary>
internal class CodeBuilder : IrWalker<object?>
{
    private readonly CSharpWriter _writer = new();
    private readonly Stack<Scope> _scopes = new();

    private bool IsModuleClassScope
        => _scopes.Count == 2 && IsTypeScope;
    private bool IsTypeScope
        => _scopes.Peek().Type is not null;
    private bool IsFunctionScope
        => _scopes.Peek().Method is not null;

    private CSharp.Namespace CurrentNamespace
        => _scopes.Single(s => s.Namespace is not null).Namespace!;
    private CSharp.Type CurrentType
        => _scopes.Peek().Type ?? throw new InvalidOperationException("Not a Type.");
    private CSharp.Method CurrentMethod
        => _scopes.Peek().Method ?? throw new InvalidOperationException("Not a Method.");

    private CSharpWriter? _tempWriter;
    private CSharpWriter Writer
    {
        get
        {
            if (_tempWriter is not null)
                return _tempWriter;

            if (IsFunctionScope)
                return _scopes.Peek().Method!.Body;

            return _writer;
        }
    }

    private IDisposable SetWriter(CSharpWriter writer)
    {
        _tempWriter = writer;
        return new EndOfScope(() => _tempWriter = null);
    }

    public override object? OnProgram(IrProgram program)
    {
        _ = base.OnProgram(program);
        var ns = _scopes.Pop().Namespace!;
        Debug.Assert(_scopes.Count == 0);

        var serializer = new CSharpSerializer(_writer);
        serializer.Write(ns);
        return ns;
    }

    public override object? OnModule(IrModule module)
    {
        var ns = CSharpFactory.CreateNamespace(module);
        var mc = CSharpFactory.CreateModuleClass(module);
        ns.AddType(mc);

        _scopes.Push(new Scope(ns));
        return null;
    }

    public override object? OnCompilation(IrCompilation compilation)
    {
        _ = OnImports(compilation.Imports);
        _ = OnExports(compilation.Exports);
        
        var ns = CurrentNamespace;
        var mc = ns.GetModuleClass();

        _scopes.Push(new Scope(mc));
        
        if (compilation.Statements.Any())
        {
            var mi = CSharpFactory.CreateModuleInitializer(mc.Name);
            mc.AddMethod(mi);
            _scopes.Push(new Scope(mi));
            _ = OnParameters(Enumerable.Empty<IrParameter>());
            _ = OnStatements(compilation.Statements);
            _scopes.Pop();
        }

        _ = OnDeclarations(compilation.Declarations);

        _scopes.Pop();
        return null;
    }

    public override object? OnImport(IrImport import)
    {
        var ns = CurrentNamespace;
        ns.AddUsing(import.SymbolName.FullName);
        return null;
    }

    public override object? OnDeclarationFunction(IrDeclarationFunction function)
    {
        var netType = MajaTypeMapper.MapToDotNetType(function.ReturnType.Symbol);
        var method = CSharpFactory.CreateMethod(function.Symbol.Name.Value, netType);

        if (function.Locality == IrLocality.Public)
            method.AccessModifiers = AccessModifiers.Public;

        CurrentType.AddMethod(method);
        _scopes.Push(new Scope(method));

        _ = OnTypeParametersGeneric(function.TypeParameters.OfType<IrTypeParameterGeneric>());
        _ = OnParameters(function.Parameters);
        _ = OnCodeBlock(function.Body);

        _ = _scopes.Pop();
        return null;
    }

    public override object? OnTypeParameterGeneric(IrTypeParameterGeneric parameter)
    {
        var p = new TypeParameter(parameter.Symbol.Name.Value);
        CurrentMethod.AddTypeParameter(p);
        return null;
    }

    public override object? OnParameter(IrParameter parameter)
    {
        var netType = MajaTypeMapper.MapToDotNetType(parameter.Type.Symbol);
        var p = new Parameter(parameter.Symbol.Name.Value, netType);
        CurrentMethod.AddParameter(p);
        return null;
    }

    public override object? OnDeclarationVariable(IrDeclarationVariable variable)
    {
        var netType = MajaTypeMapper.MapToDotNetType(variable.TypeSymbol);

        if (IsModuleClassScope)
        {
            var field = CSharpFactory.CreateField(variable.Symbol.Name.Value, netType);
            CurrentType.AddField(field);
            using var eos = SetWriter(new CSharpWriter());
            if (variable.Initializer is not null)
                _ = OnExpression(variable.Initializer);
            field.InitialValue = Writer.ToString();
        }
        else
        {
            Writer.StartVariable(netType, variable.Symbol.Name.Value);
            if (variable.Initializer is not null)
            {
                Writer.Assignment();
                _ = OnExpression(variable.Initializer);
            }

            if (variable.HasSyntax)
                Writer.EndOfLine();
        }
        return null;
    }

    public override object? OnDeclarationType(IrDeclarationType type)
    {
        if (type.Enums.Any())
        {
            return CreateEnum(type);
        }

        return CreateStruct(type);
    }

    private object? CreateEnum(IrDeclarationType type)
    {
        var enumInfo = CSharpFactory.CreateEnum(type.Symbol.Name.Value);
        CurrentNamespace.AddEnum(enumInfo);

        foreach (var opt in type.Enums)
        {
            var enumOpt = CSharpFactory.CreateEnumOption(opt.Symbol.Name.Value, opt.ValueExpression?.ConstantValue?.ToString());
            enumInfo.AddOption(enumOpt);
        }

        return null;
    }

    private object? CreateStruct(IrDeclarationType type)
    {
        var typeInfo = CSharpFactory.CreateType(type.Symbol.Name.Value, type.BaseType?.Symbol.Name.Value);
        CurrentNamespace.AddType(typeInfo);

        foreach (var field in type.Fields)
        {
            var netType = MajaTypeMapper.MapToDotNetType(field.Type.Symbol);
            var prop = CSharpFactory.CreateProperty(field.Symbol.Name.Value, netType);
            prop.AccessModifiers = AccessModifiers.Public;
            prop.FieldModifiers = FieldModifiers.None;
            prop.InitialValue = field.DefaultValue?.ConstantValue?.AsString();

            typeInfo.AddProperty(prop);
        }
        return null;
    }

    public override object? OnStatementIf(IrStatementIf statement)
    {
        Writer.Tab();
        StartStatementIf(statement.Condition);
        _ = OnCodeBlock(statement.CodeBlock);
        Writer.CloseScope();

        if (statement.ElseClause is IrElseClause elseClause)
        {
            _ = OnStatementIf_ElseClause(elseClause);
        }
        else if (statement.ElseIfClause is IrElseIfClause elseIfClause)
        {
            _ = OnStatementIf_ElseIfClause(elseIfClause);
        }
        return null;
    }
    public override object? OnStatementIf_ElseIfClause(IrElseIfClause elseIfClause)
    {
        Writer.Tab().Append("else ");
        StartStatementIf(elseIfClause.Condition);
        _ = OnCodeBlock(elseIfClause.CodeBlock);
        Writer.CloseScope();

        if (elseIfClause.ElseClause is IrElseClause nestedElse)
        {
            _ = OnStatementIf_ElseClause(nestedElse);
        }
        else if (elseIfClause.ElseIfClause is IrElseIfClause nestedElseIf)
        {
            _ = OnStatementIf_ElseIfClause(nestedElseIf);
        }
        return null;
    }
    public override object? OnStatementIf_ElseClause(IrElseClause elseClause)
    {
        Writer.Tab().Append("else");
        Writer.Newline().OpenScope();
        _ = OnCodeBlock(elseClause.CodeBlock);
        Writer.CloseScope();
        return null;
    }
    private void StartStatementIf(IrExpression condition)
    {
        Writer.Write("if (");
        _ = OnExpression(condition);
        Writer.Write(")").Newline().OpenScope();
    }

    public override object? OnStatementLoop(IrStatementLoop statement)
    {
        if (statement.Expression is null)
        {
            Writer.Tab().Append("while (true)");
        }
        else if (statement is IrCodeStatementWhileLoop whileLoop)
        {
            Writer.Tab().Append("while (");
            _ = OnExpression(whileLoop.Expression!);
            Writer.Write(")");
        }
        else if (statement is IrCodeStatementForLoop forLoop)
        {
            Writer.Tab().Append("for (");
            _ = OnDeclarationVariable(forLoop.Initializer);
            Writer.Write(";");
            _ = OnExpression(forLoop.Condition);
            Writer.Write(";");
            _ = OnStatementAssignment(forLoop.Step);
            Writer.Write(")");
        }

        Writer.Newline().OpenScope();
        _ = OnCodeBlock(statement.CodeBlock);
        Writer.CloseScope();
        return null;
    }

    public override object? OnStatementAssignment(IrStatementAssignment statement)
    {
        var name = GetCSharpName(statement.Symbol.Name, statement.Locality);
        Writer.StartAssignment(name);
        _ = OnExpression(statement.Expression);

        if (statement.HasSyntax)
            Writer.EndOfLine();
        return null;
    }

    public override object? OnStatementReturn(IrStatementReturn statement)
    {
        Writer.WriteReturn();
        _ = OnOptionalExpression(Default, statement.Expression);
        Writer.EndOfLine();
        return null;
    }

    public override object? OnExpressionBinary(IrExpressionBinary expression)
    {
        Writer.Write("(");
        OnType(expression.TypeSymbol);
        Writer.Write(")(");
        var res = base.OnExpressionBinary(expression);
        Writer.Write(")");
        return res;
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

        Writer.Write($" {netOperator} ");
        return null;
    }

    public override object? OnExpressionLiteral(IrExpressionLiteral expression)
    {
        if (expression.ConstantValue!.Value is string str)
            Writer.Write($"\"{str}\"");
        else
        {
            // cast
            if (expression.TypeSymbol.Name != TypeSymbol.Unknown.Name)
            {
                Writer.Write("(");
                OnType(expression.TypeSymbol);
                Writer.Write(")");
            }
            Writer.Write(expression.ConstantValue.AsString());
        }
        return null;
    }

    public override object? OnExpressionIdentifier(IrExpressionIdentifier identifier)
    {
        Writer.Write(identifier.Symbol.Name.Value);
        return null;
    }

    public override object? OnExpressionMemberAccess(IrExpressionMemberAccess memberAccess)
    {
        var res = OnExpression(memberAccess.Expression);

        foreach (var field in memberAccess.Members)
        {
            Writer.Write(".")
                .Write(field.Name.Value);
        }

        return res;
    }

    public override object? OnExpressionInvocation(IrExpressionInvocation invocation)
    {
        Writer.WriteSymbol(invocation.Symbol);
        return base.OnExpressionInvocation(invocation);
    }

    public override object? OnInvocationTypeArguments(IEnumerable<IrTypeArgument> arguments)
    {
        if (arguments.Any())
        {
            Writer.Write("<");
            _ = base.OnInvocationTypeArguments(arguments);
            Writer.Write(">");
        }

        return null;
    }
    public override object? OnInvocationArguments(IEnumerable<IrArgument> arguments)
    {
        Writer.Write("(");
        _ = base.OnInvocationArguments(arguments);
        Writer.Write(")");
        return null;
    }

    public override object? OnExpressionTypeInitializer(IrExpressionTypeInitializer expression)
    {
        Writer.Write("new ");
        OnType(expression.TypeSymbol);
        Writer.Write("()")
            .Newline();

        if (expression.Fields.Any())
        {
            Writer.WriteInitializer(expression.Fields, f => f.Field.Name.Value,
                f => { OnExpression(f.Expression); return String.Empty; });
        }

        return null;
    }

    public override object? OnType(IrType type)
        => OnType(type.Symbol);

    public object? OnType(TypeSymbol type)
    {
        var netType = MajaTypeMapper.MapToDotNetType(type);
        Writer.Write(netType);
        return null;
    }

    private static string GetCSharpName(SymbolName name, IrLocality locality)
    {
        return locality switch
        {
            IrLocality.Module => $"{name.Namespace.Value}.Module.{name.Value}",
            _ => name.FullName
        };
    }

    public override string ToString()
        => _writer.ToString();
}
