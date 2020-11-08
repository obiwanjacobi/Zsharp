using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.Emit
{
    public class CodeBuilder
    {
        private readonly MethodDefinition _methodDefinition;
        private readonly Dictionary<string, CodeBlock> _blocks = new Dictionary<string, CodeBlock>();
        private readonly Dictionary<string, VariableDefinition> _variables = new Dictionary<string, VariableDefinition>();
        private int _blockIndex;

        public CodeBuilder(MethodDefinition methodDefinition)
        {
            _methodDefinition = methodDefinition;
            CodeBlock = AddBlock("__entry");
        }

        public CodeBlock CodeBlock { get; set; }

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
            CodeBlock.Return();
            return MoveToNextBlock();
        }

        public string NewBlockLabel()
        {
            _blockIndex++;
            return $"{_methodDefinition.Name}{_blockIndex}";
        }

        public CodeBlock Branch(string targetLabel)
        {
            CodeBlock.Branch(targetLabel);
            var targetBlock = GetBlock(targetLabel);
            CodeBlock = targetBlock;
            return targetBlock;
        }

        public CodeBlock BranchConditional(string targetLabel, string altLabel)
        {
            CodeBlock.BranchConditional(targetLabel, altLabel);

            var altBlock = GetBlock(altLabel);
            var targetBlock = GetBlock(targetLabel);

            CodeBlock = altBlock;
            return targetBlock;
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

        public ParameterDefinition GetParameter(string name)
        {
            return _methodDefinition.Parameters.Single(p => p.Name == name);
        }

        public void Apply(ILProcessor iLProcessor)
        {
            foreach (var codeBlock in _blocks.Values)
            {
                if (codeBlock.Termination == CodeBlockTermination.Return)
                    codeBlock.Return(iLProcessor);
                else if (String.IsNullOrEmpty(codeBlock.NextBlock) &&
                    String.IsNullOrEmpty(codeBlock.NextBlockAlt) &&
                    codeBlock.Termination == CodeBlockTermination.FallThrough)
                    codeBlock.Return(iLProcessor);
            }

            foreach (var codeBlock in _blocks.Values)
            {
                AppendInstructions(iLProcessor, codeBlock);

                _blocks.TryGetValue(codeBlock.NextBlock, out CodeBlock? nextBlock);
                _blocks.TryGetValue(codeBlock.NextBlockAlt, out CodeBlock? altBlock);

                EmitBranch(iLProcessor, codeBlock, nextBlock, altBlock);
            }
        }

        private static void AppendInstructions(ILProcessor iLProcessor, CodeBlock codeBlock)
        {
            foreach (var instruction in codeBlock.Instructions)
            {
                iLProcessor.Append(instruction);
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
                case CodeBlockTermination.FallThrough:
                    // no-op
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

        internal void Return(ILProcessor? iLProcessor = null)
        {
            if (iLProcessor != null)
            {
                Add(iLProcessor.Create(OpCodes.Ret));
                // to prevent another ret
                Termination = CodeBlockTermination.FallThrough;
            }
            else
                Termination = CodeBlockTermination.Return;
        }

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
        FallThrough,
        Return,
        Branch,
        BranchConditional,

        //BranchComparison, => use 'ceq' etc instructions
    }
}
