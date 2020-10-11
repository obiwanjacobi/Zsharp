# Design Philosophy

- Modern Language features (Range, Slice).
- Strong Typed - but easy syntax.

But nothing radically new. Should look familiar.

- No hidden memory allocation.
- No hidden function calls.
- No implicit conversions.

Syntax directs thought. The way you write it down should match with the underlying model.

- Simple to learn.
- Minimal number of keywords.
- Singular meaning (as much as possible) / one way to do things.
- Easy to build (parse).

Translates well to 8-bit CPUs like Z80.

What is better in Z# than other languages?

- More consistency. Operators generally mean only one thing.
- Less noise. No unnecessary keywords and no brackets or semi-colons.
- Focus on compile-time benefits. Do as much at compile-time as possible.
- Just data and functions. You can do sort of OO but not full OOP. Makes it simpler to learn and the resulting constructs translate easier to (simpler/smaller/faster) machine code.

Goals

- Stable syntax for refactoring code units to other scopes. Lifting a local function out => making it public (export) etc.

> Explain: size, length, count, capacity.
