using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.Emit
{
    public class CodeBuilder
    {
        private readonly Dictionary<string, CodeBlock> _blocks = new Dictionary<string, CodeBlock>();
        private readonly Dictionary<string, VariableDefinition> _variables = new Dictionary<string, VariableDefinition>();

        public CodeBuilder()
        {
            CodeBlock = AddBlock("__entry");
        }

        public CodeBlock CodeBlock { get; private set; }

        public CodeBlock? MoveToNextBlock()
        {
            if (_blocks.TryGetValue(CodeBlock.NextBlock, out CodeBlock? nextBlock))
            {
                CodeBlock = nextBlock;
            }
            return nextBlock;
        }

        public CodeBlock? MoveToAltBlock()
        {
            if (_blocks.TryGetValue(CodeBlock.NextBlockAlt, out CodeBlock? altBlock))
            {
                CodeBlock = altBlock;
            }
            return altBlock;
        }

        public CodeBlock? Return()
        {
            return MoveToNextBlock();
        }

        public CodeBlock Branch(string targetLabel)
        {
            CodeBlock.Branch(targetLabel);
            var targetBlock = GetBlock(targetLabel);
            CodeBlock = targetBlock;
            return targetBlock;
        }

        public (CodeBlock Next, CodeBlock Alt) BranchConditional(string targetLabel, string altLabel)
        {
            CodeBlock.BranchConditional(targetLabel, altLabel);

            var targetBlock = GetBlock(targetLabel);
            var altBlock = GetBlock(altLabel);

            CodeBlock = altBlock;
            return (targetBlock, altBlock);
        }

        protected CodeBlock GetBlock(string label)
        {
            if (!_blocks.ContainsKey(label))
            {
                return AddBlock(label);
            }

            return _blocks[label];
        }

        protected CodeBlock AddBlock(string label)
        {
            var block = new CodeBlock(label);
            _blocks.Add(label, block);
            return block;
        }

        public IEnumerable<VariableDefinition> Variables => _variables.Values;

        public void AddVariable(string name, VariableDefinition varDef) => _variables.Add(name, varDef);

        public VariableDefinition GetVariable(string name) => _variables[name];

        public bool HasVariable(string name) => _variables.ContainsKey(name);

        public VariableDefinition GetOrAddVariable(string name, Func<string, VariableDefinition> createVariableDefinition)
        {
            if (_variables.ContainsKey(name))
                return _variables[name];

            var varDef = createVariableDefinition(name);
            _variables[name] = varDef;
            return varDef;
        }

        public void Apply(ILProcessor iLProcessor)
        {
            foreach (var codeBlock in _blocks.Values)
            {
                foreach (var instruction in codeBlock.Instructions)
                {
                    iLProcessor.Append(instruction);
                }

                _blocks.TryGetValue(codeBlock.NextBlock, out CodeBlock? nextBlock);
                _blocks.TryGetValue(codeBlock.NextBlockAlt, out CodeBlock? altBlock);

                EmitBranch(iLProcessor, codeBlock, nextBlock, altBlock);
            }
        }

        private static void EmitBranch(ILProcessor iLProcessor, CodeBlock codeBlock, CodeBlock? nextBlock, CodeBlock? altBlock)
        {
            switch (codeBlock.Termination)
            {
                case CodeBlockTermination.Branch:
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Br, nextBlock!.Instructions.First()));
                    break;
                case CodeBlockTermination.BranchConditional:
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Brtrue, altBlock!.Instructions.First()));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Br, nextBlock!.Instructions.First()));
                    break;
                case CodeBlockTermination.Return:
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
                    break;
                default:
                    throw new InvalidOperationException("Invalid CodeBlock Termination.");
            }
        }
    }

    /// <summary>
    /// A single execution path ends in a branch (or return).
    /// </summary>
    public class CodeBlock
    {
        private readonly List<Instruction> _instructions = new List<Instruction>();

        public CodeBlock(string label)
        {
            Label = label;
            NextBlock = String.Empty;
            NextBlockAlt = String.Empty;
        }

        public string Label { get; private set; }

        public CodeBlockTermination Termination { get; private set; }

        public string NextBlock { get; private set; }
        public string NextBlockAlt { get; private set; }

        internal void Branch(string targetLabel)
        {
            Termination = CodeBlockTermination.Branch;
            NextBlock = targetLabel;
        }

        internal void BranchConditional(string targetLabel, string altLabel)
        {
            Termination = CodeBlockTermination.BranchConditional;
            NextBlock = targetLabel;
            NextBlockAlt = altLabel;
        }

        public IEnumerable<Instruction> Instructions => _instructions;

        public void Add(Instruction instruction) => _instructions.Add(instruction);
    }

    public enum CodeBlockTermination
    {
        Return,
        Branch,
        BranchConditional,

        //BranchComparison, => use 'ceq' etc instructions
    }
}
