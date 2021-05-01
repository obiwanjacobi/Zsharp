using System;
using Zsharp.AST;

namespace Zsharp.EmitIL
{
    public enum IntrinsicType
    {
        I8, I16, I32, I64,
        U8, U16, U32, U64,
        F32, F64,
    }

    public static class IntrinsicTypeExtensions
    {
        private readonly static AstIdentifierIntrinsic[] _intrinsicIdentifiers =
        {
            AstIdentifierIntrinsic.I8,
            AstIdentifierIntrinsic.I16,
            AstIdentifierIntrinsic.I32,
            AstIdentifierIntrinsic.I64,

            AstIdentifierIntrinsic.U8,
            AstIdentifierIntrinsic.U16,
            AstIdentifierIntrinsic.U32,
            AstIdentifierIntrinsic.U64,

            AstIdentifierIntrinsic.F32,
            AstIdentifierIntrinsic.F64,
        };

        public static IntrinsicType ToIntrinsicType(this AstTypeDefinitionIntrinsic intrinsicType)
        {
            var identifier = intrinsicType.Identifier;

            for (int i = 0; i < _intrinsicIdentifiers.Length; i++)
            {
                if (Object.ReferenceEquals(_intrinsicIdentifiers[i], identifier))
                    return (IntrinsicType)i;
            }

            throw new InvalidOperationException(
                $"Intrinsic Type {intrinsicType.Identifier!.Name} not found.");
        }
    }
}
