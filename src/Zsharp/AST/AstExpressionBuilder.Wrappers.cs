using Antlr4.Runtime.Tree;
using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    partial class AstExpressionBuilder
    {
        // stuff to make grammar rule context object polymorphic
        private abstract class ExpressionContextWrapper
        {
            public bool HasOpenParen { get; protected set; }
            public bool HasCloseParen { get; protected set; }
            public bool IsOperand { get; protected set; }
            public IEnumerable<IParseTree> Children { get; protected set; }

            public abstract AstExpression NewExpression();
        }

        private class ArithmeticContextWrapper : ExpressionContextWrapper
        {
            private readonly Expression_arithmeticContext _context;

            public ArithmeticContextWrapper(Expression_arithmeticContext context)
            {
                _context = context;
                HasOpenParen = _context.PARENopen() is not null;
                HasCloseParen = _context.PARENclose() is not null;
                IsOperand = _context.arithmetic_operand() is not null;
                Children = _context.children;
            }

            public override AstExpression NewExpression() => new(_context);
        }

        private class LogicContextWrapper : ExpressionContextWrapper
        {
            private readonly Expression_logicContext _context;

            public LogicContextWrapper(Expression_logicContext context)
            {
                _context = context;
                HasOpenParen = _context.PARENopen() is not null;
                HasCloseParen = _context.PARENclose() is not null;
                IsOperand = _context.logic_operand() is not null;
                Children = _context.children;
            }

            public override AstExpression NewExpression() => new(_context);
        }

        private class ComparisonContextWrapper : ExpressionContextWrapper
        {
            private readonly Expression_comparisonContext _context;

            public ComparisonContextWrapper(Expression_comparisonContext context)
            {
                _context = context;
                HasOpenParen = _context.PARENopen() is not null;
                HasCloseParen = _context.PARENclose() is not null;
                IsOperand = _context.comparison_operand() is not null;
                Children = _context.children;
            }

            public override AstExpression NewExpression() => new(_context);
        }
    }
}
