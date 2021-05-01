using System;

namespace Zsharp.EmitCS
{
    internal sealed class BuilderScope : IDisposable
    {
        private readonly EmitContext _context;
        public BuilderScope(EmitContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.SetBuilder(null);
        }
    }
}
