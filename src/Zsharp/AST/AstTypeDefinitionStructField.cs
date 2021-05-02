using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeDefinitionStructField : AstTypeFieldDefinition,
        IAstTypeReferenceSite, IAstSymbolEntrySite
    {
        public AstTypeDefinitionStructField()
        { }

        public AstTypeDefinitionStructField(Struct_field_defContext context)
        {
            Context = context;
        }

        private AstTypeReference? _typeReference;
        public AstTypeReference? TypeReference => _typeReference;

        public void SetTypeReference(AstTypeReference typeReference)
        {
            if (!TrySetTypeReference(typeReference))
                throw new InvalidOperationException("Type Reference already set or null.");
        }

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        private AstSymbolEntry? _symbol;

        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public void SetSymbol(AstSymbolEntry symbolEntry)
        {
            if (!TrySetSymbol(symbolEntry))
                throw new InvalidOperationException("Symbol was already set or null.");
        }

        public bool TryResolve()
        {
            return _symbol?.Definition == this;
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionStructField(this);

        public override void VisitChildren(AstVisitor visitor)
            => TypeReference?.Accept(visitor);
    }
}
