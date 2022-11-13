using System;
using System.Collections.Generic;
using Maja.Compiler.External;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Compilation;

/// <summary>
/// Represents compilation of source code (syntax).
/// </summary>
public sealed class Compilation
{
    private readonly List<SyntaxTree> _syntaxTrees = new();
    private readonly IExternalModuleLoader _moduleLoader;

    internal Compilation(SyntaxTree syntaxTree, IExternalModuleLoader moduleLoader)
    {
        _syntaxTrees.Add(syntaxTree);
        _moduleLoader = moduleLoader;
    }

    public static Compilation Create(
        SyntaxTree syntaxTree/*,
        IEnumerable<AssemblyReference> references*/)
    {
        var moduleLoader = new AssemblyManagerBuilder()
            .AddSystemAll()
            .ToModuleLoader();
        return new Compilation(syntaxTree, moduleLoader);
    }

    public CompilationModel GetModel(SyntaxTree tree)
    {
        if (!_syntaxTrees.Contains(tree))
            throw new ArgumentException("Specified SyntaxTree does not belong to this Compilation.");

        return new CompilationModel(this, tree, _moduleLoader);
    }
}
