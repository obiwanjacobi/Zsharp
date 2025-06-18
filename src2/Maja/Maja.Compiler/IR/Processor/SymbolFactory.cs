using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR.Processor;

internal static class SymbolFactory
{
    public static DeclaredFunctionSymbol ToSymbol(this DeclarationFunctionSyntax syntax, IrScope scope)
    {
        var typeParameterSymbols = syntax.TypeParameters.ToSymbols(scope);
        var parameterSymbols = syntax.Parameters.ToSymbols(scope);
        var retTypeSymbol = syntax.ReturnType?.ToSymbol(scope);
        return new DeclaredFunctionSymbol(new(scope.FullName, syntax.Name.Text), typeParameterSymbols, parameterSymbols, retTypeSymbol);
    }

    public static IEnumerable<ParameterSymbol> ToSymbols(this IEnumerable<ParameterSyntax> syntax, IrScope scope)
    {
        return syntax.Select(p => p.ToSymbol(scope));
    }

    public static ParameterSymbol ToSymbol(this ParameterSyntax syntax, IrScope scope)
    {
        return new(syntax.Name.Text, syntax.Type.ToSymbol(scope));
    }

    public static DeclaredVariableSymbol ToSymbol(this DeclarationVariableSyntax syntax, IrScope scope)
    {
        return syntax switch
        {
            DeclarationVariableInferredSyntax dvis => dvis.ToSymbol(scope),
            DeclarationVariableTypedSyntax dvts => dvts.ToSymbol(scope),
            _ => throw new MajaException($"Unsupported DeclarationVariableSyntax class: {syntax.GetType().FullName}.")
        };
    }

    public static DeclaredVariableSymbol ToSymbol(this DeclarationVariableInferredSyntax syntax, IrScope scope)
    {
        return new(new(syntax.Name.Text), TypeSymbol.Unknown);
    }

    public static DeclaredVariableSymbol ToSymbol(this DeclarationVariableTypedSyntax syntax, IrScope scope)
    {
        return new(new(syntax.Name.Text), syntax.Type.ToSymbol(scope));
    }

    public static DeclaredTypeSymbol ToSymbol(this DeclarationTypeSyntax syntax, IrScope scope)
    {
        var typeParameterSymbols = syntax.TypeParameters.ToSymbols(scope);
        var enumSymbols = syntax.Enums?.Items.ToSymbols(scope) ?? [];
        var fieldSymbols = syntax.Fields?.Items.ToSymbols(scope) ?? [];
        var ruleSymbols = new List<RuleSymbol>();
        var baseTypeSymbol = syntax.BaseType?.ToSymbol(scope);

        return new DeclaredTypeSymbol(new(scope.FullName, syntax.Name.Text),
            typeParameterSymbols, enumSymbols, fieldSymbols, ruleSymbols, baseTypeSymbol);
    }

    public static IEnumerable<FieldSymbol> ToSymbols(this IEnumerable<MemberFieldSyntax> syntax, IrScope scope)
    {
        return syntax.Select(fs => fs.ToSymbol(scope));
    }

    public static FieldSymbol ToSymbol(this MemberFieldSyntax syntax, IrScope scope)
    {
        return new(new(syntax.Name.Text), syntax.Type.ToSymbol(scope));
    }

    public static IEnumerable<EnumSymbol> ToSymbols(this IEnumerable<MemberEnumSyntax> syntax, IrScope scope)
    {
        return syntax.Select(es => es.ToSymbol(scope));
    }

    public static EnumSymbol ToSymbol(this MemberEnumSyntax syntax, IrScope scope)
    {
        // TODO: Type and value?
        return new(new(syntax.Name.Text), TypeSymbol.Unknown, new());
    }

    public static IEnumerable<TypeParameterSymbol> ToSymbols(this IEnumerable<TypeParameterSyntax> syntax, IrScope scope)
    {
        return syntax.Select(tp => tp.ToSymbol(scope));
    }

    public static TypeParameterSymbol ToSymbol(this TypeParameterSyntax syntax, IrScope scope)
    {
        var name = syntax.Type.Name.Text;

        return syntax switch
        {
            TypeParameterGenericSyntax tpgs => new TypeParameterGenericSymbol(name),
            TypeParameterTemplateSyntax tpts => new TypeParameterTemplateSymbol(name),
            _ => throw new MajaException($"Unsupported TypeParameterSymbol class: {syntax.GetType().FullName}")
        };
    }

    public static TypeSymbol ToSymbol(this TypeSyntax syntax, IrScope scope)
    {
        return new UnresolvedTypeSymbol(new(syntax.Name.Text));
    }

    public static IEnumerable<Symbol.Symbol> ToSymbols(this ExpressionSyntax syntax, IrScope scope)
    {
        return syntax switch
        {
            ExpressionBinarySyntax ebs => ebs.ToSymbols(scope),
            ExpressionUnarySyntax eus => eus.ToSymbols(scope),
            ExpressionTypeInitializerSyntax etis => etis.ToSymbols(scope),
            ExpressionIdentifierSyntax eis => eis.ToSymbols(scope),
            ExpressionRangeSyntax ers => ers.ToSymbols(scope),
            ExpressionInvocationSyntax eis => eis.ToSymbols(scope),
            ExpressionMemberAccessSyntax emas => emas.ToSymbols(scope),
            _ => []
        };
    }

    public static IEnumerable<Symbol.Symbol> ToSymbols(this ExpressionBinarySyntax syntax, IrScope scope)
    {
        var left = syntax.Left.ToSymbols(scope);
        var right = syntax.Right.ToSymbols(scope);
        return left.Concat(right);
    }

    public static IEnumerable<Symbol.Symbol> ToSymbols(this ExpressionUnarySyntax syntax, IrScope scope)
    {
        return syntax.Operand.ToSymbols(scope);
    }

    public static IEnumerable<Symbol.Symbol> ToSymbols(this ExpressionTypeInitializerSyntax syntax, IrScope scope)
    {
        var name = new TypeSymbol(new SymbolName(syntax.Type.Name.Text));
        var init = syntax.FieldInitializers.ToSymbols(scope);
        return init.Concat([name]);
    }

    public static IEnumerable<Symbol.Symbol> ToSymbols(this IEnumerable<TypeInitializerFieldSyntax> syntax, IrScope scope)
    {
        return syntax.SelectMany(tif => tif.ToSymbols(scope));
    }

    public static IEnumerable<Symbol.Symbol> ToSymbols(this TypeInitializerFieldSyntax syntax, IrScope scope)
    {
        var name = new FieldSymbol(new SymbolName(syntax.Name.Text), TypeSymbol.Unknown);
        var init = syntax.Expression.ToSymbols(scope);
        return init.Concat([name]);
    }

    public static Symbol.Symbol ToSymbol(this ExpressionIdentifierSyntax syntax, IrScope scope)
    {
        return new UnresolvedVariableSymbol(new SymbolName(syntax.Name.Text));
    }

    public static IEnumerable<Symbol.Symbol> ToSymbols(this ExpressionRangeSyntax syntax, IrScope scope)
    {
        var start = syntax.Start?.ToSymbols(scope) ?? [];
        var end = syntax.End?.ToSymbols(scope) ?? [];
        return start.Concat(end);
    }

    public static IEnumerable<Symbol.Symbol> ToSymbols(this ExpressionInvocationSyntax syntax, IrScope scope)
    {
        var args = syntax.Arguments.ToSymbols(scope);
        var name = new UnresolvedFunctionSymbol(new(syntax.Identifier.Name.Text), args.Count());
        return args.Concat([name]);
    }

    public static IEnumerable<Symbol.Symbol> ToSymbols(this IEnumerable<ArgumentSyntax> syntax, IrScope scope)
    {
        return syntax.SelectMany(a => a.ToSymbols(scope));
    }

    public static IEnumerable<Symbol.Symbol> ToSymbols(this ArgumentSyntax syntax, IrScope scope)
    {
        return syntax.Expression.ToSymbols(scope);
    }

    public static IEnumerable<Symbol.Symbol> ToSymbols(this ExpressionMemberAccessSyntax syntax, IrScope scope)
    {
        var name = new FieldSymbol(new SymbolName(syntax.Name.Text), TypeSymbol.Unknown);
        var left = syntax.Left.ToSymbols(scope);
        return left.Concat([name]);
    }
}
