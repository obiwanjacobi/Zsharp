using Antlr4.Runtime;
using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstLiteralNumeric : AstNode
    {
        public AstLiteralNumeric(UInt64 value)
            : base(AstNodeKind.Literal)
        {
            Value = value;
        }

        private AstLiteralNumeric(ParserRuleContext context)
            : base(AstNodeKind.Literal)
        {
            Context = context;
        }

        public ParserRuleContext? Context { get; }

        public UInt64 Value { get; private set; }

        public AstNumericSign Sign { get; }

        public override void Accept(AstVisitor visitor) => visitor.VisitLiteralNumeric(this);

        public UInt32 GetBitCount()
        {
            UInt64 n = Value;
            UInt32 count = 0;
            while (n > 0)
            {
                count++;
                n >>= 1;
            }
            return count;
        }

        public static AstLiteralNumeric Create(NumberContext context)
        {
            return new AstLiteralNumeric(context)
            {
                Value = GetNumberValue(context)
            };
        }

        private static ulong GetNumberValue(NumberContext context)
        {
            string txt;

            var bin = context.NUMBERbin();
            if (bin is not null)
            {
                txt = bin.GetText();
                return ParseNumber(2, 2, txt);
            }

            var oct = context.NUMBERoct();
            if (oct is not null)
            {
                txt = oct.GetText();
                return ParseNumber(2, 8, txt);
            }

            var dec = context.NUMBERdec();
            if (dec is not null)
            {
                txt = dec.GetText();
                return ParseNumber(0, 10, txt);
            }

            var decPre = context.NUMBERdec_prefix();
            if (decPre is not null)
            {
                txt = decPre.GetText();
                return ParseNumber(2, 10, txt);
            }

            var hex = context.NUMBERhex();
            if (hex is not null)
            {
                txt = hex.GetText();
                return ParseNumber(2, 16, txt);
            }

            var ch = context.CHARACTER();
            if (ch is not null)
            {
                txt = ch.GetText();
                return (UInt64)txt[0];
            }

            return 0;
        }

        private static UInt64 ParseNumber(int offset, int radix, string text)
            => Convert.ToUInt64(text.Substring(offset).Replace("_", null), radix);
    }
}