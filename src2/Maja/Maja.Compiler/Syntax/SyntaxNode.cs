﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Base class for all nodes in the SyntaxTree.
/// </summary>
public abstract partial class SyntaxNode
{
    protected SyntaxNode(string text)
        => Text = text;

    /// <summary>
    /// The source text this node represents.
    /// </summary>
    public string Text { get; }

    public abstract SyntaxKind SyntaxKind { get; }

    public bool HasError
        => Children.Any(c => c.HasError);

    public IEnumerable<ErrorToken> GetErrors()
        => Children.Where(c => c.HasError)
            .SelectMany(c => c.Node?.GetErrors() ?? new[] { (ErrorToken)c.Token! });

    private SyntaxNode? _parent;
    /// <summary>
    /// The parent of this node. Only the root node has no parent.
    /// </summary>
    public SyntaxNode? Parent
    {
        get => _parent!;
        internal set
        {
            Debug.Assert(value is not null, "Cannot clear a SyntaxNode.Parent with null.");
            Debug.Assert(_parent is null, "SyntaxNode.Parent is already set.");
            _parent = value;
        }
    }

    /// <summary>
    /// Source location for this syntax node.
    /// </summary>
    public SyntaxLocation Location { get; init; }

    private SyntaxNodeList? _nodeList;
    /// <summary>
    /// A list of child nodes contained in this node.
    /// </summary>
    public SyntaxNodeList ChildNodes
    {
        get
        {
            Debug.Assert(_nodeList is not null, "SyntaxNode is uninitialized. ChildNodes are not set.");
            return _nodeList!;
        }
        init
        {
            Debug.Assert(value is not null, "Cannot clear the SyntaxNode.ChildNodes list with null.");
            Debug.Assert(_nodeList is null, "The SyntaxNode.ChildNodes list is already set.");
            _nodeList = value;
            _nodeList.Parent = this;
        }
    }

    private SyntaxNodeOrTokenList? _nodeOrTokenList;
    /// <summary>
    /// A list of child nodes and tokens contained in this node.
    /// </summary>
    public SyntaxNodeOrTokenList Children
    {
        get
        {
            Debug.Assert(_nodeOrTokenList is not null, "SyntaxNode is uninitialized. ChildNodes are not set.");
            return _nodeOrTokenList!;
        }
        init
        {
            Debug.Assert(value is not null, "Cannot clear the SyntaxNode.ChildNodes list with null.");
            Debug.Assert(_nodeOrTokenList is null, "The SyntaxNode.ChildNodes list is already set.");
            _nodeOrTokenList = value;
        }
    }

    /// <summary>
    /// Indicates if the LeadingTokens can be called.
    /// </summary>
    public bool HasLeadingTokens
        => _leadingTokens is not null && _leadingTokens.Count > 0;

    private SyntaxTokenList? _leadingTokens;
    /// <summary>
    /// A list of tokens preceding this syntax node.
    /// </summary>
    public SyntaxTokenList LeadingTokens
    {
        get
        {
            //Debug.Assert(_leadingTokens is not null, "SyntaxNode is uninitialized. LeadingTokens are not set.");
            return _leadingTokens!;
        }
        internal set
        {
            Debug.Assert(value is not null, "Cannot clear the SyntaxNode.LeadingTokens list with null.");
            _leadingTokens = value;
            _leadingTokens.Parent = this;
        }
    }

    /// <summary>
    /// Indicates if the TrailingTokens can be called.
    /// </summary>
    public bool HasTrailingTokens
        => _trailingTokens is not null && _trailingTokens.Count > 0;

    private SyntaxTokenList? _trailingTokens;
    /// <summary>
    /// A list of tokens following this syntax node.
    /// </summary>
    public SyntaxTokenList TrailingTokens
    {
        get
        {
            //Debug.Assert(_trailingTokens is not null, "SyntaxNode is uninitialized. TrailingTokens are not set.");
            return _trailingTokens!;
        }
        internal set
        {
            Debug.Assert(value is not null, "Cannot clear the SyntaxNode.TrailingTokens list with null.");
            _trailingTokens = value;
            _trailingTokens.Parent = this;
        }
    }

    public override string ToString()
        => $"{GetType().Name}: {Location}";

    public abstract R Accept<R>(ISyntaxVisitor<R> visitor);
}

public interface ICreateSyntaxNode<out T>
    where T : SyntaxNode
{
    static abstract T Create(
        string text,
        SyntaxLocation location,
        SyntaxNodeOrTokenList children,
        SyntaxNodeList childNodes,
        SyntaxTokenList trailingTokens);
}