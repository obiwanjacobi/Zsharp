# Conditional Compilation

How?

Direct/steer compiler by calling functions?

Conditional code attribute?

Steering value can be specified on compiler command line.
Can make steering values in code but have to be known at compile time.

```C#
if comptimeValue
    code compiled if comptimeValue is true
else
    code eliminated - excluded by compiler
```

When condition of an `if` statement is known at compile time, the compiler knows which branch to keep and what to throw away.

> We could also introduce a `#` (or `#!`) prefix to force compile-time execution. These could be placed on 'any' of the statements/keywords.

```C#
#if comptimeValue
    code compiled if comptimeValue is true
#else
    code eliminated - excluded by compiler

// or

if #!comptimeValue
    code compiled if comptimeValue is true
else
    code eliminated - excluded by compiler
```
