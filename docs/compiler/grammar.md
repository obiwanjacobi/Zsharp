# Grammar

The Syntax Tree is a tree structure of type tree nodes. Below are the different node types described.

| Node | Description |
|------|-----|
| | **Code**
| Assembly |
| Module | Mod
| Import | Use
| Export | Pub
| CodeBlock | A scope possibly indented
| | **Names**
| Identifier | Declaration of a name
| IdentifierToken | Reference / use of a name
| IdentifierQualified | Full name with namespace
| | **Functions**
| FunctionDeclaration |
| ParameterList |
| Parameter |
| ParameterSelf |
| ArgumentList |
| Argument |
| | **Types**
| TypeDeclaration |
| TypeParameterList |
| TypeParameter |
| TypeParameterGeneric |
| TypeParameterTemplate |
| TypeParameterValue |
| TypeArgumentList |
| TypeArgument |
| TypeMemberEnum |
| TypeMemberField |
| TypeMemberRule |
| | **Variables**
| VariableDeclaration |
| | **Expression**
| Expression |
| UnaryExpression | 1 operand and 1 operator
| BinaryExpression | 2 operands and 1 operator
| TernaryExpression | 3 operands and 2 operators
| ExpressionOperand |
| ExpressionOperator |
| TypeExpression | Use of a type name and optional args
| FunctionExpression | Function call and optional args
| IterationExpression |
| MatchExpression |
| | **Statements**
| Loop |
| If |
| Else |
| Elif |
| Ret |
| Brk |
| Cont |
