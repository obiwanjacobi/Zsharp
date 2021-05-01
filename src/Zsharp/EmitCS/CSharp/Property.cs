namespace Zsharp.EmitCS.CSharp
{
    internal class Property
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
}
