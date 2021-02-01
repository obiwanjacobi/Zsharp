using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace Zsharp.Emit
{
    public partial class InstructionFactory
    {
        private readonly ILProcessor _iLProcessor;

        public InstructionFactory(ILProcessor processor)
        {
            _iLProcessor = processor;
        }

        public Instruction ArithmeticAdd(bool isUnsigned)
            => _iLProcessor.Create(isUnsigned ? OpCodes.Add_Ovf : OpCodes.Add_Ovf_Un);
        public Instruction ArithmeticSubtract(bool isUnsigned)
            => _iLProcessor.Create(isUnsigned ? OpCodes.Sub_Ovf : OpCodes.Sub_Ovf_Un);
        public Instruction ArithmeticDivide(bool isUnsigned)
            => _iLProcessor.Create(isUnsigned ? OpCodes.Div : OpCodes.Div_Un);
        public Instruction ArithmeticModulo(bool isUnsigned)
            => _iLProcessor.Create(isUnsigned ? OpCodes.Rem : OpCodes.Rem_Un);
        public Instruction ArithmeticMultiple(bool isUnsigned)
            => _iLProcessor.Create(isUnsigned ? OpCodes.Mul_Ovf : OpCodes.Mul_Ovf_Un);
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
                _iLProcessor.Create(OpCodes.Ldc_I4, 31),
                _iLProcessor.Create(OpCodes.And),
                _iLProcessor.Create(OpCodes.Shl),
            };
        }
        public IEnumerable<Instruction> BitwiseShiftRight()
        {
            return new[] {
                _iLProcessor.Create(OpCodes.Ldc_I4, 31),
                _iLProcessor.Create(OpCodes.And),
                _iLProcessor.Create(OpCodes.Shr),
            };
        }

        public Instruction LoadConstant(Boolean constant) => _iLProcessor.Create(OpCodes.Ldc_I4, constant ? 1 : 0);
        public Instruction LoadConstant(SByte constant) => _iLProcessor.Create(OpCodes.Ldc_I4_S, constant);
        public Instruction LoadConstant(String constant) => _iLProcessor.Create(OpCodes.Ldstr, constant);
        public Instruction LoadConstant(UInt64 constant, UInt32 bitCount)
        {
            if (bitCount <= 32)
                return _iLProcessor.Create(OpCodes.Ldc_I4, (Int32)constant);
            else
                return _iLProcessor.Create(OpCodes.Ldc_I8, constant);
        }

        public Instruction Convert(IntrinsicType target, IntrinsicType source)
        {
            var unsigned = (source >= IntrinsicType.U8 && source <= IntrinsicType.U64);
            var table = unsigned ? _unsignedConversions : _signedConversions;
            return _iLProcessor.Create(table[(int)target]);
        }

        public Instruction LoadVariable(VariableDefinition varDef) => _iLProcessor.Create(OpCodes.Ldloc, varDef);
        public Instruction LoadVariableAddress(VariableDefinition varDef) => _iLProcessor.Create(OpCodes.Ldloca, varDef);
        public Instruction StoreVariable(VariableDefinition varDef) => _iLProcessor.Create(OpCodes.Stloc, varDef);
        public Instruction InitObject(TypeReference typeRef) => _iLProcessor.Create(OpCodes.Initobj, typeRef);

        public Instruction LoadField(FieldDefinition field)
        {
            if (field.FieldType.Name == typeof(String).Name)
                return _iLProcessor.Create(OpCodes.Ldsfld, field);
            return _iLProcessor.Create(OpCodes.Ldfld, field);
        }

        public Instruction StoreField(FieldDefinition field)
        {
            if (field.FieldType.Name == typeof(String).Name)
                return _iLProcessor.Create(OpCodes.Stsfld, field);
            return _iLProcessor.Create(OpCodes.Stfld, field);
        }

        public Instruction Call(MethodReference method) => _iLProcessor.Create(OpCodes.Call, method);
        public Instruction LoadParameter(ParameterDefinition paramDef) => _iLProcessor.Create(OpCodes.Ldarg, paramDef);
    }
}
