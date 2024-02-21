using System;

namespace Maja.Compiler;

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
    None,
    Checked,
    Overflow,
    Saturated,
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class OperatorAttribute : Attribute
{
    public OperatorAttribute(string symbol, OperatorProperty operatorProperty = OperatorProperty.None, OverflowHandling operatorOverflow = OverflowHandling.None)
    {
        Symbol = symbol;
        Property = operatorProperty;
        Overflow = operatorOverflow;
    }
    
    public string Symbol { get; }
    public OperatorProperty Property { get; }
    public OverflowHandling Overflow { get; }
}
