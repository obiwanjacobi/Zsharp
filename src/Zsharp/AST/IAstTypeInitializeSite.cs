using System.Collections.Generic;

namespace Zsharp.AST
{
    public interface IAstTypeInitializeSite
    {
        public IEnumerable<AstTypeFieldInitialization> Fields { get; }
        bool TryAddFieldInit(AstTypeFieldInitialization field);
        void AddFieldInit(AstTypeFieldInitialization field);
    }
}