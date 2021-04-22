using Mono.Cecil;
using System;

namespace Zsharp.EmitIL
{
    public class DotNet
    {
        private readonly AssemblyDefinition _runtimeAssembly;

        public DotNet()
        {
            // TODO: any way to get this?
            var runtimeFile = @"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\5.0.0\ref\net5.0\System.Runtime.dll";
            _runtimeAssembly = AssemblyDefinition.ReadAssembly(runtimeFile);
        }

        public TypeDefinition ObjectType
            => _runtimeAssembly.MainModule.Types.Find("Object")
            ?? throw new InvalidOperationException("Object type was not found!");

        public TypeDefinition StringType
            => _runtimeAssembly.MainModule.Types.Find("String")
            ?? throw new InvalidOperationException("String type was not found!");

        public TypeDefinition ValueType
            => _runtimeAssembly.MainModule.Types.Find("ValueType")
            ?? throw new InvalidOperationException("ValueType type was not found!");

        public TypeDefinition EnumType
            => _runtimeAssembly.MainModule.Types.Find("Enum")
            ?? throw new InvalidOperationException("Enum type was not found!");
    }
}
