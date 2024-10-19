# Maja Compiler

## TODO

- [ ] ErrorNode: Use context info to built better error message. Include referenced symbol location.
- [ ] Syntax/Parser: Error handling: MissingTokens, SkippedTokens (Antlr ErrorStrategy/ErrorListener?)
Started. Have ErrorToken with description.
- [ ] Syntax parsing does not report invalid trailing tokens.
- [ ] Syntax: Assignment to member access expression (l-value) (now nameIdentifier)
- [ ] Indents (dedent) are not working correct with multiple indents (`SyntaxWriter`).
- [ ] Type: Generics (type inference)
- [ ] Type: Template
- [ ] Type: Comp-parameter
- [ ] Function: Generics (type inference)
- [ ] Function: Template
- [ ] Function: Comp-parameter
- [ ] Value (custom) Types (/w rules)
- [ ] Infer Type of type parameter from usage (invocation parameter or instantiation type) (later)
- [ ] Resolve operators to functions
- [ ] local (nested) functions and types
- [ ] Emit: output `System.CodeDom.Compiler.GeneratedCode` and `Maja.AliasAttribute`
- [ ] Allow unnamed parameter(s) together with named parameters (`IrArgumentMatcher`)
- [ ] Allow named type-parameters (`IrArgumentMatcher`)
- [ ] Compile-time code: emit compile-time assembly. Call compile-time custom code.
- [ ] IR: Validate file-global statements (only StatementExpression?)
- [ ] Rename token: (): parentheses, {}: (curly) braces, []: brackets, <>: ??
- [ ] Research compiler feature flags with backwards compatibility with future versions.

### Done

- [x] Type: base type (enum/struct/value).
- [x] Syntax: serialze Syntax model into Maja code. Should be the exact same as source.
- [x] Diagnostics: Generate compiler warning for `##` comments
- [x] UnitTests for Eval
- [x] IR: Not all expression nodes have a TypeSymbol set (for instance Range)
- [x] Emit: Transform loop expression to C# compatible for-expression.
- [x] IR: Function Type (IrTypeFunction/FunctionTypeSymbol)
- [x] StatementExpression only for invocation (other?).
- [x] Loop statement
- [x] IR: Validate loop expression to be a valid loop expression
- [x] Test whitespace and comments in various syntax
- [x] Export from module.
- [x] Syntax: fix required eol at the end of code.
- [x] Discard assignment only for invocation results.
- [x] ExpressionInvocation: Check argument types against function parameter types
- [x] Syntax: Location Span is incorrect (x, x).
- [x] Rewrite Expression Types for: Invocation, Literals, Binary Operators, Variable initializers
- [x] VarDeclTyped+init: init expression type is not the same as var type.
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
- [x] `void` function can `ret` a value. Should generate compile error.

## Decisions

- SyntaxTree: Not going to build a Green/Red tree yet.
SyntaxTrees are constructed from source text and after that cannot be changed for now.
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

- Is CodeBlock a statement or an expression?
