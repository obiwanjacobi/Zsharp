using System.Collections.Generic;

namespace Zsharp.AST
{
    public interface IAstTypeFields<T>
        where T : AstTypeField
    {
        IEnumerable<T> Fields { get; }
        bool TryAddField(T field);
    }

    public static class AstTypeFieldsExtensions
    {
        public static void AddField<T>(this IAstTypeFields<T> typeFields, T field)
            where T : AstTypeField
        {
            if (!typeFields.TryAddField(field))
                throw new InternalErrorException(
                    "Type Field was already added or null.");
        }
    }
}
