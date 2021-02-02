using System.Linq;
using Zsharp.AST;

namespace Zsharp.Emit
{
    // hard-coded compiler intrinsic function implementations
    internal sealed class EmitIntrinsic : EmitExpression
    {
        public EmitIntrinsic(EmitContext context)
            : base(context)
        { }

        public void EmitFunction(AstFunctionReference function, AstFunctionDefinitionIntrinsic functionDef)
        {
            VisitChildren(function);

            // conversion
            var target = ((AstTypeDefinitionIntrinsic)
                    functionDef.TypeReference.TypeDefinition).ToIntrinsicType();
            var source = ((AstTypeDefinitionIntrinsic)
                functionDef.Parameters.First().TypeReference.TypeDefinition).ToIntrinsicType();

            EmitContext.CodeBuilder.CodeBlock.Add(
                EmitContext.InstructionFactory.Convert(target, source));
        }
    }
}
