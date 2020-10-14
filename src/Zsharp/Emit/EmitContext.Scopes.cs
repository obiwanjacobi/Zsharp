using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Zsharp.Emit
{
    partial class EmitContext
    {
        internal sealed class CodeScope : IDisposable
        {
            private readonly EmitContext _emitContext;
            private readonly ILProcessor _ilProcessor;

            public CodeScope(EmitContext emitContext, ILProcessor ilProcessor)
            {
                _emitContext = emitContext;
                _ilProcessor = ilProcessor;
            }

            public void Dispose()
            {
                var top = _emitContext._ilProcessors.Pop();
                if (!Object.ReferenceEquals(top, _ilProcessor))
                {
                    throw new Exception("Stack of ILProcessors is out of sync.");
                }
            }
        }

        internal sealed class ModuleScope : IDisposable
        {
            private readonly EmitContext _emitContext;
            private readonly TypeDefinition _moduleClass;

            public ModuleScope(EmitContext emitContext, TypeDefinition moduleClass)
            {
                _emitContext = emitContext;
                _moduleClass = moduleClass;
            }

            public void Dispose()
            {
                var top = _emitContext._moduleClasses.Pop();
                if (!Object.ReferenceEquals(top, _moduleClass))
                {
                    throw new Exception("Stack of ModuleClasses is out of sync.");
                }
            }
        }
    }
}
