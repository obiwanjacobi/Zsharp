using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Zsharp.Emit
{
    public class CodeBuilder
    {
        private readonly InstructionFactory _instructionFactory;
        private readonly List<CodeBlock> _blocks = new List<CodeBlock>();
        private readonly Dictionary<string, VariableDefinition> _varaibles = new Dictionary<string, VariableDefinition>();

        public CodeBuilder(InstructionFactory instructionFactory)
        {
            _instructionFactory = instructionFactory;
            _blocks.Add(new CodeBlock("__entry"));
        }

        public CodeBlock CodeBlock => _blocks[^1];

        public CodeBlock NewBlock(string label)
        {
            var block = new CodeBlock(label);
            _blocks.Add(block);
            return block;
        }

        public void AddVariable(string name, VariableDefinition varDef) => _varaibles.Add(name, varDef);

        public void Apply(ILProcessor iLProcessor)
        {
            foreach (var codeBlock in _blocks)
            {
                foreach (var instruction in codeBlock.Instructions)
                {
                    iLProcessor.Append(instruction);
                }
            }
        }
    }

    public class CodeBlock
    {
        private readonly List<Instruction> _instructions = new List<Instruction>();

        public CodeBlock(string label)
        {
            Label = label;
        }

        public string Label { get; private set; }

        public IEnumerable<Instruction> Instructions => _instructions;

        public void Add(Instruction instruction)
        {
            _instructions.Add(instruction);
        }
    }
}
