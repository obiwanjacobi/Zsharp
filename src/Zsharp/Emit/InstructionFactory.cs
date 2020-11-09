using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

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
        public Instruction ArithmeticModulo() => _iLProcessor.Create(OpCodes.Rem);
        public Instruction ArithmeticMultiple() => _iLProcessor.Create(OpCodes.Mul);
        public Instruction ArithmeticNegate() => _iLProcessor.Create(OpCodes.Neg);

        public Instruction CompareEqual() => _iLProcessor.Create(OpCodes.Ceq);
        public Instruction CompareGreater() => _iLProcessor.Create(OpCodes.Cgt);
        public Instruction CompareLesser() => _iLProcessor.Create(OpCodes.Clt);

        public IEnumerable<Instruction> CompareNotEqual()
        {
            return new[] {
                _iLProcessor.Create(OpCodes.Ceq),
                _iLProcessor.Create(OpCodes.Ldc_I4, 0),
                _iLProcessor.Create(OpCodes.Ceq),
            };
        }
        public IEnumerable<Instruction> CompareGreaterEqual()
        {
            return new[] {
                _iLProcessor.Create(OpCodes.Clt),
                _iLProcessor.Create(OpCodes.Ldc_I4, 0),
                _iLProcessor.Create(OpCodes.Ceq),
            };
        }
        public IEnumerable<Instruction> CompareLesserEqual()
        {
            return new[] {
                _iLProcessor.Create(OpCodes.Cgt),
                _iLProcessor.Create(OpCodes.Ldc_I4, 0),
                _iLProcessor.Create(OpCodes.Ceq),
            };
        }

        public Instruction BitwiseAnd() => _iLProcessor.Create(OpCodes.And);
        public Instruction BitwiseOr() => _iLProcessor.Create(OpCodes.Or);
        public Instruction BitwiseXor() => _iLProcessor.Create(OpCodes.Xor);
        public IEnumerable<Instruction> BitwiseShiftLeft()
        {
            return new[] {
                _iLProcessor.Create(OpCodes.Ldc_I4_S, 31),
                _iLProcessor.Create(OpCodes.And),
                _iLProcessor.Create(OpCodes.Shl),
            };
        }
        public IEnumerable<Instruction> BitwiseShiftRight()
        {
            return new[] {
                _iLProcessor.Create(OpCodes.Ldc_I4_S, 31),
                _iLProcessor.Create(OpCodes.And),
                _iLProcessor.Create(OpCodes.Shr),
            };
        }

        public Instruction LoadConstant(Boolean constant) => _iLProcessor.Create(OpCodes.Ldc_I4, constant ? 1 : 0);
        public Instruction LoadConstant(Int32 constant) => _iLProcessor.Create(OpCodes.Ldc_I4, constant);
        public Instruction LoadConstant(String constant) => _iLProcessor.Create(OpCodes.Ldstr, constant);

        public Instruction LoadVariable(VariableDefinition varDef) => _iLProcessor.Create(OpCodes.Ldloc, varDef);
        public Instruction StoreVariable(VariableDefinition varDef) => _iLProcessor.Create(OpCodes.Stloc, varDef);

        public Instruction Call(MethodReference method) => _iLProcessor.Create(OpCodes.Call, method);
        public Instruction LoadParameter(ParameterDefinition paramDef) => _iLProcessor.Create(OpCodes.Ldarg, paramDef);

    }
}
