# Maja Compiler

## TODO

- [ ] SyntaxNode: DescendentNodes, AncestorsAndSelf, AncestorOrSelf etc.
- [ ] ErrorNode: Use context info to built better error message.
- [ ] VarDeclTyped+init: init expression type is not the same as var type.

### Done

- [x] Parser: Error handling: MissingTokens, SkippedTokens
Started. Have ErrorToken with description.
- [x] SyntaxNode: Children to include tokens. ChildNodes current Children?
Made SyntaxNodeOrToken public and added SyntaxNodeOrTokenList.
- [x] Expression: Expression kind enum (logical, arithmetic, bitwise etc expression)
- [x] ExpressionOperator: kind enum (with precedence?)
- [x] ExpressionOperator: Implement mapping multi-token operators to kind.
- [x] Compiler: move to SyntaxTree and remove.
- [x] Syntax: Else /ElseIf is not a statement. Should not derive from Statement base.
- [x] Syntax: Now records. Use class for ref-equality? /bc immutable state
- [x] TypeSymbol: add enums, fields and rules symbols
- [x] BinaryExpression precedence check not executed. Check in Syntax.
- [x] 'if true' places the 'if' token at the ExpressionLiteralBoolSyntax node.

## Descisions

- SyntaxTree: Not going to build a Green/Red tree yet.
SyntaxTrees are constructuted from source text and after that cannot be changed for now.
No SyntaxFactory or WithXxxx methods.
- SyntaxTree: No lazyness.
Based on the Antlr parse tree the syntax tree is built in full.
- [NotDoing] Syntax: Text property should represent contained nodes and tokens.
Only necessary when tree is allowed to change.
- [NotDoing] SyntaxRewriter (abstract base): (SyntaxVisitor) allows constructing a new tree multiple node (type)s at a time.
Only necessary when tree is allowed to change.
- [NotDoing] SyntaxNodeBuilder: new class to create/change a SyntaxTree. Fluent interface.
Only necessary when tree is allowed to change.
- Assignment is now a statement. This prohibits assignment in if-conditions and does not require a different operator for equals.

## Notes

- CodeBlock is a statement?
