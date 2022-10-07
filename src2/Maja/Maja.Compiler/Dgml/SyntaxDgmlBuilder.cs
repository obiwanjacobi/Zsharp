using Maja.Compiler.Syntax;

namespace Maja.Dgml
{
    public class SyntaxDgmlBuilder
    {
        private readonly DgmlBuilder _builder = new("SyntaxNode");

        public SyntaxDgmlBuilder()
        {
            _builder.CreateCommon();
        }

        public static void Save(SyntaxNode syntaxNode, string filePath = "parser.dgml")
        {
            var builder = new SyntaxDgmlBuilder();
            builder.WriteNode(syntaxNode);
            builder._builder.SaveAs(filePath);
        }

        public Node WriteNode(SyntaxNode syntaxNode)
        {
            var typeName = syntaxNode.GetType().Name;
            var node = _builder.CreateNode(typeName, syntaxNode.Text, typeName);

            foreach (var child in syntaxNode.Children)
            {
                var childNode = WriteNode(child);
                var link = _builder.CreateLink(node.Id, childNode.Id);
            }
            return node;
        }
    }
}
