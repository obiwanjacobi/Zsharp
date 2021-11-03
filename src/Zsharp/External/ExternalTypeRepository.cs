using System.Collections.Generic;
using Zsharp.AST;
using Zsharp.External.Metadata;

namespace Zsharp.External
{
    public class ExternalTypeRepository
    {
        private readonly Dictionary<string, AstTypeReference> _typeReferences = new();

        public AstTypeReference GetTypeReference(TypeMetadata typeReference)
        {
            var key = typeReference.FullName;
            if (!_typeReferences.ContainsKey(key))
            {
                if (TryZsharpName(typeReference, out var name))
                {
                    if (!_zsharpTypeMap.TryGetValue(typeReference.Name, out var identifier))
                        identifier = new AstIdentifier(name!, AstIdentifierKind.Type);

                    var typeRef = new AstTypeReferenceType(identifier);
                    _typeReferences.Add(key, typeRef);
                }
                else
                {
                    name = AstName.FromExternal(typeReference.Namespace, typeReference.Name);
                    
                    var typeRef = new AstTypeReferenceExternal(typeReference);
                    typeRef.SetIdentifier(new AstIdentifier(name, AstIdentifierKind.Type));
                    _typeReferences.Add(key, typeRef);
                }
            }

            return _typeReferences[key].MakeCopy();
        }

        private static bool TryZsharpName(TypeMetadata typeReference, out AstName? zsharpName)
        {
            if (typeReference.IsArray)
            {
                if (typeReference.ContainsGenericParameter)
                { 
                    zsharpName = AstName.ParseFullName("Array%1");
                }
                else
                { 
                    var elementType = typeReference.GetElementType();
                    zsharpName = AstName.ParseFullName($"Array;{ToZsharpScalarType(elementType.Name)}");
                }
                return true;
            }

            if (_zsharpTypeMap.TryGetValue(typeReference.Name, out var identifier))
            {
                zsharpName = AstName.ParseFullName(identifier.CanonicalFullName);
                return true;
            }

            zsharpName = null;
            return false;
        }

        private static string ToZsharpScalarType(string nativeName)
        {
            if (_zsharpTypeMap.TryGetValue(nativeName, out var identifier))
                return identifier.CanonicalFullName;
            return nativeName;
        }

        private static Dictionary<string, AstIdentifier> _zsharpTypeMap = new()
        {
            { "Byte", AstIdentifierIntrinsic.U8 },
            { "UInt16", AstIdentifierIntrinsic.U16 },
            { "UInt32", AstIdentifierIntrinsic.U32 },
            { "UInt64", AstIdentifierIntrinsic.U64 },
            { "SByte", AstIdentifierIntrinsic.I8 },
            { "Int16", AstIdentifierIntrinsic.I16 },
            { "Int32", AstIdentifierIntrinsic.I32 },
            { "Int64", AstIdentifierIntrinsic.I64 },
            { "Half", AstIdentifierIntrinsic.F16 },
            { "Single", AstIdentifierIntrinsic.F32 },
            { "Double", AstIdentifierIntrinsic.F64 },
            { "String", AstIdentifierIntrinsic.F96 },
            { "Boolean", AstIdentifierIntrinsic.Bool },
            { "Void", AstIdentifierIntrinsic.Void }
        };
    }
}
