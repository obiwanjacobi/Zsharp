using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionReference : AstFunction
    {
        public AstFunctionReference(Function_callContext context)
        {
            Context = context;
        }

        public Function_callContext Context { get; }

        public AstFunctionDefinition? FunctionDefinition => Symbol?.DefinitionAs<AstFunctionDefinition>();

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitFunctionReference(this);
        }
    }
}
