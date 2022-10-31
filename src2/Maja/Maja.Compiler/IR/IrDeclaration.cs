﻿using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrDeclaration : IrNode
{
    public IrDeclaration(SyntaxNode syntax)
        : base(syntax)
    { }
}

internal sealed class IrFunctionDeclaration : IrDeclaration
{
    public IrFunctionDeclaration(FunctionDeclarationSyntax syntax, FunctionSymbol symbol,
        IEnumerable<IrParameter> parameters, IrType? returnType, IrScope scope, IrCodeBlock codeBlock)
        : base(syntax)
    {
        Parameters = parameters.ToImmutableArray();
        ReturnType = returnType;
        Scope = scope;
        Symbol = symbol;
        Body = codeBlock;
    }

    public FunctionSymbol Symbol { get; }
    public ImmutableArray<IrParameter> Parameters { get; }
    public IrType? ReturnType { get; }
    public IrCodeBlock Body { get; }
    public IrScope Scope { get; }

    public new FunctionDeclarationSyntax Syntax
        => (FunctionDeclarationSyntax)base.Syntax;
}

internal sealed class IrVariableDeclaration : IrDeclaration
{
    public IrVariableDeclaration(VariableDeclarationSyntax syntax, VariableSymbol symbol, TypeSymbol type, IrExpression? initializer)
        : base(syntax)
    {
        Symbol = symbol;
        Type = type;
        Initializer = initializer;
    }

    public VariableSymbol Symbol { get; }
    public TypeSymbol Type { get; }
    public IrExpression? Initializer { get; }

    public new VariableDeclarationSyntax Syntax
        => (VariableDeclarationSyntax)base.Syntax;
}

internal sealed class IrTypeDeclaration : IrDeclaration
{
    public IrTypeDeclaration(TypeDeclarationSyntax syntax, TypeSymbol symbol,
        IEnumerable<IrTypeMemberEnum> enums, IEnumerable<IrTypeMemberField> fields, IEnumerable<IrTypeMemberRule> rules)
        : base(syntax)
    {
        Symbol = symbol;
        Enums = enums;
        Fields = fields;
        Rules = rules;
    }

    public TypeSymbol Symbol { get; }
    public IEnumerable<IrTypeMemberEnum> Enums { get; }
    public IEnumerable<IrTypeMemberField> Fields { get; }
    public IEnumerable<IrTypeMemberRule> Rules { get; }

    public new TypeDeclarationSyntax Syntax
        => (TypeDeclarationSyntax)base.Syntax;
}