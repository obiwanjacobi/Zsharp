using System;
using System.Runtime.CompilerServices;

namespace Zsharp.Runtime
{
    public static class Conversion
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 U16(Byte self) => self;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 U32(Byte self) => self;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 U32(UInt16 self) => self;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 U64(Byte self) => self;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 U64(UInt16 self) => self;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 U64(UInt32 self) => self;

        public static class Checked
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Byte U8(UInt16 self) => checked((Byte)self);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Byte U8(UInt32 self) => checked((Byte)self);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Byte U8(UInt64 self) => checked((Byte)self);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static UInt16 U16(UInt32 self) => checked((UInt16)self);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static UInt16 U16(UInt64 self) => checked((UInt16)self);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static UInt32 U32(UInt64 self) => checked((UInt32)self);
        }
    }
}
