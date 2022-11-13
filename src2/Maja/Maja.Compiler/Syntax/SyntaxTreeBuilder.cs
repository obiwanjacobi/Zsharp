using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
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

    public static CompilationUnitSyntax Build(CompilationUnitContext context, string source = "")
    {
        var builder = new SyntaxTreeBuilder(source);
        var nodes = builder.VisitCompilationUnit(context);
        return (CompilationUnitSyntax)nodes[0].Node!;
    }

    private SyntaxTreeBuilder(string sourceName)
        => SourceName = sourceName ?? throw new System.ArgumentNullException(nameof(sourceName));

    protected override SyntaxNodeOrToken[] DefaultResult
        => Empty;

    protected override SyntaxNodeOrToken[] AggregateResult(SyntaxNodeOrToken[] aggregate, SyntaxNodeOrToken[] nextResult)
    {
        if (aggregate is null) return nextResult;
        return aggregate.Append(nextResult);
    }

    private SyntaxLocation Location(ParserRuleContext context)
        => new(SourceName, context.Start.Line, context.Start.Column,
            new SyntaxSpan(context.SourceInterval.a, context.SourceInterval.b));

    private SyntaxLocation Location(ITerminalNode node)
        => new(SourceName, node.Symbol.Line, node.Symbol.Column,
            new SyntaxSpan(node.SourceInterval.a, node.SourceInterval.b));

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
                        tokens.Remove(lastWhitespace);

                        lastWhitespace = lastWhitespace.Append(whitespace);
                        token = lastWhitespace;
                    }
                    else lastWhitespace = whitespace;
                }
                else lastWhitespace = null;

                tokens.Add(token);
            }
            if (child.Node is SyntaxNode node)
            {
                if (tokens.Count > 0)
                {
                    if (lastNode is null)
                        node.LeadingTokens = SyntaxTokenList.New(tokens);
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
            lastNode.TrailingTokens = SyntaxTokenList.New(tokens);
            tokens.Clear();
        }

        return new ChildrenLists(children, childNodes, tokens);
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

    //
    // Module
    //

    public override SyntaxNodeOrToken[] VisitCompilationUnit(CompilationUnitContext context)
    {
        var children = Children(base.VisitCompilationUnit, context);

        return new[] { new SyntaxNodeOrToken(
            new CompilationUnitSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitDirectiveMod(DirectiveModContext context)
    {
        var children = Children(base.VisitDirectiveMod, context);

        return new[] { new SyntaxNodeOrToken(
            new ModuleSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitDirectivePub(DirectivePubContext context)
    {
        var children = Children(base.VisitDirectivePub, context);

        return new[] { new SyntaxNodeOrToken(
            new PublicExportSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitDirectiveUse(DirectiveUseContext context)
    {
        var children = Children(base.VisitDirectiveUse, context);

        return new[] { new SyntaxNodeOrToken(
            new UseImportSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitCodeBlock(CodeBlockContext context)
    {
        var children = Children(base.VisitCodeBlock, context);

        return new[] { new SyntaxNodeOrToken(
            new CodeBlockSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    //
    // Functions
    //

    public override SyntaxNodeOrToken[] VisitDeclarationFunction(DeclarationFunctionContext context)
    {
        var children = Children(base.VisitDeclarationFunction, context);

        return new[]{ new SyntaxNodeOrToken(
            new FunctionDeclarationSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitParameter(ParameterContext context)
    {
        var children = Children(base.VisitParameter, context);

        return new[] { new SyntaxNodeOrToken(
            new ParameterSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    //
    // Types
    //

    public override SyntaxNodeOrToken[] VisitDeclarationType(DeclarationTypeContext context)
    {
        var children = Children(base.VisitDeclarationType, context);

        return new[] { new SyntaxNodeOrToken(
            new TypeDeclarationSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitDeclarationTypeMemberListEnum(DeclarationTypeMemberListEnumContext context)
    {
        var children = Children(base.VisitDeclarationTypeMemberListEnum, context);

        return new[] { new SyntaxNodeOrToken(
            new TypeMemberListSyntax<MemberEnumSyntax>(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitDeclarationTypeMemberListField(DeclarationTypeMemberListFieldContext context)
    {
        var children = Children(base.VisitDeclarationTypeMemberListField, context);

        return new[] { new SyntaxNodeOrToken(
            new TypeMemberListSyntax<MemberFieldSyntax>(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitDeclarationTypeMemberListRule(DeclarationTypeMemberListRuleContext context)
    {
        var children = Children(base.VisitDeclarationTypeMemberListRule, context);

        return new[] { new SyntaxNodeOrToken(
            new TypeMemberListSyntax<MemberRuleSyntax>(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitMemberEnum(MemberEnumContext context)
    {
        var children = Children(base.VisitMemberEnum, context);

        return new[] { new SyntaxNodeOrToken(
            new MemberEnumSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitMemberEnumValue([NotNull] MemberEnumValueContext context)
    {
        var children = Children(base.VisitMemberEnumValue, context);

        return new[] { new SyntaxNodeOrToken(
            new MemberEnumSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitMemberField(MemberFieldContext context)
    {
        var children = Children(base.VisitMemberField, context);

        return new[] { new SyntaxNodeOrToken(
            new MemberFieldSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitMemberRule(MemberRuleContext context)
    {
        var children = Children(base.VisitMemberRule, context);

        return new[] { new SyntaxNodeOrToken(
            new MemberRuleSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitTypeArgument(TypeArgumentContext context)
    {
        var children = Children(base.VisitTypeArgument, context);

        return new[] { new SyntaxNodeOrToken(
            new TypeArgumentSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitTypeParameterTemplate(TypeParameterTemplateContext context)
    {
        var children = Children(base.VisitTypeParameterTemplate, context);

        return new[] { new SyntaxNodeOrToken(
            new TypeParameterTemplateSyntax(context.GetText())
        {
            Location= Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitTypeParameterGeneric(TypeParameterGenericContext context)
    {
        var children = Children(base.VisitTypeParameterGeneric, context);

        return new[] { new SyntaxNodeOrToken(
            new TypeParameterGenericSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitTypeParameterValue(TypeParameterValueContext context)
    {
        var children = Children(base.VisitTypeParameterValue, context);

        return new[] { new SyntaxNodeOrToken(
            new TypeParameterValueSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitType(TypeContext context)
    {
        var children = Children(base.VisitType, context);

        return new[] { new SyntaxNodeOrToken(
            new TypeSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    //
    // Variables
    //

    public override SyntaxNodeOrToken[] VisitDeclarationVariableTyped(DeclarationVariableTypedContext context)
    {
        var children = Children(base.VisitDeclarationVariableTyped, context);

        return new[] { new SyntaxNodeOrToken(
            new VariableDeclarationTypedSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitDeclarationVariableInferred(DeclarationVariableInferredContext context)
    {
        var children = Children(base.VisitDeclarationVariableInferred, context);

        return new[] { new SyntaxNodeOrToken(
            new VariableDeclarationInferredSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    //
    // Expressions
    //

    public override SyntaxNodeOrToken[] VisitExpressionPrecedence(ExpressionPrecedenceContext context)
    {
        var children = base.VisitExpressionPrecedence(context);
        for (int i = 0; i < children.Length; i++)
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
    {
        var children = Children(base.VisitExpressionBinary, context);

        return new[] { new SyntaxNodeOrToken(
            new ExpressionBinarySyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitExpressionLiteral(ExpressionLiteralContext context)
    {
        var children = Children(base.VisitExpressionLiteral, context);

        return new[] { new SyntaxNodeOrToken(
            new ExpressionLiteralSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitExpressionLiteralBool(ExpressionLiteralBoolContext context)
    {
        var children = Children(base.VisitExpressionLiteralBool, context);

        return new[] { new SyntaxNodeOrToken(
            new ExpressionLiteralBoolSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitExpressionIdentifier(ExpressionIdentifierContext context)
    {
        var children = Children(base.VisitExpressionIdentifier, context);

        return new[] { new SyntaxNodeOrToken(
            new ExpressionIdentifierSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitNumber(NumberContext context)
    {
        var children = Children(base.VisitNumber, context);

        return new[] { new SyntaxNodeOrToken(
            new LiteralNumberSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitString(StringContext context)
    {
        var children = Children(base.VisitString, context);

        return new[] { new SyntaxNodeOrToken(
            new LiteralStringSyntax(context.GetText().Trim('"'))
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitExpressionInvocation(ExpressionInvocationContext context)
    {
        var children = Children(base.VisitExpressionInvocation, context);

        return new[] { new SyntaxNodeOrToken(
            new ExpressionInvocationSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitArgument(ArgumentContext context)
    {
        var children = Children(base.VisitArgument, context);

        return new[] { new SyntaxNodeOrToken(
            new ArgumentSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        }
        )};
    }

    public override SyntaxNodeOrToken[] VisitExpressionRule(ExpressionRuleContext context)
    {
        return base.VisitExpressionRule(context);
    }

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
        var precedence = kind.ToOperatorPrecedence(cardinality);

        return new[] { new SyntaxNodeOrToken(
            new ExpressionOperatorSyntax(context.GetText())
        {
            Precedence = precedence,
            OperatorKind = kind,
            OperatorCategory = category,
            OperatorCardinality = cardinality,
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
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

    public override SyntaxNodeOrToken[] VisitExpressionOperatorAssignment(ExpressionOperatorAssignmentContext context)
        => CreateOperator(
            ExpressionOperatorCategory.Assignment,
            ExpressionOperatorCardinality.Unary,
            base.VisitExpressionOperatorAssignment, context);

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
    {
        var children = Children(base.VisitNameIdentifier, context);

        return new[] { new SyntaxNodeOrToken(
            new NameSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitNameQualified(NameQualifiedContext context)
    {
        var children = Children(base.VisitNameQualified, context);

        return new[] { new SyntaxNodeOrToken(
            new QualifiedNameSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    //
    // Statements
    //

    public override SyntaxNodeOrToken[] VisitStatementIf(StatementIfContext context)
    {
        var children = Children(base.VisitStatementIf, context);

        return new[] { new SyntaxNodeOrToken(
            new StatementIfSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitStatementElse(StatementElseContext context)
    {
        var children = Children(base.VisitStatementElse, context);

        return new[] { new SyntaxNodeOrToken(
            new StatementElseSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitStatementElseIf(StatementElseIfContext context)
    {
        var children = Children(base.VisitStatementElseIf, context);

        return new[] { new SyntaxNodeOrToken(
            new StatementElseIfSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitStatementRet(StatementRetContext context)
    {
        var children = Children(base.VisitStatementRet, context);

        return new[] { new SyntaxNodeOrToken(
            new StatementReturnSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }

    public override SyntaxNodeOrToken[] VisitStatementExpression(StatementExpressionContext context)
    {
        var children = Children(base.VisitStatementExpression, context);

        return new[] { new SyntaxNodeOrToken(
            new StatementExpressionSyntax(context.GetText())
        {
            Location = Location(context),
            Children = children.All,
            ChildNodes = children.Nodes,
            TrailingTokens = children.Tokens
        } )};
    }
}