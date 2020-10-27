using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace Zsharp.Emit
{
    /// <summary>
    /// Provides storage for 'local' variables.
    /// </summary>
    public interface ILocalStorageProvider
    {
        void CreateSlot(string name, TypeReference typeReference);
    }

    public abstract class Scope : IDisposable
    {
        protected Scope(EmitContext context)
        {
            Context = context;
        }

        protected EmitContext Context { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            var scope = Context.Scopes.Pop();
            if (!Object.ReferenceEquals(scope, this))
                throw new InvalidOperationException("Scope stack out of sync.");
        }
    }

    public sealed class FunctionScope : Scope, ILocalStorageProvider
    {
        private readonly MethodDefinition _methodDefinition;

        public FunctionScope(EmitContext emitContext, MethodDefinition methodDefinition)
            : base(emitContext)
        {
            _methodDefinition = methodDefinition;
            InstructionFactory = new InstructionFactory(methodDefinition.Body.GetILProcessor());
            CodeBuilder = new CodeBuilder();
        }

        public InstructionFactory InstructionFactory { get; }

        public CodeBuilder CodeBuilder { get; }

        public void CreateSlot(string name, TypeReference typeReference)
        {
            var varDef = new VariableDefinition(typeReference);
            _methodDefinition.Body.Variables.Add(varDef);
            CodeBuilder.AddVariable(name, varDef);
        }

        protected override void Dispose(bool disposing)
        {
            CodeBuilder.Apply(_methodDefinition.Body.GetILProcessor());
            _methodDefinition.Body.InitLocals = CodeBuilder.Variables.Any();
            base.Dispose(disposing);
        }
    }

    public sealed class ModuleScope : Scope, ILocalStorageProvider
    {
        public ModuleScope(EmitContext emitContext, ClassBuilder classBuilder)
            : base(emitContext)
        {
            ClassBuilder = classBuilder;
        }

        public ClassBuilder ClassBuilder { get; }

        public void CreateSlot(string name, TypeReference typeReference)
        {
            ClassBuilder.AddField(name, typeReference);
        }
    }
}
