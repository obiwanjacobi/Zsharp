using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Maja.Dgml
{
    public class ParserDgmlBuilder
    {
        private readonly DgmlBuilder _builder = new("ParserRuleContext");

        public ParserDgmlBuilder()
        {
            _builder.CreateCategory("Token", "Token");
            _builder.CreateCategory("Error", "Error");
            var style = _builder.CreateStyleForCaregory("Token");
            style.Setters.Add("Background", "Green");
            style = _builder.CreateStyleForCaregory("Error");
            style.Setters.Add("Background", "Red");

            IncludeTokens = true;
        }

        public bool IncludeTokens { get; set; }

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
                else if (IncludeTokens)
                {
                    if (child is IErrorNode errChild)
                    {
                        var childNode = WriteTerminal((TerminalNodeImpl)errChild);
                        childNode.Category = "Error";
                        var link = _builder.CreateLink(node.Id, childNode.Id);
                    }
                    else if (child is TerminalNodeImpl termChild)
                    {
                        var childNode = WriteTerminal(termChild);
                        childNode.Category = "Token";
                        var link = _builder.CreateLink(node.Id, childNode.Id);
                    }
                }
            }
            return node;
        }

        public Node WriteTerminal(TerminalNodeImpl terminalNode)
        {
            var typeName = "Token";
            return _builder.CreateNode(typeName, $"'{terminalNode.GetText()}'", typeName);
        }
    }
}
