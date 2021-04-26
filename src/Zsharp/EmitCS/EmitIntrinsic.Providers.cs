namespace Zsharp.EmitCS
{
    partial class EmitIntrinsic
    {
        private interface ICodeProvider
        {
            string OverloadKey { get; }
            void Build(CsBuilder builder);
        }
    }
}
