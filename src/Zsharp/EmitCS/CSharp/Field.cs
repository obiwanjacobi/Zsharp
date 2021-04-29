namespace Zsharp.EmitCS.CSharp
{
    internal class Field
    {
        public AccessModifiers AccessModifiers { get; set; }

        public FieldModifiers FieldModifiers { get; set; }

        public string Name { get; set; }

        public string TypeName { get; set; }
    }
}
