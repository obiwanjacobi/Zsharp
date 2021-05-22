using System;
using System.Collections.Generic;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.EmitCS
{
    // hard-coded compiler intrinsic function implementations
    internal sealed partial class EmitIntrinsic : EmitExpression
    {
        private static readonly ConversionCodeProvider _conversion = new ConversionCodeProvider();

        private static Dictionary<string, CodeProvider> Providers = new()
        {
            { BuildKey(AstFunctionDefinitionIntrinsic.ConvertU8ToU16), _conversion },
            { BuildKey(AstFunctionDefinitionIntrinsic.ConvertU8ToU32), _conversion },
            { BuildKey(AstFunctionDefinitionIntrinsic.ConvertU8ToU64), _conversion },
            { BuildKey(AstFunctionDefinitionIntrinsic.ConvertU16ToU32), _conversion },
            { BuildKey(AstFunctionDefinitionIntrinsic.ConvertU16ToU64), _conversion },
            { BuildKey(AstFunctionDefinitionIntrinsic.ConvertU32ToU64), _conversion },
        };

        // name:ret(params)
        private static string BuildKey(AstFunctionDefinition functionDef)
        {
            var parameters = String.Join(",", functionDef.Parameters
                .Select(p => p.TypeReference!.Identifier!.CanonicalName));

            return $"{functionDef.Identifier!.CanonicalName}:{functionDef.TypeReference!.Identifier!.CanonicalName}({parameters})";
        }

        public EmitIntrinsic(CsBuilder builder)
            : base(builder)
        { }

        public void EmitFunction(AstFunctionReference function, AstFunctionDefinitionIntrinsic functionDef)
        {
            var key = BuildKey(functionDef);

            if (!Providers.TryGetValue(key, out CodeProvider? codeProvider))
                throw new InternalErrorException(
                    $"No intrinsic code provider for {functionDef.Identifier!.CanonicalName} ({key}).");

            codeProvider.Build(this.CsBuilder, functionDef);
            this.CsBuilder.Append("(");
            VisitChildren(function);
            this.CsBuilder.Append(")");
        }
    }
}
