using System;
using System.Linq;

namespace Zsharp.AST
{
    public abstract class AstFunctionDefinition : AstFunction<AstFunctionParameterDefinition>
    {
        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var param in Parameters)
            {
                param.Accept(visitor);
            }
        }

        public override string? ToString()
        {
            if (TypeReference?.Identifier != null && Identifier != null)
                return $"{TypeReference.Identifier.Name} {Identifier.Name}({String.Join(", ", Parameters.Select(p => $"{p.TypeReference.Identifier.Name} {p.Identifier.Name}"))})";

            return base.ToString();
        }
    }
}
