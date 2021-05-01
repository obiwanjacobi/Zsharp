using System;

namespace Zsharp.EmitCS.CSharp
{
    internal class Field
    {
        public AccessModifiers AccessModifiers { get; set; }

        public FieldModifiers FieldModifiers { get; set; }

        public string Name { get; set; }

        public string TypeName { get; set; }

        public string InitExpression
        {
            get { return _valueBuilder?.ToString() ?? String.Empty; }
        }

        private CsBuilder? _valueBuilder;
        public CsBuilder ValueBuilder
        {
            get
            {
                if (_valueBuilder == null)
                    _valueBuilder = new CsBuilder();
                return _valueBuilder;
            }
        }
    }
}
