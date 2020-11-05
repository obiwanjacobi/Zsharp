using Antlr4.Runtime;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionParameterDefinition : AstFunctionParameter
    {
        private readonly Function_parameterContext? _paramCtx;
        private readonly Function_parameter_selfContext? _selfCtx;

        public AstFunctionParameterDefinition(Function_parameterContext context)
        {
            _paramCtx = context;
        }

        public AstFunctionParameterDefinition(Function_parameter_selfContext context)
        {
            _selfCtx = context;
        }

        public AstFunctionParameterDefinition(AstIdentifier identifier)
        {
            SetIdentifier(identifier);
        }

        public ParserRuleContext? Context => (ParserRuleContext?)_paramCtx ?? (ParserRuleContext?)_selfCtx;

        public override void Accept(AstVisitor visitor) => visitor.VisitFunctionParameterDefinition(this);
    }
}
