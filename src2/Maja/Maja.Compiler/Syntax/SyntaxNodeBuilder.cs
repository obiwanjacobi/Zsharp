using Antlr4.Runtime;
using Maja.Compiler.Parser;
using static Maja.Compiler.Parser.MajaParser;

namespace Maja.Compiler.Syntax;

internal sealed class SyntaxNodeBuilder : MajaParserBaseVisitor<SyntaxNode[]>
{
    private readonly string _sourceName;

    public SyntaxNodeBuilder(string sourceName)
        => _sourceName = sourceName ?? throw new System.ArgumentNullException(nameof(sourceName));

    protected override SyntaxNode[] AggregateResult(SyntaxNode[] aggregate, SyntaxNode[] nextResult)
    {
        if (aggregate is null) return nextResult;
        return aggregate.Append(nextResult);
    }

    private SyntaxLocation Location(ParserRuleContext context)
        => new(_sourceName, context.Start.Line, context.Start.Column,
            new SyntaxSpan(context.SourceInterval.a, context.SourceInterval.b));

    //
    // Module
    //

    public override SyntaxNode[] VisitCompilation_unit(Compilation_unitContext context)
        => new[] { new CompilationUnitSyntax
        {
            Location = Location(context),
            Children = new SyntaxNodeList(base.VisitCompilation_unit(context))
        } };

    public override SyntaxNode[] VisitPub1_decl(Pub1_declContext context)
        => new[] { new PublicExportSyntax
        {
            Location = Location(context),
            Children = new SyntaxNodeList(base.VisitPub1_decl(context))
        } };

    public override SyntaxNode[] VisitPub2_decl(Pub2_declContext context)
        => new[] { new PublicExportSyntax
        {
            Location = Location(context),
            Children = new SyntaxNodeList(base.VisitPub2_decl(context))
        } };

    public override SyntaxNode[] VisitUse_decl(Use_declContext context)
        => new[] { new UseImportSyntax
        {
            Location = Location(context),
            Children = new SyntaxNodeList(base.VisitUse_decl(context))
        } };

    public override SyntaxNode[] VisitCode_block(Code_blockContext context)
        => new[] { new CodeBlockSyntax
        {
            Location = Location(context),
            Children = new SyntaxNodeList(base.VisitCode_block(context))
        } };

    public override SyntaxNode[] VisitMembers_decl(Members_declContext context)
    {
        return base.VisitMembers_decl(context);
    }

    //
    // Functions
    //

    public override SyntaxNode[] VisitFunction_decl(Function_declContext context)
        => new[]{ new FunctionDelcarationSyntax
        {
            Location = Location(context),
            Children = new SyntaxNodeList(base.VisitFunction_decl(context))
        } };

    public override SyntaxNode[] VisitFunction_decl_local(Function_decl_localContext context)
    {
        return base.VisitFunction_decl_local(context);
    }

    public override SyntaxNode[] VisitParameter_list(Parameter_listContext context)
    {
        return base.VisitParameter_list(context);
    }

    public override SyntaxNode[] VisitParameter(ParameterContext context)
    {
        return base.VisitParameter(context);
    }

    public override SyntaxNode[] VisitParameter_self(Parameter_selfContext context)
    {
        return base.VisitParameter_self(context);
    }

    //
    // Types
    //

    public override SyntaxNode[] VisitType_decl(Type_declContext context)
    {
        return base.VisitType_decl(context);
    }

    public override SyntaxNode[] VisitType_decl_members(Type_decl_membersContext context)
    {
        return base.VisitType_decl_members(context);
    }

    public override SyntaxNode[] VisitMember_enum(Member_enumContext context)
    {
        return base.VisitMember_enum(context);
    }

    public override SyntaxNode[] VisitMember_field(Member_fieldContext context)
    {
        return base.VisitMember_field(context);
    }

    public override SyntaxNode[] VisitMember_rule(Member_ruleContext context)
    {
        return base.VisitMember_rule(context);
    }

    public override SyntaxNode[] VisitType_parameter_list(Type_parameter_listContext context)
    {
        return base.VisitType_parameter_list(context);
    }

    public override SyntaxNode[] VisitType_parameter(Type_parameterContext context)
    {
        return base.VisitType_parameter(context);
    }

    public override SyntaxNode[] VisitType_argument_list(Type_argument_listContext context)
    {
        return base.VisitType_argument_list(context);
    }

    public override SyntaxNode[] VisitType_argument(Type_argumentContext context)
    {
        return base.VisitType_argument(context);
    }

    public override SyntaxNode[] VisitTemplate_parameter(Template_parameterContext context)
    {
        return base.VisitTemplate_parameter(context);
    }

    public override SyntaxNode[] VisitGeneric_parameter(Generic_parameterContext context)
    {
        return base.VisitGeneric_parameter(context);
    }

    public override SyntaxNode[] VisitValue_parameter(Value_parameterContext context)
    {
        return base.VisitValue_parameter(context);
    }

    public override SyntaxNode[] VisitType(TypeContext context)
    {
        return base.VisitType(context);
    }

    //
    // Variables
    //

    public override SyntaxNode[] VisitVariable_decl(Variable_declContext context)
    {
        return base.VisitVariable_decl(context);
    }

    //
    // Expressions
    //

    public override SyntaxNode[] VisitExpression(ExpressionContext context)
    {
        return base.VisitExpression(context);
    }

    public override SyntaxNode[] VisitExpression_const(Expression_constContext context)
    {
        return base.VisitExpression_const(context);
    }

    public override SyntaxNode[] VisitExpression_rule(Expression_ruleContext context)
    {
        return base.VisitExpression_rule(context);
    }

    //
    // Identifiers
    //

    public override SyntaxNode[] VisitName_identifier(Name_identifierContext context)
        => new[] { new NameSyntax(context.GetText())
        {
            Location = Location(context),
            Children = new SyntaxNodeList()
        } };

    public override SyntaxNode[] VisitName_qualified(Name_qualifiedContext context)
        => new[] { new QualifiedNameSyntax(context.GetText())
        {
            Location = Location(context),
            Children = new SyntaxNodeList()
        } };

    //
    // Statements
    //

    public override SyntaxNode[] VisitStatement(StatementContext context)
    {
        return base.VisitStatement(context);
    }

    public override SyntaxNode[] VisitStatement_flow(Statement_flowContext context)
    {
        return base.VisitStatement_flow(context);
    }

    public override SyntaxNode[] VisitStatement_ret(Statement_retContext context)
        => new[] { new StatementReturnSyntax
        {
            Location = Location(context),
            Children = new SyntaxNodeList()
        } };
}