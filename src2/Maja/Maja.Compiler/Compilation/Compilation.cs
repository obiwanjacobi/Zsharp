using Maja.Compiler.Syntax;

namespace Maja.Compiler.Compilation;

/// <summary>
/// Represents compilation of source code (syntax).
/// </summary>
public sealed class Compilation
{
    private readonly SyntaxTree _syntaxTree;

    internal Compilation(SyntaxTree syntaxTree)
    {
        _syntaxTree = syntaxTree;
    }

    public static Compilation Create(
        SyntaxTree syntaxTree/*,
        IEnumerable<AssemblyReference> references*/)
    {
        return new Compilation(syntaxTree);
    }

    public SemanticModel GetSemanticModel(SyntaxTree tree)
    {
        return new SemanticModel(this);
    }
}
