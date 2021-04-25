using System;

namespace Zsharp.EmitCS
{
    public sealed class LinkedScopes : IDisposable
    {
        private readonly Scope[] _scopes;

        // add scopes in reverse stack order
        public LinkedScopes(params Scope[] scopes)
        {
            _scopes = scopes;
        }

        public void Dispose()
        {
            foreach (var scope in _scopes)
            {
                scope.Dispose();
            }
        }
    }
}