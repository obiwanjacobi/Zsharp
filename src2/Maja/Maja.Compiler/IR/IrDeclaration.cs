using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrDeclaration : IrNode
{
    protected IrDeclaration(SyntaxNode syntax, IrLocality locality)
        : base(syntax)
    {
        Locality = locality;
    }

    public IrLocality Locality { get; }
}

internal sealed class IrDeclarationFunction : IrDeclaration
{
    public IrDeclarationFunction(FunctionDeclarationSyntax syntax, FunctionSymbol symbol,
        IEnumerable<IrTypeParameter> typeParameters, IEnumerable<IrParameter> parameters, IrType returnType,
        IrFunctionScope scope, IrCodeBlock codeBlock, IrLocality locality)
        : base(syntax, locality)
    {
        TypeParameters = typeParameters.ToImmutableArray();
        Parameters = parameters.ToImmutableArray();
        ReturnType = returnType;
        Scope = scope;
        Symbol = symbol;
        Body = codeBlock;
    }

    public FunctionSymbol Symbol { get; }
    public ImmutableArray<IrTypeParameter> TypeParameters { get; }
    public ImmutableArray<IrParameter> Parameters { get; }
    public IrType ReturnType { get; }
    public IrCodeBlock Body { get; }
    public IrFunctionScope Scope { get; }

    public new FunctionDeclarationSyntax Syntax
        => (FunctionDeclarationSyntax)base.Syntax;
}

internal sealed class IrDeclarationVariable : IrDeclaration
{
    public IrDeclarationVariable(VariableDeclarationSyntax syntax, VariableSymbol symbol, TypeSymbol type, IrExpression? initializer)
        : base(syntax, IrLocality.None)
    {
        Symbol = symbol;
        TypeSymbol = type;
        Initializer = initializer;
    }

    public VariableSymbol Symbol { get; }
    public TypeSymbol TypeSymbol { get; }
    public IrExpression? Initializer { get; }

    public new VariableDeclarationSyntax Syntax
        => (VariableDeclarationSyntax)base.Syntax;
}

internal sealed class IrDeclarationType : IrDeclaration
{
    public IrDeclarationType(TypeDeclarationSyntax syntax, TypeSymbol symbol,
        IEnumerable<IrTypeMemberEnum> enums, IEnumerable<IrTypeMemberField> fields, IEnumerable<IrTypeMemberRule> rules, IrLocality locality)
        : base(syntax, locality)
    {
        Symbol = symbol;
        Enums = enums.ToImmutableArray();
        Fields = fields.ToImmutableArray();
        Rules = rules.ToImmutableArray();
    }

    public TypeSymbol Symbol { get; }
    public ImmutableArray<IrTypeMemberEnum> Enums { get; }
    public ImmutableArray<IrTypeMemberField> Fields { get; }
    public ImmutableArray<IrTypeMemberRule> Rules { get; }

    public new TypeDeclarationSyntax Syntax
        => (TypeDeclarationSyntax)base.Syntax;
}