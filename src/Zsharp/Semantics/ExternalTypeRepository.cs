using Mono.Cecil;
using System.Collections.Generic;
using Zsharp.AST;

namespace Zsharp.Semantics
{
    public class ExternalTypeRepository
    {
        private readonly Dictionary<string, AstTypeReferenceExternal> _typeReferences = new Dictionary<string, AstTypeReferenceExternal>();

        public AstTypeReference GetTypeReference(TypeReference typeReference)
        {
            var key = typeReference.FullName;
            if (!_typeReferences.ContainsKey(key))
            {
                var typeRef = new AstTypeReferenceExternal(typeReference);
                typeRef.SetIdentifier(new AstIdentifierExternal(typeReference.Name, AstIdentifierType.Type));
                _typeReferences.Add(key, typeRef);
            }

            return new AstTypeReferenceExternal(_typeReferences[key]);
        }
    }
}
