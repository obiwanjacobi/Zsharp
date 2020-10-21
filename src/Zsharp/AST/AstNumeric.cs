using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstNumeric : AstNode
    {
        public AstNumeric(NumberContext context)
            : base(AstNodeType.Numeric)
        {
            Context = context;
        }

        public NumberContext Context { get; }

        public UInt64 Value { get; set; }

        public AstNumericSign Sign { get; }

        public UInt64 AsUnsigned() => Value;

        // TODO: Numeric does not know about the negate-operator yet.
        public Int64 AsSigned() => (Int64)Value;

        public override void Accept(AstVisitor visitor) => visitor.VisitNumeric(this);

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
    }
}