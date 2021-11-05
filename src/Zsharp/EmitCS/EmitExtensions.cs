using System;
using Zsharp.AST;

namespace Zsharp.EmitCS
{
    public static class EmitExtensions
    {
        public static string ToCode(this AstType? astType)
        {
            if (astType is not null)
            {
                if (astType is AstTypeReference typeRef &&
                    typeRef.TypeDefinition is not null)
                {
                    astType = typeRef.TypeDefinition;
                }

                if (astType is AstTypeDefinitionIntrinsic typeDef)
                {
                    if (typeDef.SystemType is not null)
                        return typeDef.SystemType.FullName!;

                    return "void";
                }

                return astType.Identifier!.SymbolName.CanonicalName.FullName;
            }
            return String.Empty;
        }
    }
}
