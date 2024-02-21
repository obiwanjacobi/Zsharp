using System;
using System.Runtime.CompilerServices;
using Maja.Compiler;

namespace Maja;

public static class Operators
{
    // Unsigned Integer

    [Operator("+", OperatorProperty.Associative | OperatorProperty.Commutative | OperatorProperty.Distributive)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt16 ArithmeticAdd(Byte left, Byte right) => (UInt16)(left + right);

    [Operator("+", OperatorProperty.Associative | OperatorProperty.Commutative | OperatorProperty.Distributive)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt32 ArithmeticAdd(UInt16 left, UInt16 right) => (UInt32)left + (UInt32)right;

    [Operator("+", OperatorProperty.Associative | OperatorProperty.Commutative | OperatorProperty.Distributive)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt64 ArithmeticAdd(UInt32 left, UInt32 right) => (UInt64)left + (UInt64)right;

    // Signed Integer

    [Operator("+", OperatorProperty.Associative | OperatorProperty.Commutative | OperatorProperty.Distributive)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int16 ArithmeticAdd(SByte left, SByte right) => (Int16)(left + right);

    [Operator("+", OperatorProperty.Associative | OperatorProperty.Commutative | OperatorProperty.Distributive)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 ArithmeticAdd(Int16 left, Int16 right) => left + right;

    [Operator("+", OperatorProperty.Associative | OperatorProperty.Commutative | OperatorProperty.Distributive)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int64 ArithmeticAdd(Int32 left, Int32 right) => (Int64)left + (Int64)right;

    // Floating Point

    [Operator("+", OperatorProperty.Associative | OperatorProperty.Commutative | OperatorProperty.Distributive)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Single ArithmeticAdd(Half left, Half right) => (Single)left + (Single)right;

    [Operator("+", OperatorProperty.Associative | OperatorProperty.Commutative | OperatorProperty.Distributive)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Double ArithmeticAdd(Single left, Single right) => (Double)left + (Double)right;
}
