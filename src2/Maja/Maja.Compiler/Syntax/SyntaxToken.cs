using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Maja.Compiler.Syntax;

public abstract record SyntaxToken(string Text)
{
    private SyntaxNode? _parent;
    public SyntaxNode? Parent
    {
        get { return _parent!; }
        internal set
        {
            Debug.Assert(value is not null, "Cannot clear a SyntaxToken.Parent with null.");
            _parent = value;
        }
    }

    public SyntaxLocation Location { get; init; }

    public static SyntaxToken New(string text, SyntaxLocation location)
    {
        if (WhitespaceToken.IsValid(text))
            return new WhitespaceToken(text)
            {
                Location = location
            };

        if (NewlineToken.IsValid(text))
            return new NewlineToken(text)
            {
                Location = location
            };

        if (PunctuationToken.IsValid(text))
            return new PunctuationToken(text)
            {
                Location = location
            };

        if (GroupToken.IsValid(text))
            return new GroupToken(text)
            {
                Location = location
            };

        if (KeywordToken.IsValid(text))
            return new KeywordToken(text)
            {
                Location = location
            };

        throw new InvalidOperationException("No suitable SyntaxToken.");
    }
}

public record KeywordToken : SyntaxToken
{
    public KeywordToken(string Text)
        : base(Text)
    { }

    public static IReadOnlyList<string> Keywords =
        new ReadOnlyCollection<string>(new[]
        {
            "mod",
            "pub",
            "use",
            "self",
            "ret",
            "brk",
            "cnt",
            "loop",
            "if",
            "else",
            "elif",
            "in",
            "not",
            "and",
            "or"
        });

    public static bool IsValid(string text)
        => Keywords.Contains(text);
}

public record WhitespaceToken : SyntaxToken
{
    public WhitespaceToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(string text)
        => text is not null && String.IsNullOrWhiteSpace(text);
}

public record NewlineToken : SyntaxToken
{
    public NewlineToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(string text)
        => text is "\r" or "\n" or "\r\n";
}

// .,;:
public record PunctuationToken : SyntaxToken
{
    public PunctuationToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(string text)
        => text.Length == 1 &&
        text[0] is '.' or ',' or ';' or ':';
}

// () {} [] <>
public record GroupToken : SyntaxToken
{
    public GroupToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(string text)
        => text.Length == 1 &&
        text[0] is '(' or ')'
        or '[' or ']' 
        or '{' or '}'
        or '<' or '>';
}
