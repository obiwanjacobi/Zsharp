﻿using System;
using Maja.Compiler.IR;

namespace Maja.Compiler.EmitCS.CSharp;

/// <summary>
/// Creates C# structure object for Ir nodes
/// </summary>
internal static class CSharpFactory
{
    public static Namespace CreateNamespace(IrModule module)
        => new(!String.IsNullOrEmpty(module.Symbol.Name.Namespace.Value)
            ? module.Symbol.Name.Namespace.Value
            : module.Symbol.Name.Value);

    public static Type CreateModuleClass(IrModule module)
        => new(module.Symbol.Name.Value, TypeKeyword.Class)
        {
            AccessModifiers = AccessModifiers.Internal,
            TypeModifiers = TypeModifiers.Static
        };

    public static Method CreateModuleInitializer(string typeName)
        => new(typeName, String.Empty)
        {
            AccessModifiers = AccessModifiers.None,
            MethodModifiers = MethodModifiers.Static
        };

    public static Method CreateMethod(string name, string returnType)
        => new(name, returnType)
        {
            AccessModifiers = AccessModifiers.Private,
            MethodModifiers = MethodModifiers.Static
        };

    public static Enum CreateEnum(string name, string? baseTypeName = null)
        => new(name)
        {
            AccessModifiers = AccessModifiers.Private,
            BaseTypeName = baseTypeName
        };

    public static EnumOption CreateEnumOption(string name, string? value = null)
        => new(name, value);

    public static Type CreateType(string name, string? baseTypeName = null)
        => new(name, TypeKeyword.Record)
        {
            AccessModifiers = AccessModifiers.Private,
            BaseTypeName = baseTypeName
        };

    public static TypeParameter CreateTypeParameter(string typeName)
        => new(typeName);

    public static Parameter CreateParameter(string name, string typeName)
        => new(name, typeName);

    public static Field CreateField(string name, string typeName)
        => new(name, typeName)
        {
            AccessModifiers = AccessModifiers.Private,
            FieldModifiers = FieldModifiers.Static
        };
}
