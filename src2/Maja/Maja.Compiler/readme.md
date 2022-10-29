# Maja Compiler

## TODO

[ ] Syntax: Now records. Use class for ref-equality? /bc immutable state
[ ] SyntaxNode: DescendentNodes, AncestorsAndSelf, AncestorOrSelf etc.
[ ] ErrorNode: Use context info to built better error message.

### Done
[x] Parser: Error handling: MissingTokens, SkippedTokens
Started. Have ErrorToken with description.
[x] SyntaxNode: Children to include tokens. ChildNodes current Children?
Made SyntaxNodeOrToken public and added SyntaxNodeOrTokenList.
[x] Expression: Expression kind enum (logical, arithmetic, bitwise etc expression)
[x] ExpressionOperator: kind enum (with precedence?)
[x] ExpressionOperator: Implement mapping multi-token operators to kind.
[x] Compiler: move to SyntaxTree and remove.
[x] Syntax: Else /ElseIf is not a statement. Should not derive from Statement base.

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


## Notes

- Lexer does actual value extraction? (number token to int)
- assignment expression are not binary expression. You need a different tree (right-assoc).
Analyze if all our assignment variations (arrays etc) will fit in an expression.
- CodeBlock is a statement?
- No Binder/Binding/BoundNode: internal Intermediate Representation (IR)? Incl. Symbols & lowering/rewriting etc.
Analysis is done through the public SemanticModel. Symbols are also public.
