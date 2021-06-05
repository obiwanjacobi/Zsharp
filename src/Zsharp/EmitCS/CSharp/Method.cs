using System.Collections.Generic;

namespace Zsharp.EmitCS.CSharp
{
    internal class Method
    {
        public Method(string name, string typeName)
        {
            Name = name;
            TypeName = typeName;
        }

        private CsBuilder? _body;
        public CsBuilder GetBody(int indent)
        {
            if (_body is null)
                _body = new CsBuilder(indent);
            return _body;
        }

        public AccessModifiers AccessModifiers { get; set; }

        public MethodModifiers MethodModifiers { get; set; }

        public string Name { get; }

        public string TypeName { get; }

        private readonly List<Parameter> _parameters = new();
        public IEnumerable<Parameter> Parameters => _parameters;

        public void AddParameter(Parameter parameter)
        {
            _parameters.Add(parameter);
        }
    }

    internal class Parameter
    {
        public Parameter(string name, string typeName)
        {
            Name = name;
            TypeName = typeName;
        }

        public string Name { get; }
        public string TypeName { get; }
    }
}
