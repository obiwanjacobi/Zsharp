using Zsharp.Parser;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeConversionBuilder : ZsharpBaseVisitor<AstNode>
    {
        private readonly AstBuilderContext _buildercontext;

        public AstTypeConversionBuilder(AstBuilderContext buildercontext)
        {
            _buildercontext = buildercontext;
        }

        public override AstNode VisitType_conv_U16(Type_conv_U16Context context)
        {
            var function = new AstFunctionReference(context);
            function.SetIdentifier(AstIdentifierIntrinsic.U16);
            return function;
        }

        public override AstNode VisitType_conv_U32(Type_conv_U32Context context)
        {
            var function = new AstFunctionReference(context);
            function.SetIdentifier(AstIdentifierIntrinsic.U32);
            return function;
        }

        public override AstNode VisitType_conv_U64(Type_conv_U64Context context)
        {
            var function = new AstFunctionReference(context);
            function.SetIdentifier(AstIdentifierIntrinsic.U64);
            return function;
        }

        public override AstNode VisitType_conv_I8(Type_conv_I8Context context)
        {
            var function = new AstFunctionReference(context);
            function.SetIdentifier(AstIdentifierIntrinsic.I8);
            return function;
        }

        public override AstNode VisitType_conv_I16(Type_conv_I16Context context)
        {
            var function = new AstFunctionReference(context);
            function.SetIdentifier(AstIdentifierIntrinsic.I16);
            return function;
        }

        public override AstNode VisitType_conv_I32(Type_conv_I32Context context)
        {
            var function = new AstFunctionReference(context);
            function.SetIdentifier(AstIdentifierIntrinsic.I32);
            return function;
        }

        public override AstNode VisitType_conv_I64(Type_conv_I64Context context)
        {
            var function = new AstFunctionReference(context);
            function.SetIdentifier(AstIdentifierIntrinsic.I64);
            return function;
        }
    }
}
