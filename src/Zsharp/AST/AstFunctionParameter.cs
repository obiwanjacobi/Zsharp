using static ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionParameter : AstNode, IAstIdentifierSite, IAstTypeReferenceSite
    {
        private readonly Function_parameterContext? _paramCtx;
        private readonly Function_parameter_selfContext? _selfCtx;

        public AstFunctionParameter()
            : base(AstNodeType.FunctionParameter)
        { }

        public AstFunctionParameter(Function_parameterContext ctx)
            : base(AstNodeType.FunctionParameter)
        {
            _paramCtx = ctx;
        }

        public AstFunctionParameter(Function_parameter_selfContext ctx)
            : base(AstNodeType.FunctionParameter)
        {
            _selfCtx = ctx;
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitFunctionParameter(this);
        }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool SetIdentifier(AstIdentifier identifier)
        {
            return this.SafeSetParent(ref _identifier, identifier);
        }

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool SetTypeReference(AstTypeReference typeRef)
        {
            return Ast.SafeSet(ref _typeRef, typeRef);
        }
    }
}
