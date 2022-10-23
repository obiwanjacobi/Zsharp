using Maja.Compiler.Syntax;

namespace Maja.Compiler.Compilation;

/// <summary>
/// Represents compilation of source code (syntax).
/// </summary>
public sealed class Compilation
{
    public static Compilation Create(
        SyntaxTree syntaxTree/*,
        IEnumerable<AssemblyReference> references*/)
    {
        return new Compilation();
    }
}
