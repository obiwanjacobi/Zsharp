using System.Linq;
using Zsharp.AST;

namespace Zsharp.EmitCS
{
    // hard-coded compiler intrinsic function implementations
    internal sealed partial class EmitIntrinsic : EmitExpression
    {
        private static ICodeProvider[] Providers =
        {

        };

        public EmitIntrinsic(EmitContext context)
            : base(context)
        { }

        public void EmitFunction(AstFunctionReference function, AstFunctionDefinitionIntrinsic functionDef)
        {
            VisitChildren(function);

            // conversion
            var target = (AstTypeDefinitionIntrinsic?)
                    functionDef.TypeReference!.TypeDefinition;
            var source = (AstTypeDefinitionIntrinsic?)
                functionDef.Parameters.First().TypeReference!.TypeDefinition;


            //EmitContext.CsBuilder;
        }
    }
}
