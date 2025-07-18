using System.Linq;
using Maja.Compiler.Syntax;

namespace Maja.UnitTests.Compiler.Syntax;

public class VariableSyntaxTests
{
    private readonly ITestOutputHelper _output;

    public VariableSyntaxTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void VarType()
    {
        const string code =
            "x: U8" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<DeclarationVariableTypedSyntax>();
        v.Name.Text.Should().Be("x");
        v.Type!.Name.Text.Should().Be("U8");

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void VarType_Init()
    {
        const string code =
            "x: U8 = 42" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<DeclarationVariableTypedSyntax>();
        v.Name.Text.Should().Be("x");
        v.Type!.Name.Text.Should().Be("U8");
        v.Expression.Should().NotBeNull();
        v.Expression.As<ExpressionLiteralSyntax>().LiteralNumber!.Text.Should().Be("42");

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void VarTypeArgs()
    {
        const string code =
            "x: Bit<U8, 4>" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<DeclarationVariableTypedSyntax>();
        v.Name.Text.Should().Be("x");
        v.Type!.Name.Text.Should().Be("Bit");
        v.Type!.TypeArguments.Should().HaveCount(2);
        var arg = v.Type!.TypeArguments.First().As<TypeArgumentSyntax>();
        arg.Type.Should().NotBeNull();
        arg.Type!.Name.Text.Should().Be("U8");
        arg = v.Type!.TypeArguments.Skip(1).First().As<TypeArgumentSyntax>();
        arg.Expression.Should().NotBeNull();
        arg.Expression!.As<ExpressionLiteralSyntax>().LiteralNumber!.Text.Should().Be("4");

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void VarAssignmentInfer()
    {
        const string code =
            "x := 42" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<DeclarationVariableInferredSyntax>();
        v.Name.Text.Should().Be("x");
        v.Expression.As<ExpressionLiteralSyntax>().LiteralNumber!.Text.Should().Be("42");

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void VarAssignment()
    {
        const string code =
            "x: U8" + Tokens.Eol +
            "x = 42" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);

        var v = result.Members.First().As<DeclarationVariableTypedSyntax>();
        v.Name.Text.Should().Be("x");
        v.Type.Name.Text.Should().Be("U8");

        result.Statements.Should().HaveCount(1);
        var stat = result.Statements.First().As<StatementAssignmentSyntax>();
        stat.Name.Text.Should().Be("x");
        stat.Expression.As<ExpressionLiteralSyntax>().Text.Should().Be("42");

        stat.AssignmentOperator.Operators.Should().HaveCount(1);

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void VarInferred_AssignmentOperators()
    {
        const string code =
            "x :<= 42" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<DeclarationVariableInferredSyntax>();

        v.AssignmentOperator.Should().NotBeNull();
        var o = v.AssignmentOperator!.Operators.ToList();
        o.Should().HaveCount(2);
        o[0].Text.Should().Be("<");
        o[1].Text.Should().Be("=");

        Syntax.RoundTrip(code, _output);
    }
}