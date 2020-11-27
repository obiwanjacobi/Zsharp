using Antlr4.Runtime;
using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstLiteralNumeric : AstNode
    {
        public AstLiteralNumeric(UInt64 value)
            : base(AstNodeType.Literal)
        {
            Value = value;
        }

        private AstLiteralNumeric(NumberContext context)
            : base(AstNodeType.Literal)
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