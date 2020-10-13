using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using static ZsharpParser;

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

        public AstNumeric? Test(ParserRuleContext ctx)
        {
            Visit(ctx);
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
            _numeric.SetParent(_parent);
            _numeric.Value = VisitChildren(context);
            ;
            return _numeric.Value;
        }

        public override UInt64 VisitNumber_bin([NotNull] Number_binContext context)
        {
            var txt = context.NUMBERbin().GetText();
            return ParseNumber(2, 2, txt);
        }

        public override UInt64 VisitNumber_oct([NotNull] Number_octContext context)
        {
            var txt = context.NUMBERoct().GetText();
            return ParseNumber(2, 8, txt);
        }

        public override UInt64 VisitNumber_dec([NotNull] Number_decContext context)
        {
            var dec = context.NUMBERdec();
            if (dec != null)
            {
                var txt = dec.GetText();
                return ParseNumber(0, 10, txt);
            }
            var dec_pre = context.NUMBERdec_prefix();
            if (dec_pre != null)
            {
                var txt = dec_pre.GetText();
                return ParseNumber(2, 10, txt);
            }
            return 0;
        }

        public override UInt64 VisitNumber_hex([NotNull] Number_hexContext context)
        {
            var txt = context.NUMBERhex().GetText();
            return ParseNumber(2, 16, txt);
        }

        public override UInt64 VisitNumber_char([NotNull] Number_charContext context)
        {
            var txt = context.character().CHARACTER().GetText();
            return (UInt64)txt[0];
        }

        private static UInt64 ParseNumber(int offset, int radix, string text)
        {
            return Convert.ToUInt64(text.Substring(offset).Replace("_", null), radix);
        }
    }
}