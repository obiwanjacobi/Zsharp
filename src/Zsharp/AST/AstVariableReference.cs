using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstVariableReference : AstVariable
    {
        public AstVariableReference(Variable_refContext context)
        {
            Context = context;
        }

        public AstVariableReference(Variable_assign_structContext context)
        {
            Context = context;
        }

        public AstVariableReference(Variable_assign_valueContext context)
        {
            Context = context;
        }

        public bool HasDefinition
            => VariableDefinition != null || ParameterDefinition != null;

        public AstVariableDefinition? VariableDefinition
            => Symbol?.DefinitionAs<AstVariableDefinition>();

        public AstFunctionParameter? ParameterDefinition
            => Symbol?.DefinitionAs<AstFunctionParameter>();

        public override void Accept(AstVisitor visitor)
            => visitor.VisitVariableReference(this);
    }
}