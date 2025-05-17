using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maja.Compiler.Parser;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Serializes SyntaxNodes into text.
/// </summary>
public sealed class SyntaxWriter
{
    private readonly StringBuilder _writer = new();
    private readonly Stack<string> _tabs = new();
    private string _indent = String.Empty;

    public string Serialize(CompilationUnitSyntax node)
    {
        Write(node);
        return _writer.ToString();
    }

    private void Write(SyntaxNodeOrToken nodeOrToken)
    {
        if (nodeOrToken.Node is not null)
            Write(nodeOrToken.Node);
        else if (nodeOrToken.Token is not null)
            Write(nodeOrToken.Token);
    }

    private void Write(SyntaxNode node)
    {
        var txt = node switch
        {
            ExpressionLiteralSyntax el => el.Text,
            QualifiedNameSyntax => String.Empty,
            NameSyntax n => n.Text,
            _ => String.Empty
        };

        _writer.Append(_indent);
        _indent = String.Empty;

        _writer.Append(txt);
        WriteChildren(node);
    }

    private void Write(SyntaxToken token)
    {
        _writer.Append(token.Text);

        if (token is NewlineToken)
            _indent = String.Concat(_tabs.Select(t => t));

        if (token is IndentToken it)
        {
            if (it.TokenTypeId == MajaLexer.Indent)
                _tabs.Push(it.Text);
            else
                _tabs.Pop();
        }
    }

    private void WriteChildren(SyntaxNode node)
    {
        foreach (var child in node.Children)
        {
            Write(child);
        }
    }
}
