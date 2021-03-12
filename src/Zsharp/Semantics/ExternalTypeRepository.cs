﻿using Mono.Cecil;
using System.Collections.Generic;
using Zsharp.AST;

namespace Zsharp.Semantics
{
    public class ExternalTypeRepository
    {
        private readonly Dictionary<string, AstTypeReferenceExternal> _typeReferences = new();

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

            return _typeReferences[key].MakeProxy();
        }

        private static string ToZsharpName(string nativeName)
        {
            return nativeName switch
            {
                "Byte" => AstIdentifierIntrinsic.U8.CanonicalName,
                "UInt16" => AstIdentifierIntrinsic.U16.CanonicalName,
                "UInt32" => AstIdentifierIntrinsic.U32.CanonicalName,
                "UInt64" => AstIdentifierIntrinsic.U64.CanonicalName,
                "SByte" => AstIdentifierIntrinsic.I8.CanonicalName,
                "Int16" => AstIdentifierIntrinsic.I16.CanonicalName,
                "Int32" => AstIdentifierIntrinsic.I32.CanonicalName,
                "Int64" => AstIdentifierIntrinsic.I64.CanonicalName,
                "Single" => AstIdentifierIntrinsic.F32.CanonicalName,
                "Double" => AstIdentifierIntrinsic.F64.CanonicalName,
                "String" => AstIdentifierIntrinsic.Str.CanonicalName,
                "Boolean" => AstIdentifierIntrinsic.Bool.CanonicalName,
                "Void" => AstIdentifierIntrinsic.Void.CanonicalName,
                _ => nativeName,
            };
        }
    }
}
