using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeDefinitionEnum : AstTypeDefinition, IAstCodeBlockItem
    {
        public AstTypeDefinitionEnum(Enum_defContext context)
            : base(AstNodeType.Enum)
        {
            Context = context;
        }

        public int Indent { get; set; }

        public new IEnumerable<AstTypeDefinitionEnumOption> Fields
            => base.Fields.Cast<AstTypeDefinitionEnumOption>();

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionEnum(this);
    }
}
