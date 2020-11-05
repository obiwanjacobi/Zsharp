using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionReference : AstFunction
    {
        private readonly List<AstFunctionParameterReference> _parameters = new List<AstFunctionParameterReference>();

        public AstFunctionReference(Function_callContext context)
        {
            Context = context;
        }

        public Function_callContext Context { get; }

        public AstFunctionDefinition? FunctionDefinition => Symbol?.DefinitionAs<AstFunctionDefinition>();

        public IEnumerable<AstFunctionParameterReference> Parameters => _parameters;

        public void AddParameter(AstFunctionParameterReference parameter)
        {
            _parameters.Add(parameter);
        }

        public override void Accept(AstVisitor visitor) => visitor.VisitFunctionReference(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var param in _parameters)
            {
                param.Accept(visitor);
            }
        }
    }
}
