using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Maja.Compiler.Parser;
using static Maja.Compiler.Parser.MajaParser;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Visits the Antlr parse tree context objects and generates the Syntax Model instances.
/// </summary>
internal sealed class SyntaxNodeBuilder : MajaParserBaseVisitor<SyntaxNodeOrToken[]>
{
    private static readonly SyntaxNodeOrToken[] Empty = Array.Empty<SyntaxNodeOrToken>();
    private readonly string SourceName;

    public SyntaxNodeBuilder(string sourceName)
        => SourceName = sourceName ?? throw new System.ArgumentNullException(nameof(sourceName));

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


    public override SyntaxNodeOrToken[] VisitTerminal(ITerminalNode node)
    {
        var location = Location(node);
        SyntaxToken? token = SyntaxToken.TryNew(node.GetText(), location);
        if (token is not null)
        {
            return new[] { new SyntaxNodeOrToken(token!) };
        }
        return Empty;
    }

    private static SyntaxNodeList Children<T>(Func<T, SyntaxNodeOrToken[]> visitChildren, T context)
        where T: ParserRuleContext
    {
        var children = new List<SyntaxNode>();
        var tokens = new List<SyntaxToken>();
        SyntaxNode? lastNode = null;

        foreach (var child in visitChildren(context))
        {
            if (child.Token is SyntaxToken token)
            {
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
                children.Add(node);
                lastNode = node;
            }
        }

        if (children.Count == 0) return SyntaxNodeList.New();
        return SyntaxNodeList.New(children);
    }

    //
    // Module
    //

    public override SyntaxNodeOrToken[] VisitCompilationUnit(CompilationUnitContext context)
        => new[] { new SyntaxNodeOrToken(
            new CompilationUnitSyntax(context.GetText())
        {
            Location = Location(context),
            Children = Children(base.VisitCompilationUnit, context)
        } )};

    public override SyntaxNodeOrToken[] VisitPub1Decl(Pub1DeclContext context)
        => new[] { new SyntaxNodeOrToken(
            new PublicExportSyntax(context.GetText())
        {
            Location = Location(context),
            Children = Children(base.VisitPub1Decl, context)
        } )};

    public override SyntaxNodeOrToken[] VisitPub2Decl(Pub2DeclContext context)
        => new[] { new SyntaxNodeOrToken(
            new PublicExportSyntax(context.GetText())
        {
            Location = Location(context),
            Children = Children(base.VisitPub2Decl, context)
        } )};

    public override SyntaxNodeOrToken[] VisitUseDecl(UseDeclContext context)
        => new[] { new SyntaxNodeOrToken(
            new UseImportSyntax(context.GetText())
        {
            Location = Location(context),
            Children = Children(base.VisitUseDecl, context)
        } )};

    public override SyntaxNodeOrToken[] VisitCodeBlock(CodeBlockContext context)
        => new[] { new SyntaxNodeOrToken(
            new CodeBlockSyntax(context.GetText())
        {
            Location = Location(context),
            Children = Children(base.VisitCodeBlock, context)
        } )};

    public override SyntaxNodeOrToken[] VisitMembersDecl(MembersDeclContext context)
    {
        // This level is represented by a base class.
        return base.VisitMembersDecl(context);
    }

    //
    // Functions
    //

    public override SyntaxNodeOrToken[] VisitFunctionDecl(FunctionDeclContext context)
        => new[]{ new SyntaxNodeOrToken(
            new FunctionDelcarationSyntax(context.GetText())
        {
            Location = Location(context),
            Children = Children(base.VisitFunctionDecl, context)
        } )};

    public override SyntaxNodeOrToken[] VisitFunctionDeclLocal(FunctionDeclLocalContext context)
    {
        return base.VisitFunctionDeclLocal(context);
    }

    public override SyntaxNodeOrToken[] VisitParameterList(ParameterListContext context)
    {
        // This level is not represented.
        return base.VisitParameterList(context);
    }

    public override SyntaxNodeOrToken[] VisitParameter(ParameterContext context)
        => new[] { new SyntaxNodeOrToken(
            new ParameterSyntax(context.GetText())
        {
            Location = Location(context),
            Children = Children(base.VisitParameter, context)
        } )};

    //
    // Types
    //

    public override SyntaxNodeOrToken[] VisitTypeDecl(TypeDeclContext context)
    {
        return base.VisitTypeDecl(context);
    }

    public override SyntaxNodeOrToken[] VisitTypeDeclMembers(TypeDeclMembersContext context)
    {
        return base.VisitTypeDeclMembers(context);
    }

    public override SyntaxNodeOrToken[] VisitMemberEnum(MemberEnumContext context)
    {
        return base.VisitMemberEnum(context);
    }

    public override SyntaxNodeOrToken[] VisitMemberField(MemberFieldContext context)
    {
        return base.VisitMemberField(context);
    }

    public override SyntaxNodeOrToken[] VisitMemberRule(MemberRuleContext context)
    {
        return base.VisitMemberRule(context);
    }

    public override SyntaxNodeOrToken[] VisitTypeParameterList(TypeParameterListContext context)
    {
        return base.VisitTypeParameterList(context);
    }

    public override SyntaxNodeOrToken[] VisitTypeParameter(TypeParameterContext context)
    {
        return base.VisitTypeParameter(context);
    }

    public override SyntaxNodeOrToken[] VisitTypeArgumentList(TypeArgumentListContext context)
    {
        return base.VisitTypeArgumentList(context);
    }

    public override SyntaxNodeOrToken[] VisitTypeArgument(TypeArgumentContext context)
    {
        return base.VisitTypeArgument(context);
    }

    public override SyntaxNodeOrToken[] VisitParameterTemplate(ParameterTemplateContext context)
    {
        return base.VisitParameterTemplate(context);
    }

    public override SyntaxNodeOrToken[] VisitParameterGeneric(ParameterGenericContext context)
    {
        return base.VisitParameterGeneric(context);
    }

    public override SyntaxNodeOrToken[] VisitParameterValue(ParameterValueContext context)
    {
        return base.VisitParameterValue(context);
    }

    public override SyntaxNodeOrToken[] VisitType(TypeContext context)
        => new[] { new SyntaxNodeOrToken(
            new TypeSyntax(context.GetText())
        {
            Location = Location(context),
            Children = Children(base.VisitType, context)
        } )};

    //
    // Variables
    //

    public override SyntaxNodeOrToken[] VisitVariableDeclTyped(VariableDeclTypedContext context)
        => new[] { new SyntaxNodeOrToken(
            new VariableDeclarationTypedSyntax(context.GetText())
        {
            Location = Location(context),
            Children = Children(base.VisitVariableDeclTyped, context)
        } )};

    public override SyntaxNodeOrToken[] VisitVariableDeclInferred(VariableDeclInferredContext context)
        => new[] { new SyntaxNodeOrToken(
            new VariableDeclarationInferredSyntax(context.GetText())
        {
            Location = Location(context),
            Children = Children(base.VisitVariableDeclInferred, context)
        } )};

    //
    // Expressions
    //

    public override SyntaxNodeOrToken[] VisitExpression(ExpressionContext context)
    {
        var precedence = context.ParenOpen() is not null;
        var children = Children(base.VisitExpression, context);
        if (children.Count == 1 && !precedence) 
            return new[] { new SyntaxNodeOrToken(children[0]) };

        var newChildren = new List<SyntaxNode>();
        foreach (var child in children)
        {
            // unpack nested ExpressionSyntax nodes
            if (child is ExpressionSyntax childExpr &&
                !childExpr.Precedence &&
                // because 'is' also matches on derived types
                child.GetType().Name == nameof(ExpressionSyntax))
                newChildren.AddRange(childExpr.Children);
            else
                newChildren.Add(child);
        }

        return new[] { new SyntaxNodeOrToken(
            new ExpressionSyntax(context.GetText(), precedence)
        {
            Location = Location(context),
            Children = SyntaxNodeList.New(newChildren)
        } )};
    }

    public override SyntaxNodeOrToken[] VisitExpressionConst(ExpressionConstContext context)
    {
        // This level is represented by a base class.
        return base.VisitExpressionConst(context);
    }

    public override SyntaxNodeOrToken[] VisitExpressionLiteral(ExpressionLiteralContext context)
        => new[] { new SyntaxNodeOrToken(
            new ExpressionLiteralSyntax(context.GetText())
        {
            Location = Location(context),
            Children = Children(base.VisitExpressionLiteral, context)
        } )};

    public override SyntaxNodeOrToken[] VisitExpressionLiteralBool(ExpressionLiteralBoolContext context)
        => new[] { new SyntaxNodeOrToken(
            new ExpressionLiteralBoolSyntax(context.GetText())
        {
            Location = Location(context),
            Children = SyntaxNodeList.New()
        } )};

    public override SyntaxNodeOrToken[] VisitNumber(NumberContext context)
        => new[] { new SyntaxNodeOrToken(
            new LiteralNumberSyntax(context.GetText())
        {
            Location = Location(context),
            Children = SyntaxNodeList.New()
        } )};

    public override SyntaxNodeOrToken[] VisitString(StringContext context)
        => new[] { new SyntaxNodeOrToken(
            new LiteralStringSyntax(context.GetText())
        {
            Location = Location(context),
            Children = SyntaxNodeList.New()
        } )};

    public override SyntaxNodeOrToken[] VisitExpressionRule(ExpressionRuleContext context)
    {
        return base.VisitExpressionRule(context);
    }

    //
    // Expression Operators
    //

    public override SyntaxNodeOrToken[] VisitExpressionOperatorBinary(ExpressionOperatorBinaryContext context)
        => new[] { new SyntaxNodeOrToken(
            new ExpressionOperatorSyntax(context.GetText())
        {
            Location = Location(context),
            Children = SyntaxNodeList.New()
        } )};

    public override SyntaxNodeOrToken[] VisitExpressionOperatorUnaryPrefix(ExpressionOperatorUnaryPrefixContext context)
        => new[] { new SyntaxNodeOrToken(
            new ExpressionOperatorSyntax(context.GetText())
        {
            Location = Location(context),
            Children = SyntaxNodeList.New()
        } )};

    //
    // Identifiers
    //

    public override SyntaxNodeOrToken[] VisitNameIdentifier(NameIdentifierContext context)
        => new[] { new SyntaxNodeOrToken(
            new NameSyntax(context.GetText())
        {
            Location = Location(context),
            Children = SyntaxNodeList.New()
        } )};

    public override SyntaxNodeOrToken[] VisitNameQualified(NameQualifiedContext context)
        => new[] { new SyntaxNodeOrToken(
            new QualifiedNameSyntax(context.GetText())
        {
            Location = Location(context),
            Children = SyntaxNodeList.New()
        } )};

    //
    // Statements
    //

    public override SyntaxNodeOrToken[] VisitStatement(StatementContext context)
    {
        // This level is represented by a base class.
        return base.VisitStatement(context);
    }

    public override SyntaxNodeOrToken[] VisitStatementFlow(StatementFlowContext context)
    {
        // This level is not represented.
        return base.VisitStatementFlow(context);
    }

    public override SyntaxNodeOrToken[] VisitStatementRet(StatementRetContext context)
        => new[] { new SyntaxNodeOrToken(
            new StatementReturnSyntax(context.GetText())
        {
            Location = Location(context),
            Children = Children(base.VisitStatementRet, context)
        } )};
}