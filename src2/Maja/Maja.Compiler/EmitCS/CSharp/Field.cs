namespace Maja.Compiler.EmitCS.CSharp;

internal sealed class Field
{
    public Field(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
    }

    public AccessModifiers AccessModifiers { get; set; }

    public FieldModifiers FieldModifiers { get; set; }

    public string Name { get; }

    public string TypeName { get; }

    public string? InitialValue { get; set; }
}
