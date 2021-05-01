using System;
using Zsharp.AST;

namespace Zsharp.EmitCS
{
    public static class EmitExtensions
    {
        public static string ToCode(this AstType? astType)
        {
            if (astType != null)
            {
                if (astType is AstTypeReference typeRef &&
                    typeRef.TypeDefinition != null)
                {
                    astType = typeRef.TypeDefinition;
                }

                if (astType is AstTypeDefinitionIntrinsic typeDef)
                {
                    if (typeDef.SystemType != null)
                        return typeDef.SystemType.FullName;

                    return "void";
                }

                return astType.Identifier!.CanonicalName;
            }
            return String.Empty;
        }
    }
}
