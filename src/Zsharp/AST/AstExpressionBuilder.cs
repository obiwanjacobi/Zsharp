using Antlr4.Runtime;
using System.Collections.Generic;
using static ZsharpParser;

namespace Zsharp.AST
{
    public partial class AstExpressionBuilder : ZsharpBaseVisitor<object?>
    {
        private readonly Stack<AstExpressionOperand> _values = new Stack<AstExpressionOperand>();
        private readonly Stack<AstExpression> _operators = new Stack<AstExpression>();
        private readonly AstBuilderContext _builderContext;

        public AstExpressionBuilder(AstBuilderContext context)
        {
            _builderContext = context;
        }

        public AstExpression? Build(Expression_valueContext expressionCtx)
        {
            var val = VisitExpression_value(expressionCtx);
            return Cast(val);
        }

        public AstExpression? Build(Expression_logicContext expressionCtx)
        {
            var val = VisitExpression_logic(expressionCtx);
            return Cast(val);
        }

        public AstExpression? Test(ParserRuleContext ctx)
        {
            var val = Visit(ctx);
            return Cast(val);
        }

        private AstExpression? Cast(object? result)
        {
            if (result == null)
                return BuildExpression(0);
            return result as AstExpression;
        }

        private object? ProcessExpression(IExpressionContextWrapper ctx)
        {
            if (ctx.HasOpenParen)
            {
                var expr = ctx.NewExpression();
                expr.Operator = AstExpressionOperator.Open;
                _operators.Push(expr);
            }

            var operatorPosition = _operators.Count;

            foreach (var child in ctx.Children)
            {
                var retVal = child.Accept(this);

                if (retVal == null)
                    continue;
                if (retVal is AstExpressionOperator op)
                {
                    // there is an extra expression node with just the operand.
                    // return its value and be done.
                    if (ctx.IsOperand)
                    {
                        return retVal;
                    }

                    if (op != AstExpressionOperator.None)
                    {
                        var expr = ctx.NewExpression();
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

            if (ctx.HasCloseParen)
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
                expr.RHS?.Numeric != null)
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

        public override object? VisitExpression_value(Expression_valueContext ctx)
        {
            var number = ctx.number();
            if (number != null)
            {
                var nr = VisitNumber(number);
                Ast.Guard<AstExpressionOperand>(nr);
                _values.Push((AstExpressionOperand)nr!);

                var expr = new AstExpression(ctx);
                _operators.Push(expr);
                return null;
            }

            if (ctx.expression_bool() != null)
            {
                var operand = VisitChildren(ctx);
                Ast.Guard<AstExpressionOperand>(operand);
                _values.Push((AstExpressionOperand)operand!);

                var expr = new AstExpression(ctx);
                _operators.Push(expr);
                return null;
            }

            return base.VisitChildren(ctx);
        }

        public override object? VisitExpression_arithmetic(Expression_arithmeticContext ctx)
        {
            return ProcessExpression(new ArithmeticContextWrapper(ctx));
        }

        public override object? VisitExpression_logic(Expression_logicContext ctx)
        {
            return ProcessExpression(new LogicContextWrapper(ctx));
        }

        public override object? VisitExpression_comparison(Expression_comparisonContext ctx)
        {
            return ProcessExpression(new ComparisonContextWrapper(ctx));
        }

        public override object? VisitLiteral_bool(Literal_boolContext ctx)
        {
            return new AstExpressionOperand(ctx);
        }

        public override object? VisitFunction_call(Function_callContext ctx)
        {
            return new AstExpressionOperand(ctx);
        }

        public override object? VisitVariable_ref(Variable_refContext ctx)
        {
            var varRef = new AstVariableReference(ctx);

            _builderContext.SetCurrent(varRef);

            VisitChildren(ctx);

            _builderContext.RevertCurrent();

            return new AstExpressionOperand(varRef);
        }

        public override object? VisitNumber(NumberContext ctx)
        {
            var builder = new AstNumericBuilder();
            var numeric = builder.Build(ctx);
            Ast.Guard(numeric, "AstNumericBuilder did not produce instance.");
            var operand = new AstExpressionOperand(numeric!);
            return operand;
        }

        public override object? VisitOperator_arithmetic(Operator_arithmeticContext ctx)
        {
            if (ctx.DIV() != null)
                return AstExpressionOperator.Divide;
            if (ctx.MINUS_NEG() != null)
                return AstExpressionOperator.Minus;
            if (ctx.MOD() != null)
                return AstExpressionOperator.Modulo;
            if (ctx.MULT_PTR() != null)
                return AstExpressionOperator.Multiply;
            if (ctx.PLUS() != null)
                return AstExpressionOperator.Plus;
            if (ctx.POW() != null)
                return AstExpressionOperator.Power;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_arithmetic_unary(Operator_arithmetic_unaryContext ctx)
        {
            if (ctx.MINUS_NEG() != null)
                return AstExpressionOperator.Negate;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_logic(Operator_logicContext ctx)
        {
            if (ctx.AND() != null)
                return AstExpressionOperator.And;
            if (ctx.OR() != null)
                return AstExpressionOperator.Or;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_logic_unary(Operator_logic_unaryContext ctx)
        {
            if (ctx.NOT() != null)
                return AstExpressionOperator.Not;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_comparison(Operator_comparisonContext ctx)
        {
            if (ctx.EQ_ASSIGN() != null)
                return AstExpressionOperator.Equal;
            if (ctx.GREAT_ANGLEclose() != null)
                return AstExpressionOperator.Greater;
            if (ctx.GREQ() != null)
                return AstExpressionOperator.GreaterEqual;
            if (ctx.NEQ() != null)
                return AstExpressionOperator.NotEqual;
            if (ctx.SMALL_ANGLEopen() != null)
                return AstExpressionOperator.Smaller;
            if (ctx.SMEQ() != null)
                return AstExpressionOperator.SmallerEqual;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_bits(Operator_bitsContext ctx)
        {
            if (ctx.BIT_AND() != null)
                return AstExpressionOperator.BitAnd;
            if (ctx.BIT_OR() != null)
                return AstExpressionOperator.BitOr;
            if (ctx.BIT_ROLL() != null)
                return AstExpressionOperator.BitRollLeft;
            if (ctx.BIT_ROLR() != null)
                return AstExpressionOperator.BitRollRight;
            if (ctx.BIT_SHL() != null)
                return AstExpressionOperator.BitShiftLeft;
            if (ctx.BIT_SHR() != null)
                return AstExpressionOperator.BitShiftRight;
            if (ctx.BIT_XOR_IMM() != null)
                return AstExpressionOperator.BitXor;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_bits_unary(Operator_bits_unaryContext ctx)
        {
            if (ctx.BIT_NOT() != null)
                return AstExpressionOperator.BitNegate;
            return AstExpressionOperator.None;
        }

        public override object? VisitIdentifier_type(Identifier_typeContext ctx)
        {
            bool success = _builderContext.AddIdentifier(new AstIdentifier(ctx));
            Ast.Guard(success, "AddIdentifier(Type) failed");
            return null;
        }

        public override object? VisitIdentifier_var(Identifier_varContext ctx)
        {
            bool success = _builderContext.AddIdentifier(new AstIdentifier(ctx));
            Ast.Guard(success, "AddIdentifier(Variable) failed");
            return null;
        }

        public override object? VisitIdentifier_param(Identifier_paramContext ctx)
        {
            bool success = _builderContext.AddIdentifier(new AstIdentifier(ctx));
            Ast.Guard(success, "AddIdentifier(Parameter) failed");
            return null;
        }

        public override object? VisitIdentifier_func(Identifier_funcContext ctx)
        {
            bool success = _builderContext.AddIdentifier(new AstIdentifier(ctx));
            Ast.Guard(success, "AddIdentifier(Function) failed");
            return null;
        }
    }
}