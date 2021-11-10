using System;
using System.Linq;
using Zsharp.AST;

#nullable disable

namespace Zsharp.Dgml
{
    public class AstDgmlBuilder : DgmlBuilder
    {
        private const string FieldsCategory = "Fields";

        public static void Save(AstNode node, string filePath = "node.dgml")
        {
            var builder = new AstDgmlBuilder();
            builder.CreateCommon();
            _ = builder.WriteNode(node);
            builder.SaveAs(filePath);
        }

        public static void SaveFile(AstFile file, string filePath = "file.dgml")
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

        public Node WriteNode(AstNode node, string parentId = "")
        {
            switch (node.NodeKind)
            {
                case AstNodeKind.None:
                    return null;
                case AstNodeKind.Module:
                    //return WriteModule(node);
                    return null;
                case AstNodeKind.File:
                    return WriteFile((AstFile)node);
                case AstNodeKind.Function:
                    return WriteFunction((AstFunctionDefinitionImpl)node, parentId);
                case AstNodeKind.Struct:
                    return WriteStruct((AstTypeDefinitionStruct)node, parentId);
                case AstNodeKind.Enum:
                    return WriteEnum((AstTypeDefinitionEnum)node, parentId);
                case AstNodeKind.Type:
                    return null;
                case AstNodeKind.CodeBlock:
                    return WriteCodeBlock((AstCodeBlock)node, parentId);
                case AstNodeKind.Assignment:
                    return WriteAssignment((AstAssignment)node, parentId);
                case AstNodeKind.Branch:
                    return WriteBranch((AstBranch)node, parentId);
                case AstNodeKind.Expression:
                    return WriteExpression((AstExpression)node, parentId);
                case AstNodeKind.Operand:
                    return null;
                case AstNodeKind.Literal:
                    return null;
                case AstNodeKind.Variable:
                    return null;
                case AstNodeKind.FunctionParameter:
                    return null;
                case AstNodeKind.TemplateParameter:
                    return null;
                case AstNodeKind.GenericParameter:
                    return null;
                case AstNodeKind.Field:
                    return null;
                case AstNodeKind.EnumOption:
                    return null;
                default:
                    break;
            }
            return null;
        }

        public Node WriteFile(AstFile file)
        {
            string moduleName = String.Empty;
            var module = (AstModule)(file.Parent);
            if (module is not null)
            {
                moduleName = module.Identifier.NativeFullName;
            }

            var node = CreateNode(moduleName, file.NodeKind);

            WriteCodeBlock(file.CodeBlock, node.Id);

            return node;
        }

        public Node WriteFunction(AstFunctionDefinitionImpl function, string parentId)
        {
            var identifier = function.Identifier;
            var name = identifier.NativeFullName;
            var node = CreateNode(name, function.NodeKind);
            _ = CreateLink(parentId, node.Id);

            var paramNames = String.Join(", ", function.FunctionType.Parameters
                .Select(p => $"{p.Identifier.NativeFullName}: {p.TypeReference.Identifier.NativeFullName}"));
            if (paramNames.Length > 0)
            {
                node.Group = DefaultGroup;
                var paramNode = CreateNode(name, paramNames);
                _ = CreateLink(node.Id, paramNode.Id, ContainsCategory);
            }

            if (function.CodeBlock.Lines.Any())
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
            foreach (AstNode line in codeBlock.Lines)
            {
                var itemNode = WriteCodeBlockItem(line, node.Id);
                var link = FindLink(node.Id, itemNode.Id);
                link.Label = i.ToString();
                i++;
            }

            if (codeBlock.Symbols.Symbols.Any())
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
            var name = astEnum.Identifier.NativeFullName;
            var node = CreateNode(name, astEnum.NodeKind);
            _ = CreateLink(parentId, node.Id);
            node.Group = DefaultGroup;

            var fields = String.Join("\r\n",
                astEnum.Fields.Select(f => $"{f.Identifier.NativeFullName} = {f.Expression.AsString()}"));

            var fieldsNode = CreateNode("Fields", fields);
            fieldsNode.Category = FieldsCategory;
            _ = CreateLink(node.Id, fieldsNode.Id, ContainsCategory);

            return node;
        }

        private Node WriteStruct(AstTypeDefinitionStruct astStruct, string parentId)
        {
            var name = astStruct.Identifier.NativeFullName;
            var node = CreateNode(name, astStruct.NodeKind);
            _ = CreateLink(parentId, node.Id);
            node.Group = DefaultGroup;

            var fields = String.Join("\r\n",
                astStruct.Fields.Select(f => $"{f.Identifier.NativeFullName}: {f.TypeReference.Identifier.NativeFullName}"));

            var fieldNode = CreateNode("Fields", fields);
            _ = CreateLink(node.Id, fieldNode.Id, ContainsCategory);
            return node;
        }

        public Node WriteAssignment(AstAssignment assignment, string parentId)
        {
            var name = assignment.Variable.Identifier.NativeFullName;
            string typeName = String.Empty;
            if (assignment.Variable.HasTypeReference)
                typeName = assignment.Variable.TypeReference.Identifier.NativeFullName;
            var nodeName = $"{name}: {typeName}";
            if (assignment.Expression is not null)
                nodeName += " = " + assignment.Expression.AsString();

            var node = CreateNode(nodeName, assignment.NodeKind);
            _ = CreateLink(parentId, node.Id);
            node.Group = DefaultGroup;

            var fields = String.Join("\r\n",
                assignment.Fields.Select(f => $"{f.Identifier.NativeFullName} = {f.Expression.AsString()}"));

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
                if (conditional.HasExpression)
                {
                    node.Label = conditional.Expression.AsString();
                }

                var code = conditional.CodeBlock;
                if (code is not null)
                {
                    var blockNode = WriteCodeBlock(code, node.Id);
                    if (conditional.HasExpression)
                    {
                        var link = FindLink(node.Id, blockNode.Id);
                        link.Label = "if";
                    }
                }

                if (conditional.HasSubBranch)
                {
                    var subNode = WriteBranch(conditional.SubBranch, node.Id);
                    var link = FindLink(node.Id, subNode.Id);
                    link.Label = "else";
                }
            }

            return node;
        }

        public Node WriteExpression(AstExpression expression, string parentId)
        {
            var nodeName = expression.Operator.AsString();
            var node = CreateNode(nodeName, expression.NodeKind);
            if (!String.IsNullOrEmpty(parentId))
            {
                _ = CreateLink(parentId, node.Id);
            }

            if (expression.HasLHS)
            {
                WriteOperand(expression.LHS, node.Id, "lhs");
            }

            if (expression.HasRHS)
            {
                WriteOperand(expression.RHS, node.Id, "rhs");
            }
            return node;
        }

        public Node WriteOperand(AstExpressionOperand operand, string parentId, string linkLabel = "")
        {
            if (operand.HasExpression)
            {
                return WriteExpression(operand.Expression, parentId);
            }

            var type = AstNodeKind.None;
            var value = String.Empty;

            var num = operand.LiteralNumeric;
            if (num is not null)
            {
                type = num.NodeKind;
                value = num.Value.ToString();
            }

            var bl = operand.LiteralBoolean;
            if (bl is not null)
            {
                type = bl.NodeKind;
                value = bl.Value.ToString();
            }

            var str = operand.LiteralString;
            if (str is not null)
            { 
                type = str.NodeKind;
                value = str.Value;
            }

            var varRef = operand.VariableReference;
            if (varRef is not null)
            { 
                type = varRef.NodeKind;
                value = varRef.AsString();
            }

            var funRef = operand.FunctionReference;
            if (funRef is not null)
            { 
                type = funRef.NodeKind;
                value = funRef.AsString();
            }

            var node = CreateNode(value, type);

            if (!String.IsNullOrEmpty(parentId))
            {
                var link = CreateLink(parentId, node.Id);
                link.Label = linkLabel;
            }

            return node;
        }

        public Node WriteSymbolTable(AstSymbolTable symbolTable, string parentId)
        {
            var node = CreateNode("Symbols", "", "Symbols");
            node.Group = DefaultGroup;
            _ = CreateLink(parentId, node.Id, ContainsCategory);

            var symbols = String.Join("\r\n",
                symbolTable.Symbols.Select(s => $"{s.SymbolName}: {s.SymbolKind} ({s.SymbolLocality})"));

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
