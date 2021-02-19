using Antlr4.Runtime;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstVariableDefinition : AstVariable,
        IAstCodeBlockItem
    {
        public AstVariableDefinition(Variable_def_typedContext context)
        {
            Context = context;
        }

        public AstVariableDefinition(Variable_assign_valueContext context)
        {
            Context = context;
        }

        public AstVariableDefinition(AstTypeReference? typeReference)
            : base(typeReference)
        { }

        internal AstVariableDefinition(ParserRuleContext context)
        {
            Context = context;
        }

        public uint Indent { get; set; }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitVariableDefinition(this);
    }
}
