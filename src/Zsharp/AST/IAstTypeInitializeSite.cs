using System.Collections.Generic;

namespace Zsharp.AST
{
    public interface IAstTypeInitializeSite
    {
        IEnumerable<AstTypeFieldInitialization> Fields { get; }
        bool TryAddFieldInit(AstTypeFieldInitialization? field);
    }

    public static class AstTypeInitializeSiteExtensions
    {
        public static void AddFieldInit(this IAstTypeInitializeSite typeInitializeSite, AstTypeFieldInitialization field)
        {
            if (!typeInitializeSite.TryAddFieldInit(field))
                throw new InternalErrorException(
                    "TypeField is already set or null.");
        }
    }
}