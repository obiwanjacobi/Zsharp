using System;

namespace Zsharp.EmitCS.CSharp
{
    internal class Field
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

        public string InitExpression
        {
            get { return _valueBuilder?.ToString() ?? String.Empty; }
        }

        private CsBuilder? _valueBuilder;
        public CsBuilder ValueBuilder
        {
            get
            {
                if (_valueBuilder is null)
                    _valueBuilder = new CsBuilder();
                return _valueBuilder;
            }
        }
    }
}
