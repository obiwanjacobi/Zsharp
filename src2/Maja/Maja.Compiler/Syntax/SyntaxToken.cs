using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Maja.Compiler.Parser;

namespace Maja.Compiler.Syntax;

/// <summary>
/// A common type for all syntax tokens.
/// </summary>
/// <param name="Text">The textual representation of the token.</param>
public abstract record SyntaxToken(string Text)
{
    private SyntaxNode? _parent;
    /// <summary>
    /// The parent syntax node of this token.
    /// </summary>
    public SyntaxNode Parent
    {
        get { return _parent!; }
        internal set
        {
            Debug.Assert(value is not null, "Cannot clear a SyntaxToken.Parent with null.");
            Debug.Assert(_parent is null, "SyntaxToken.Parent is already set.");
            _parent = value;
        }
    }

    /// <summary>
    /// The source location of this syntax token.
    /// </summary>
    public SyntaxLocation Location { get; init; }

    /// <summary>
    /// Creates a new token based on the tokenId.
    /// </summary>
    /// <param name="tokenId">The (Lexer) token id.</param>
    /// <param name="text">The textual representation of the token.</param>
    /// <param name="location">The source location of the token.</param>
    /// <returns>Returns null if the token could not be created.</returns>
    public static SyntaxToken? TryNew(int tokenId, string text, SyntaxLocation location)
    {
        if (NewlineToken.IsValid(tokenId))
            return new NewlineToken(text)
            {
                Location = location
            };

        if (WhitespaceToken.IsValid(tokenId))
            return new WhitespaceToken(text)
            {
                Location = location
            };

        if (PunctuationToken.IsValid(tokenId))
            return new PunctuationToken(text)
            {
                Location = location
            };

        if (RoundBracketToken.IsValid(tokenId))
            return new RoundBracketToken(text)
            {
                Location = location
            };

        if (SquareBracketToken.IsValid(tokenId))
            return new SquareBracketToken(text)
            {
                Location = location
            };

        if (CurlyBracketToken.IsValid(tokenId))
            return new CurlyBracketToken(text)
            {
                Location = location
            };

        if (AngleBracketToken.IsValid(tokenId))
            return new AngleBracketToken(text)
            {
                Location = location
            };

        if (CommentToken.IsValid(tokenId))
            return new CommentToken(text)
            {
                Location = location
            };

        if (KeywordToken.IsValid(tokenId))
            return new KeywordToken(text)
            {
                Location = location
            };

        return null;
    }
}

/// <summary>
/// Represents a keyword token.
/// </summary>
public sealed record KeywordToken : SyntaxToken
{
    public KeywordToken(string Text)
        : base(Text)
    { }

    public readonly static IReadOnlyList<int> Tokens =
        new ReadOnlyCollection<int>(new[]
        {
            MajaLexer.And,
            MajaLexer.Brk,
            MajaLexer.Cnt,
            MajaLexer.Elif,
            MajaLexer.Else,
            MajaLexer.False,
            MajaLexer.If,
            MajaLexer.In,
            MajaLexer.Loop,
            MajaLexer.Mod,
            MajaLexer.Not,
            MajaLexer.Or,
            MajaLexer.Pub,
            MajaLexer.Ret,
            MajaLexer.Self,
            MajaLexer.True,
            MajaLexer.Use
        });

    public static bool IsValid(int tokenId)
        => Tokens.Contains(tokenId);
}

/// <summary>
/// Represents a whitespace token.
/// </summary>
public sealed record WhitespaceToken : SyntaxToken
{
    public WhitespaceToken(string Text)
        : base(Text)
    { }

    public WhitespaceToken Append(WhitespaceToken token)
        => new(Text + token.Text)
        {
            Location = Location.Append(token.Location)
        };

    public static bool IsValid(int tokenId)
        => tokenId == MajaLexer.Sp;
}

/// <summary>
/// Represents a newline token.
/// </summary>
public sealed record NewlineToken : SyntaxToken
{
    public NewlineToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(int tokenId)
        => tokenId == MajaLexer.Eol;
}

/// <summary>
/// Represents a punctuation token: .,;:
/// </summary>
public sealed record PunctuationToken : SyntaxToken
{
    public PunctuationToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(int tokenId)
        => tokenId is MajaLexer.Dot or MajaLexer.Comma
            or MajaLexer.Colon or MajaLexer.SemiColon;
}

/// <summary>
/// Represents a parenthesis token: ()
/// </summary>
public sealed record RoundBracketToken : SyntaxToken
{
    public RoundBracketToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(int tokenId)
        => tokenId is MajaLexer.ParenOpen or MajaLexer.ParenClose;
}

/// <summary>
/// Represents the token: []
/// </summary>
public sealed record SquareBracketToken : SyntaxToken
{
    public SquareBracketToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(int tokenId)
        => tokenId is MajaLexer.BracketOpen or MajaLexer.BracketClose;
}

/// <summary>
/// Represents the token: {}
/// </summary>
public sealed record CurlyBracketToken : SyntaxToken
{
    public CurlyBracketToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(int tokenId)
        => tokenId is MajaLexer.CurlyOpen or MajaLexer.CurlyClose;
}

/// <summary>
/// Represents the token: <>
/// </summary>
public sealed record AngleBracketToken : SyntaxToken
{
    public AngleBracketToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(int tokenId)
        => tokenId is MajaLexer.AngleOpen or MajaLexer.AngleClose;
}

/// <summary>
/// Represents a single line comment token.
/// </summary>
public sealed record CommentToken : SyntaxToken
{
    public CommentToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(int tokenId)
         => tokenId == MajaLexer.Comment;
}

/// <summary>
/// Represents a syntax error token.
/// </summary>
public sealed record ErrorToken : SyntaxToken
{
    public ErrorToken(string Text)
        : base(Text)
    { }
}