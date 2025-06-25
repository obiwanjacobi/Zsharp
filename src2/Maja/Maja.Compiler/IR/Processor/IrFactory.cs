using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR.Processor;

internal class IrFactory
{
    private readonly DiagnosticList _diagnostics = new();
    private readonly IIrProcessorContext _context;
    private readonly IrProcess _item;

    public IrFactory(IIrProcessorContext context, IrProcess item)
    {
        _context = context;
        _item = item;
    }

    public List<IrNode> Nodes(IEnumerable<SyntaxNode> syntax, IrScope scope)
    {
        var nodes = new List<IrNode>();
        foreach (var synNode in syntax)
        {
            var node = Node(synNode, scope);
            nodes.Add(node);
        }
        return nodes;
    }

    private IrNode Node(SyntaxNode synNode, IrScope scope)
    {
        return synNode switch
        {
            StatementSyntax statement => Statement(statement, scope),
            DeclarationMemberSyntax declaration => Declaration(declaration, scope),
            _ => throw new MajaException($"IR: Invalid code block root syntax object: {synNode.GetType().FullName}.")
        };
    }

    public List<IrDeclaration> Declarations(IEnumerable<DeclarationMemberSyntax> syntax, IrScope scope)
    {
        var declarations = new List<IrDeclaration>();

        foreach (var synDecl in syntax)
        {
            var decl = Declaration(synDecl, scope);
            declarations.Add(decl);
        }

        return declarations;
    }

    public IrDeclaration Declaration(DeclarationMemberSyntax syntax, IrScope scope)
    {
        return syntax switch
        {
            DeclarationFunctionSyntax fds => NextDeclarationFunction(fds),
            DeclarationVariableTypedSyntax vdt => NextDeclarationVariableTyped(vdt),
            DeclarationVariableInferredSyntax vdi => NextDeclarationVariableInferred(vdi),
            DeclarationTypeSyntax tds => NextDeclarationType(tds),
            _ => throw new NotSupportedException($"IR: No support for Declaration '{syntax.SyntaxKind}'")
        };
    }

    private IrDeclarationType NextDeclarationType(DeclarationTypeSyntax syntax)
    {
        var typeItem = _item.FindReference(syntax);
        Debug.Assert(typeItem.Model is not null);
        return typeItem.Model;
    }
    private IrDeclarationFunction NextDeclarationFunction(DeclarationFunctionSyntax syntax)
    {
        var functionItem = _item.FindReference(syntax);
        Debug.Assert(functionItem.Model is not null);
        return functionItem.Model;
    }
    private IrDeclarationVariable NextDeclarationVariableInferred(DeclarationVariableInferredSyntax syntax)
    {
        var varItem = _item.FindReference(syntax);
        Debug.Assert(varItem.Model is not null);
        return varItem.Model;
    }
    private IrDeclarationVariable NextDeclarationVariableTyped(DeclarationVariableTypedSyntax syntax)
    {
        var varItem = _item.FindReference(syntax);
        Debug.Assert(varItem.Model is not null);
        return varItem.Model;
    }
    private IrCodeBlock NextCodeBlock(CodeBlockSyntax syntax)
    {
        var codeBlockItem = _item.FindReference(syntax);
        Debug.Assert(codeBlockItem.Model is not null);
        return codeBlockItem.Model;
    }

    public IrDeclarationFunction DeclarationFunction(
        DeclarationFunctionSyntax syntax, DeclaredFunctionSymbol symbol, IrFunctionScope scope)
    {
        Debug.Assert(!symbol.IsUnresolved);

        var typeParameters = TypeParameters(syntax.TypeParameters, symbol.TypeParameters, scope);
        var parameters = Parameters(syntax.Parameters, symbol.Parameters, scope);
        var returnType = Type(syntax.ReturnType, symbol.ReturnType, scope) ?? IrType.Void;
        var block = NextCodeBlock(syntax.CodeBlock);

        if (returnType == IrType.Void)
        {
            var invalidReturns = block.Nodes
                .GetDescendantsOfType<IrStatementReturn>()
                .Where(r => r.Expression is not null);

            foreach (var ret in invalidReturns)
            {
                _diagnostics.VoidFunctionCannotReturnValue(ret.Expression!.Syntax.Location, symbol.Name.FullOriginalName);
            }
        }

        scope.Freeze();

        // TODO: Local function
        var locality = scope.Parent.IsExport(symbol.Name) || syntax.IsPublic
            ? IrLocality.Public
            : IrLocality.None;

        var functionDecl = new IrDeclarationFunction(syntax, symbol, typeParameters, parameters, returnType, scope, block, locality);

        // templates are registered for later instantiation processing
        if (functionDecl.IsTemplate &&
            !scope.Parent.TryRegisterTemplateFunction(functionDecl))
        {
            _diagnostics.FunctionTemplateAlreadyDeclared(syntax.Location, symbol.Name.FullOriginalName);
        }

        return functionDecl;
    }

    public IEnumerable<IrParameter> Parameters(IEnumerable<ParameterSyntax> syntax, IEnumerable<ParameterSymbol> symbols, IrFunctionScope scope)
    {
        var parameters = new List<IrParameter>();

        Debug.Assert(syntax.Count() == symbols.Count());
        for (var i = 0; i < syntax.Count(); i++)
        {
            var param = syntax.ElementAt(i);
            var symbol = symbols.ElementAt(i);

            var prm = Parameter(param, symbol, scope);
            parameters.Add(prm);
        }

        return parameters;
    }

    public IrParameter Parameter(ParameterSyntax syntax, ParameterSymbol symbol, IrFunctionScope scope)
    {
        Debug.Assert(!symbol.IsUnresolved);

        var type = Type(syntax.Type, symbol.Type, scope);
        Debug.Assert(type is not null);

        return new IrParameter(syntax, symbol, type);
    }

    public IrDeclarationType DeclarationType(DeclarationTypeSyntax syntax, DeclaredTypeSymbol symbol, IrScope scope)
    {
        Debug.Assert(!symbol.IsUnresolved);

        var typeScope = new IrTypeScope(symbol.Name.FullName, scope);
        var typeParameters = TypeParameters(syntax.TypeParameters, symbol.TypeParameters, typeScope);

        var enums = TypeMemberEnums(syntax.Enums, symbol.Enums, typeScope);
        var fields = TypeMemberFields(syntax.Fields, symbol.Fields, typeScope);
        var rules = TypeMemberRules(syntax.Rules, symbol.Rules, typeScope);

        var locality = IrLocality.Module;   // TODO
        var baseType = Type(syntax.BaseType, symbol.BaseType, scope);

        return new IrDeclarationType(syntax, symbol, typeParameters, enums, fields, rules, baseType, typeScope, locality);
    }

    private IEnumerable<IrTypeMemberField> TypeMemberFields(TypeMemberListSyntax<MemberFieldSyntax>? syntax, IEnumerable<FieldSymbol> symbols, IrTypeScope scope)
    {
        if (syntax is null)
            return Enumerable.Empty<IrTypeMemberField>();

        Debug.Assert(syntax.Items.Count() == symbols.Count());

        var fields = new List<IrTypeMemberField>();

        for (var i = 0; i < syntax.Items.Count(); i++)
        {
            var synFld = syntax.Items.ElementAt(i);
            var symbol = symbols.ElementAt(i);

            var type = Type(synFld.Type, symbol.Type, scope)!;
            IrExpression? defExpr = null;
            if (synFld.Expression is ExpressionSyntax synExpr)
                defExpr = Expression(synExpr, symbol, scope);

            var fld = new IrTypeMemberField(synFld, symbol, type, defExpr);
            fields.Add(fld);
        }

        return fields;
    }

    private IEnumerable<IrTypeMemberEnum> TypeMemberEnums(TypeMemberListSyntax<MemberEnumSyntax>? syntax, IEnumerable<EnumSymbol> symbols, IrTypeScope scope)
    {
        if (syntax is null)
            return Enumerable.Empty<IrTypeMemberEnum>();

        Debug.Assert(syntax.Items.Count() == symbols.Count());

        var enums = new List<IrTypeMemberEnum>();

        var id = 0;
        for (var i = 0; i < syntax.Items.Count(); i++)
        {
            var synEnum = syntax.Items.ElementAt(i);
            var symbol = symbols.ElementAt(i);

            IrExpression? expr = null;
            if (synEnum.Expression is ExpressionSyntax synExpr)
            {
                expr = Expression(synExpr, symbol, scope);

                if (expr.ConstantValue is null)
                    _diagnostics.EnumValueNotConstant(synExpr.Location, synExpr.Text);
            }

            var val = expr?.ConstantValue?.Value ?? id;
            var typeSymbol = expr?.TypeSymbol ?? TypeSymbol.I64;
            var enm = new IrTypeMemberEnum(synEnum, symbol, expr, val);
            enums.Add(enm);
            id++;
        }

        return enums;
    }
    private IEnumerable<IrTypeMemberRule> TypeMemberRules(TypeMemberListSyntax<MemberRuleSyntax>? syntax, IEnumerable<RuleSymbol> symbols, IrTypeScope scope)
    {
        if (syntax is null)
            return Enumerable.Empty<IrTypeMemberRule>();

        Debug.Assert(syntax.Items.Count() == symbols.Count());

        var rules = new List<IrTypeMemberRule>();

        foreach (var synRule in syntax.Items)
        {
            var name = new SymbolName(synRule.Name.Text);
            var symbol = new RuleSymbol(name);
            var expr = Expression(synRule.Expression, symbol, scope);

            var fld = new IrTypeMemberRule(synRule, symbol, expr);
            rules.Add(fld);
        }

        return rules;
    }

    public IEnumerable<IrTypeParameter> TypeParameters(IEnumerable<TypeParameterSyntax> syntax, IEnumerable<TypeParameterSymbol> symbols, IrScope scope)
    {
        var typeParams = new List<IrTypeParameter>();

        Debug.Assert(syntax.Count() == symbols.Count());
        for (var i = 0; i < syntax.Count(); i++)
        {
            var typeParamSyntax = syntax.ElementAt(i);
            var symbol = symbols.ElementAt(i);

            IrTypeParameter typeParam = typeParamSyntax switch
            {
                TypeParameterGenericSyntax tpg => TypeParameterGeneric(tpg, (TypeParameterGenericSymbol)symbol, scope),
                TypeParameterTemplateSyntax tpt => TypeParameterTemplate(tpt, (TypeParameterTemplateSymbol)symbol, scope),
                _ => throw new NotSupportedException($"IR: No support for TypeParameter '{typeParamSyntax.SyntaxKind}'")
            };

            typeParams.Add(typeParam);
        }

        return typeParams;
    }

    public IrTypeParameterGeneric TypeParameterGeneric(TypeParameterGenericSyntax syntax, TypeParameterGenericSymbol symbol, IrScope scope)
    {
        Debug.Assert(!symbol.IsUnresolved);
        var type = Type(syntax.DefaultType, symbol, scope);

        return new(syntax, type, symbol);
    }

    public IrTypeParameterTemplate TypeParameterTemplate(TypeParameterTemplateSyntax syntax, TypeParameterTemplateSymbol symbol, IrScope scope)
    {
        Debug.Assert(!symbol.IsUnresolved);
        var type = Type(syntax.DefaultType, symbol, scope);

        return new(syntax, type, symbol);
    }

    public IrType? Type(TypeSyntax? syntax, TypeSymbol? symbol, IrScope scope)
    {
        if (syntax is null) return null;
        if (symbol is null) throw new MajaException("Symbol parameter is null.");

        Debug.Assert(!symbol!.IsUnresolved);

        return new IrType(syntax, symbol);
    }

    public IrExpression Expression(ExpressionSyntax syntax, Symbol.Symbol symbol, IrScope scope)
    {
        return syntax switch
        {
            ExpressionBinarySyntax be => BinaryExpression(be, symbol, scope),
            ExpressionLiteralSyntax le => LiteralExpression(le, symbol, scope),
            ExpressionLiteralBoolSyntax lbe => LiteralBoolExpression(lbe, symbol, scope),
            ExpressionInvocationSyntax ie => InvocationExpression(ie, symbol, scope),
            ExpressionTypeInitializerSyntax eti => TypeInitializerExpression(eti, symbol, scope),
            ExpressionMemberAccessSyntax ema => MemberAccessExpression(ema, symbol, scope),
            ExpressionIdentifierSyntax ide => IdentifierExpression(ide, symbol, scope),
            ExpressionRangeSyntax ers => RangeExpression(ers, symbol, scope),
            _ => throw new NotSupportedException($"IR: No support for Expression '{syntax.SyntaxKind}'.")
        };
    }

    private IrExpressionBinary BinaryExpression(ExpressionBinarySyntax syntax, Symbol.Symbol symbol, IrScope scope)
    {
        throw new NotImplementedException();
    }
    private IrExpressionInvocation InvocationExpression(ExpressionInvocationSyntax syntax, Symbol.Symbol symbol, IrScope scope)
    {
        throw new NotImplementedException();
    }
    private IrExpressionTypeInitializer TypeInitializerExpression(ExpressionTypeInitializerSyntax syntax, Symbol.Symbol symbol, IrScope scope)
    {
        throw new NotImplementedException();
    }
    private IrExpressionMemberAccess MemberAccessExpression(ExpressionMemberAccessSyntax syntax, Symbol.Symbol symbol, IrScope scope)
    {
        throw new NotImplementedException();
    }
    private IrExpressionIdentifier IdentifierExpression(ExpressionIdentifierSyntax syntax, Symbol.Symbol symbol, IrScope scope)
    {
        //return new IrExpressionIdentifier(syntax, symbol, symbol.Type);
        throw new NotImplementedException();
    }
    private IrExpressionRange RangeExpression(ExpressionRangeSyntax syntax, Symbol.Symbol symbol, IrScope scope)
    {
        var start = syntax.Start is null
            ? null
            : Expression(syntax.Start, symbol, scope);

        var end = syntax.End is null
            ? null
            : Expression(syntax.End, symbol, scope);

        return new IrExpressionRange(syntax, start, end);
    }
    private IrExpressionLiteral LiteralBoolExpression(ExpressionLiteralBoolSyntax syntax, Symbol.Symbol symbol, IrScope scope)
    {
        throw new NotImplementedException();
    }
    private IrExpressionLiteral LiteralExpression(ExpressionLiteralSyntax syntax, Symbol.Symbol symbol, IrScope scope)
    {
        throw new NotImplementedException();
    }

    public IrCodeBlock CodeBlock(CodeBlockSyntax syntax, IrScope scope)
    {
        var nodes = Nodes(syntax.ChildNodes, scope);
        return new IrCodeBlock(syntax, nodes);
    }

    public IrStatement Statement(StatementSyntax statement, IrScope scope)
    {
        return statement switch
        {
            //StatementAssignmentSyntax sa => StatementAssignment(sa),
            //StatementIfSyntax ifs => StatementIf(ifs),
            //StatementReturnSyntax ret => StatementReturn(ret),
            //StatementExpressionSyntax ses => StatementExpression(ses),
            //StatementLoopSyntax sls => StatementLoop(sls),
            _ => throw new NotSupportedException($"IR: No support for Statement '{statement.SyntaxKind}'.")
        };
    }
}