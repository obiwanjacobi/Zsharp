﻿using System.Collections.Generic;
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
    public IrDeclarationFunction(DeclarationFunctionSyntax syntax, DeclaredFunctionSymbol symbol,
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

    public bool IsTemplate
        => TypeParameters.OfType<IrTypeParameterTemplate>().Any();

    public DeclaredFunctionSymbol Symbol { get; }
    public ImmutableArray<IrTypeParameter> TypeParameters { get; }
    public ImmutableArray<IrParameter> Parameters { get; }
    public IrType ReturnType { get; }
    public IrCodeBlock Body { get; }
    public IrFunctionScope Scope { get; }

    public IrTypeFunction Type { get; }

    public new DeclarationFunctionSyntax Syntax
        => (DeclarationFunctionSyntax)base.Syntax;

    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
        => TypeParameters.GetDescendantsOfType<T>()
        .Concat(Parameters.GetDescendantsOfType<T>())
        .Concat(ReturnType.GetDescendantsOfType<T>())
        .Concat(Body.GetDescendantsOfType<T>());
}

internal sealed class IrDeclarationVariable : IrDeclaration, IrContainer
{
    internal IrDeclarationVariable(DeclaredVariableSymbol symbol, TypeSymbol type, IrOperatorAssignment? assignmentOperator, IrExpression? initializer)
        : base(IrLocality.None)
    {
        Symbol = symbol;
        TypeSymbol = type;
        AssignmentOperator = assignmentOperator;
        Initializer = initializer;
    }
    public IrDeclarationVariable(DeclarationVariableSyntax syntax, DeclaredVariableSymbol symbol, TypeSymbol type, IrOperatorAssignment? assignmentOperator, IrExpression? initializer)
        : base(syntax, IrLocality.None)
    {
        Symbol = symbol;
        TypeSymbol = type;
        AssignmentOperator = assignmentOperator;
        Initializer = initializer;
    }

    public DeclaredVariableSymbol Symbol { get; }
    public TypeSymbol TypeSymbol { get; }
    public IrOperatorAssignment? AssignmentOperator { get; }
    public IrExpression? Initializer { get; }

    public new DeclarationVariableSyntax Syntax
        => (DeclarationVariableSyntax)base.Syntax;

    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
        => Initializer.GetDescendantsOfType<T>();
}

internal sealed class IrDeclarationType : IrDeclaration, IrContainer
{
    public IrDeclarationType(DeclarationTypeSyntax syntax, DeclaredTypeSymbol symbol, IEnumerable<IrTypeParameter> typeParameters,
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

    public bool IsTemplate
        => TypeParameters.OfType<IrTypeParameterTemplate>().Any();

    public DeclaredTypeSymbol Symbol { get; }
    public IrTypeScope Scope { get; }
    public IrType? BaseType { get; }
    public ImmutableArray<IrTypeParameter> TypeParameters { get; }
    public ImmutableArray<IrTypeMemberEnum> Enums { get; }
    public ImmutableArray<IrTypeMemberField> Fields { get; }
    public ImmutableArray<IrTypeMemberRule> Rules { get; }

    public new DeclarationTypeSyntax Syntax
        => (DeclarationTypeSyntax)base.Syntax;

    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
        => TypeParameters.GetDescendantsOfType<T>()
        .Concat(Enums.GetDescendantsOfType<T>())
        .Concat(Fields.GetDescendantsOfType<T>())
        .Concat(Rules.GetDescendantsOfType<T>());
}