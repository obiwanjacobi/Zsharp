using Antlr4.Runtime;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionParameterDefinition : AstFunctionParameter
    {
        public AstFunctionParameterDefinition(Function_parameterContext context)
        {
            Context = context;
            IsSelf = false;
        }

        public AstFunctionParameterDefinition(Function_parameter_selfContext context)
        {
            Context = context;
            IsSelf = true;
        }

        public AstFunctionParameterDefinition(AstIdentifier identifier)
        {
            this.SetIdentifier(identifier);
            IsSelf = identifier.IsEqual(AstIdentifierIntrinsic.Self);
        }

        public ParserRuleContext? Context { get; }

        public bool IsSelf { get; }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitFunctionParameterDefinition(this);

        public static AstFunctionParameterDefinition Create(string name, AstTypeDefinition astType)
        {
            var identifier = new AstIdentifier(name, AstIdentifierKind.Parameter);
            var param = new AstFunctionParameterDefinition(identifier);
            param.SetTypeReference(AstTypeReferenceType.From(astType));
            return param;
        }
    }
}
