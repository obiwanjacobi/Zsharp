using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using Zsharp.Parser;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstNumericBuilder : ZsharpBaseVisitor<UInt64>
    {
        private readonly AstNode? _parent;

        public AstNumericBuilder()
        { }

        public AstNumericBuilder(AstNode parent)
        {
            _parent = parent;
        }

        private AstNumeric? _numeric;
        public AstNumeric? Build(NumberContext numberCtx)
        {
            VisitNumber(numberCtx);
            return _numeric;
        }

        public AstNumeric? Test(ParserRuleContext context)
        {
            Visit(context);
            return _numeric;
        }

        protected override UInt64 AggregateResult(UInt64 aggregate, UInt64 nextResult)
        {
            if (nextResult == 0)
            {
                return aggregate;
            }
            return nextResult;
        }

        public override UInt64 VisitNumber([NotNull] NumberContext context)
        {
            _numeric = new AstNumeric(context);
            _numeric.TrySetParent(_parent);
            _numeric.Value = GetNumberValue(context);
            return _numeric.Value;
        }

        private static ulong GetNumberValue(NumberContext context)
        {
            string txt;
            if (context.NUMBERbin() != null)
            {
                txt = context.NUMBERbin().GetText();
                return ParseNumber(2, 2, txt);
            }

            if (context.NUMBERoct() != null)
            {
                txt = context.NUMBERoct().GetText();
                return ParseNumber(2, 8, txt);
            }

            if (context.NUMBERdec() != null)
            {
                txt = context.NUMBERdec().GetText();
                return ParseNumber(0, 10, txt);
            }

            if (context.NUMBERdec_prefix() != null)
            {
                txt = context.NUMBERdec_prefix().GetText();
                return ParseNumber(2, 10, txt);
            }

            if (context.NUMBERhex() != null)
            {
                txt = context.NUMBERhex().GetText();
                return ParseNumber(2, 16, txt);
            }

            if (context.CHARACTER() != null)
            {
                txt = context.CHARACTER().GetText();
                return (UInt64)txt[0];
            }

            return 0;
        }

        private static UInt64 ParseNumber(int offset, int radix, string text)
            => Convert.ToUInt64(text.Substring(offset).Replace("_", null), radix);
    }
}