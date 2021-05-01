using System.Collections.Generic;

namespace Zsharp.EmitCS.CSharp
{
    internal class Method
    {
        public Method(string name)
        {
            Name = name;
        }

        private CsBuilder? _body;
        public CsBuilder GetBody(int indent)
        {
            if (_body == null)
                _body = new CsBuilder(indent);
            return _body;
        }

        public AccessModifiers AccessModifiers { get; set; }

        public MethodModifiers MethodModifiers { get; set; }

        public string Name { get; set; }

        public string TypeName { get; set; }

        private readonly List<Parameter> _parameters = new();
        public IEnumerable<Parameter> Parameters => _parameters;

        public void AddParameter(Parameter parameter)
        {
            _parameters.Add(parameter);
        }
    }

    internal class Parameter
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
    }
}
