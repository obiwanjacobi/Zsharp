# Z\#

> Under development!

Z# is the first programming language I designed.
The language is not an Object Oriented language and was first intended for small (8-bit) processors.
The initial compiler `Zlang` was written in C++ but after struggling with LLVM for a good while, I gave up and decided to target .NET instead.
I ported my C++ code to C# and renamed the project to Zsharp.
Targeting .NET will mean that some of my initial ideas will need to change a bit and that I need to map between Z# and .NET (IL) concepts.
For the most part it seems like that should work (in theory).

The `docs` folder contains the Z# language documentation and is also still in active development.
This contents is [published as 'GIT Pages' here](https://obiwanjacobi.github.io/Zsharp/).

The `src` folder contains the source code and unit tests for the compiler.

The `Zsharp` folder contains the compiler project.
The `Zsharp.g4` file is an `Antlr4` grammar file that contains the Tokens and Parser rules for a subset of the Zsharp syntax.
I decided to get to all aspects of the compiler (parsing, building an Abstract Syntax Tree and emitting IL code) as soon as possible in order to run into any problems early.
The `Parser` sub folder contains the code generated by Antlr from the Zsharp grammar.
Note that I use Visual Studio Code and the excellent Antlr4 extension for editing the grammar file.

The `UnitTests` folder contains the unit test project.
At this stage there are only unit tests that verify the correct behavior of the code.
There is no exe that will accept command line argument to compile a file.

## Community

If you like, you can chat about the Z# language on [Discord](https://discord.gg/5r9YMXHrYU).
