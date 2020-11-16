using Antlr4.Runtime;
using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public partial class AstExpressionBuilder : AstNodeBuilder
    {
        private readonly Stack<AstExpressionOperand> _values = new Stack<AstExpressionOperand>();
        private readonly Stack<AstExpression> _operators = new Stack<AstExpression>();

        public AstExpressionBuilder(AstBuilderContext context, string ns)
            : base(context, ns)
        { }

        public AstExpression? Build(Expression_valueContext context)
        {
            var operand = VisitChildren(context);

            if (operand != null)
            {
                _values.Push(new AstExpressionOperand((AstNode)operand));

                var expr = new AstExpression(context);
                _operators.Push(expr);
            }

            return BuildExpression(0);
        }

        public AstExpression? Build(Expression_logicContext context)
        {
            var val = VisitExpression_logic(context);
            return Cast(val);
        }

        public AstExpression? Test(ParserRuleContext context)
        {
            var val = Visit(context);
            return Cast(val);
        }

        private AstExpression? Cast(object? result)
        {
            if (result == null)
                return BuildExpression(0);
            return result as AstExpression;
        }

        private object? ProcessExpression(IExpressionContextWrapper context)
        {
            if (context.HasOpenParen)
            {
                var expr = context.NewExpression();
                expr.Operator = AstExpressionOperator.Open;
                _operators.Push(expr);
            }

            var operatorPosition = _operators.Count;

            foreach (var child in context.Children)
            {
                var retVal = child.Accept(this);

                if (retVal == null)
                    continue;
                if (retVal is AstExpressionOperator op)
                {
                    // there is an extra expression node with just the operand.
                    // return its value and be done.
                    if (context.IsOperand)
                    {
                        return retVal;
                    }

                    if (op != AstExpressionOperator.None)
                    {
                        var expr = context.NewExpression();
                        expr.Operator = op;

                        if (_operators.Count > 0)
                        {
                            var lowerOp = _operators.Peek();

                            if (lowerOp.Operator != AstExpressionOperator.Open &&
                                lowerOp.Precedence > expr.Precedence)
                            {
                                var popExpr = PopExpression();
                                Ast.Guard(lowerOp == popExpr, "Expression stack is out of sync.");

                                AddOperand(popExpr);
                            }
                        }
                        _operators.Push(expr);
                    }
                }
                else if (retVal is AstExpressionOperand exprOp)
                {
                    _values.Push(exprOp);
                }
            }

            if (context.HasCloseParen)
            {
                var expr = BuildExpression(operatorPosition);
                Ast.Guard(expr, "BuildExpression did not produce an instance.");
                AddOperand(expr!);

                // open operator only used as marker - discard
                Ast.Guard(_operators.Peek().Operator == AstExpressionOperator.Open, "Expression Operator stack out of sync.");
                _operators.Pop();
            }

            return null;
        }

        private void AddOperand(AstExpression expr)
        {
            var operand = new AstExpressionOperand(expr);
            _values.Push(operand);
        }

        private AstExpression PopExpression()
        {
            Ast.Guard(_values.Count > 0, "Expression stack underflow.");

            AstExpression expr = _operators.Pop();

            if (expr.Add(_values.Peek()))
            {
                _values.Pop();
            }

            if (_values.Count > 0 &&
                expr.Add(_values.Peek()))
            {
                _values.Pop();
            }

            if (expr.LHS == null &&
                expr.Operator == AstExpressionOperator.None &&
                expr.RHS?.LiteralNumeric != null)
            {
                // expression used as a number wrapper
                expr.Operator = AstExpressionOperator.Number;
            }

            return expr;
        }

        private AstExpression? BuildExpression(int stopAtCount)
        {
            AstExpression? expr = null;

            while (_operators.Count > stopAtCount)
            {
                expr = PopExpression();

                if (_operators.Count > stopAtCount)
                {
                    AddOperand(expr);
                }
            }

            if (expr == null &&
                _values.Count > 0)
            {
                expr = new AstExpression(_values.Pop());
                Ast.Guard(_values.Count == 0, "Orphan ExpressionOperands.");
            }

            return expr;
        }

        protected override object? AggregateResult(object? aggregate, object? nextResult)
        {
            if (nextResult == null)
            {
                return aggregate;
            }
            return nextResult;
        }

        public override object? VisitExpression_arithmetic(Expression_arithmeticContext context)
            => ProcessExpression(new ArithmeticContextWrapper(context));

        public override object? VisitExpression_logic(Expression_logicContext context)
            => ProcessExpression(new LogicContextWrapper(context));

        public override object? VisitExpression_comparison(Expression_comparisonContext context)
            => ProcessExpression(new ComparisonContextWrapper(context));

        public override object? VisitArithmetic_operand(Arithmetic_operandContext context)
        {
            var node = (AstNode?)base.VisitChildren(context);
            if (node != null)
                return new AstExpressionOperand(node);
            return null;
        }

        public override object? VisitLogic_operand(Logic_operandContext context)
        {
            var node = (AstNode?)base.VisitChildren(context);
            if (node != null)
                return new AstExpressionOperand(node);
            return null;
        }

        public override object? VisitComparison_operand(Comparison_operandContext context)
        {
            var node = (AstNode?)base.VisitChildren(context);
            if (node != null)
                return new AstExpressionOperand(node);
            return null;
        }

        public override object? VisitFunction_call(Function_callContext context)
        {
            var function = new AstFunctionReference(context);

            BuilderContext.SetCurrent(function);
            _ = VisitChildren(context);
            BuilderContext.RevertCurrent();

            var symbols = BuilderContext.GetCurrent<IAstSymbolTableSite>();
            symbols.Symbols.Add(function);

            return (function);
        }

        public override object? VisitType_conv(Type_convContext context)
        {
            var builder = new AstTypeConversionBuilder(BuilderContext);
            var function = (AstFunctionReference)builder.VisitType_conv(context);

            BuilderContext.SetCurrent(function);
            _ = VisitChildren(context);
            BuilderContext.RevertCurrent();

            var symbols = BuilderContext.GetCurrent<IAstSymbolTableSite>();
            symbols.Symbols.Add(function);

            return function;
        }

        public override object? VisitVariable_ref(Variable_refContext context)
        {
            var varRef = new AstVariableReference(context);

            BuilderContext.SetCurrent(varRef);
            VisitChildren(context);
            BuilderContext.RevertCurrent();

            var symbols = BuilderContext.GetCurrent<IAstSymbolTableSite>();
            symbols.Symbols.Add(varRef);

            return varRef;
        }

        public override object? VisitLiteral_bool(Literal_boolContext context)
            => new AstLiteralBoolean(context);

        public override object? VisitNumber(NumberContext context)
            => AstLiteralNumeric.Create(context);

        public override object? VisitString(StringContext context)
            => new AstLiteralString(context);

        public override object? VisitOperator_arithmetic(Operator_arithmeticContext context)
        {
            if (context.DIV() != null)
                return AstExpressionOperator.Divide;
            if (context.MINUS_NEG() != null)
                return AstExpressionOperator.Minus;
            if (context.MOD() != null)
                return AstExpressionOperator.Modulo;
            if (context.MULT_PTR() != null)
                return AstExpressionOperator.Multiply;
            if (context.PLUS() != null)
                return AstExpressionOperator.Plus;
            if (context.POW() != null)
                return AstExpressionOperator.Power;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_arithmetic_unary(Operator_arithmetic_unaryContext context)
        {
            if (context.MINUS_NEG() != null)
                return AstExpressionOperator.Negate;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_logic(Operator_logicContext context)
        {
            if (context.AND() != null)
                return AstExpressionOperator.And;
            if (context.OR() != null)
                return AstExpressionOperator.Or;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_logic_unary(Operator_logic_unaryContext context)
        {
            if (context.NOT() != null)
                return AstExpressionOperator.Not;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_comparison(Operator_comparisonContext context)
        {
            if (context.EQ_ASSIGN() != null)
                return AstExpressionOperator.Equal;
            if (context.GREAT_ANGLEclose() != null)
                return AstExpressionOperator.Greater;
            if (context.GREQ() != null)
                return AstExpressionOperator.GreaterEqual;
            if (context.NEQ() != null)
                return AstExpressionOperator.NotEqual;
            if (context.SMALL_ANGLEopen() != null)
                return AstExpressionOperator.Smaller;
            if (context.SMEQ() != null)
                return AstExpressionOperator.SmallerEqual;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_bits(Operator_bitsContext context)
        {
            if (context.BIT_AND() != null)
                return AstExpressionOperator.BitAnd;
            if (context.BIT_OR() != null)
                return AstExpressionOperator.BitOr;
            if (context.BIT_ROLL() != null)
                return AstExpressionOperator.BitRollLeft;
            if (context.BIT_ROLR() != null)
                return AstExpressionOperator.BitRollRight;
            if (context.BIT_SHL() != null)
                return AstExpressionOperator.BitShiftLeft;
            if (context.BIT_SHR() != null)
                return AstExpressionOperator.BitShiftRight;
            if (context.BIT_XOR_IMM() != null)
                return AstExpressionOperator.BitXor;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_bits_unary(Operator_bits_unaryContext context)
        {
            if (context.BIT_NOT() != null)
                return AstExpressionOperator.BitNegate;
            return AstExpressionOperator.None;
        }
    }
}