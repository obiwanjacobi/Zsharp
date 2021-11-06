using Zsharp.AST;

namespace Zsharp.EmitCS
{
    partial class EmitIntrinsic
    {
        private abstract class CodeProvider
        {
            protected CodeProvider()
            { }

            public virtual void Build(CsBuilder builder)
                => throw new ZsharpException("Not Implemented.");
            public virtual void Build(CsBuilder builder, AstFunctionDefinition functionDef)
                => throw new ZsharpException("Not Implemented.");
        }

        // Fn: TargetType(self: SourceType): TargetType
        // C#: (TargetType)SourceType
        private class ConversionCodeProvider : CodeProvider
        {
            public override void Build(CsBuilder builder, AstFunctionDefinition functionDef)
            {
                var conversion = "Conversion";
                builder.Append($"Zsharp.Runtime.{conversion}.{functionDef.Identifier.SymbolName.CanonicalName.FullName}");
            }
        }
    }
}
