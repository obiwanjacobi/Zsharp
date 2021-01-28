using System.Collections.Generic;

namespace Zsharp.AST
{
    public interface IAstTypeInitializeSite
    {
        IEnumerable<AstTypeFieldInitialization> Fields { get; }
        bool TryAddFieldInit(AstTypeFieldInitialization field);
        void AddFieldInit(AstTypeFieldInitialization field);
    }
}