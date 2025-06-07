using System;

namespace Maja.Compiler;

/// <summary>
/// Binary operator laws
/// </summary>
[Flags]
public enum OperatorProperty
{
    None = 0,
    /// <summary>
    /// https://en.wikipedia.org/wiki/Associative_property
    /// </summary>
    Associative = 1,
    /// <summary>
    /// https://en.wikipedia.org/wiki/Commutative_property
    /// </summary>
    Commutative = 2,
    /// <summary>
    /// https://en.wikipedia.org/wiki/Distributive_property
    /// </summary>
    Distributive = 4,
}

public enum OverflowHandling
{
    /// <summary>Not set, undefined</summary>
    None,
    /// <summary>High bits are ignored</summary>
    WrapAround,
    /// <summary>Max value when overflow, Min value when underflow.</summary>
    Saturate,
    /// <summary>OverflowException (dotnet checked)</summary>
    Exception,
    /// <summary>ErrOfT when over/underflow</summary>
    Error,
    /// <summary>OptOfT(Nothing) when over/underflow</summary>
    Nothing
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class UnaryOperatorAttribute : Attribute
{
    public UnaryOperatorAttribute(string symbol, OverflowHandling operatorOverflow = OverflowHandling.None)
    {
        Symbol = symbol;
        Overflow = operatorOverflow;
    }

    public string Symbol { get; }
    public OverflowHandling Overflow { get; }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class BinaryOperatorAttribute : Attribute
{
    public BinaryOperatorAttribute(string symbol, OperatorProperty operatorProperty = OperatorProperty.None, OverflowHandling operatorOverflow = OverflowHandling.None)
    {
        Symbol = symbol;
        Property = operatorProperty;
        Overflow = operatorOverflow;
    }

    public string Symbol { get; }
    public OperatorProperty Property { get; }
    public OverflowHandling Overflow { get; }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class TernaryOperatorAttribute : Attribute
{
    public TernaryOperatorAttribute(string symbolLeft, string symbolRight, OverflowHandling operatorOverflow = OverflowHandling.None)
    {
        SymbolLeft = symbolLeft;
        SymbolRight = symbolRight;
        Overflow = operatorOverflow;
    }

    public string SymbolLeft { get; }
    public string SymbolRight { get; }
    public OverflowHandling Overflow { get; }
}
