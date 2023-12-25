using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Maja.Compiler.EmitCS.CSharp;
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

    private CSharp.Type CurrentType
        => _scopes.Peek().Type ?? throw new InvalidOperationException("Not a Type.");
    private CSharp.Method CurrentMethod
        => _scopes.Peek().Method ?? throw new InvalidOperationException("Not a Method.");

    public override object? OnProgram(IrProgram program)
    {
        _ = base.OnProgram(program);
        _writer.CloseScope();
        var ns = _scopes.Pop().Namespace;

        Debug.Assert(_scopes.Count == 0);
        return ns;
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
        var result = OnImports(compilation.Imports);
        result = AggregateResult(result, OnExports(compilation.Exports));

        var ns = _scopes.Peek().Namespace!;
        var mc = ns.GetModuleClass();
        if (compilation.Exports.Any())
            mc.AccessModifiers = AccessModifiers.Public;

        _writer.StartType(mc);
        _scopes.Push(new Scope(mc));

        if (compilation.Statements.Any())
        {
            var mi = CSharpFactory.CreateModuleInitializer(mc.Name);
            mc.AddMethod(mi);

            _writer.StartMethod(mi);
            OnParameters(Enumerable.Empty<IrParameter>());
            _writer.OpenMethodBody();
            _scopes.Push(new Scope(mi));
            result = AggregateResult(result, OnStatements(compilation.Statements));
            _scopes.Pop();
            _writer.CloseScope();
        }

        result = AggregateResult(result, OnDeclarations(compilation.Declarations));
        _scopes.Pop();
        _writer.CloseScope();
        return result;
    }

    public override object? OnImport(IrImport import)
    {
        _writer.Using(import.SymbolName.FullName);
        return null;
    }

    public override object? OnDeclarationFunction(IrDeclarationFunction function)
    {
        var netType = MajaTypeMapper.MapToDotNetType(function.ReturnType.Symbol);
        var method = CSharpFactory.CreateMethod(
            function.Symbol.Name.Value, netType);
        CurrentType.AddMethod(method);
        _scopes.Push(new Scope(method));

        // TODO: is export?

        _writer.StartMethod(method);

        var result = OnTypeParameters(function.TypeParameters.OfType<IrTypeParameterGeneric>());
        result = AggregateResult(result, OnParameters(function.Parameters));
        _writer.OpenMethodBody();
        result = AggregateResult(result, OnCodeBlock(function.Body));
        _writer.CloseScope();

        _ = _scopes.Pop();
        return result;
    }

    public override object? OnTypeParameters(IEnumerable<IrTypeParameterGeneric> parameters)
    {
        object? res = null;
        if (parameters.Any())
        {
            _writer.Write("<");
            res = base.OnTypeParameters(parameters);
            _writer.Write(">");
        }
        return res;
    }

    public override object? OnTypeParameter(IrTypeParameterGeneric parameter)
    {
        if (CurrentMethod.TypeParameters.Any())
            _writer.WriteComma();

        var p = CSharpFactory.CreateTypeParameter(parameter.Symbol.Name.Value);
        CurrentMethod.AddTypeParameter(p);

        _writer.WriteTypeParameter(p);
        return null;
    }

    public override object? OnParameters(IEnumerable<IrParameter> parameters)
    {
        _writer.Write("(");
        var res = base.OnParameters(parameters);
        _writer.Write(")");
        return res;
    }

    public override object? OnParameter(IrParameter parameter)
    {
        if (CurrentMethod.Parameters.Any())
            _writer.WriteComma();

        var netType = MajaTypeMapper.MapToDotNetType(parameter.Type.Symbol);
        var p = CSharpFactory.CreateParameter(parameter.Symbol.Name.Value, netType);
        CurrentMethod.AddParameter(p);

        _writer.WriteParameter(p);
        return null;
    }

    public override object? OnDeclarationVariable(IrDeclarationVariable variable)
    {
        var netType = MajaTypeMapper.MapToDotNetType(variable.TypeSymbol);

        if (IsModuleClassScope)
        {
            var field = CSharpFactory.CreateField(variable.Symbol.Name.Value, netType);
            CurrentType.AddField(field);
            _writer.StartField(field);
        }
        else
        {
            _writer.WriteVariable(netType, variable.Symbol.Name.Value);
        }

        var result = Default;
        var init = variable.Initializer;
        if (init is not null)
        {
            _writer.Assignment();
            result = OnExpression(init);
        }
        _writer.EndOfLine();

        return result;
    }

    public override object? OnStatementAssignment(IrStatementAssignment statement)
    {
        _writer.StartAssignment(statement.Symbol.Name.FullName);
        var result = OnExpression(statement.Expression);
        _writer.EndOfLine();
        return result;
    }

    public override object? OnStatementReturn(IrStatementReturn statement)
    {
        _writer.WriteReturn();
        var result = OnOptionalExpression(Default, statement.Expression);
        _writer.EndOfLine();
        return result;
    }

    public override object? OnExpressionBinary(IrExpressionBinary expression)
    {
        _writer.Write("(");
        OnType(expression.TypeSymbol);
        _writer.Write(")(");
        var res = base.OnExpressionBinary(expression);
        _writer.Write(")");
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

        _writer.Write($" {netOperator} ");
        return null;
    }

    public override object? OnExpressionLiteral(IrExpressionLiteral expression)
    {
        if (expression.ConstantValue!.Value is string str)
            _writer.Write($"\"{str}\"");
        else
        {
            // cast
            if (expression.TypeSymbol.Name != TypeSymbol.Unknown.Name)
            {
                _writer.Write("(");
                OnType(expression.TypeSymbol);
                _writer.Write(")");
            }
            _writer.Write(expression.ConstantValue!.Value.ToString());
        }
        return null;
    }

    public override object? OnExpressionIdentifier(IrExpressionIdentifier identifier)
    {
        _writer.Write(identifier.Symbol.Name.Value);
        return null;
    }

    public override object? OnExpressionInvocation(IrExpressionInvocation invocation)
    {
        _writer.WriteSymbol(invocation.Symbol);
        return base.OnExpressionInvocation(invocation);
    }

    public override object? OnInvocationTypeArguments(IEnumerable<IrTypeArgument> arguments)
    {
        object? res = null;

        if (arguments.Any())
        {
            _writer.Write("<");
            res = base.OnInvocationTypeArguments(arguments);
            _writer.Write(">");
        }

        return res;
    }
    public override object? OnInvocationArguments(IEnumerable<IrArgument> arguments)
    {
        _writer.Write("(");
        var res = base.OnInvocationArguments(arguments);
        _writer.Write(")");
        return res;
    }

    public override object? OnType(IrType type)
        => OnType(type.Symbol);

    public object? OnType(TypeSymbol type)
    {
        var netType = MajaTypeMapper.MapToDotNetType(type);
        _writer.Write(netType);
        return null;
    }

    public override string ToString()
        => _writer.ToString();
}
