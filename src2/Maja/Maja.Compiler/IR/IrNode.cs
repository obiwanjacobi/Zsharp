using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrNode
{
    protected IrNode()
    { }

    protected IrNode(SyntaxNode syntax)
    {
        _syntax = syntax;
    }

    public bool HasSyntax
        => _syntax is not null;

    private readonly SyntaxNode? _syntax;
    public SyntaxNode Syntax => _syntax 
        ?? throw new MajaException("No Syntax was set for this generated IR node.");
}
