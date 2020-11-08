using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionReference : AstFunction<AstFunctionParameterReference>
    {
        public AstFunctionReference(Function_callContext context)
        {
            Context = context;
        }

        public AstFunctionDefinition? FunctionDefinition
        {
            get
            {
                Ast.Guard(Symbol, $"No Symbol is set for Function reference {Identifier.Name}.");
                var entry = Symbol!;

                if (entry.HasOverloads)
                {
                    return entry.FindOverloadDefinition(this);
                }

                return entry.DefinitionAs<AstFunctionDefinition>();
            }
        }

        public override void Accept(AstVisitor visitor) => visitor.VisitFunctionReference(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var param in Parameters)
            {
                param.Accept(visitor);
            }
        }
    }
}
