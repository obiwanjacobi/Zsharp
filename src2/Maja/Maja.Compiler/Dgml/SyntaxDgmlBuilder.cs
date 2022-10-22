using Maja.Compiler.Syntax;

namespace Maja.Dgml
{
    public class SyntaxDgmlBuilder
    {
        private readonly DgmlBuilder _builder = new("SyntaxNode");

        public SyntaxDgmlBuilder()
        {
            _builder.CreateCategory("LeadingTokens", "LeadingTokens");
            _builder.CreateCategory("TrailingTokens", "TrailingTokens");
            var style = _builder.CreateStyleForCaregory("LeadingTokens", StyleTargetType.Link);
            style.Setters.Add("Stroke", "Yellow");
            style = _builder.CreateStyleForCaregory("TrailingTokens", StyleTargetType.Link);
            style.Setters.Add("Stroke", "Blue");

            _builder.CreateCategory("Token", "Token");
            _builder.CreateCategory("Error", "Error");
            style = _builder.CreateStyleForCaregory("Token");
            style.Setters.Add("Background", "Green");
            style = _builder.CreateStyleForCaregory("Error");
            style.Setters.Add("Background", "Red");
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
                WriteTokens(syntaxNode.LeadingTokens, node.Id, "LeadingTokens");
            }

            foreach (var child in syntaxNode.ChildNodes)
            {
                var childNode = WriteNode(child);
                var link = _builder.CreateLink(node.Id, childNode.Id);
            }

            if (syntaxNode.HasTrailingTokens)
            {
                WriteTokens(syntaxNode.TrailingTokens, node.Id, "TrailingTokens");
            }

            return node;
        }

        private void WriteTokens(SyntaxTokenList tokens, string nodeId, string category)
        {
            foreach (var child in tokens)
            {
                var childToken = WriteToken(child);
                childToken.Category = child.HasError ? "Error" : "Token";
                var link = _builder.CreateLink(nodeId, childToken.Id, category);
            }
        }

        public Node WriteToken(SyntaxToken syntaxToken)
        {
            var typeName = syntaxToken.GetType().Name;
            var node = _builder.CreateNode(typeName, $"'{syntaxToken.Text}'", typeName);
            return node;
        }
    }
}
