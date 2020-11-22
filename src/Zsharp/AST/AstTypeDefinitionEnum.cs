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
    }
}
