using Mono.Cecil;
using System;
using System.Collections.Generic;
using Zsharp.AST;

namespace Zsharp.Emit
{
    public sealed partial class EmitContext
    {
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

        public AssemblyDefinition Assembly { get; private set; }

        public ModuleDefinition Module { get; private set; }

        internal Stack<Scope> Scopes { get; } = new Stack<Scope>();

        internal ModuleScope ModuleScope
        {
            get
            {
                if (Scopes.Peek() is ModuleScope moduleScope)
                {
                    return moduleScope;
                }
                throw new InvalidOperationException($"No current active Module Class Scope.");
            }
        }

        internal FunctionScope FunctionScope
        {
            get
            {
                if (Scopes.Peek() is FunctionScope functionScope)
                {
                    return functionScope;
                }
                throw new InvalidOperationException($"No current active Function Scope.");
            }
        }

        public ClassBuilder ModuleClass => ModuleScope.ClassBuilder;

        public InstructionFactory InstructionFactory => FunctionScope.InstructionFactory;

        public CodeBuilder CodeBuilder => FunctionScope.CodeBuilder;

        public IDisposable AddModule(AstModulePublic module)
        {
            if (Module.Types.Find(module.Name) != null)
                throw new ArgumentException($"ModuleClass for {module.Name} already exists.");

            var classBuilder = ClassBuilder.Create(this, module);
            var scope = new ModuleScope(this, classBuilder);
            Scopes.Push(scope);
            return scope;
        }

        public IDisposable AddFunction(AstFunction function)
        {
            var classBuilder = ModuleClass ??
                throw new InvalidOperationException("There is no module class to add a function to.");

            var func = classBuilder.AddFunction(function);
            var scope = new FunctionScope(this, func);
            Scopes.Push(scope);
            return scope;
        }

        public bool HasVariable(string name)
        {
            var scope = Scopes.Peek();

            if (scope is ModuleScope moduleScope)
            {
                return moduleScope.ClassBuilder.HasField(name);
            }
            if (scope is FunctionScope functionScope)
            {
                return functionScope.CodeBuilder.HasVariable(name);
            }
            return false;
        }

        public void AddVariable(AstVariableDefinition variable)
        {
            var provider = (ILocalStorageProvider)Scopes.Peek();

            provider.CreateSlot(variable.Identifier.Name, ToTypeReference(variable.TypeReference));
        }

        internal TypeReference ToTypeReference(AstTypeReference typeReference)
        {
            if (typeReference == null)
            {
                // TODO: Replace with Zsharp.Void
                return Module.TypeSystem.Void;
            }

            if (typeReference.TypeDefinition.IsIntrinsic)
            {
                // Map intrinsic data types to .NET data types
                var type = ((AstTypeIntrinsic)typeReference.TypeDefinition).SystemType;
                return Module.ImportReference(type);
            }

            // TODO: what does this do?
            IMetadataScope? metadataScope = null;

            var typeRef = new TypeReference("Todo",
                typeReference.TypeDefinition.Identifier.Name,
                Module,
                metadataScope, true);

            return typeRef;
        }
    }
}
