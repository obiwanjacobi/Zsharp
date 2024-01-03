using System;
using System.Collections.Generic;
using System.Linq;
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

    public CSharpWriter StartNamespace(string namespaceName)
    {
        _indent = 0;
        _writer.Append("namespace ").AppendLine(namespaceName);
        OpenScope();
        return this;
    }

    public CSharpWriter StartType(Type type)
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
        return this;
    }

    public CSharpWriter StartMethod(Method method)
    {
        Tab()
            .Append(method.AccessModifiers.ToCode())
            .Append(SpaceChar)
            .Append(method.MethodModifiers.ToCode())
            .Append(SpaceChar)
            .Append(method.TypeName)
            .Append(SpaceChar)
            .Append(method.Name);

        return this;
    }

    public CSharpWriter WriteTypeParameter(TypeParameter parameter)
    {
        _writer.Append(parameter.TypeName);
        return this;
    }

    public CSharpWriter WriteParameter(Parameter parameter, string? defaultValue = null)
    {
        _writer.Append(parameter.TypeName)
            .Append(SpaceChar)
            .Append(parameter.Name);

        if (!String.IsNullOrEmpty(defaultValue))
            _writer.Append("=").Append(defaultValue);

        return this;
    }

    public CSharpWriter OpenMethodBody()
    {
        _writer.AppendLine();
        OpenScope();
        return this;
    }

    public CSharpWriter StartField(Field field)
    {
        Tab()
            .Append(field.AccessModifiers.ToCode())
            .Append(SpaceChar)
            .Append(field.FieldModifiers.ToCode())
            .Append(SpaceChar)
            .Append(field.TypeName)
            .Append(SpaceChar)
            .Append(field.Name);

        return this;
    }

    public CSharpWriter StartEnum(Enum @enum)
    {
        Tab()
            .Append(@enum.AccessModifiers.ToCode())
            .Append(SpaceChar)
            .Append("enum")
            .Append(SpaceChar)
            .Append(@enum.Name);
        
        if (@enum.BaseTypeName is not null)
        {
            _writer.Append(" : ")
                .Append(@enum.BaseTypeName);
        }
        _writer.AppendLine();
        OpenScope();

        return this;
    }

    public CSharpWriter WriteVariable(string? netType, string name)
    {
        Tab()
            .Append(netType is null ? "var" : netType)
            .Append(SpaceChar)
            .Append(name);

        return this;
    }

    public CSharpWriter WriteInitializer<T>(IEnumerable<T> initializers, Func<T, string> nameFn, Func<T, string> valueFn)
    {
        if (initializers.Any())
        {
            OpenScope();
            foreach (var init in initializers)
            {
                Tab()
                    .Append(nameFn(init))
                    .Append(" = ")
                    .Append(valueFn(init))
                    .Append(",")
                    .AppendLine();
            }
            CloseScope();
        }

        return this;
    }

    public CSharpWriter WriteSymbol(Symbol.Symbol symbol)
    {
        _writer.Append(symbol.Name.Value);
        return this;
    }

    public CSharpWriter StartAssignment(string name)
    {
        Tab()
            .Append(name)
            .Append(" = ");

        return this;
    }

    public CSharpWriter WriteReturn()
    {
        Tab()
            .Append("return ");

        return this;
    }

    public CSharpWriter Write(string? text)
    {
        _writer.Append(text);
        return this;
    }

    /// <summary>
    /// <'}\n'
    /// </summary>
    public CSharpWriter CloseScope()
    {
        Dedent();
        Tab()
            .Append(CloseScopeChar)
            .AppendLine();

        return this;
    }

    /// <summary>
    /// 'using <name>;'
    /// </summary>
    public CSharpWriter Using(string usingName)
    {
        Tab()
            .Append("using ")
            .Append(usingName)
            .Append(SemiColonChar);

        return this;
    }

    /// <summary>
    /// ', '
    /// </summary>
    public CSharpWriter WriteComma()
    {
        _writer.Append(", ");
        return this;
    }

    /// <summary>
    /// ';\n'
    /// </summary>
    public CSharpWriter EndOfLine()
    {
        _writer
            .Append(SemiColonChar)
            .AppendLine();

        return this;
    }

    /// <summary>
    /// '\n'
    /// </summary>
    public CSharpWriter Newline()
    {
        _writer.AppendLine();
        return this;
    }

    /// <summary>
    /// ' = '
    /// </summary>
    public CSharpWriter Assignment()
    {
        _writer.Append(" = ");
        return this;
    }

    public override string ToString()
        => _writer.ToString();

    public StringBuilder Tab()
        => _writer.Append(_tab);

    private void OpenScope()
    {
        Tab()
            .Append(OpenScopeChar)
            .AppendLine();
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
