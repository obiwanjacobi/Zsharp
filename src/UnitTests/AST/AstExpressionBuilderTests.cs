using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;

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
            exp.RHS.LiteralNumeric.AsUnsigned().Should().Be(42);
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
            exp.LHS.LiteralNumeric.Should().NotBeNull();
            exp.RHS.LiteralNumeric.Should().NotBeNull();
            exp.LHS.LiteralNumeric.AsSigned().Should().Be(2);
            exp.RHS.LiteralNumeric.AsSigned().Should().Be(4);
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
            exp.LHS.LiteralNumeric.AsSigned().Should().Be(2);
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
            exp.RHS.LiteralNumeric.AsUnsigned().Should().Be(4);
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
            exp.LHS.LiteralNumeric.AsSigned().Should().Be(2);
            exp.RHS.Expression.Operator.Should().Be(AstExpressionOperator.Negate);
            exp.RHS.Expression.RHS.LiteralNumeric.AsSigned().Should().Be(4);
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
            exp.RHS.LiteralNumeric.AsSigned().Should().Be(6);
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
            exp.RHS.LiteralNumeric.AsSigned().Should().Be(3);
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
            exp.LHS.LiteralNumeric.AsUnsigned().Should().Be(2);
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
            exp.LHS.LiteralNumeric.AsSigned().Should().Be(2);
            exp.RHS.LiteralNumeric.AsSigned().Should().Be(4);
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
            exp.LHS.LiteralNumeric.AsSigned().Should().Be(2);
            exp.RHS.LiteralNumeric.AsSigned().Should().Be(4);
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
            exp.RHS.LiteralBoolean.Should().NotBeNull();
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

        [TestMethod]
        public void VariableReference()
        {
            const string code =
                "b = x" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            var exp = assign.Expression;

            exp.Should().NotBeNull();
            exp.RHS.VariableReference.Should().NotBeNull();
            exp.RHS.VariableReference.Symbol.Should().NotBeNull();
        }

        [TestMethod]
        public void VariableReferencePlus()
        {
            const string code =
                "b = x + 1" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            var exp = assign.Expression;

            exp.Should().NotBeNull();
            exp.LHS.VariableReference.Should().NotBeNull();
            exp.LHS.VariableReference.Symbol.Should().NotBeNull();
        }

        [TestMethod]
        public void FunctionCall()
        {
            const string code =
                "r = fn()" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            var exp = assign.Expression;

            exp.Should().NotBeNull();
            exp.RHS.FunctionReference.Should().NotBeNull();
            exp.RHS.FunctionReference.Symbol.Should().NotBeNull();
        }
    }
}
