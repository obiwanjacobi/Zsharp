# Zlang Compiler

- [Checked Functions](checked.md)
- [Meta Programming](meta.md)
- [Conditional Compilation](conditional.md)
- [Libraries](libraries.md)
- [Packages](packages.md)
- [Extensions](extensions.md)
- [Compiler Options](options.md)
- [Assembly](assembly.md)

---

> Compiler as static analyzer / linter

> Compiler as Language Server (LSP)

> Compiler as (remote) debugger. How to interface with the hardware? See also Z80 In-Circuit-Emulation (ICE).

> Compiler as (remote) console. Coded/hashed errors are transmitted (Serial) to the compiler that can translate it into readable diagnostic text. This way the debug binary does not explode with debugging strings.

> Compiler as Assembler (byte-code?) Cross CPU Abstract Assembler Language? (WASM?)

> Compiler as linker (linker map file, late optimizations)

> Compiler as (language supported) build system.

> Compiler as REPL (because we support Meta)

> Compiler as Language VM? (used by REPL) - zs-script

> Compiler as a specializer (taking source and static input and compiling an optimized version).

> Compiler as profiler?

> Compiler as build-system? Use REPL/VM to execute zs-script using build-task lib...?

> Semantic compiler rules available as functions to code (traits?).

---

> Literal strings can add up. How to optimize? (interning?, string-builder for efficient construction?)

> entry function (main?)? no command line parameters? Code Attribute? Environment/command line etc. as singleton object?

> build in extra grammar/semantics for verifying correctness of the compiler. Inline unit tests.

> TBD: Compiler reports each 'issue' based on config. A compiler-profile can make an 'issue' a warning or error etc.

> TBD: Macros/meta programming based on AST of compiler.
