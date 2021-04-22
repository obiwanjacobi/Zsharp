using System;
using System.Linq;
using Zsharp.EmitIL;

namespace Zsharp.Dgml
{
    public class EmitDgmlBuilder : DgmlBuilder
    {
        private const string InstructionsCategory = "Instructions";

        public static void Save(CodeBuilder codeBuilder, string filePath = "codeBlocks.dgml")
        {
            var builder = new EmitDgmlBuilder();
            builder.CreateCommon();
            _ = builder.WriteCodeBuilder(codeBuilder);
            builder.SaveAs(filePath);
        }

        public override void CreateCommon()
        {
            base.CreateCommon();
            _ = CreateCategory(InstructionsCategory, InstructionsCategory);
        }

        public Node WriteCodeBuilder(CodeBuilder codeBuilder)
        {
            var node = CreateNode("Code", codeBuilder.MethodDefinition.Name);

            var codeBlocks = codeBuilder.CodeBlocks.ToDictionary(cb => cb.Label);
            var blockNodes = codeBuilder.CodeBlocks
                .ToDictionary(cb => cb.Label, WriteCodeBlock);

            foreach (var blockLabel in blockNodes.Keys)
            {
                var codeBlock = codeBlocks[blockLabel];
                var blockNode = blockNodes[blockLabel];

                // TODO: another way to detect 1st block? (no incoming links?)
                if (blockLabel == "__entry")
                    CreateLink(node.Id, blockNode.Id);

                if (!String.IsNullOrEmpty(codeBlock.NextBlock))
                {
                    var nextNode = blockNodes[codeBlock.NextBlock];
                    _ = CreateLink(blockNode.Id, nextNode.Id);
                }
                if (!String.IsNullOrEmpty(codeBlock.NextBlockAlt))
                {
                    var nextNode = blockNodes[codeBlock.NextBlockAlt];
                    var link = CreateLink(blockNode.Id, nextNode.Id);
                    link.Label = "alt";
                }
            }

            return node;
        }

        private Node WriteCodeBlock(CodeBlock codeBlock)
        {
            var node = CreateNode("CodeBlock", $"{codeBlock.Label} [{codeBlock.Termination.ToString()}]");

            var instructions = String.Join("\r\n",
                codeBlock.Instructions.Select(i => $"{i.OpCode} {i.Operand}"));

            if (!String.IsNullOrEmpty(instructions))
            {
                var instrNode = CreateNode("Instructions", instructions);
                instrNode.Category = InstructionsCategory;
                var link = CreateLink(node.Id, instrNode.Id);
                link.Category = ContainsCategory;
                node.Group = DefaultGroup;
            }
            return node;
        }
    }
}
