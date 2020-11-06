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

        public Instruction LoadConstant(Boolean constant) => _iLProcessor.Create(OpCodes.Ldc_I4, constant ? 1 : 0);
        public Instruction LoadConstant(Int32 constant) => _iLProcessor.Create(OpCodes.Ldc_I4, constant);
        public Instruction LoadConstant(String constant) => _iLProcessor.Create(OpCodes.Ldstr, constant);

        public Instruction LoadVariable(VariableDefinition varDef) => _iLProcessor.Create(OpCodes.Ldloc, varDef);
        public Instruction StoreVariable(VariableDefinition varDef) => _iLProcessor.Create(OpCodes.Stloc, varDef);

        public Instruction Call(MethodReference method) => _iLProcessor.Create(OpCodes.Call, method);
        public Instruction LoadParameter(ParameterDefinition paramDef) => _iLProcessor.Create(OpCodes.Ldarg, paramDef);
    }
}
