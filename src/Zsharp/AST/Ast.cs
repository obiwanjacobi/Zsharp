using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Zsharp.AST
{
    internal static class Ast
    {
        public static bool SafeSet<T>(ref T? storage, T? value)
            where T : class
        {
            if (storage is null &&
                value is not null)
            {
                storage = value;
                return true;
            }
            return false;
        }

        public static bool SafeSetParent<P, T>(this P? parent, ref T? storage, T? value)
            where T : AstNode
            where P : AstNode
        {
            if (SafeSet<T>(ref storage, value) &&
                storage!.TrySetParent(parent))
            {
                return true;
            }
            return false;
        }

        [Conditional("DEBUG")]
        public static void Guard<T>(object? instance, [CallerMemberName] string caller = "")
            where T : class
        {
            if (instance is null)
                throw new InternalErrorException(
                    $"{caller}: Object not of the expected type ({typeof(T).Name}) because it was null.");
            if (!(instance is T))
                throw new InternalErrorException($"{caller}: Object of type '{instance.GetType().Name}' is not of the expected type: {typeof(T).Name}");
        }

        [Conditional("DEBUG")]
        public static void Guard(bool trueIsValid, string message, [CallerMemberName] string caller = "")
        {
            if (!trueIsValid)
                throw new InternalErrorException($"{caller}: {message}");
        }

        [Conditional("DEBUG")]
        public static void Guard<T>(T? instance, string message, [CallerMemberName] string caller = "")
            where T : class
        {
            if (instance is null)
                throw new InternalErrorException($"{caller}: {typeof(T).Name} is null: {message}");
        }
    }
}
