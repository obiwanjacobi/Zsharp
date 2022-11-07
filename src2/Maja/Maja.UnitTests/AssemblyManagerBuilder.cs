using System;
using System.IO;
using System.Runtime.InteropServices;
using Maja.Compiler.External;

namespace Maja.UnitTests;
internal class AssemblyManagerBuilder
{
    private static readonly string _dotNetBasePath = RuntimeEnvironment.GetRuntimeDirectory();
    private readonly AssemblyManager _assemblyManager = new(_dotNetBasePath);

    public AssemblyManagerBuilder(bool preloadDependencies = true)
    {
        _assemblyManager.PreloadDependencies = preloadDependencies;
    }

    public AssemblyManager AssemblyManager => _assemblyManager;

    public IExternalModuleLoader ToModuleLoader()
        => new ExternalModuleLoader(_assemblyManager);

    public AssemblyManagerBuilder AddAssembly(string assemlbyPath)
    {
        _assemblyManager.LoadAssembly(assemlbyPath);
        return this;
    }

    public AssemblyManagerBuilder AddMajaRuntime()
    {
        _assemblyManager.LoadAssembly("Maja.Runtime.dll");
        return this;
    }

    public AssemblyManagerBuilder AddSystemAll()
    {
        AddMsCoreLib();

        var files = Directory.EnumerateFiles(_dotNetBasePath, "System.*.dll");

        foreach (var file in files)
        {
            try
            {
                _assemblyManager.LoadAssembly(file);
            }
            catch (BadImageFormatException)
            { }
        }
        return this;
    }

    public AssemblyManagerBuilder AddMsCoreLib()
    {
        _assemblyManager.LoadAssembly(Path.Combine(_dotNetBasePath, "mscorlib.dll"));
        return this;
    }

    public AssemblyManagerBuilder AddSystemConsole()
    {
        _assemblyManager.LoadAssembly(Path.Combine(_dotNetBasePath, "System.Console.dll"));
        return this;
    }

    public AssemblyManagerBuilder AddSystemRuntime()
    {
        _assemblyManager.LoadAssembly(Path.Combine(_dotNetBasePath, "System.Runtime.dll"));
        return this;
    }

    public AssemblyManagerBuilder AddSystemCollections()
    {
        _assemblyManager.LoadAssembly(Path.Combine(_dotNetBasePath, "System.Collections.dll"));
        return this;
    }
}