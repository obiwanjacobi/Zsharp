# Maja Compiler

## TODO

- Syntax: Text property should represent contained nodes and tokens.
- Expression: Operator kind enum (with precedence?)
- Expression: Expression kind enum (logical, arithmetic, bitwise etc expression)
- Parser: Error handling: MissingTokens, SkippedTokens
- SyntaxNodeBuilder: new class to create/change a SyntaxTree. Fluent interface.
- SyntaxRewriter (abstract base): (SyntaxVisitor) allows constructing a new tree multiple node (type)s at a time.