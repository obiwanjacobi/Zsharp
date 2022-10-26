using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrNode
{
    protected IrNode(SyntaxNode syntax)
    {
        Syntax = syntax;
    }

    public SyntaxNode Syntax { get; }
}
