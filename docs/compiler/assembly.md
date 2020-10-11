# Assembly

It is possible to include literal Z80 assembly in the compiler output.

The compiler will parse the assembly, incorporate it in its machine representation and perform any optimizations on it as it were its own code. That means that there is a distinct possibility that the assembly code from the source file will not be present as a whole. Of course the resulting code is guaranteed to perform that same function.

There are several ways to include assembly into the compiler output:

- Separate File
- Inline

---

> TBD: interfacing with variable, function parameters and return values.

> Calling assembly entry points

> Calling Z# functions from assembly.
