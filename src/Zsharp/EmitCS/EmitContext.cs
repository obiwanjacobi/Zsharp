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
        private readonly CsBuilder _builder;

        private EmitContext(string assemblyName, Version? version)
        {
            _assemblyName = assemblyName;
            _version = version ?? new Version(1, 0, 0, 0);
            _builder = new CsBuilder();
        }

        public static EmitContext Create(string assemblyName, Version? version = null)
        {
            return new EmitContext(assemblyName, version);
        }

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

        public CodeBuilder CodeBuilder => FunctionScope.CodeBuilder;

        internal CsBuilder CsBuilder => _builder;

        public void Imports(AstSymbolTable symbolTable)
        {
            var modules = symbolTable.FindEntries(AstSymbolKind.Module)
                .Select(s => s.Definition)
                .OfType<AstModuleExternal>();

            foreach (var module in modules)
            {
                CsBuilder.Using(module.Namespace);
            }
        }

        public IDisposable AddModule(AstModulePublic module)
        {
            if (Scopes.Count > 0)
                throw new InvalidOperationException(
                    "A Module must be added first before other AddFunction calls.");

            var classBuilder = ClassBuilder.Create(this, module);
            var modScope = new ModuleScope(this, classBuilder);
            Scopes.Push(modScope);

            var funScope = new FunctionScope(this, new CsBuilder(_builder.Indent));
            // static constructor
            funScope.CodeBuilder.StartMethod(AccessModifiers.None, MethodModifiers.Static, String.Empty, module.Identifier.Name);

            Scopes.Push(funScope);

            return new LinkedScopes(funScope, modScope);
        }

        public IDisposable AddFunction(AstFunctionDefinition function)
        {
            if (Scopes.Count == 0)
                throw new InvalidOperationException("A Module must be added first.");

            var retType = GetCodeTypeName(function.TypeReference);
            var access = function.Symbol!.SymbolLocality == AstSymbolLocality.Exported ? AccessModifiers.Public : AccessModifiers.Private;
            var modifiers = MethodModifiers.Static;
            var parameters = function.Parameters
                .Select(p => (name: p.Identifier!.CanonicalName, type: GetCodeTypeName(p.TypeReference)))
                .ToArray();
            var scope = new FunctionScope(this);
            scope.CodeBuilder.StartMethod(access, modifiers, retType, function.Identifier!.CanonicalName, parameters);

            Scopes.Push(scope);
            return scope;
        }

        public string GetCodeTypeName(AstType? astType)
        {
            if (astType != null)
            {
                if (astType is AstTypeReference typeRef)
                {
                    astType = typeRef.TypeDefinition;
                }

                if (astType is AstTypeDefinitionIntrinsic typeDef)
                {
                    if (typeDef.SystemType != null)
                        return typeDef.SystemType.FullName;

                    return "void";
                }

                return astType!.Identifier!.CanonicalName;
            }
            return "void";
        }
    }
}
