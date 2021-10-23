using System;
using System.Collections.Generic;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.EmitCS
{
    public sealed class EmitContext
    {
        private readonly string _assemblyName;
        private readonly Version _version;
        private readonly CSharp.Namespace _namespace;

        public EmitContext(string assemblyName, Version? version)
        {
            _assemblyName = assemblyName;
            _version = version ?? new Version(1, 0, 0, 0);
            _namespace = new CSharp.Namespace(assemblyName);
        }

        internal CSharp.Namespace Namespace => _namespace;

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

        internal ClassBuilder ModuleClass => ModuleScope.ClassBuilder;

        private CodeBuilder? _codeBuilder;
        internal CodeBuilder CodeBuilder
            => _codeBuilder ?? FunctionScope.CodeBuilder;

        // pass null to reset
        internal IDisposable? SetBuilder(CsBuilder? builder)
        {
            if (builder is null)
            {
                _codeBuilder = null;
                return null;
            }

            _codeBuilder = new CodeBuilder(builder);
            return new BuilderScope(this);
        }

        internal void Imports(AstSymbolTable symbolTable)
        {
            var modules = symbolTable.FindSymbols(AstSymbolKind.Module)
                .Select(s => s.Definition)
                .OfType<AstModuleExternal>();

            foreach (var module in modules)
            {
                Namespace.AddUsing(module.ExternalName.Namespace);
            }
        }

        public IDisposable AddModule(AstModuleImpl module)
        {
            if (Scopes.Count > 0)
                throw new InvalidOperationException(
                    "A Module must be added first before other AddFunction calls.");

            var classBuilder = ClassBuilder.Create(this, module);
            var modScope = new ModuleScope(this, classBuilder);
            Scopes.Push(modScope);

            // static constructor
            var method = new CSharp.Method(module.Identifier!.CanonicalName, String.Empty)
            {
                AccessModifiers = AccessModifiers.None,
                MethodModifiers = MethodModifiers.Static,
            };
            classBuilder.ModuleClass.AddMethod(method);

            var funScope = new FunctionScope(this, method.GetBody(8));
            Scopes.Push(funScope);
            return new LinkedScopes(funScope, modScope);
        }

        public IDisposable AddFunction(AstFunctionDefinition function)
        {
            if (Scopes.Count == 0)
                throw new InvalidOperationException("A Module must be added first.");

            var method = ModuleClass.AddFunction(function);

            var scope = new FunctionScope(this, method.GetBody(8));
            Scopes.Push(scope);
            return scope;
        }
    }
}
