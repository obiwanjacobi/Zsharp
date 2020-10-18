using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;
using static Zsharp.Parser.ZsharpParser;

namespace UnitTests.AST
{
    [TestClass]
    public class AstExpressionBuilderTests
    {
        [TestMethod]
        public void Assignment1()
        {
            const string code =
                "a = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            var exp = assign.Expression;

            exp.Should().NotBeNull();
            exp.Operator.Should().Be(AstExpressionOperator.Number);
            exp.LHS.Should().BeNull();
            exp.RHS.Numeric.AsUnsigned().Should().Be(42);
        }

        [TestMethod]
        public void Arithmetic1()
        {
            const string code =
                "a = 2 + 4" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            var exp = assign.Expression;

            exp.Should().NotBeNull();
            exp.Operator.Should().Be(AstExpressionOperator.Plus);
            exp.LHS.Numeric.Should().NotBeNull();
            exp.RHS.Numeric.Should().NotBeNull();
            exp.LHS.Numeric.AsSigned().Should().Be(2);
            exp.RHS.Numeric.AsSigned().Should().Be(4);
        }

        [TestMethod]
        public void Arithmetic2()
        {
            const string code =
                "a = 2 + 4 * 6" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            var exp = assign.Expression;

            exp.Should().NotBeNull();
            exp.Operator.Should().Be(AstExpressionOperator.Plus);
            exp.LHS.Numeric.AsSigned().Should().Be(2);
            exp.RHS.Expression.Operator.Should().Be(AstExpressionOperator.Multiply);
        }

        [TestMethod]
        public void ArithmeticUnary1()
        {
            const string code =
                "a = -4" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            var exp = assign.Expression;

            exp.Should().NotBeNull();
            exp.Operator.Should().Be(AstExpressionOperator.Negate);
            exp.LHS.Should().BeNull();
            exp.RHS.Numeric.AsUnsigned().Should().Be(4);
        }

        [TestMethod]
        public void ArithmeticUnary2()
        {
            const string code =
                "a = 2 + -4" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            var exp = assign.Expression;

            exp.Should().NotBeNull();
            exp.Operator.Should().Be(AstExpressionOperator.Plus);
            exp.LHS.Numeric.AsSigned().Should().Be(2);
            exp.RHS.Expression.Operator.Should().Be(AstExpressionOperator.Negate);
            exp.RHS.Expression.RHS.Numeric.AsSigned().Should().Be(4);
        }

        [TestMethod]
        public void ArithmeticParenth()
        {
            const string code =
                "a = (2 + 4) * 6" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            var exp = assign.Expression;

            exp.Should().NotBeNull();
            exp.Operator.Should().Be(AstExpressionOperator.Multiply);
            exp.LHS.Expression.Operator.Should().Be(AstExpressionOperator.Plus);
            exp.RHS.Numeric.AsSigned().Should().Be(6);
        }

        [TestMethod]
        public void ArithmeticNestedParent()
        {
            const string code =
                "a = ((2 + 4) / 3) % 3" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            var exp = assign.Expression;

            exp.Should().NotBeNull();
            exp.Operator.Should().Be(AstExpressionOperator.Modulo);
            exp.RHS.Numeric.AsSigned().Should().Be(3);
            exp.LHS.Expression.Operator.Should().Be(AstExpressionOperator.Divide);
            exp.LHS.Expression.LHS.Expression.Operator.Should().Be(AstExpressionOperator.Plus);
        }

        [TestMethod]
        public void ArithmeticUnaryParenth1()
        {
            const string code =
                "a = 2 + -(4 * 3)" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            var exp = assign.Expression;

            exp.Should().NotBeNull();
            exp.Operator.Should().Be(AstExpressionOperator.Plus);
            exp.RHS.Expression.Operator.Should().Be(AstExpressionOperator.Negate);
            exp.LHS.Numeric.AsUnsigned().Should().Be(2);
        }

        [TestMethod]
        public void Comparison1()
        {
            const string code =
                "c = 2 > 4" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            var exp = assign.Expression;

            exp.Should().NotBeNull();
            exp.Operator.Should().Be(AstExpressionOperator.Greater);
            exp.LHS.Numeric.AsSigned().Should().Be(2);
            exp.RHS.Numeric.AsSigned().Should().Be(4);
        }

        [TestMethod]
        public void ComparisonEqualAssign()
        {
            const string code =
                "c = 2 = 4" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            var exp = assign.Expression;

            exp.Should().NotBeNull();
            exp.Operator.Should().Be(AstExpressionOperator.Equal);
            exp.LHS.Numeric.AsSigned().Should().Be(2);
            exp.RHS.Numeric.AsSigned().Should().Be(4);
        }

        [TestMethod]
        public void Logical1()
        {
            const string code =
                "b = not true" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            var exp = assign.Expression;

            exp.Should().NotBeNull();
            exp.Operator.Should().Be(AstExpressionOperator.Not);
            exp.LHS.Should().BeNull();
            exp.RHS.Context.Should().BeOfType<Literal_boolContext>();
        }

        [TestMethod]
        public void LogicalComparison1()
        {
            const string code =
                "b = not (42 > 100)" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            var exp = assign.Expression;

            exp.Should().NotBeNull();
            exp.Operator.Should().Be(AstExpressionOperator.Not);
            exp.LHS.Should().BeNull();
            exp.RHS.Expression.Operator.Should().Be(AstExpressionOperator.Greater);
        }
    }
}
