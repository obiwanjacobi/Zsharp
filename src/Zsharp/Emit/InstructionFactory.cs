using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Zsharp.Emit
{
    public class InstructionFactory
    {
        private readonly ILProcessor _iLProcessor;

        public InstructionFactory(ILProcessor processor)
        {
            _iLProcessor = processor;
        }

        public Instruction ArithmeticAdd() => _iLProcessor.Create(OpCodes.Add);
        public Instruction ArithmeticSubtract() => _iLProcessor.Create(OpCodes.Sub);
        public Instruction ArithmeticDivide() => _iLProcessor.Create(OpCodes.Div);
        public Instruction ArithmeticMultiple() => _iLProcessor.Create(OpCodes.Mul);

        public Instruction BitwiseAnd() => _iLProcessor.Create(OpCodes.And);
        public Instruction BitwiseOr() => _iLProcessor.Create(OpCodes.Or);
        public Instruction BitwiseXor() => _iLProcessor.Create(OpCodes.Xor);

        public Instruction LoadConstant(Int32 constant) => _iLProcessor.Create(OpCodes.Ldc_I4, constant);

        public Instruction LoadVariable(VariableDefinition varDef) => _iLProcessor.Create(OpCodes.Ldloc, varDef);
        public Instruction StoreVariable(VariableDefinition varDef) => _iLProcessor.Create(OpCodes.Stloc, varDef);

        public Instruction Call(MethodDefinition method) => _iLProcessor.Create(OpCodes.Call, method);
    }
}
