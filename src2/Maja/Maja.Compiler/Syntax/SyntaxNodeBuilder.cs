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

    public override SyntaxNode[] VisitDecl_pub1(Decl_pub1Context context)
        => new[] { new PublicExportSyntax
        {
            Location = Location(context),
            Children = new SyntaxNodeList(base.VisitDecl_pub1(context))
        } };

    public override SyntaxNode[] VisitDecl_pub2(Decl_pub2Context context)
        => new[] { new PublicExportSyntax
        {
            Location = Location(context),
            Children = new SyntaxNodeList(base.VisitDecl_pub2(context))
        } };

    public override SyntaxNode[] VisitDecl_use(Decl_useContext context)
        => new[] { new UseImportSyntax
        {
            Location = Location(context),
            Children = new SyntaxNodeList(base.VisitDecl_use(context))
        } };

    public override SyntaxNode[] VisitCode_block(Code_blockContext context)
    {
        return base.VisitCode_block(context);
    }

    public override SyntaxNode[] VisitDecl_member(Decl_memberContext context)
    {
        return base.VisitDecl_member(context);
    }

    //
    // Functions
    //

    public override SyntaxNode[] VisitDecl_function(Decl_functionContext context)
    {
        return base.VisitDecl_function(context);
    }

    public override SyntaxNode[] VisitDecl_function_local(Decl_function_localContext context)
    {
        return base.VisitDecl_function_local(context);
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

    public override SyntaxNode[] VisitDecl_type(Decl_typeContext context)
    {
        return base.VisitDecl_type(context);
    }

    public override SyntaxNode[] VisitDecl_type_members(Decl_type_membersContext context)
    {
        return base.VisitDecl_type_members(context);
    }

    public override SyntaxNode[] VisitDecl_member_enum(Decl_member_enumContext context)
    {
        return base.VisitDecl_member_enum(context);
    }

    public override SyntaxNode[] VisitDecl_member_field(Decl_member_fieldContext context)
    {
        return base.VisitDecl_member_field(context);
    }

    public override SyntaxNode[] VisitDecl_member_rule(Decl_member_ruleContext context)
    {
        return base.VisitDecl_member_rule(context);
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

    public override SyntaxNode[] VisitDecl_variable(Decl_variableContext context)
    {
        return base.VisitDecl_variable(context);
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
            Location = Location(context)
        } };

    public override SyntaxNode[] VisitName_qualified(Name_qualifiedContext context)
        => new[] { new QualifiedNameSyntax(context.GetText())
        {
            Location = Location(context)
        } };

    public override SyntaxNode[] VisitName_rule(Name_ruleContext context)
    {
        return base.VisitName_rule(context);
    }

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
}