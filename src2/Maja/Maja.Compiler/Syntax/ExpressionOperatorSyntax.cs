using System.Linq;
using Maja.Compiler.Parser;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents an expression operator.
/// </summary>
public sealed class ExpressionOperatorSyntax : SyntaxNode
{
    private ExpressionOperatorSyntax(string text)
        : base(text)
    { }

    public int Precedence { get; init; }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.ExpressionOperator;

    public ExpressionOperatorKind OperatorKind { get; init; }

    public ExpressionOperatorCategory OperatorCategory { get; init; }

    public ExpressionOperatorCardinality OperatorCardinality { get; init; }

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionOperator(this);

    public static ExpressionOperatorSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens,
        int precedence, ExpressionOperatorKind kind, ExpressionOperatorCategory category,
        ExpressionOperatorCardinality cardinality)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens,
            Precedence = precedence,
            OperatorKind = kind,
            OperatorCategory = category,
            OperatorCardinality = cardinality
        };
}

internal static class ExpressionOperatorMap
{
    public static int ToOperatorPrecedence(this ExpressionOperatorCardinality cardinality)
        => (int)cardinality;

    public static ExpressionOperatorKind ToOperatorKind(this int tokenTypeId,
        ExpressionOperatorCategory category)
    {
        return category switch
        {
            ExpressionOperatorCategory.Arithmetic =>
                tokenTypeId switch
                {
                    MajaLexer.Plus => ExpressionOperatorKind.Plus,
                    MajaLexer.Minus => ExpressionOperatorKind.Minus,
                    MajaLexer.Multiply => ExpressionOperatorKind.Multiply,
                    MajaLexer.Divide => ExpressionOperatorKind.Divide,
                    MajaLexer.Modulo => ExpressionOperatorKind.Modulo,
                    MajaLexer.Power => ExpressionOperatorKind.Power,
                    MajaLexer.Root => ExpressionOperatorKind.Root,
                    _ => ExpressionOperatorKind.Unknown
                },
            ExpressionOperatorCategory.Bitwise =>
                tokenTypeId switch
                {
                    MajaLexer.BitAnd => ExpressionOperatorKind.BitAnd,
                    MajaLexer.BitNot => ExpressionOperatorKind.BitNot,
                    MajaLexer.BitOr => ExpressionOperatorKind.BitOr,
                    MajaLexer.BitRollL => ExpressionOperatorKind.BitRollLeft,
                    MajaLexer.BitRollR => ExpressionOperatorKind.BitRollRight,
                    MajaLexer.BitShiftL => ExpressionOperatorKind.BitShiftLeft,
                    //MajaLexer.BitAnd => ExpressionOperatorKind.BitShiftRight,
                    //MajaLexer.BitAnd => ExpressionOperatorKind.BitShiftRightSign,
                    MajaLexer.BitXor_Imm => ExpressionOperatorKind.BitXor,
                    _ => ExpressionOperatorKind.Unknown
                },
            ExpressionOperatorCategory.Comparison =>
                tokenTypeId switch
                {
                    MajaLexer.Eq => ExpressionOperatorKind.Equals,
                    MajaLexer.Neq => ExpressionOperatorKind.NotEquals,
                    MajaLexer.AngleClose => ExpressionOperatorKind.Greater,
                    MajaLexer.GtEq => ExpressionOperatorKind.GreaterOrEquals,
                    MajaLexer.AngleOpen => ExpressionOperatorKind.Lesser,
                    MajaLexer.LtEq => ExpressionOperatorKind.LesserOrEquals,
                    _ => ExpressionOperatorKind.Unknown
                },
            ExpressionOperatorCategory.Logic =>
                tokenTypeId switch
                {
                    MajaLexer.And => ExpressionOperatorKind.And,
                    MajaLexer.Not => ExpressionOperatorKind.Not,
                    MajaLexer.Or => ExpressionOperatorKind.Or,
                    //MajaLexer.Xor => ExpressionOperatorKind.Xor,
                    _ => ExpressionOperatorKind.Unknown
                },
            _ => ExpressionOperatorKind.Unknown
        };
    }

    public static ExpressionOperatorKind ToOperatorKind(this int[] tokenTypeIds,
        ExpressionOperatorCategory category)
    {
        if (tokenTypeIds.Length == 1)
            return tokenTypeIds[0].ToOperatorKind(category);

        if (category != ExpressionOperatorCategory.Bitwise)
            return ExpressionOperatorKind.Unknown;

        if (!tokenTypeIds.All(t => t == MajaLexer.AngleClose))
            return ExpressionOperatorKind.Unknown;

        return tokenTypeIds.Length switch
        {
            2 => ExpressionOperatorKind.BitShiftRight,
            3 => ExpressionOperatorKind.BitShiftRightSign,
            _ => ExpressionOperatorKind.Unknown
        };
    }
}