namespace Maja.Compiler.Compilation
{
    public sealed class SemanticModel
    {
        private readonly Compilation _compilation;

        internal SemanticModel(Compilation compilation)
        {
            _compilation = compilation;
        }
    }
}