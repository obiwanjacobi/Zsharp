﻿# Syntax Model

The purpose of the syntax model is:

- Abstract Antlr away from the rest of the compiler code.
- Provide a way to (re)format source code (future).
- Capture all tokens (incl whitespace and comments)


## Structure

The syntax model contains an object model that represents the syntax tree as it is parsed from the source code.
All classes are (publically) immutable.

### SyntaxNode

The root (base) class of all nodes in the syntax tree.
It implements a `Location` property that represents where in the source code the node was created for and a `Children` list that contains nested `SyntaxNode` instances.

### SyntaxTree

A container for a parsed source file.

### CompilationUnitSyntax

The `CompilationUnitSyntax` is the root of the syntax tree.

### SyntaxToken

A token that is not central to understanding the code.
`SyntaxToken`s are added to `SyntaxNode`s either in the `LeadingTokens` or the `TrailingTokens` list based on where they occurred.