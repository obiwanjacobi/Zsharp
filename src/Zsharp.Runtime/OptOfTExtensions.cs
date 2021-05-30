using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Zsharp.Runtime.Compiler;

namespace Zsharp.Runtime
{
    public static class OptOfTExtensions
    {
        // conditional add
        public static void Add<T>(this ICollection<T> collection, Opt<T> option)
        {
            if (option.HasValue)
                collection.Add(option.Value);
        }

        // Wrap all TryXxxx methods

        [SymbolAlias("TryParseBool")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<bool> TryParseBoolean(this string text)
            => Boolean.TryParse(text, out bool val) ? new Opt<bool>(val) : Opt<bool>.Nothing;

        [SymbolAlias("TryParseU8")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<byte> TryParseByte(this string text)
            => Byte.TryParse(text, out byte val) ? new Opt<byte>(val) : Opt<byte>.Nothing;

        [SymbolAlias("TryParseI8")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<sbyte> TryParseSByte(this string text)
            => SByte.TryParse(text, out sbyte val) ? new Opt<sbyte>(val) : Opt<sbyte>.Nothing;

        [SymbolAlias("TryParseC16")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<char> TryParseChar(this string text)
            => Char.TryParse(text, out char val) ? new Opt<char>(val) : Opt<char>.Nothing;

        [SymbolAlias("TryParseU16")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<ushort> TryParseUInt16(this string text)
            => UInt16.TryParse(text, out ushort val) ? new Opt<ushort>(val) : Opt<ushort>.Nothing;

        [SymbolAlias("TryParseI16")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<short> TryParseInt16(this string text)
            => Int16.TryParse(text, out short val) ? new Opt<short>(val) : Opt<short>.Nothing;

        [SymbolAlias("TryParseU32")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<uint> TryParseUInt32(this string text)
            => UInt32.TryParse(text, out uint val) ? new Opt<uint>(val) : Opt<uint>.Nothing;

        [SymbolAlias("TryParseI32")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<int> TryParseInt32(this string text)
            => Int32.TryParse(text, out int val) ? new Opt<int>(val) : Opt<int>.Nothing;

        [SymbolAlias("TryParseU64")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<ulong> TryParseUInt64(this string text)
            => UInt64.TryParse(text, out ulong val) ? new Opt<ulong>(val) : Opt<ulong>.Nothing;

        [SymbolAlias("TryParseI64")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<long> TryParseInt64(this string text)
            => Int64.TryParse(text, out long val) ? new Opt<long>(val) : Opt<long>.Nothing;

        [SymbolAlias("TryParseF32")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<float> TryParseSingle(this string text)
            => Single.TryParse(text, out float val) ? new Opt<float>(val) : Opt<float>.Nothing;

        [SymbolAlias("TryParseF64")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<double> TryParseDouble(this string text)
            => Double.TryParse(text, out double val) ? new Opt<double>(val) : Opt<double>.Nothing;

        [SymbolAlias("TryParseF96")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<decimal> TryParseDecimal(this string text)
            => Decimal.TryParse(text, out decimal val) ? new Opt<decimal>(val) : Opt<decimal>.Nothing;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<DateTime> TryParseDateTime(this string text)
            => DateTime.TryParse(text, out DateTime val) ? new Opt<DateTime>(val) : Opt<DateTime>.Nothing;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<DateTimeOffset> TryParseDateTimeOffset(this string text)
            => DateTimeOffset.TryParse(text, out DateTimeOffset val) ? new Opt<DateTimeOffset>(val) : Opt<DateTimeOffset>.Nothing;

        // Wrap all XxxOrDefault methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<T> FirstOrNothing<T>(this IEnumerable<T> source)
            => Enumerable.Any(source) ? new Opt<T>(Enumerable.First(source)) : Opt<T>.Nothing;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<T> FirstOrNothing<T>(this IEnumerable<T> source, Func<T, bool> predicate)
            => Enumerable.Any(source) ? new Opt<T>(Enumerable.First(source, predicate)) : Opt<T>.Nothing;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<T> SingleOrNothing<T>(this IEnumerable<T> source)
            => Enumerable.Any(source) ? new Opt<T>(Enumerable.Single(source)) : Opt<T>.Nothing;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<T> SingleOrNothing<T>(this IEnumerable<T> source, Func<T, bool> predicate)
            => Enumerable.Any(source) ? new Opt<T>(Enumerable.Single(source, predicate)) : Opt<T>.Nothing;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<T> LastOrNothing<T>(this IEnumerable<T> source)
            => Enumerable.Any(source) ? new Opt<T>(Enumerable.Last(source)) : Opt<T>.Nothing;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<T> LastOrNothing<T>(this IEnumerable<T> source, Func<T, bool> predicate)
            => Enumerable.Any(source) ? new Opt<T>(Enumerable.Last(source, predicate)) : Opt<T>.Nothing;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Opt<T> ElementAtOrNothing<T>(this IEnumerable<T> source, int index)
            => Enumerable.Count(source) > index ? new Opt<T>(Enumerable.ElementAt(source, index)) : Opt<T>.Nothing;
    }
}
