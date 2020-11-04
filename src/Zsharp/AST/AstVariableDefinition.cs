using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstVariableDefinition : AstVariable
    {
        public AstVariableDefinition(Variable_def_typedContext context)
        {
            Context = context;
        }

        public AstVariableDefinition(Variable_def_typed_initContext context)
        {
            Context = context;
        }

        public AstVariableDefinition(AstTypeReference? typeReference)
            : base(typeReference)
        { }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitVariableDefinition(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            TypeReference?.Accept(visitor);
        }
    }
}