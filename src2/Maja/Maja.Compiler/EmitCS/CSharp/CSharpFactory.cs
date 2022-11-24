using System;
using Maja.Compiler.IR;

namespace Maja.Compiler.EmitCS.CSharp;

/// <summary>
/// Creates C# structure object for Ir nodes
/// </summary>
internal static class CSharpFactory
{
    public static Namespace CreateNamespace(IrModule module)
        => new(module.Symbol.Name.Namespace.Value);

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

    public static Field CreateField(string name, string typeName)
        => new Field(name, typeName)
        {
            AccessModifiers = AccessModifiers.Private,
            FieldModifiers = FieldModifiers.Static
        };

    

}
