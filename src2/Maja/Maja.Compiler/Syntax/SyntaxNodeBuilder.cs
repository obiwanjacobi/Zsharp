using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Maja.Compiler.Parser;
using static Maja.Compiler.Parser.MajaParser;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Visits the Antlr parse tree context objects and generates the Syntax Model instances.
/// </summary>
internal sealed class SyntaxNodeBuilder : MajaParserBaseVisitor<SyntaxNode[]>
{
    private readonly string SourceName;

    public SyntaxNodeBuilder(string sourceName)
        => SourceName = sourceName ?? throw new System.ArgumentNullException(nameof(sourceName));

    protected override SyntaxNode[] AggregateResult(SyntaxNode[] aggregate, SyntaxNode[] nextResult)
    {
        if (aggregate is null) return nextResult;
        return aggregate.Append(nextResult);
    }

    private SyntaxLocation Location(ParserRuleContext context)
        => new(SourceName, context.Start.Line, context.Start.Column,
            new SyntaxSpan(context.SourceInterval.a, context.SourceInterval.b));

    //
    // Module
    //

    public override SyntaxNode[] VisitCompilationUnit(CompilationUnitContext context)
        => new[] { new CompilationUnitSyntax
        {
            Location = Location(context),
            Children = SyntaxNodeList.New(base.VisitCompilationUnit(context))
        } };

    public override SyntaxNode[] VisitPub1Decl(Pub1DeclContext context)
        => new[] { new PublicExportSyntax
        {
            Location = Location(context),
            Children = SyntaxNodeList.New(base.VisitPub1Decl(context))
        } };

    public override SyntaxNode[] VisitPub2Decl(Pub2DeclContext context)
        => new[] { new PublicExportSyntax
        {
            Location = Location(context),
            Children = SyntaxNodeList.New(base.VisitPub2Decl(context))
        } };

    public override SyntaxNode[] VisitUseDecl(UseDeclContext context)
        => new[] { new UseImportSyntax
        {
            Location = Location(context),
            Children = SyntaxNodeList.New(base.VisitUseDecl(context))
        } };

    public override SyntaxNode[] VisitCodeBlock(CodeBlockContext context)
        => new[] { new CodeBlockSyntax
        {
            Location = Location(context),
            Children = SyntaxNodeList.New(base.VisitCodeBlock(context))
        } };

    public override SyntaxNode[] VisitMembersDecl(MembersDeclContext context)
    {
        // This level is represented by a base class.
        return base.VisitMembersDecl(context);
    }

    //
    // Functions
    //

    public override SyntaxNode[] VisitFunctionDecl(FunctionDeclContext context)
        => new[]{ new FunctionDelcarationSyntax
        {
            Location = Location(context),
            Children = SyntaxNodeList.New(base.VisitFunctionDecl(context))
        } };

    public override SyntaxNode[] VisitFunctionDeclLocal(FunctionDeclLocalContext context)
    {
        return base.VisitFunctionDeclLocal(context);
    }

    public override SyntaxNode[] VisitParameterList(ParameterListContext context)
    {
        // This level is not represented.
        return base.VisitParameterList(context);
    }

    public override SyntaxNode[] VisitParameter(ParameterContext context)
        => new[] { new ParameterSyntax
        {
            Location = Location(context),
            Children = SyntaxNodeList.New(base.VisitParameter(context))
        } };

    //
    // Types
    //

    public override SyntaxNode[] VisitTypeDecl(TypeDeclContext context)
    {
        return base.VisitTypeDecl(context);
    }

    public override SyntaxNode[] VisitTypeDeclMembers(TypeDeclMembersContext context)
    {
        return base.VisitTypeDeclMembers(context);
    }

    public override SyntaxNode[] VisitMemberEnum(MemberEnumContext context)
    {
        return base.VisitMemberEnum(context);
    }

    public override SyntaxNode[] VisitMemberField(MemberFieldContext context)
    {
        return base.VisitMemberField(context);
    }

    public override SyntaxNode[] VisitMemberRule(MemberRuleContext context)
    {
        return base.VisitMemberRule(context);
    }

    public override SyntaxNode[] VisitTypeParameterList(TypeParameterListContext context)
    {
        return base.VisitTypeParameterList(context);
    }

    public override SyntaxNode[] VisitTypeParameter(TypeParameterContext context)
    {
        return base.VisitTypeParameter(context);
    }

    public override SyntaxNode[] VisitTypeArgumentList(TypeArgumentListContext context)
    {
        return base.VisitTypeArgumentList(context);
    }

    public override SyntaxNode[] VisitTypeArgument(TypeArgumentContext context)
    {
        return base.VisitTypeArgument(context);
    }

    public override SyntaxNode[] VisitParameterTemplate(ParameterTemplateContext context)
    {
        return base.VisitParameterTemplate(context);
    }

    public override SyntaxNode[] VisitParameterGeneric(ParameterGenericContext context)
    {
        return base.VisitParameterGeneric(context);
    }

    public override SyntaxNode[] VisitParameterValue(ParameterValueContext context)
    {
        return base.VisitParameterValue(context);
    }

    public override SyntaxNode[] VisitType(TypeContext context)
        => new[] { new TypeSyntax
        {
            Location = Location(context),
            Children = SyntaxNodeList.New(base.VisitType(context))
        } };

    //
    // Variables
    //

    public override SyntaxNode[] VisitVariableDecl(VariableDeclContext context)
        => new[] { new VariableDeclarationSyntax
        {
            Location = Location(context),
            Children = SyntaxNodeList.New(base.VisitVariableDecl(context))
        } };

    //
    // Expressions
    //

    public override SyntaxNode[] VisitExpression(ExpressionContext context)
    {
        var precedence = context.ParenOpen() is not null;
        var children = base.VisitExpression(context);
        if (children.Length == 1 && !precedence) return children;

        var newChildren = new List<SyntaxNode>();
        foreach (var child in children)
        {
            // unpack nested ExpressionSyntax nodes
            if (child is ExpressionSyntax childExpr &&
                !childExpr.Precedence &&
                // because 'is' also matches on derived types
                child.GetType().Name == nameof(ExpressionSyntax))
                newChildren.AddRange(child.Children);
            else
                newChildren.Add(child);
        }

        return new[] { new ExpressionSyntax(precedence)
        {
            Location = Location(context),
            Children = SyntaxNodeList.New(newChildren)
        } };
    }

    public override SyntaxNode[] VisitExpressionConst(ExpressionConstContext context)
    {
        // This level is represented by a base class.
        return base.VisitExpressionConst(context);
    }

    public override SyntaxNode[] VisitExpressionLiteral(ExpressionLiteralContext context)
            // TODO: child can be Number or String literal
            => new[] { new ExpressionLiteralSyntax(context.GetText())
        {
            Location = Location(context),
            Children = SyntaxNodeList.New()
        } };

    public override SyntaxNode[] VisitExpressionLiteralBool(ExpressionLiteralBoolContext context)
        => new[] { new ExpressionLiteralBoolSyntax(context.GetText())
        {
            Location = Location(context),
            Children = SyntaxNodeList.New()
        } };

    public override SyntaxNode[] VisitExpressionRule(ExpressionRuleContext context)
    {
        return base.VisitExpressionRule(context);
    }

    //
    // Expression Operators
    //

    public override SyntaxNode[] VisitExpressionOperatorBinary(ExpressionOperatorBinaryContext context)
        => new[] { new ExpressionOperatorSyntax(context.GetText())
        {
            Location = Location(context),
            Children = SyntaxNodeList.New()
        } };

    public override SyntaxNode[] VisitExpressionOperatorUnaryPrefix(ExpressionOperatorUnaryPrefixContext context)
        => new[] { new ExpressionOperatorSyntax(context.GetText())
        {
            Location = Location(context),
            Children = SyntaxNodeList.New()
        } };

    //
    // Identifiers
    //

    public override SyntaxNode[] VisitNameIdentifier(NameIdentifierContext context)
        => new[] { new NameSyntax(context.GetText())
        {
            Location = Location(context),
            Children = SyntaxNodeList.New()
        } };

    public override SyntaxNode[] VisitNameQualified(NameQualifiedContext context)
        => new[] { new QualifiedNameSyntax(context.GetText())
        {
            Location = Location(context),
            Children = SyntaxNodeList.New()
        } };

    //
    // Statements
    //

    public override SyntaxNode[] VisitStatement(StatementContext context)
    {
        // This level is represented by a base class.
        return base.VisitStatement(context);
    }

    public override SyntaxNode[] VisitStatementFlow(StatementFlowContext context)
    {
        // This level is not represented.
        return base.VisitStatementFlow(context);
    }

    public override SyntaxNode[] VisitStatementRet(StatementRetContext context)
        => new[] { new StatementReturnSyntax
        {
            Location = Location(context),
            Children = SyntaxNodeList.New(base.VisitStatementRet(context))
        } };
}