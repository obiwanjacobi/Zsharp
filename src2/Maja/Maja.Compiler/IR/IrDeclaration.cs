using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrDeclaration : IrNode
{
    protected IrDeclaration(IrLocality locality)
    {
        Locality = locality;
    }
    protected IrDeclaration(SyntaxNode syntax, IrLocality locality)
        : base(syntax)
    {
        Locality = locality;
    }

    public IrLocality Locality { get; }
}

internal sealed class IrDeclarationFunction : IrDeclaration, IrContainer
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

        Type = new IrTypeFunction(syntax, symbol.Type,
            typeParameters, parameters.Select(p => p.Type), returnType);
    }

    public FunctionSymbol Symbol { get; }
    public ImmutableArray<IrTypeParameter> TypeParameters { get; }
    public ImmutableArray<IrParameter> Parameters { get; }
    public IrType ReturnType { get; }
    public IrCodeBlock Body { get; }
    public IrFunctionScope Scope { get; }

    public IrTypeFunction Type { get; }

    public new FunctionDeclarationSyntax Syntax
        => (FunctionDeclarationSyntax)base.Syntax;

    public IEnumerable<T> GetDescendentsOfType<T>() where T : IrNode
        => TypeParameters.GetDescendentsOfType<T>()
        .Concat(Parameters.GetDescendentsOfType<T>())
        .Concat(ReturnType.GetDescendentsOfType<T>())
        .Concat(Body.GetDescendentsOfType<T>());
}

internal sealed class IrDeclarationVariable : IrDeclaration, IrContainer
{
    internal IrDeclarationVariable(VariableSymbol symbol, TypeSymbol type, IrExpression? initializer)
        : base(IrLocality.None)
    {
        Symbol = symbol;
        TypeSymbol = type;
        Initializer = initializer;
    }
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

    public IEnumerable<T> GetDescendentsOfType<T>() where T : IrNode
        => Initializer.GetDescendentsOfType<T>();
}

internal sealed class IrDeclarationType : IrDeclaration, IrContainer
{
    public IrDeclarationType(TypeDeclarationSyntax syntax, TypeSymbol symbol, IEnumerable<IrTypeParameter> typeParameters,
        IEnumerable<IrTypeMemberEnum> enums, IEnumerable<IrTypeMemberField> fields, IEnumerable<IrTypeMemberRule> rules,
        IrType? baseType, IrTypeScope scope, IrLocality locality)
        : base(syntax, locality)
    {
        Symbol = symbol;
        Scope = scope;
        BaseType = baseType;
        TypeParameters = typeParameters.ToImmutableArray();
        Enums = enums.ToImmutableArray();
        Fields = fields.ToImmutableArray();
        Rules = rules.ToImmutableArray();
    }

    public TypeSymbol Symbol { get; }
    public IrTypeScope Scope { get; }
    public IrType? BaseType { get; }
    public ImmutableArray<IrTypeParameter> TypeParameters { get; }
    public ImmutableArray<IrTypeMemberEnum> Enums { get; }
    public ImmutableArray<IrTypeMemberField> Fields { get; }
    public ImmutableArray<IrTypeMemberRule> Rules { get; }

    public new TypeDeclarationSyntax Syntax
        => (TypeDeclarationSyntax)base.Syntax;

    public IEnumerable<T> GetDescendentsOfType<T>() where T : IrNode
        => TypeParameters.GetDescendentsOfType<T>()
        .Concat(Enums.GetDescendentsOfType<T>())
        .Concat(Fields.GetDescendentsOfType<T>())
        .Concat(Rules.GetDescendentsOfType<T>());
}