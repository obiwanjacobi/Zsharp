using System;

namespace Zsharp.EmitIL
{
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
}
