using System;
using System.Linq;
using Zsharp.AST;

#nullable disable

namespace Zsharp.Dgml
{
    public class AstDgmlBuilder : DgmlBuilder
    {
        private const string FieldsCategory = "Fields";

        public static void Save(AstFile file, string filePath = "file.dgml")
        {
            var builder = new AstDgmlBuilder();
            builder.CreateCommon();
            _ = builder.WriteFile(file);
            builder.SaveAs(filePath);
        }

        public override void CreateCommon()
        {
            base.CreateCommon();
            _ = CreateCategory(FieldsCategory, FieldsCategory);
        }

        public Node WriteFile(AstFile file)
        {
            string moduleName = String.Empty;
            var module = (AstModule)(file.Parent);
            if (module is not null)
            {
                moduleName = module.Identifier.Name;
            }

            var node = CreateNode(moduleName, file.NodeKind);

            WriteCodeBlock(file.CodeBlock, node.Id);

            return node;
        }

        public Node WriteFunction(AstFunctionDefinitionImpl function, string parentId)
        {
            var identifier = function.Identifier;
            var name = identifier.Name;
            var node = CreateNode(name, function.NodeKind);
            _ = CreateLink(parentId, node.Id);

            var paramNames = String.Join(", ", function.FunctionType.Parameters
                .Select(p => $"{p.Identifier.Name}: {p.TypeReference.Identifier.Name}"));
            if (paramNames.Length > 0)
            {
                node.Group = DefaultGroup;
                var paramNode = CreateNode(name, paramNames);
                _ = CreateLink(node.Id, paramNode.Id, ContainsCategory);
            }

            if (function.CodeBlock.Items.Any())
            {
                WriteCodeBlock(function.CodeBlock, node.Id);
            }

            return node;
        }

        public Node WriteCodeBlock(AstCodeBlock codeBlock, string parentId)
        {
            string name = "";
            var node = CreateNode(name, codeBlock.NodeKind);
            _ = CreateLink(parentId, node.Id);

            int i = 0;
            foreach (AstNode item in codeBlock.Items)
            {
                var itemNode = WriteCodeBlockItem(item, node.Id);
                var link = FindLink(node.Id, itemNode.Id);
                link.Label = i.ToString();
                i++;
            }

            if (codeBlock.Symbols.Entries.Any())
                WriteSymbolTable(codeBlock.Symbols, node.Id);

            return node;
        }

        public Node WriteCodeBlockItem(AstNode codeBlockItem, string codeBlockId)
        {
            switch (codeBlockItem.NodeKind)
            {
                case AstNodeKind.Assignment:
                    return WriteAssignment((AstAssignment)codeBlockItem, codeBlockId);
                case AstNodeKind.Branch:
                    return WriteBranch((AstBranch)codeBlockItem, codeBlockId);
                case AstNodeKind.Function:
                    return WriteFunction((AstFunctionDefinitionImpl)codeBlockItem, codeBlockId);
                case AstNodeKind.Enum:
                    return WriteEnum((AstTypeDefinitionEnum)codeBlockItem, codeBlockId);
                case AstNodeKind.Struct:
                    return WriteStruct((AstTypeDefinitionStruct)codeBlockItem, codeBlockId);
                default:
                    var name = $"[{codeBlockItem.NodeKind}] <Not Implemented>";
                    var node = CreateNode(name, name);
                    _ = CreateLink(codeBlockId, node.Id);
                    return node;
            }
        }

        private Node WriteEnum(AstTypeDefinitionEnum astEnum, string parentId)
        {
            var name = astEnum.Identifier.Name;
            var node = CreateNode(name, astEnum.NodeKind);
            _ = CreateLink(parentId, node.Id);
            node.Group = DefaultGroup;

            var fields = String.Join("\r\n",
                astEnum.Fields.Select(f => $"{f.Identifier.Name} = {f.Expression.AsString()}"));

            var fieldsNode = CreateNode("Fields", fields);
            fieldsNode.Category = FieldsCategory;
            _ = CreateLink(node.Id, fieldsNode.Id, ContainsCategory);

            return node;
        }

        private Node WriteStruct(AstTypeDefinitionStruct astStruct, string parentId)
        {
            var name = astStruct.Identifier.Name;
            var node = CreateNode(name, astStruct.NodeKind);
            _ = CreateLink(parentId, node.Id);
            node.Group = DefaultGroup;

            var fields = String.Join("\r\n",
                astStruct.Fields.Select(f => $"{f.Identifier.Name}: {f.TypeReference?.Identifier.Name}"));

            var fieldNode = CreateNode("Fields", fields);
            _ = CreateLink(node.Id, fieldNode.Id, ContainsCategory);
            return node;
        }

        public Node WriteAssignment(AstAssignment assignment, string parentId)
        {
            var name = assignment.Variable.Identifier.Name;
            var typeName = assignment.Variable.TypeReference?.Identifier.Name;
            var nodeName = $"{name}: {typeName}";
            if (assignment.Expression is not null)
                nodeName += " = " + assignment.Expression.AsString();

            var node = CreateNode(nodeName, assignment.NodeKind);
            _ = CreateLink(parentId, node.Id);
            node.Group = DefaultGroup;

            var fields = String.Join("\r\n",
                assignment.Fields.Select(f => $"{f.Identifier.Name} = {f.Expression.AsString()}"));

            var fieldNode = CreateNode("Fields", fields);
            _ = CreateLink(node.Id, fieldNode.Id, ContainsCategory);
            return node;
        }

        public Node WriteBranch(AstBranch branch, string parentId)
        {
            var name = BranchTypeToName(branch.BranchKind);
            var node = CreateNode(name, branch.NodeKind);
            _ = CreateLink(parentId, node.Id);

            var conditional = branch.ToConditional();
            if (conditional is not null)
            {
                var expression = conditional.Expression;
                if (expression is not null)
                {
                    node.Label = conditional.Expression.AsString();
                }

                var code = conditional.CodeBlock;
                if (code is not null)
                {
                    var blockNode = WriteCodeBlock(code, node.Id);
                    if (expression is not null)
                    {
                        var link = FindLink(node.Id, blockNode.Id);
                        link.Label = "if";
                    }
                }

                var subBranch = conditional.SubBranch;
                if (subBranch is not null)
                {
                    var subNode = WriteBranch(subBranch, node.Id);
                    var link = FindLink(node.Id, subNode.Id);
                    link.Label = "else";
                }
            }

            return node;
        }

        public Node WriteSymbolTable(AstSymbolTable symbolTable, string parentId)
        {
            var node = CreateNode("Symbols", "", "Symbols");
            node.Group = DefaultGroup;
            _ = CreateLink(parentId, node.Id, ContainsCategory);

            var symbols = String.Join("\r\n",
                symbolTable.Entries.Select(s => $"{s.SymbolName}: {s.SymbolKind} ({s.SymbolLocality})"));

            var entryNode = CreateNode("Symbols", symbols);
            _ = CreateLink(node.Id, entryNode.Id, ContainsCategory);

            return node;
        }

        private Node CreateNode(string label, AstNodeKind nodeKind)
        {
            return CreateNode(nodeKind.ToString(), label, nodeKind.ToString());
        }

        private static string BranchTypeToName(AstBranchKind branchKind)
        {
            return branchKind switch
            {
                AstBranchKind.Conditional => "If|Else",
                AstBranchKind.ExitFunction => "Ret",
                AstBranchKind.ExitIteration => "Cont",
                AstBranchKind.ExitLoop => "Brk",
                _ => "-",
            };
        }
    }
}
