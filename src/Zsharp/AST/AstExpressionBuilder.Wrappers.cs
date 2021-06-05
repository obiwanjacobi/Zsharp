using Antlr4.Runtime.Tree;
using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    partial class AstExpressionBuilder
    {
        // stuff to make grammar rule context object polymorphic
        private interface IExpressionContextWrapper
        {
            bool HasOpenParen { get; }
            bool HasCloseParen { get; }
            bool IsOperand { get; }
            IEnumerable<IParseTree> Children { get; }

            AstExpression NewExpression();
        }

        private class ArithmeticContextWrapper : IExpressionContextWrapper
        {
            private readonly Expression_arithmeticContext _context;

            public ArithmeticContextWrapper(Expression_arithmeticContext context)
            {
                _context = context;
            }

            public bool HasOpenParen => _context.PARENopen() is not null;

            public bool HasCloseParen => _context.PARENclose() is not null;

            public bool IsOperand => _context.arithmetic_operand() is not null;

            public IEnumerable<IParseTree> Children => _context.children;

            public AstExpression NewExpression() => new(_context);
        }

        private class LogicContextWrapper : IExpressionContextWrapper
        {
            private readonly Expression_logicContext _context;

            public LogicContextWrapper(Expression_logicContext context)
            {
                _context = context;
            }

            public bool HasOpenParen => _context.PARENopen() is not null;

            public bool HasCloseParen => _context.PARENclose() is not null;

            public bool IsOperand => _context.logic_operand() is not null;

            public IEnumerable<IParseTree> Children => _context.children;

            public AstExpression NewExpression() => new(_context);
        }

        private class ComparisonContextWrapper : IExpressionContextWrapper
        {
            private readonly Expression_comparisonContext _context;

            public ComparisonContextWrapper(Expression_comparisonContext context)
            {
                _context = context;
            }

            public bool HasOpenParen => _context.PARENopen() is not null;

            public bool HasCloseParen => _context.PARENclose() is not null;

            public bool IsOperand => _context.comparison_operand() is not null;

            public IEnumerable<IParseTree> Children => _context.children;

            public AstExpression NewExpression() => new(_context);
        }
    }
}
