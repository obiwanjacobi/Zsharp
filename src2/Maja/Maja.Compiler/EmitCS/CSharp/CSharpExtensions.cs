using System;

namespace Maja.Compiler.EmitCS.CSharp;

internal static class CSharpExtensions
{
    public static string ToCode(this AccessModifiers modifier)
        => modifier == AccessModifiers.None ? String.Empty : modifier.ToString().ToLowerInvariant();

    public static string ToCode(this TypeKeyword keyword)
        => keyword switch
        {
            TypeKeyword.Class => "class",
            TypeKeyword.Struct => "struct",
            TypeKeyword.Record => "record class",
            TypeKeyword.RecordStruct => "record struct",
            TypeKeyword.ReadonlyStruct => "readonly struct",
            TypeKeyword.ReadonlyRecordStruct => "readonly record struct",
            _ => String.Empty
        };

    public static string ToCode(this TypeModifiers modifier)
        => modifier switch
        {
            TypeModifiers.Static => "static",
            _ => String.Empty
        };

    public static string ToCode(this FieldModifiers modifier)
        => modifier switch
        {
            FieldModifiers.Static => "static",
            FieldModifiers.ReadOnly => "readonly",
            _ => String.Empty
        };

    public static string ToCode(this MethodModifiers modifier)
        => modifier switch
        {
            MethodModifiers.Override => "override",
            MethodModifiers.Static => "static",
            MethodModifiers.Virtual => "virtual",
            _ => String.Empty
        };
}
