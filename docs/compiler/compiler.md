# Z# Compiler

- [Checked Functions](checked.md)
- [Meta Programming](meta.md)
- [Conditional Compilation](conditional.md)
- [Libraries](libraries.md)
- [Packages](packages.md)
- [Extensions](extensions.md)
- [Compiler Options](options.md)
- [Assembly](assembly.md)

---

>TBD

- Compiler as static analyzer / linter
- Compiler as Language Server (LSP)
- Compiler as (remote) debugger.
- Compiler as REPL (because we support Meta)
- Compiler as Language VM? (used by REPL) - zs-script
- Compiler as a specializer (taking source and static input and compiling an optimized version).
- Compiler as profiler?
- Compiler as build-system? Use REPL/VM to execute zs-script using build-task lib...?
- Semantic compiler rules available as functions to code (traits?).
- Emit C#, WASM (and through a Roslyn Source Generator?).

---

> entry function (main?)? no command line parameters? Code Attribute? Environment/command line etc. as singleton object?

> TBD: Compiler reports each 'issue' based on config. A compiler-profile can make an 'issue' a warning or error etc.

> TBD: Macros/meta programming based on AST of compiler.

---

> Rethink these wild ideas in light of supporting .NET.

> How to parallelize compilation?
> CodeBlock could be processed asynchronously?
> Have Linq on AST to ask questions about the structure of the code.
