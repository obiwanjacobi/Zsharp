using Mono.Cecil.Cil;

namespace Zsharp.EmitIL
{
    partial class InstructionFactory
    {
        private readonly OpCode[] _signedConversions =
        {
            OpCodes.Conv_Ovf_I1, OpCodes.Conv_Ovf_I2, OpCodes.Conv_Ovf_I4, OpCodes.Conv_Ovf_I8,
            OpCodes.Conv_Ovf_U1, OpCodes.Conv_Ovf_U2, OpCodes.Conv_Ovf_U4, OpCodes.Conv_Ovf_U8,
            OpCodes.Conv_R4, OpCodes.Conv_R8,
        };

        private readonly OpCode[] _unsignedConversions =
        {
            OpCodes.Conv_Ovf_I1_Un, OpCodes.Conv_Ovf_I2_Un, OpCodes.Conv_Ovf_I4_Un, OpCodes.Conv_Ovf_I8_Un,
            OpCodes.Conv_Ovf_U1_Un, OpCodes.Conv_Ovf_U2_Un, OpCodes.Conv_Ovf_U4_Un, OpCodes.Conv_Ovf_U8_Un,
            OpCodes.Conv_R_Un, OpCodes.Conv_R8,
        };
    }
}
