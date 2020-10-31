using System;
using System.IO;
using Zsharp.AST;

#nullable disable

namespace Zsharp.Dgml
{
    public class DgmlBuilder
    {
        private const string ContainsCategory = "Contains";
        private const Group DefaultGroup = Group.Collapsed;

        private readonly Graph _graph;
        private int _id;

        public DgmlBuilder()
        {
            _graph = new Graph();
            CreateInitial();
        }

        private static string BranchTypeToName(AstBranchType branchType)
        {
            return branchType switch
            {
                AstBranchType.Conditional => "If|Else",
                AstBranchType.ExitFunction => "Ret",
                AstBranchType.ExitIteration => "Cont",
                AstBranchType.ExitLoop => "Brk",
                _ => "-",
            };
        }

        public Node WriteFile(AstFile file)
        {
            string moduleName = String.Empty;
            var module = (AstModule?)(file.Parent);
            if (module != null)
            {
                moduleName = module.Name;
            }

            var node = CreateNode(moduleName, moduleName, "File");

            foreach (var function in file.Functions)
            {
                WriteFunction(function, node.Id);
            }

            WriteSymbolTable(file.Symbols, node.Id);

            return node;
        }

        public Node WriteFunction(AstFunctionDefinition function, string parentId)
        {
            var identifier = function.Identifier;
            var name = identifier.Name;
            var node = CreateNode(name, name, "Function");
            var link = CreateLink(parentId, node.Id);

            string paramNames = String.Empty;
            foreach (var p in function.Parameters)
            {
                if (paramNames.Length > 0)
                    paramNames += ", ";

                paramNames += p.Identifier.Name;
            }
            if (paramNames.Length > 0)
            {
                node.Group = DefaultGroup;
                var paramNode = CreateNode(name, paramNames, "Parameter");
                var paramLink = CreateLink(node.Id, paramNode.Id, ContainsCategory);
            }

            var codeBlock = function.CodeBlock;
            if (codeBlock != null)
            {
                WriteCodeBlock(codeBlock, node.Id);
            }

            return node;
        }

        public Node WriteCodeBlock(AstCodeBlock codeBlock, string parentId)
        {
            string name = "";
            var node = CreateNode(name, name, "CodeBlock");
            var link = CreateLink(parentId, node.Id);

            int i = 0;
            foreach (var item in codeBlock.Items)
            {
                i++;
                var itemNode = WriteCodeBlockItem(item, node.Id);
                if (itemNode != null)
                {
                    link = FindLink(node.Id, itemNode.Id);
                    if (link != null)
                    {
                        link.Label = i.ToString();
                    }
                }
            }

            WriteSymbolTable(codeBlock.Symbols, node.Id);

            return node;
        }

        public Node WriteCodeBlockItem(AstCodeBlockItem codeBlockItem, string codeBlockId)
        {
            switch (codeBlockItem.NodeType)
            {
                case AstNodeType.Assignment:
                    return WriteAssignment((AstAssignment)codeBlockItem, codeBlockId);
                case AstNodeType.Branch:
                    return WriteBranch((AstBranch)codeBlockItem, codeBlockId);
                default:
                    var name = "NotImplemented-" + ((int)codeBlockItem.NodeType).ToString();
                    var node = CreateNode(name, name);
                    var link = CreateLink(codeBlockId, node.Id);
                    return node;
            }
        }

        public Node WriteAssignment(AstAssignment assignment, string parentId)
        {
            var name = assignment.Variable.Identifier.Name;
            var node = CreateNode(name, name, "Assignment");
            var link = CreateLink(parentId, node.Id);
            return node;
        }

        public Node WriteBranch(AstBranch branch, string parentId)
        {
            var name = BranchTypeToName(branch.BranchType);
            var node = CreateNode(name, name, "Branch");
            var link = CreateLink(parentId, node.Id);

            var conditional = branch.ToConditional();
            if (conditional != null)
            {
                var expression = conditional.Expression;
                if (expression != null)
                {
                    var exprNode = CreateNode(name, expression.AsString());
                    var exprLink = CreateLink(node.Id, exprNode.Id, ContainsCategory);
                    node.Group = DefaultGroup;
                }

                var code = conditional.CodeBlock;
                if (code != null)
                {
                    WriteCodeBlock(code, node.Id);
                }

                var subBranch = conditional.SubBranch;
                if (subBranch != null)
                {
                    WriteBranch(subBranch, node.Id);
                }
            }

            return node;
        }

        public Node WriteSymbolTable(AstSymbolTable symbolTable, string parentId)
        {
            var node = CreateNode("", "", "Symbols");
            node.Group = DefaultGroup;
            var link = CreateLink(parentId, node.Id, ContainsCategory);

            foreach (var entry in symbolTable.Entries)
            {
                var entryNode = CreateNode("Symbol", entry.SymbolName, "Symbol");
                var entryLink = CreateLink(node.Id, entryNode.Id, ContainsCategory);
            }

            return node;
        }

        public void Serialize(Stream output)
        {
            _graph.Serialize(output);
        }

        public void SaveAs(string filePath)
        {
            using var stream = File.OpenWrite(filePath);
            Serialize(stream);
        }

        private void CreateInitial()
        {
            var contains = CreateCategory(ContainsCategory, ContainsCategory);
            contains.IsContainment = true;
        }

        private Node CreateNode(string id, string label, string typeName = null)
        {
            var node = new Node
            {
                Id = id + NextId().ToString(),
                Label = label
            };
            if (typeName != null)
            {
                node.TypeName = typeName;
            }

            _graph.Nodes.Add(node);
            return node;
        }

        private Link CreateLink(string sourceId, string targetId, string category = null)
        {
            var link = new Link
            {
                Source = sourceId,
                Target = targetId
            };
            if (category != null)
            {
                link.Category = category;
            }

            _graph.Links.Add(link);
            return link;
        }

        private Category CreateCategory(string id, string label)
        {
            var category = new Category
            {
                Id = id + NextId().ToString(),
                Label = label
            };

            _graph.Categories.Add(category);
            return category;
        }

        private Link FindLink(string sourceId, string targetId)
        {
            foreach (var l in _graph.Links)
            {
                if (l.Source == sourceId && l.Target == targetId)
                {
                    return l;
                }
            }

            return null;
        }

        private int NextId() => ++_id;
    }
}
