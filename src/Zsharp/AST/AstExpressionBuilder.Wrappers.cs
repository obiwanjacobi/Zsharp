using Antlr4.Runtime.Tree;
using System.Collections.Generic;
using static ZsharpParser;

namespace Zsharp.AST
{
    partial class AstExpressionBuilder
    {
        // stuff to make grammar rule context object polymorphic
        private interface IExpressionContextWrapper
        {
            bool HasOpenParam { get; }
            bool HasCloseParam { get; }
            bool IsOperand { get; }
            IEnumerable<IParseTree> Children { get; }

            AstExpression NewExpression();
        }

        private class ArithmeticContextWrapper : IExpressionContextWrapper
        {
            private readonly Expression_arithmeticContext _context;

            public ArithmeticContextWrapper(Expression_arithmeticContext ctx)
            {
                _context = ctx;
            }

            public bool HasOpenParam => _context.PARENopen() != null;

            public bool HasCloseParam => _context.PARENclose() != null;

            public bool IsOperand => _context.arithmetic_operand() != null;

            public IEnumerable<IParseTree> Children => _context.children;

            public AstExpression NewExpression()
            {
                return new AstExpression(_context);
            }
        }

        private class LogicContextWrapper : IExpressionContextWrapper
        {
            private readonly Expression_logicContext _context;

            public LogicContextWrapper(Expression_logicContext ctx)
            {
                _context = ctx;
            }

            public bool HasOpenParam => _context.PARENopen() != null;

            public bool HasCloseParam => _context.PARENclose() != null;

            public bool IsOperand => _context.logic_operand() != null;

            public IEnumerable<IParseTree> Children => _context.children;

            public AstExpression NewExpression()
            {
                return new AstExpression(_context);
            }
        }

        private class ComparisonContextWrapper : IExpressionContextWrapper
        {
            private readonly Expression_comparisonContext _context;

            public ComparisonContextWrapper(Expression_comparisonContext ctx)
            {
                _context = ctx;
            }

            public bool HasOpenParam => _context.PARENopen() != null;

            public bool HasCloseParam => _context.PARENclose() != null;

            public bool IsOperand => _context.comparison_operand() != null;

            public IEnumerable<IParseTree> Children => _context.children;

            public AstExpression NewExpression()
            {
                return new AstExpression(_context);
            }
        }
    }
}
