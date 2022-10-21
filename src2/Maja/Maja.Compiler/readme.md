# Maja Compiler

## TODO

- Syntax: Text property should represent contained nodes and tokens.
- Syntax: Now records. Use class for ref-equality? /bc immutable state
- SyntaxNode: DescendentNodes, AncestorsAndSelf, AncestorOrSelf etc.
- SyntaxNode: Children to include tokens. ChildNodes current Children?
- Expression: Operator kind enum (with precedence?)
- Expression: Expression kind enum (logical, arithmetic, bitwise etc expression)
- Parser: Error handling: MissingTokens, SkippedTokens
- SyntaxNodeBuilder: new class to create/change a SyntaxTree. Fluent interface.
- SyntaxRewriter (abstract base): (SyntaxVisitor) allows constructing a new tree multiple node (type)s at a time.

## Descisions

- SyntaxTree: Not going to build a Green/Red tree yet.
SyntaxTrees are constructuted from source text and after that cannot be changed for now.
No SyntaxFactory or WithXxxx methods.
- SyntaxTree: No lazyness.
Based on the Antlr parse tree the syntax tree is built in full.
