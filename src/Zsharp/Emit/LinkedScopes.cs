using System;

namespace Zsharp.Emit
{
    public sealed class LinkedScopes : IDisposable
    {
        private readonly Scope[] _scopes;

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