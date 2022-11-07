namespace Maja.Compiler.External;

internal sealed class ExternalModuleLoader : IExternalModuleLoader
{
    private readonly AssemblyManager _assemblyManager;

    public ExternalModuleLoader(AssemblyManager assemblyManager)
    {
        _assemblyManager = assemblyManager;
    }
}
