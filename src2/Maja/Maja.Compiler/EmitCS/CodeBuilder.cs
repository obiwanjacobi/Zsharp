using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Maja.Compiler.EmitCS.CSharp;
using Maja.Compiler.IR;

namespace Maja.Compiler.EmitCS;

/// <summary>
/// Generates code for all the Ir objects
/// </summary>
internal class CodeBuilder : IrWalker<object?>
{
    private readonly CSharpWriter _writer = new();
    private readonly Stack<Scope> _scopes = new();

    public override object OnProgram(IrProgram program)
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

    public override string ToString()
        => _writer.ToString();
}
