using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionParameter : AstNode, IAstIdentifierSite, IAstTypeReferenceSite
    {
        private readonly Function_parameterContext? _paramCtx;
        private readonly Function_parameter_selfContext? _selfCtx;

        public AstFunctionParameter()
            : base(AstNodeType.FunctionParameter)
        { }

        public AstFunctionParameter(Function_parameterContext context)
            : base(AstNodeType.FunctionParameter)
        {
            _paramCtx = context;
        }

        public AstFunctionParameter(Function_parameter_selfContext context)
            : base(AstNodeType.FunctionParameter)
        {
            _selfCtx = context;
        }

        public override void Accept(AstVisitor visitor) => visitor.VisitFunctionParameter(this);

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier identifier) => this.SafeSetParent(ref _identifier, identifier);

        public void SetIdentifier(AstIdentifier identifier)
        {
            if (!TrySetIdentifier(identifier))
                throw new InvalidOperationException(
                    "Identifier is already set or null.");
        }

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool TrySetTypeReference(AstTypeReference typeReference) => Ast.SafeSet(ref _typeRef, typeReference);

        public void SetTypeReference(AstTypeReference typeReference)
        {
            if (!TrySetTypeReference(typeReference))
                throw new InvalidOperationException(
                    "TypeReference is already set or null.");
        }
    }
}
