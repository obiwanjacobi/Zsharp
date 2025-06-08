
using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.Parser;

namespace Maja.Compiler.Syntax;

public sealed class OperatorAssignmentSyntax : SyntaxNode, ICreateSyntaxNode<OperatorAssignmentSyntax>
{
    public OperatorAssignmentSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.AssignmentOperator;

    /// <summary>
    /// The assignment operators (tokens).
    /// </summary>
    public IEnumerable<SyntaxToken> Operators
        => Children
            .Where(c => c.Token is not null)
            .Select(c => c.Token!);

    //public OperatorWrapperTypeSyntax? WrapperType
    //    => ChildNodes.OfType<OperatorWrapperTypeSyntax>().SingleOrDefault();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnOperatorAssignment(this);

    public static OperatorAssignmentSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };

    public static AssignmentOperatorKind DetermineKind(SyntaxToken token)
    {
        return token.TokenTypeId switch
        {
            MajaLexer.Eq => AssignmentOperatorKind.Assign,
            MajaLexer.AngleOpen => AssignmentOperatorKind.Copy,
            _ => AssignmentOperatorKind.Invalid
        };
    }
}
