using System;
using static ZsharpParser;

namespace Zlang.NET.AST
{
    public enum AstNumericSign
    {
        NotSet,
        Unsigned,
        Signed,
    }

    public class AstNumeric : AstNode
    {
        public AstNumeric(NumberContext ctx)
            : base(AstNodeType.Numeric)
        {
            Context = ctx;
        }

        public NumberContext Context { get; }

        public UInt64 Value { get; set; }
        public AstNumericSign Sign { get; }

        public UInt64 AsUnsigned() => Value;

        // TODO: Numeric does not know about the negate-operator yet.
        public Int64 AsSigned() => (Int64)Value;

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitNumeric(this);
        }

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