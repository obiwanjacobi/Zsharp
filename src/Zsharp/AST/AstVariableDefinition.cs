using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstVariableDefinition : AstVariable,
        IAstCodeBlockLine
    {
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
