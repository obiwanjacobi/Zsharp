using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maja.Compiler.EmitCS.CSharp;

internal sealed class CSharpWriter
{
    private const int SpacesPerTab = 4;
    internal const char OpenScopeChar = '{';
    internal const char CloseScopeChar = '}';
    internal const char SpaceChar = ' ';
    internal const char SemiColonChar = ';';

    private readonly StringBuilder _writer = new();
    private int _indent;
    private string _tab;

    public CSharpWriter(int indent = 0)
    {
        _indent = indent;
        _tab = new String(SpaceChar, _indent * SpacesPerTab);
    }

    public CSharpWriter OpenScope()
    {
        Tab()
            .Append(OpenScopeChar)
            .AppendLine();
        Indent();
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

    public CSharpWriter StartVariable(string? netType, string name)
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
        _writer.Append(symbol.Name.FullOriginalName);
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
    public CSharpWriter Write(char c)
    {
        _writer.Append(c);
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

    internal void Render(CSharpWriter target)
    {
        var lines = _writer.ToString().Split(Environment.NewLine);
        foreach (var line in lines)
        {
            target.Tab().AppendLine(line);
        }
    }

    public StringBuilder Tab()
        => _writer.Append(_tab);

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
