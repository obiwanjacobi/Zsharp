using System;
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

    public CSharpWriter(int indent = 0)
    {
        _indent = indent;
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

    public string Parameter(Parameter parameter)
        => $"{parameter.TypeName} {parameter.Name}";

    public void OpenMethodBody()
    {
        _writer.AppendLine(")");
        OpenScope();
    }

    public void WriteVariable(string? netType, string name)
    {
        Tab().Append(netType is null ? "var" : netType)
            .Append(SpaceChar)
            .Append(name);
    }

    public void Write(string? text)
        => _writer.Append(text);

    public void CloseScope()
    {
        _indent--;
        Tab().Append(CloseScopeChar).AppendLine();
    }

    public void Using(string usingName)
        => Tab().Append("using ").Append(usingName).Append(SemiColonChar);

    public void Semicolon()
        => _writer.Append(SemiColonChar).AppendLine();
    public void Assignment()
        => _writer.Append(" = ");

    public override string ToString()
        => _writer.ToString();

    private StringBuilder Tab()
        => _writer.Append(new String(SpaceChar, _indent * SpacesPerTab));
    private StringBuilder Newline()
        => _writer.AppendLine();

    private void OpenScope()
    {
        Tab().Append(OpenScopeChar).AppendLine();
        _indent++;
    }
}
