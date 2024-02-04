using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maja.Compiler.Parser;
using static System.Net.Mime.MediaTypeNames;

namespace Maja.Compiler.Syntax;

public sealed class SyntaxWriter
{
    private readonly StringBuilder _writer = new();

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
            QualifiedNameSyntax qn => String.Empty,
            NameSyntax n => n.Text,
            _ => String.Empty
        };

        _writer.Append(txt);
        WriteChildren(node);
    }

    private void WriteChildren(SyntaxNode node)
    {
        foreach (var child in node.Children)
        {
            Write(child);
        }
    }

    private readonly Stack<string> _tabs = new();

    private void Write(SyntaxToken token)
    {
        _writer.Append(token.Text);

        if (token is NewlineToken)
            _writer.Append(
                String.Join(String.Empty, _tabs.Select(t => t)));
        
        if (token is IndentToken it)
        {
            if (it.TokenTypeId == MajaLexer.Indent)
                _tabs.Push(it.Text);
            else
                _tabs.Pop();
        }
    }
}
