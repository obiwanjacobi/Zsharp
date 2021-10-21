using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;
using Zsharp.Semantics;

namespace Zsharp.UnitTests.Semantics
{
    [TestClass]
    public class ExpressionValueTests
    {
        [TestMethod]
        public void LiteralNumeric()
        {
            var expression = new AstExpression(
                new AstExpressionOperand(
                    new AstLiteralNumeric(42)))
            {
                Operator = AstExpressionOperator.Number
            };

            var constValue = new ExpressionValue();
            expression.Accept(constValue);

            constValue.ValueInt.Should().Be(42);
        }

        [TestMethod]
        public void LiteralNumericPlus()
        {
            var expression = new AstExpression(
                new AstExpressionOperand(
                    new AstLiteralNumeric(42)))
            {
                Operator = AstExpressionOperator.Plus
            };

            expression.Add(new AstExpressionOperand(
                new AstLiteralNumeric(101)));

            var constValue = new ExpressionValue();
            expression.Accept(constValue);

            constValue.HasValue.Should().BeTrue();
            constValue.ValueInt.Should().Be(42 + 101);
        }

        [TestMethod]
        public void LiteralNumericMinus()
        {
            var expression = new AstExpression(
                new AstExpressionOperand(
                    new AstLiteralNumeric(42)))
            {
                Operator = AstExpressionOperator.Minus
            };

            expression.Add(new AstExpressionOperand(
                new AstLiteralNumeric(101)));

            var constValue = new ExpressionValue();
            expression.Accept(constValue);

            constValue.HasValue.Should().BeTrue();
            constValue.ValueInt.Should().Be(42 - 101);
        }

        [TestMethod]
        public void LiteralNumericMultiply()
        {
            var expression = new AstExpression(
                new AstExpressionOperand(
                    new AstLiteralNumeric(42)))
            {
                Operator = AstExpressionOperator.Multiply
            };

            expression.Add(new AstExpressionOperand(
                new AstLiteralNumeric(101)));

            var constValue = new ExpressionValue();
            expression.Accept(constValue);

            constValue.HasValue.Should().BeTrue();
            constValue.ValueInt.Should().Be(42 * 101);
        }

        [TestMethod]
        public void LiteralNumericDivide()
        {
            var expression = new AstExpression(
                new AstExpressionOperand(
                    new AstLiteralNumeric(101)))
            {
                Operator = AstExpressionOperator.Divide
            };

            expression.Add(new AstExpressionOperand(
                new AstLiteralNumeric(42)));

            var constValue = new ExpressionValue();
            expression.Accept(constValue);

            constValue.HasValue.Should().BeTrue();
            constValue.ValueInt.Should().Be(101 / 42);
        }

        [TestMethod]
        public void LiteralNumericEqual()
        {
            var expression = new AstExpression(
                new AstExpressionOperand(
                    new AstLiteralNumeric(101)))
            {
                Operator = AstExpressionOperator.Equal
            };

            expression.Add(new AstExpressionOperand(
                new AstLiteralNumeric(42)));

            var constValue = new ExpressionValue();
            expression.Accept(constValue);

            constValue.HasValue.Should().BeTrue();
            constValue.ValueBool.Should().BeFalse();
        }

        [TestMethod]
        public void LiteralNumericPlusEqual()
        {
            // 42 + 101 == 143
            var expression = new AstExpression(
                new AstExpressionOperand(
                    new AstLiteralNumeric(42)))
            {
                Operator = AstExpressionOperator.Plus
            };

            expression.Add(new AstExpressionOperand(
                new AstLiteralNumeric(101)));

            expression = new AstExpression(
                new AstExpressionOperand(expression))
            {
                Operator = AstExpressionOperator.Equal
            };

            expression.Add(new AstExpressionOperand(
                new AstLiteralNumeric(143)));

            var constValue = new ExpressionValue();
            expression.Accept(constValue);

            constValue.HasValue.Should().BeTrue();
            constValue.ValueBool.Should().BeTrue();
        }
    }
}
