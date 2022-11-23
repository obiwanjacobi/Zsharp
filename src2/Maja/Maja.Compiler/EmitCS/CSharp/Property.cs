namespace Maja.Compiler.EmitCS.CSharp;

internal sealed class Property
{
    public Property(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
    }

    public AccessModifiers AccessModifiers { get; set; }

    public string Name { get; }

    public string TypeName { get; }
}
