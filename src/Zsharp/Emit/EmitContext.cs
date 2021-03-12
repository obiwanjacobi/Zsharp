using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.Emit
{
    public sealed partial class EmitContext
    {
        public static readonly DotNet DotNet = new();

        private EmitContext(AssemblyDefinition assembly)
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            Module = assembly.MainModule;
        }

        public static EmitContext Create(string assemblyName, Version? version = null)
        {
            var assembly = AssemblyDefinition.CreateAssembly(
                    new AssemblyNameDefinition(assemblyName,
                        version ?? new Version(1, 0)),
                    assemblyName, ModuleKind.Dll);

            // add [assembly: TargetFramework(".NETCoreApp,Version=v5.0")]
            var attrCtor = assembly.MainModule.ImportReference(
                    typeof(System.Runtime.Versioning.TargetFrameworkAttribute)
                        .GetConstructor(new[] { typeof(string) })
                );

            var stringType = assembly.MainModule.ImportReference(DotNet.StringType);
            var targetFxAttr = new CustomAttribute(attrCtor);
            targetFxAttr.ConstructorArguments.Add(
                new CustomAttributeArgument(stringType, ".NETCoreApp,Version=v5.0"));
            assembly.CustomAttributes.Add(targetFxAttr);

            return new EmitContext(assembly);
        }

        public AssemblyDefinition Assembly { get; private set; }

        public ModuleDefinition Module { get; private set; }

        internal Stack<Scope> Scopes { get; } = new Stack<Scope>();

        internal ModuleScope ModuleScope
        {
            get
            {
                var scopes = Scopes.ToArray();

                if (scopes.Length > 0 &&
                    scopes[^1] is ModuleScope moduleScope)
                {
                    return moduleScope;
                }
                throw new InvalidOperationException($"No current active Module Class Scope.");
            }
        }

        internal FunctionScope FunctionScope => (FunctionScope)Scopes.Peek();

        public ClassBuilder ModuleClass => ModuleScope.ClassBuilder;

        public InstructionFactory InstructionFactory => FunctionScope.InstructionFactory;

        public CodeBuilder CodeBuilder => FunctionScope.CodeBuilder;

        public IDisposable AddModule(AstModulePublic module)
        {
            if (Module.Types.Find(module.Name) != null)
                throw new ArgumentException($"ModuleClass for {module.Name} already exists.");
            if (Scopes.Count > 0)
                throw new InvalidOperationException("A Module must be added first.");

            var classBuilder = ClassBuilder.Create(this, module);
            var modScope = new ModuleScope(this, classBuilder);
            Scopes.Push(modScope);

            var funScope = new FunctionScope(this, classBuilder.ModuleInitializer);
            Scopes.Push(funScope);

            return new LinkedScopes(funScope, modScope);
        }

        public IDisposable AddFunction(AstFunctionDefinition function)
        {
            if (Scopes.Count == 0)
                throw new InvalidOperationException("A Module must be added first.");
            var classBuilder = ModuleClass ??
                throw new InvalidOperationException("There is no module class to add a function to.");

            var func = classBuilder.AddFunction(function);
            var scope = new FunctionScope(this, func);
            Scopes.Push(scope);
            return scope;
        }

        public MethodReference GetFunctionReference(AstFunctionDefinition function)
        {
            if (function is AstFunctionExternal externalFunction)
            {
                return Module.ImportReference(externalFunction.MethodDefinition);
            }

            return Module.Types
                .SelectMany(t => t.Methods)
                .Single(m => m.Name == function.Identifier?.CanonicalName);
        }

        public TypeDefinition GetTypeDefinition(AstTypeReference typeReference)
        {
            //if (typeReference is AstTypeReferenceExternal externalType)
            //{
            //    return Module.ImportReference(externalType.??);
            //}

            var nameParts = typeReference.Symbol.FullName.Split('.');
            if (nameParts.Length == 0)
                throw new ArgumentException(
                    "[Emit] Type (symbol) name was empty.", nameof(typeReference));

            var types = Module.Types;
            TypeDefinition typeDef = null;
            foreach (var name in nameParts)
            {
                typeDef = types.Find(name);
                Ast.Guard(typeDef, $"[Emit] Type not found {typeReference.Symbol.FullName} ({name}).");

                types = typeDef!.NestedTypes;
            }
            return typeDef;
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
                var type = ((AstTypeDefinitionIntrinsic)typeReference.TypeDefinition).SystemType;
                if (type == null)
                {
                    return Module.TypeSystem.Void;
                }
                return Module.ImportReference(type);
            }

            // TODO: what does this do?
            IMetadataScope? metadataScope = null;

            var typeRef = new TypeReference("Todo",
                typeReference.TypeDefinition.Identifier.CanonicalName,
                Module,
                metadataScope, true);

            return typeRef;
        }
    }
}
