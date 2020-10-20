using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Zsharp.AST;

namespace Zsharp.Emit
{
    public sealed partial class EmitContext
    {
        private readonly Stack<TypeDefinition> _moduleClasses = new Stack<TypeDefinition>();
        private readonly Stack<ILProcessor> _ilProcessors = new Stack<ILProcessor>();

        private EmitContext(AssemblyDefinition assembly)
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            Module = assembly.MainModule;
        }

        public static EmitContext Create(string assemblyName, Version? version = null)
        {

            return new EmitContext(
                AssemblyDefinition.CreateAssembly(
                    new AssemblyNameDefinition(assemblyName,
                        version ?? new Version(1, 0)),
                    assemblyName, ModuleKind.Dll)
            );
        }

        public IDisposable AddModuleClass(AstModule module)
        {
            if (Module.Types.Find(module.Name) != null)
                throw new ArgumentException($"ModuleClass for {module.Name} already exists.");

            var moduleClass = CreateModuleClass(module);
            _moduleClasses.Push(moduleClass);

            return new ModuleScope(this, moduleClass);
        }

        public AssemblyDefinition Assembly { get; private set; }

        public ModuleDefinition Module { get; private set; }

        public TypeDefinition ModuleClass => _moduleClasses.Peek();

        public ILProcessor ILProcessor => _ilProcessors.Peek();

        public IDisposable Add(MethodDefinition function)
        {
            ModuleClass.Methods.Add(function);
            var ilProcessor = function.Body.GetILProcessor();
            _ilProcessors.Push(ilProcessor);
            return new CodeScope(this, ilProcessor);
        }

        public TypeDefinition GetModuleClassFor(AstNode node)
        {
            var module = node.GetParentRecursive<AstModule>();
            if (module != null)
            {
                var modClass = Module.Types.Find(module.Name);

                if (modClass == null)
                {
                    throw new ArgumentException($"ModuleClass {module.Name} is not found for Node of {node.NodeType}.");
                }

                return modClass;
            }

            throw new InvalidOperationException($"Node {node.NodeType} has no Module Parent.");
        }

        private TypeDefinition CreateModuleClass(AstModule module)
        {
            var modClass = new TypeDefinition(Module.Name, module.Name, ToTypeAttributes(module));
            Module.Types.Add(modClass);
            return modClass;
        }

        private static TypeAttributes ToTypeAttributes(AstModule module)
        {
            var attrs = TypeAttributes.Class;
            attrs |= module.HasExports ? TypeAttributes.Public : TypeAttributes.NotPublic;
            return attrs;
        }


    }
}
