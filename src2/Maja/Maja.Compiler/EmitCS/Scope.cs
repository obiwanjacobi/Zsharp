using Maja.Compiler.EmitCS.CSharp;

namespace Maja.Compiler.EmitCS;

internal class Scope
{
    public Scope(Namespace ns)
        => Namespace = ns;

    public Scope(Type type)
        => Type = type;

    public Scope(Method method)
        => Method = method;

    public Namespace? Namespace { get; }
    public Type? Type { get; }
    public Method? Method { get; }
}
