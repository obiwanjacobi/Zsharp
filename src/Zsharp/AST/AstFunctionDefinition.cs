using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public abstract class AstFunctionDefinition : AstFunction
    {
        private readonly List<AstFunctionParameter> _parameters = new List<AstFunctionParameter>();

        public IEnumerable<AstFunctionParameter> Parameters => _parameters;

        public bool TryAddParameter(AstFunctionParameter param)
        {
            if (param != null &&
                param.TrySetParent(this))
            {
                _parameters.Add(param);
                return true;
            }
            return false;
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var param in _parameters)
            {
                param.Accept(visitor);
            }
        }

        public override string? ToString()
        {
            if (TypeReference?.Identifier != null && Identifier != null)
                return $"{TypeReference.Identifier.Name} {Identifier.Name}({String.Join(", ", _parameters.Select(p => $"{p.TypeReference.Identifier.Name} {p.Identifier.Name}"))})";

            return base.ToString();
        }
    }
}
