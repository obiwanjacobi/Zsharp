using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Maja.Compiler.EmitCS.CSharp;

internal sealed class CSharpWriter
{
    private const int SpacesPerTab = 4;
    private const char OpenScopeChar = '{';
    private const char CloseScopeChar = '}';
    private const char SpaceChar = ' ';
    private const char SemiColonChar = ';';

    private readonly StringBuilder _writer = new();
    private int _indent;
    private string _tab;

    public CSharpWriter(int indent = 0)
    {
        _indent = indent;
        _tab = new String(SpaceChar, _indent * SpacesPerTab);
    }

    public void StartNamespace(string namespaceName)
    {
        _indent = 0;
        _writer.Append("namespace ").AppendLine(namespaceName);
        OpenScope();
    }

    public void StartType(Type type)
    {
        Tab()
            .Append(type.AccessModifiers.ToCode())
            .Append(SpaceChar)
            .Append(type.TypeModifiers.ToCode())
            .Append(SpaceChar)
            .Append(type.Keyword.ToCode())
            .Append(SpaceChar)
            .Append(type.Name)
            ;

        if (!String.IsNullOrEmpty(type.BaseTypeName))
            _writer.Append(" : ")
                .Append(type.BaseTypeName);
        Newline();
        OpenScope();
    }

    public void StartMethod(Method method)
    {
        Tab()
            .Append(method.AccessModifiers.ToCode())
            .Append(SpaceChar)
            .Append(method.MethodModifiers.ToCode())
            .Append(SpaceChar)
            .Append(method.TypeName)
            .Append(SpaceChar)
            .Append(method.Name)
            .Append('(');
    }

    public void WriteParameter(Parameter parameter, string? defaultValue = null)
    {
        _writer.Append(parameter.TypeName)
            .Append(SpaceChar)
            .Append(parameter.Name);

        if (!String.IsNullOrEmpty(defaultValue))
            _writer.Append("=").Append(defaultValue);
    }

    public void OpenMethodBody()
    {
        _writer.AppendLine(")");
        OpenScope();
    }

    public void StartField(Field field)
    {
        Tab().Append(field.AccessModifiers.ToCode())
            .Append(SpaceChar)
            .Append(field.FieldModifiers.ToCode())
            .Append(SpaceChar)
            .Append(field.TypeName)
            .Append(SpaceChar)
            .Append(field.Name);
    }

    public void WriteVariable(string? netType, string name)
    {
        Tab().Append(netType is null ? "var" : netType)
            .Append(SpaceChar)
            .Append(name);
    }

    public void StartAssignment(string name)
    {
        Tab().Append(name)
            .Append(" = ");
    }

    public void WriteReturn()
        => Tab().Append("return ");

    public void Write(string? text)
        => _writer.Append(text);

    public void CloseScope()
    {
        Dedent();
        Tab().Append(CloseScopeChar).AppendLine();
    }

    public void Using(string usingName)
        => Tab().Append("using ").Append(usingName).Append(SemiColonChar);

    public void WriteComma()
        => _writer.Append(", ");
    public void EndOfLine()
        => _writer.Append(SemiColonChar).AppendLine();
    public void Assignment()
        => _writer.Append(" = ");

    public override string ToString()
        => _writer.ToString();

    private StringBuilder Tab()
        => _writer.Append(_tab);
    private void Newline()
        => _writer.AppendLine();

    private void OpenScope()
    {
        Tab().Append(OpenScopeChar).AppendLine();
        Indent();
    }

    private void Indent()
    {
        _indent++;
        _tab = new String(SpaceChar, _indent * SpacesPerTab);
    }
    private void Dedent()
    {
        _indent--;
        _tab = new String(SpaceChar, _indent * SpacesPerTab);
    }
}
