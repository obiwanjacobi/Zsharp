namespace Maja.Compiler.EmitCS.CSharp;

internal enum AccessModifiers
{
    None,
    Private,
    Internal,
    Protected,
    Public
}

internal enum TypeKeyword
{
    None,
    Class,
    Record,
    Struct,
    ReadonlyStruct,
    RecordStruct,
    ReadonlyRecordStruct,
}

internal enum TypeModifiers
{
    None,
    Static,
}

internal enum MethodModifiers
{
    None,
    Virtual,
    Override,
    Static
}

internal enum FieldModifiers
{
    None,
    ReadOnly,
    Static
}
