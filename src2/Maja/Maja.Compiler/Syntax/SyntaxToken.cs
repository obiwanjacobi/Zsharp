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

    public static SyntaxToken? TryNew(string text, SyntaxLocation location)
    {
        // TODO: create based on token.type (id) from lexer
        // How to deal with Sp+ ??

        if (NewlineToken.IsValid(text))
            return new NewlineToken(text)
            {
                Location = location
            };

        if (WhitespaceToken.IsValid(text))
            return new WhitespaceToken(text)
            {
                Location = location
            };

        if (PunctuationToken.IsValid(text))
            return new PunctuationToken(text)
            {
                Location = location
            };

        if (RoundBracketToken.IsValid(text))
            return new RoundBracketToken(text)
            {
                Location = location
            };

        if (SquareBracketToken.IsValid(text))
            return new SquareBracketToken(text)
            {
                Location = location
            };

        if (CurlyBracketToken.IsValid(text))
            return new CurlyBracketToken(text)
            {
                Location = location
            };

        if (AngleBracketToken.IsValid(text))
            return new AngleBracketToken(text)
            {
                Location = location
            };

        if (KeywordToken.IsValid(text))
            return new KeywordToken(text)
            {
                Location = location
            };

        return null;
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

    public WhitespaceToken Append(WhitespaceToken token)
        => new(Text + token.Text)
        {
            Location = Location.Append(token.Location)
        };

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

public record RoundBracketToken : SyntaxToken
{
    public RoundBracketToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(string text)
        => text.Length == 1 &&
        text[0] is '(' or ')';
}

public record SquareBracketToken : SyntaxToken
{
    public SquareBracketToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(string text)
        => text.Length == 1 &&
        text[0] is '[' or ']';
}

public record CurlyBracketToken : SyntaxToken
{
    public CurlyBracketToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(string text)
        => text.Length == 1 &&
        text[0] is '{' or '}';
}

public record AngleBracketToken : SyntaxToken
{
    public AngleBracketToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(string text)
        => text.Length == 1 &&
        text[0] is '<' or '>';
}

