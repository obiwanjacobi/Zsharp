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
public abstract class SyntaxToken
{
    public const string Separator = ".";
    public const string Discard = "_";

    protected SyntaxToken(string text)
    {
        Text = text;
    }

    private SyntaxNode? _parent;
    /// <summary>
    /// The parent syntax node of this token.
    /// </summary>
    public SyntaxNode Parent
    {
        get => _parent!;
        internal set
        {
            Debug.Assert(value is not null, "Cannot clear a SyntaxToken.Parent with null.");
            Debug.Assert(_parent is null || _parent == value, "SyntaxToken.Parent is already set.");
            _parent = value;
        }
    }

    /// <summary>
    /// The source location of this syntax token.
    /// </summary>
    public SyntaxLocation Location { get; init; }

    /// <summary>
    /// Unique (Lexer) type Id for the token.
    /// </summary>
    public int TokenTypeId { get; init; }

    public virtual bool HasError
        => false;

    public string Text { get; }

    /// <summary>
    /// Creates a new token based on the tokenTypeId.
    /// </summary>
    /// <param name="tokenTypeId">The (Lexer) token id.</param>
    /// <param name="text">The textual representation of the token.</param>
    /// <param name="location">The source location of the token.</param>
    /// <returns>Returns null if the token could not be created.</returns>
    public static SyntaxToken? TryNew(int tokenTypeId, string text, SyntaxLocation location)
    {
        if (NewlineToken.IsValid(tokenTypeId))
            return new NewlineToken(text)
            {
                TokenTypeId = tokenTypeId,
                Location = location
            };

        if (WhitespaceToken.IsValid(tokenTypeId))
            return new WhitespaceToken(text)
            {
                TokenTypeId = tokenTypeId,
                Location = location
            };

        if (PunctuationToken.IsValid(tokenTypeId))
            return new PunctuationToken(text)
            {
                TokenTypeId = tokenTypeId,
                Location = location
            };

        if (RoundBracketToken.IsValid(tokenTypeId))
            return new RoundBracketToken(text)
            {
                TokenTypeId = tokenTypeId,
                Location = location
            };

        if (SquareBracketToken.IsValid(tokenTypeId))
            return new SquareBracketToken(text)
            {
                TokenTypeId = tokenTypeId,
                Location = location
            };

        if (CurlyBracketToken.IsValid(tokenTypeId))
            return new CurlyBracketToken(text)
            {
                TokenTypeId = tokenTypeId,
                Location = location
            };

        if (AngleBracketToken.IsValid(tokenTypeId))
            return new AngleBracketToken(text)
            {
                TokenTypeId = tokenTypeId,
                Location = location
            };

        if (CommentToken.IsValid(tokenTypeId))
            return new CommentToken(text)
            {
                TokenTypeId = tokenTypeId,
                Location = location
            };

        if (KeywordToken.IsValid(tokenTypeId))
            return new KeywordToken(text)
            {
                TokenTypeId = tokenTypeId,
                Location = location
            };

        return null;
    }
}

/// <summary>
/// Represents a keyword token.
/// </summary>
public sealed class KeywordToken : SyntaxToken
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

    public static bool IsValid(int tokenTypeId)
        => Tokens.Contains(tokenTypeId);
}

/// <summary>
/// Represents a whitespace token.
/// </summary>
public sealed class WhitespaceToken : SyntaxToken
{
    public WhitespaceToken(string Text)
        : base(Text)
    { }

    public WhitespaceToken Append(WhitespaceToken token)
        => new(Text + token.Text)
        {
            Location = Location.Append(token.Location)
        };

    public static bool IsValid(int tokenTypeId)
        => tokenTypeId == MajaLexer.Sp;
}

/// <summary>
/// Represents a newline token.
/// </summary>
public sealed class NewlineToken : SyntaxToken
{
    public NewlineToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(int tokenTypeId)
        => tokenTypeId == MajaLexer.Eol;
}

/// <summary>
/// Represents a punctuation token: .,;:
/// </summary>
public sealed class PunctuationToken : SyntaxToken
{
    public PunctuationToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(int tokenTypeId)
        => tokenTypeId is MajaLexer.Dot or MajaLexer.Comma
            or MajaLexer.Colon or MajaLexer.SemiColon
            or MajaLexer.Range;
}

/// <summary>
/// Represents a parenthesis token: ()
/// </summary>
public sealed class RoundBracketToken : SyntaxToken
{
    public RoundBracketToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(int tokenTypeId)
        => tokenTypeId is MajaLexer.ParenOpen or MajaLexer.ParenClose;
}

/// <summary>
/// Represents the token: []
/// </summary>
public sealed class SquareBracketToken : SyntaxToken
{
    public SquareBracketToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(int tokenTypeId)
        => tokenTypeId is MajaLexer.BracketOpen or MajaLexer.BracketClose;
}

/// <summary>
/// Represents the token: {}
/// </summary>
public sealed class CurlyBracketToken : SyntaxToken
{
    public CurlyBracketToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(int tokenTypeId)
        => tokenTypeId is MajaLexer.CurlyOpen or MajaLexer.CurlyClose;
}

/// <summary>
/// Represents the token: <>
/// </summary>
public sealed class AngleBracketToken : SyntaxToken
{
    public AngleBracketToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(int tokenTypeId)
        => tokenTypeId is MajaLexer.AngleOpen or MajaLexer.AngleClose;
}

/// <summary>
/// Represents a single line comment token.
/// </summary>
public sealed class CommentToken : SyntaxToken
{
    public CommentToken(string Text)
        : base(Text)
    { }

    public static bool IsValid(int tokenTypeId)
         => tokenTypeId == MajaLexer.Comment;
}

/// <summary>
/// Represents a syntax error token.
/// TokenTypeId holds the expected token type id.
/// </summary>
public sealed class ErrorToken : SyntaxToken
{
    public ErrorToken(string Text)
        : base(Text)
    { }

    public override bool HasError
        => true;
}