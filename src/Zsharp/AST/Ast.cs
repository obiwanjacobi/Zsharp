﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Zsharp.AST
{
    public static class Ast
    {
        public static List<AstNode> ToNodeList(this AstNode node)
        {
            return new List<AstNode>
            {
                node
            };
        }

        public static List<T> ToList<T>(this T instance)
        {
            return new List<T>
            {
                instance
            };
        }

        public static bool SafeSet<T>(ref T? storage, T? value)
            where T : class
        {
            if (storage == null &&
                value != null)
            {
                storage = value;
                return true;
            }
            return false;
        }

        public static bool SafeSetParent<P, T>(this P? parent, ref T? storage, T value)
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
        public static void Guard<T>(object? instance)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException(typeof(T).Name,
                    $"Object not of the expected type ({typeof(T).Name}) because it was null.");
            if (!(instance is T))
                throw new ArgumentException($"Object not of the expected type: {typeof(T).Name}");
        }

        [Conditional("DEBUG")]
        public static void Guard(bool trueIsValid, string message)
        {
            if (!trueIsValid)
                throw new ArgumentException(message);
        }

        [Conditional("DEBUG")]
        public static void Guard<T>(T? instance, string message)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException(typeof(T).Name, message);
        }
    }
}
