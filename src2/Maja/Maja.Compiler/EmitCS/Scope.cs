using Maja.Compiler.EmitCS.CSharp;

namespace Maja.Compiler.EmitCS;

internal class Scope
{
    public Scope(Namespace ns)
        => Namespace = ns;

    public Namespace? Namespace { get; }

    public Scope(Type type)
        => Type = type;

    public Type? Type { get; }

    public Scope(Method method)
        => Method = method;

    public Method? Method { get; }
}
