# Compilation

The `Compilation` represents the complete set of data that is needed to compile code.
It maintains a list of `SyntaxTree`'s, `AssemblyReference`'s and compiler options.
It also describes the target assembly.
The compilation can Emit code into the target assembly if no Error diagnostics are present in the `Compilation`.

A `CompilationModel` can be returned for a specific `SyntaxTree`.
The `CompilationModel` can answer questions about the code, it's symbols and perform flow analysis (future).
