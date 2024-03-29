using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public partial class AstExpressionBuilder : AstNodeBuilder
    {
        private readonly Stack<AstExpressionOperand> _values = new();
        private readonly Stack<AstExpression> _operators = new();

        public AstExpressionBuilder(AstBuilderContext context, string ns)
            : base(context, ns)
        { }

        public AstExpression? Build(ParserRuleContext context)
        {
            var operand = VisitChildren(context);

            if (operand is not null)
            {
                if (operand is AstExpressionOperand expressionOperand)
                    _values.Push(expressionOperand);
                else
                    _values.Push(new AstExpressionOperand((AstNode)operand));

                var expr = new AstExpression(context);
                _operators.Push(expr);
            }

            return BuildExpression(0);
        }

        private object? ProcessExpression(ExpressionContextWrapper context)
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

                if (retVal is null)
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

            if (!expr.HasLHS &&
                expr.Operator == AstExpressionOperator.None &&
                expr.HasRHS && expr.RHS.LiteralNumeric is not null)
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

            if (expr is null &&
                _values.Count > 0)
            {
                expr = new AstExpression(_values.Pop());
                Ast.Guard(_values.Count == 0, "Orphan ExpressionOperands.");
            }

            return expr;
        }

        protected override object? AggregateResult(object? aggregate, object? nextResult)
        {
            if (nextResult is null)
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

        public override object? VisitExpression_value(Expression_valueContext context)
            => ToExpressionOperand(context);

        public override object? VisitArithmetic_operand(Arithmetic_operandContext context)
            => ToExpressionOperand(context);

        public override object? VisitLogic_operand(Logic_operandContext context)
            => ToExpressionOperand(context);

        public override object? VisitComparison_operand(Comparison_operandContext context)
            => ToExpressionOperand(context);

        private object? ToExpressionOperand(ParserRuleContext context)
        {
            var node = (AstNode?)base.VisitChildren(context);
            if (node is not null)
                return new AstExpressionOperand(node);
            return null;
        }

        public override object? VisitFunction_call(Function_callContext context)
            => CreateFunctionReference(context);

        public override object? VisitVariable_field_ref(Variable_field_refContext context)
        {
            var varRefCtx = context.variable_ref();
            var varRef = (AstVariableReference)VisitVariable_ref(varRefCtx)!;
            var fieldRef = New.AstTypeFieldReferenceStructField(context);

            BuilderContext.SetCurrent(fieldRef);
            VisitChildrenExcept(context, varRefCtx);
            BuilderContext.RevertCurrent();

            var symbols = BuilderContext.GetCurrent<IAstSymbolTableSite>();
            symbols.SymbolTable.Add(fieldRef);

            varRef.TrySetTypeFieldReference(fieldRef);

            return varRef;
        }

        public override object? VisitEnum_option_use(Enum_option_useContext context)
        {
            var enumOpt = New.AstTypeFieldReferenceEnumOption(context);

            // not using standard VisitChildren because 
            // Identifier needs special processing with dot-name.
            var identifier = New.AstIdentifier(context);
            enumOpt.SetIdentifier(identifier);

            var symbols = BuilderContext.GetCurrent<IAstSymbolTableSite>();
            symbols.SymbolTable.Add(enumOpt);

            return enumOpt;
        }

        public override object? VisitLiteral_bool(Literal_boolContext context)
            => New.AstLiteralBoolean(context);

        public override object? VisitNumber(NumberContext context)
            => AstLiteralNumeric.Create(context);

        public override object? VisitString(StringContext context)
            => new AstLiteralString(context);

        public override object? VisitOperator_arithmetic(Operator_arithmeticContext context)
        {
            if (context.DIV() is not null)
                return AstExpressionOperator.Divide;
            if (context.MINUS_NEG() is not null)
                return AstExpressionOperator.Minus;
            if (context.MOD() is not null)
                return AstExpressionOperator.Modulo;
            if (context.MULT_PTR() is not null)
                return AstExpressionOperator.Multiply;
            if (context.PLUS() is not null)
                return AstExpressionOperator.Plus;
            if (context.POW() is not null)
                return AstExpressionOperator.Power;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_arithmetic_unary(Operator_arithmetic_unaryContext context)
        {
            if (context.MINUS_NEG() is not null)
                return AstExpressionOperator.Negate;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_logic(Operator_logicContext context)
        {
            if (context.AND() is not null)
                return AstExpressionOperator.And;
            if (context.OR() is not null)
                return AstExpressionOperator.Or;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_logic_unary(Operator_logic_unaryContext context)
        {
            if (context.NOT() is not null)
                return AstExpressionOperator.Not;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_comparison(Operator_comparisonContext context)
        {
            if (context.EQ_ASSIGN() is not null)
                return AstExpressionOperator.Equal;
            if (context.GREAT_ANGLEclose() is not null)
                return AstExpressionOperator.Greater;
            if (context.GREQ() is not null)
                return AstExpressionOperator.GreaterEqual;
            if (context.NEQ() is not null)
                return AstExpressionOperator.NotEqual;
            if (context.SMALL_ANGLEopen() is not null)
                return AstExpressionOperator.Smaller;
            if (context.SMEQ() is not null)
                return AstExpressionOperator.SmallerEqual;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_bits(Operator_bitsContext context)
        {
            if (context.BIT_AND() is not null)
                return AstExpressionOperator.BitAnd;
            if (context.BIT_OR() is not null)
                return AstExpressionOperator.BitOr;
            if (context.BIT_ROLL() is not null)
                return AstExpressionOperator.BitRollLeft;
            if (context.BIT_ROLR() is not null)
                return AstExpressionOperator.BitRollRight;
            if (context.BIT_SHL() is not null)
                return AstExpressionOperator.BitShiftLeft;
            if (context.GREAT_ANGLEclose()?.Length == 2)
                return AstExpressionOperator.BitShiftRight;
            if (context.BIT_XOR_IMM() is not null)
                return AstExpressionOperator.BitXor;
            return AstExpressionOperator.None;
        }

        public override object? VisitOperator_bits_unary(Operator_bits_unaryContext context)
        {
            if (context.BIT_NOT() is not null)
                return AstExpressionOperator.BitNegate;
            return AstExpressionOperator.None;
        }

        //
        // Range
        //

        public override object VisitRange(RangeContext context)
        {
            var range = New.AstExpressionRange(context);

            BuilderContext.SetCurrent(range);
            VisitChildren(context);
            BuilderContext.RevertCurrent();
            return range;
        }

        public override object VisitRange_begin(Range_beginContext context)
        {
            var range = BuilderContext.GetCurrent<AstExpressionRange>();
            var expression = (AstExpressionOperand)VisitChildren(context)!;
            range.SetBegin(expression);
            return range;
        }

        public override object VisitRange_end(Range_endContext context)
        {
            var range = BuilderContext.GetCurrent<AstExpressionRange>();
            var expression = (AstExpressionOperand)VisitChildren(context)!;
            range.SetEnd(expression);
            return range;
        }

        public override object VisitRange_step(Range_stepContext context)
        {
            var range = BuilderContext.GetCurrent<AstExpressionRange>();
            var expression = (AstExpressionOperand)VisitChildren(context)!;
            range.SetStep(expression);
            return range;
        }

        public static AstExpression CreateLiteral(int value)
        {
            var numeric = new AstLiteralNumeric((UInt64)value);
            return new AstExpression(new AstExpressionOperand(numeric))
            {
                Operator = AstExpressionOperator.Number
            };
        }
    }
}