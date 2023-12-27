using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrTypeInitializer : IrNode
{
    protected IrTypeInitializer(SyntaxNode syntax)
        : base(syntax)
    { }
}

internal sealed class IrTypeInitializerField : IrTypeInitializer
{
    public IrTypeInitializerField(TypeInitializerFieldSyntax syntax, FieldSymbol fieldSymbol, IrExpression expression)
        : base(syntax)
    {
        Field = fieldSymbol;
        Expression = expression;
    }

    public FieldSymbol Field { get; }
    public IrExpression Expression { get; }

    public new TypeInitializerFieldSyntax Syntax
        => (TypeInitializerFieldSyntax)base.Syntax;
}