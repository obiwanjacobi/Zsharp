using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.Parser;
using static Maja.Compiler.Parser.MajaParser;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Visits the Antlr parse tree context objects and generates the Syntax Model instances.
/// </summary>
internal sealed class SyntaxTreeBuilder : MajaParserBaseVisitor<SyntaxNodeOrToken[]>
{
    private static readonly SyntaxNodeOrToken[] Empty = Array.Empty<SyntaxNodeOrToken>();
    private readonly string SourceName;

    public SyntaxTreeBuilder(string sourceName)
    {
        Diagnostics = new DiagnosticList();
        SourceName = sourceName ?? throw new System.ArgumentNullException(nameof(sourceName));
    }

    public DiagnosticList Diagnostics { get; }

    protected override SyntaxNodeOrToken[] DefaultResult
        => Empty;

    protected override SyntaxNodeOrToken[] AggregateResult(SyntaxNodeOrToken[] aggregate, SyntaxNodeOrToken[] nextResult)
        => aggregate is null
            ? nextResult
            : aggregate.Append(nextResult);

    private SyntaxLocation Location(ParserRuleContext context)
        => SyntaxLocation.From(context, SourceName);

    private SyntaxLocation Location(ITerminalNode node)
        => SyntaxLocation.From(node, SourceName);

    private readonly struct ChildrenLists
    {
        public readonly SyntaxNodeOrTokenList All;
        public readonly SyntaxNodeList Nodes;
        public readonly SyntaxTokenList Tokens;

        public ChildrenLists(IList<SyntaxNodeOrToken> children,
            IList<SyntaxNode> nodes, IList<SyntaxToken> tokens)
        {
            All = SyntaxNodeOrTokenList.New(children);
            Nodes = SyntaxNodeList.New(nodes);
            Tokens = SyntaxTokenList.New(tokens);
        }
    }

    private static ChildrenLists Children<T>(Func<T, SyntaxNodeOrToken[]> visitChildren, T context)
        where T : ParserRuleContext
    {
        var children = visitChildren(context);
        var childNodes = new List<SyntaxNode>();
        var tokens = new List<SyntaxToken>();
        var leadingTokens = new List<SyntaxToken>();
        SyntaxNode? lastNode = null;
        WhitespaceToken? lastWhitespace = null;

        foreach (var child in children)
        {
            if (child.Token is SyntaxToken token)
            {
                // special whitespace chaining
                if (token is WhitespaceToken whitespace)
                {
                    if (lastWhitespace is not null)
                    {
                        _ = tokens.Remove(lastWhitespace);

                        lastWhitespace = lastWhitespace.Append(whitespace);
                        token = lastWhitespace;
                    }
                    else lastWhitespace = whitespace;
                }
                else lastWhitespace = null;

                tokens.Add(token);
            }
            else if (child.Node is SyntaxNode node)
            {
                if (tokens.Count > 0)
                {
                    if (lastNode is null)
                        leadingTokens.AddRange(tokens);
                    else
                        lastNode.TrailingTokens = SyntaxTokenList.New(tokens);
                    tokens.Clear();
                }
                childNodes.Add(node);
                lastNode = node;
            }
        }

        if (lastNode is not null && tokens.Count > 0)
        {
            lastNode.TrailingTokens = lastNode.HasTrailingTokens
                ? SyntaxTokenList.New(
                    lastNode.TrailingTokens.Concat(tokens).ToList())
                : SyntaxTokenList.New(tokens);
            tokens.Clear();
        }

        return new ChildrenLists(children, childNodes,
            leadingTokens.Concat(tokens).ToList());
    }

    public override SyntaxNodeOrToken[] VisitTerminal(ITerminalNode node)
    {
        var location = Location(node);
        SyntaxToken? token = SyntaxToken.TryNew(node.Symbol.Type, node.GetText(), location);
        if (token is SyntaxToken knownToken)
        {
            return new[] { new SyntaxNodeOrToken(knownToken) };
        }
        return Empty;
    }

    public override SyntaxNodeOrToken[] VisitErrorNode(IErrorNode node)
        // TODO: use context information to build a better error message.
        => new[] { new SyntaxNodeOrToken(
            new ErrorToken(node.GetText())
        {
            TokenTypeId = node.Symbol.Type,
            Location = Location(node)
        } )};

    private SyntaxNodeOrToken[] CreateSyntaxNode<C, S>(C context, Func<C, SyntaxNodeOrToken[]> visitFn)
        where C : ParserRuleContext
        where S : SyntaxNode, ICreateSyntaxNode<S>
    {
        var children = Children(visitFn, context);

        var location = Location(context);
        var node = new SyntaxNodeOrToken(
            S.Create(context.GetText(),
                location,
                children.All,
                children.Nodes,
                children.Tokens)
            );

        if (context.exception is not null)
        {
            var err = new SyntaxNodeOrToken(
                new ErrorToken(context.exception.Message)
                {
                    Location = location
                });
            return [err, node];
        }

        return [node];
    }

    //
    // Module
    //

    public override SyntaxNodeOrToken[] VisitCompilationUnit(CompilationUnitContext context)
        => CreateSyntaxNode<CompilationUnitContext, CompilationUnitSyntax>(context, base.VisitCompilationUnit);

    public override SyntaxNodeOrToken[] VisitDirectiveMod(DirectiveModContext context)
        => CreateSyntaxNode<DirectiveModContext, ModuleSyntax>(context, base.VisitDirectiveMod);

    public override SyntaxNodeOrToken[] VisitDirectivePub(DirectivePubContext context)
        => CreateSyntaxNode<DirectivePubContext, PublicExportSyntax>(context, base.VisitDirectivePub);

    public override SyntaxNodeOrToken[] VisitDirectiveUse(DirectiveUseContext context)
        => CreateSyntaxNode<DirectiveUseContext, UseImportSyntax>(context, base.VisitDirectiveUse);

    public override SyntaxNodeOrToken[] VisitCodeBlock(CodeBlockContext context)
        => CreateSyntaxNode<CodeBlockContext, CodeBlockSyntax>(context, base.VisitCodeBlock);

    //
    // Functions
    //

    public override SyntaxNodeOrToken[] VisitDeclarationFunction(DeclarationFunctionContext context)
        => CreateSyntaxNode<DeclarationFunctionContext, FunctionDeclarationSyntax>(context, base.VisitDeclarationFunction);

    public override SyntaxNodeOrToken[] VisitParameter(ParameterContext context)
        => CreateSyntaxNode<ParameterContext, ParameterSyntax>(context, base.VisitParameter);

    //
    // Types
    //

    public override SyntaxNodeOrToken[] VisitDeclarationType(DeclarationTypeContext context)
        => CreateSyntaxNode<DeclarationTypeContext, TypeDeclarationSyntax>(context, base.VisitDeclarationType);

    public override SyntaxNodeOrToken[] VisitDeclarationTypeMemberListEnum(DeclarationTypeMemberListEnumContext context)
        => CreateSyntaxNode<DeclarationTypeMemberListEnumContext, TypeMemberListSyntax<MemberEnumSyntax>>(context, base.VisitDeclarationTypeMemberListEnum);

    public override SyntaxNodeOrToken[] VisitDeclarationTypeMemberListField(DeclarationTypeMemberListFieldContext context)
        => CreateSyntaxNode<DeclarationTypeMemberListFieldContext, TypeMemberListSyntax<MemberFieldSyntax>>(context, base.VisitDeclarationTypeMemberListField);

    public override SyntaxNodeOrToken[] VisitDeclarationTypeMemberListRule(DeclarationTypeMemberListRuleContext context)
        => CreateSyntaxNode<DeclarationTypeMemberListRuleContext, TypeMemberListSyntax<MemberRuleSyntax>>(context, base.VisitDeclarationTypeMemberListRule);

    public override SyntaxNodeOrToken[] VisitMemberEnum(MemberEnumContext context)
        => CreateSyntaxNode<MemberEnumContext, MemberEnumSyntax>(context, base.VisitMemberEnum);

    public override SyntaxNodeOrToken[] VisitMemberEnumValue(MemberEnumValueContext context)
        => CreateSyntaxNode<MemberEnumValueContext, MemberEnumSyntax>(context, base.VisitMemberEnumValue);

    public override SyntaxNodeOrToken[] VisitMemberField(MemberFieldContext context)
    {
        var children = Children(base.VisitMemberField, context);

        var node = MemberFieldSyntax.Create(context.GetText(),
            Location(context), children.All, children.Nodes, children.Tokens);

        if (!(node.Expression?.IsPrecedenceValid ?? true))
        {
            Diagnostics.ExpressionPrecedenceNotSpecified(node.Location, node.Text);
        }

        return [new SyntaxNodeOrToken(node)];
    }

    public override SyntaxNodeOrToken[] VisitMemberRule(MemberRuleContext context)
        => CreateSyntaxNode<MemberRuleContext, MemberRuleSyntax>(context, base.VisitMemberRule);

    public override SyntaxNodeOrToken[] VisitTypeArgument(TypeArgumentContext context)
    {
        var children = Children(base.VisitTypeArgument, context);

        var node = TypeArgumentSyntax.Create(context.GetText(),
            Location(context),
            children.All,
            children.Nodes,
            children.Tokens);

        if (!(node.Expression?.IsPrecedenceValid ?? true))
        {
            Diagnostics.ExpressionPrecedenceNotSpecified(node.Location, node.Text);
        }

        return [new SyntaxNodeOrToken(node)];
    }

    public override SyntaxNodeOrToken[] VisitTypeParameterTemplate(TypeParameterTemplateContext context)
        => CreateSyntaxNode<TypeParameterTemplateContext, TypeParameterTemplateSyntax>(context, base.VisitTypeParameterTemplate);

    public override SyntaxNodeOrToken[] VisitTypeParameterGeneric(TypeParameterGenericContext context)
        => CreateSyntaxNode<TypeParameterGenericContext, TypeParameterGenericSyntax>(context, base.VisitTypeParameterGeneric);

    public override SyntaxNodeOrToken[] VisitTypeParameterValue(TypeParameterValueContext context)
        => CreateSyntaxNode<TypeParameterValueContext, TypeParameterValueSyntax>(context, base.VisitTypeParameterValue);

    public override SyntaxNodeOrToken[] VisitType(TypeContext context)
        => CreateSyntaxNode<TypeContext, TypeSyntax>(context, base.VisitType);

    public override SyntaxNodeOrToken[] VisitTypeInitializerField([NotNull] TypeInitializerFieldContext context)
        => CreateSyntaxNode<TypeInitializerFieldContext, TypeInitializerFieldSyntax>(context, base.VisitTypeInitializerField);

    //
    // Variables
    //

    public override SyntaxNodeOrToken[] VisitDeclarationVariableTyped(DeclarationVariableTypedContext context)
    {
        var children = Children(base.VisitDeclarationVariableTyped, context);

        var node = VariableDeclarationTypedSyntax.Create(context.GetText(),
            Location(context),
            children.All,
            children.Nodes,
            children.Tokens);

        if (!(node.Expression?.IsPrecedenceValid ?? true))
        {
            Diagnostics.ExpressionPrecedenceNotSpecified(node.Location, node.Text);
        }

        return [new SyntaxNodeOrToken(node)];
    }

    public override SyntaxNodeOrToken[] VisitDeclarationVariableInferred(DeclarationVariableInferredContext context)
    {
        var children = Children(base.VisitDeclarationVariableInferred, context);

        var node = VariableDeclarationInferredSyntax.Create(context.GetText(),
            Location(context),
            children.All,
            children.Nodes,
            children.Tokens);

        if (!node.Expression!.IsPrecedenceValid)
        {
            Diagnostics.ExpressionPrecedenceNotSpecified(node.Location, node.Text);
        }

        return [new SyntaxNodeOrToken(node)];
    }

    //
    // Expressions
    //

    public override SyntaxNodeOrToken[] VisitExpressionPrecedence(ExpressionPrecedenceContext context)
    {
        var children = base.VisitExpressionPrecedence(context);
        for (var i = 0; i < children.Length; i++)
        {
            var child = children[i];
            if (child.Node is ExpressionSyntax expr)
            {
                expr.Precedence = true;
            }
        }
        return children;
    }

    public override SyntaxNodeOrToken[] VisitExpressionBinary(ExpressionBinaryContext context)
        => CreateSyntaxNode<ExpressionBinaryContext, ExpressionBinarySyntax>(context, base.VisitExpressionBinary);

    public override SyntaxNodeOrToken[] VisitExpressionLiteral(ExpressionLiteralContext context)
        => CreateSyntaxNode<ExpressionLiteralContext, ExpressionLiteralSyntax>(context, base.VisitExpressionLiteral);

    public override SyntaxNodeOrToken[] VisitExpressionLiteralBool(ExpressionLiteralBoolContext context)
        => CreateSyntaxNode<ExpressionLiteralBoolContext, ExpressionLiteralBoolSyntax>(context, base.VisitExpressionLiteralBool);

    public override SyntaxNodeOrToken[] VisitExpressionIdentifier(ExpressionIdentifierContext context)
        => CreateSyntaxNode<ExpressionIdentifierContext, ExpressionIdentifierSyntax>(context, base.VisitExpressionIdentifier);

    public override SyntaxNodeOrToken[] VisitNumber(NumberContext context)
        => CreateSyntaxNode<NumberContext, LiteralNumberSyntax>(context, base.VisitNumber);

    public override SyntaxNodeOrToken[] VisitString(StringContext context)
        => CreateSyntaxNode<StringContext, LiteralStringSyntax>(context, base.VisitString);

    public override SyntaxNodeOrToken[] VisitExpressionInvocation(ExpressionInvocationContext context)
        => CreateSyntaxNode<ExpressionInvocationContext, ExpressionInvocationSyntax>(context, base.VisitExpressionInvocation);

    public override SyntaxNodeOrToken[] VisitExpressionTypeInitializer([NotNull] ExpressionTypeInitializerContext context)
        => CreateSyntaxNode<ExpressionTypeInitializerContext, ExpressionTypeInitializerSyntax>(context, base.VisitExpressionTypeInitializer);

    public override SyntaxNodeOrToken[] VisitArgument(ArgumentContext context)
    {
        var children = Children(base.VisitArgument, context);

        var node = ArgumentSyntax.Create(context.GetText(),
            Location(context),
            children.All,
            children.Nodes,
            children.Tokens);

        if (!node.Expression.IsPrecedenceValid)
        {
            Diagnostics.ExpressionPrecedenceNotSpecified(node.Location, node.Text);
        }

        return [new SyntaxNodeOrToken(node)];
    }

    //public override SyntaxNodeOrToken[] VisitExpressionRule(ExpressionRuleContext context)
    //{
    //    return base.VisitExpressionRule(context);
    //}

    //
    // Expression Operators
    //

    private SyntaxNodeOrToken[] CreateOperator<T>(
        ExpressionOperatorCategory category, ExpressionOperatorCardinality cardinality,
        Func<T, SyntaxNodeOrToken[]> getChildren, T context)
        where T : ParserRuleContext
    {
        var children = Children(getChildren, context);

        var kind = DetermineOperatorKind(category, context);
        var precedence = cardinality.ToOperatorPrecedence();

        var syntaxNode = ExpressionOperatorSyntax.Create(context.GetText(),
            Location(context),
            children.All,
            children.Nodes,
            children.Tokens,
            precedence, kind, category, cardinality);

        return [new SyntaxNodeOrToken(syntaxNode)];
    }

    private ExpressionOperatorKind DetermineOperatorKind(
        ExpressionOperatorCategory category, ParserRuleContext context)
    {
        var tokens = context.children.OfType<TerminalNodeImpl>();
        var tokenCount = tokens.Count();

        if (tokenCount < 1)
            return ExpressionOperatorKind.Unknown;

        return tokens.Select(t => t.Symbol.Type)
            .ToArray()
            .ToOperatorKind(category);
    }

    public override SyntaxNodeOrToken[] VisitExpressionOperatorArithmetic(ExpressionOperatorArithmeticContext context)
        => CreateOperator(
            ExpressionOperatorCategory.Arithmetic,
            ExpressionOperatorCardinality.Binary,
            base.VisitExpressionOperatorArithmetic, context);

    public override SyntaxNodeOrToken[] VisitExpressionOperatorArithmeticUnaryPrefix(ExpressionOperatorArithmeticUnaryPrefixContext context)
        => CreateOperator(
            ExpressionOperatorCategory.Arithmetic,
            ExpressionOperatorCardinality.Unary,
            base.VisitExpressionOperatorArithmeticUnaryPrefix, context);

    public override SyntaxNodeOrToken[] VisitExpressionOperatorBits(ExpressionOperatorBitsContext context)
        => CreateOperator(
            ExpressionOperatorCategory.Bitwise,
            ExpressionOperatorCardinality.Binary,
            base.VisitExpressionOperatorBits, context);

    public override SyntaxNodeOrToken[] VisitExpressionOperatorBitsUnaryPrefix(ExpressionOperatorBitsUnaryPrefixContext context)
        => CreateOperator(
            ExpressionOperatorCategory.Bitwise,
            ExpressionOperatorCardinality.Unary,
            base.VisitExpressionOperatorBitsUnaryPrefix, context);

    public override SyntaxNodeOrToken[] VisitExpressionOperatorComparison(ExpressionOperatorComparisonContext context)
        => CreateOperator(
            ExpressionOperatorCategory.Comparison,
            ExpressionOperatorCardinality.Binary,
            base.VisitExpressionOperatorComparison, context);

    public override SyntaxNodeOrToken[] VisitExpressionOperatorLogic(ExpressionOperatorLogicContext context)
        => CreateOperator(
            ExpressionOperatorCategory.Logic,
            ExpressionOperatorCardinality.Binary,
            base.VisitExpressionOperatorLogic, context);

    public override SyntaxNodeOrToken[] VisitExpressionOperatorLogicUnaryPrefix(ExpressionOperatorLogicUnaryPrefixContext context)
        => CreateOperator(
            ExpressionOperatorCategory.Logic,
            ExpressionOperatorCardinality.Unary,
            base.VisitExpressionOperatorLogicUnaryPrefix, context);

    //
    // Identifiers
    //

    public override SyntaxNodeOrToken[] VisitNameIdentifier(NameIdentifierContext context)
        => CreateSyntaxNode<NameIdentifierContext, NameSyntax>(context, base.VisitNameIdentifier);

    public override SyntaxNodeOrToken[] VisitNameQualified(NameQualifiedContext context)
        => CreateSyntaxNode<NameQualifiedContext, QualifiedNameSyntax>(context, base.VisitNameQualified);

    //
    // Statements
    //

    public override SyntaxNodeOrToken[] VisitStatementAssignment(StatementAssignmentContext context)
        => CreateSyntaxNode<StatementAssignmentContext, StatementAssignmentSyntax>(context, base.VisitStatementAssignment);

    public override SyntaxNodeOrToken[] VisitStatementIf(StatementIfContext context)
        => CreateSyntaxNode<StatementIfContext, StatementIfSyntax>(context, base.VisitStatementIf);

    public override SyntaxNodeOrToken[] VisitStatementElse(StatementElseContext context)
        => CreateSyntaxNode<StatementElseContext, StatementElseSyntax>(context, base.VisitStatementElse);

    public override SyntaxNodeOrToken[] VisitStatementElseIf(StatementElseIfContext context)
        => CreateSyntaxNode<StatementElseIfContext, StatementElseIfSyntax>(context, base.VisitStatementElseIf);

    public override SyntaxNodeOrToken[] VisitStatementRet(StatementRetContext context)
        => CreateSyntaxNode<StatementRetContext, StatementReturnSyntax>(context, base.VisitStatementRet);

    public override SyntaxNodeOrToken[] VisitStatementExpression(StatementExpressionContext context)
    {
        var children = Children(base.VisitStatementExpression, context);

        var node = StatementExpressionSyntax.Create(context.GetText(),
            Location(context),
            children.All,
            children.Nodes,
            children.Tokens);

        if (!node.Expression.IsPrecedenceValid)
        {
            Diagnostics.ExpressionPrecedenceNotSpecified(node.Location, node.Text);
        }

        return [new SyntaxNodeOrToken(node)];
    }
}