using Maja.Compiler.Syntax;

namespace Maja.Dgml
{
    public class SyntaxDgmlBuilder
    {
        private readonly DgmlBuilder _builder = new("SyntaxNode");

        public SyntaxDgmlBuilder()
        {
            _builder.CreateCommon();
            _builder.CreateCategory("LT", "LT");
            _builder.CreateCategory("TT", "TT");
        }

        public static void Save(SyntaxNode syntaxNode, string filePath = "syntax.dgml")
        {
            var builder = new SyntaxDgmlBuilder();
            builder.WriteNode(syntaxNode);
            builder._builder.SaveAs(filePath);
        }

        public Node WriteNode(SyntaxNode syntaxNode)
        {
            var typeName = syntaxNode.GetType().Name;
            var label = syntaxNode.Text;
            if (syntaxNode is ExpressionSyntax exprSyntax &&
                exprSyntax.Precedence)
                label = $"({label})";

            var node = _builder.CreateNode(typeName, label, typeName);

            if (syntaxNode.HasLeadingTokens)
            {
                foreach (var child in syntaxNode.LeadingTokens)
                {
                    var childToken = WriteToken(child);
                    var link = _builder.CreateLink(node.Id, childToken.Id, "LT");
                }
            }

            foreach (var child in syntaxNode.ChildNodes)
            {
                var childNode = WriteNode(child);
                var link = _builder.CreateLink(node.Id, childNode.Id);
            }

            if (syntaxNode.HasTrailingTokens)
            {
                foreach (var child in syntaxNode.TrailingTokens)
                {
                    var childToken = WriteToken(child);
                    var link = _builder.CreateLink(node.Id, childToken.Id, "TT");
                }
            }

            return node;
        }

        public Node WriteToken(SyntaxToken syntaxToken)
        {
            var typeName = syntaxToken.GetType().Name;
            var node = _builder.CreateNode(typeName, $"'{syntaxToken.Text}'", typeName);
            return node;
        }
    }
}
