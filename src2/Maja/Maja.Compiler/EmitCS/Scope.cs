using System;
using Maja.Compiler.EmitCS.CSharp;

namespace Maja.Compiler.EmitCS;

internal sealed class Scope
{
    public Scope(CSharp.Namespace ns)
        => Namespace = ns;

    public Scope(CSharp.Type type)
        => Type = type;

    public Scope(CSharp.Method method)
        => Method = method;

    public CSharp.Namespace? Namespace { get; }
    public CSharp.Type? Type { get; }
    public CSharp.Method? Method { get; }
}

internal sealed class EndOfScope(Action endAction) : IDisposable
{
    public void Dispose() => endAction();
}