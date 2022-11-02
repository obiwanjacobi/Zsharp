using System;
using System.Collections.Generic;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Compilation;

/// <summary>
/// Represents compilation of source code (syntax).
/// </summary>
public sealed class Compilation
{
    private readonly List<SyntaxTree> _syntaxTrees = new();

    internal Compilation(SyntaxTree syntaxTree)
    {
        _syntaxTrees.Add(syntaxTree);
    }

    public static Compilation Create(
        SyntaxTree syntaxTree/*,
        IEnumerable<AssemblyReference> references*/)
    {
        return new Compilation(syntaxTree);
    }

    public CompilationModel GetModel(SyntaxTree tree)
    {
        if (!_syntaxTrees.Contains(tree))
            throw new ArgumentException("Specified SyntaxTree does not belong to this Compilation.");

        return new CompilationModel(this, tree);
    }
}
