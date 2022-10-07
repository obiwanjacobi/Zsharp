using Antlr4.Runtime;

namespace Maja.Dgml
{
    public class ParserDgmlBuilder //: MajaParserBaseVisitor<Node>
    {
        private readonly DgmlBuilder _builder = new("ParserRuleContext");

        public ParserDgmlBuilder()
        {
            _builder.CreateCommon();
        }

        public static void Save(ParserRuleContext context, string filePath = "parser.dgml")
        {
            var builder = new ParserDgmlBuilder();
            builder.WriteContext(context);
            builder._builder.SaveAs(filePath);
        }

        public Node WriteContext(ParserRuleContext context)
        {
            var typeName = context.GetType().Name;
            var node = _builder.CreateNode(typeName, context.GetText(), typeName);

            foreach (var child in context.children)
            {
                if (child is ParserRuleContext childCtx)
                { 
                    var childNode = WriteContext(childCtx);
                    var link = _builder.CreateLink(node.Id, childNode.Id);
                }
            }
            return node;
        }
    }
}
