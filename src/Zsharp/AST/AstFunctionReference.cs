using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionReference : AstFunction
    {
        private readonly List<AstFunctionParameterReference> _params = new List<AstFunctionParameterReference>();

        public AstFunctionReference(Function_callContext context)
        {
            Context = context;
        }

        public Function_callContext Context { get; }

        public AstFunctionDefinition? FunctionDefinition => Symbol?.DefinitionAs<AstFunctionDefinition>();

        public override void Accept(AstVisitor visitor) => visitor.VisitFunctionReference(this);

        public IEnumerable<AstFunctionParameterReference> Parameters => _params;

        public void AddParameter(AstFunctionParameterReference parameter)
        {
            _params.Add(parameter);
        }
    }
}
