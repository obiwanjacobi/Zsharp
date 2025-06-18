using System;
using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.External;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR.Processor;

internal sealed class IrProcessBuilder
{
    private readonly IrProcessManager _processMgr = new();
    private readonly Stack<IrScope> _scopes = new();
    private readonly IExternalModuleLoader _moduleLoader;

    private IrProcessBuilder(IExternalModuleLoader moduleLoader, IrScope? parentScope)
    {
        _scopes.Push(parentScope ?? new IrGlobalScope());
        _moduleLoader = moduleLoader;
    }

    private T PopScope<T>()
        where T : IrScope
    {
        var scope = (T)_scopes.Pop();
        //scope.Freeze();
        return scope;
    }
    private void PushScope(IrScope scope)
        => _scopes.Push(scope);
    private IrScope CurrentScope
        => _scopes.Peek();

    public const string DefaultModuleName = "DefMod";

    // entry point for builder
    public static IrProgram Program(SyntaxTree syntaxTree, IExternalModuleLoader moduleLoader, IrScope? parentScope = null)
    {
        if (syntaxTree.Diagnostics.Any())
            throw new InvalidOperationException("Cannot Compile when there are syntax errors.");

        var builder = new IrProcessBuilder(moduleLoader, parentScope);

        builder._processMgr.Start();
        builder.Module(syntaxTree.Root);
        var result = builder._processMgr.GetResult();
        builder._processMgr.Stop();

        return new IrProgram(syntaxTree.Root, result.Module, result.Diagnostics);
    }

    private void Module(CompilationUnitSyntax syntax)
    {
        var moduleScope = new IrModuleScope(syntax.Module?.Text ?? DefaultModuleName, CurrentScope);

        // TODO: process imports and exports
        // if (!ProcessImport(syntax.Imports, moduleScope)){ diag }

        foreach (var decl in syntax.Members)
        {
            var declItem = IrProcessBuilder.Declaration(decl, CurrentScope);
            IrProcessReference.Connect(declItem, declItem);

            _processMgr.Enqueue(declItem);
        }

        //foreach (var stat in item.Syntax.Statements)
        //{
        //    IrProcessBuilder.Statement(stat, item.Scope);
        //}
    }

    internal static IrProcess Declaration(DeclarationMemberSyntax declarationSyntax, IrScope parentScope)
    {
        return declarationSyntax switch
        {
            DeclarationFunctionSyntax fun => DeclarationFunction(fun, parentScope),
            DeclarationTypeSyntax typ => DeclarationType(typ, parentScope),
            DeclarationVariableSyntax var => CreateVariable(var, parentScope),
            _ => throw new NotSupportedException($"Cannot create a process object for '{declarationSyntax.GetType().Name}' syntax.")
        };
    }

    internal static IrProcessFunction DeclarationFunction(DeclarationFunctionSyntax syntax, IrScope parentScope)
    {
        var scope = new IrFunctionScope(syntax.Name.Text, parentScope);
        return new IrProcessFunction(syntax, scope);
    }

    internal static IrProcessType DeclarationType(DeclarationTypeSyntax syntax, IrScope parentScope)
    {
        var scope = new IrTypeScope(syntax.Name.Text, parentScope);
        return new IrProcessType(syntax, scope);
    }

    internal static IrProcessVariable CreateVariable(DeclarationVariableSyntax syntax, IrScope parentScope)
    {
        return new IrProcessVariable(syntax, parentScope);
    }
}
