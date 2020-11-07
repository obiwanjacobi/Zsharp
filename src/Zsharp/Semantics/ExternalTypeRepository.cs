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
                typeRef.SetIdentifier(new AstIdentifierExternal(
                    ToZsharpName(typeReference.Name), typeReference.Name, AstIdentifierType.Type));
                _typeReferences.Add(key, typeRef);
            }

            return new AstTypeReferenceExternal(_typeReferences[key]);
        }

        private static string ToZsharpName(string nativeName)
        {
            return nativeName switch
            {
                "Byte" => AstIdentifierIntrinsic.U8.Name,
                "UInt16" => AstIdentifierIntrinsic.U16.Name,
                "UInt32" => AstIdentifierIntrinsic.U32.Name,
                "UInt64" => AstIdentifierIntrinsic.U64.Name,
                "SByte" => AstIdentifierIntrinsic.I8.Name,
                "Int16" => AstIdentifierIntrinsic.I16.Name,
                "Int32" => AstIdentifierIntrinsic.I32.Name,
                "Int64" => AstIdentifierIntrinsic.I64.Name,
                "Single" => AstIdentifierIntrinsic.F32.Name,
                "Double" => AstIdentifierIntrinsic.F64.Name,
                "String" => AstIdentifierIntrinsic.Str.Name,
                "Boolean" => AstIdentifierIntrinsic.Bool.Name,
                _ => nativeName,
            };
        }
    }
}
