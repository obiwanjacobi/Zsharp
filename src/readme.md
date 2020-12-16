# Zsharp Compiler Project

## Bugs

## When to choose for Reference Types or Value Types

The nature of Z# is more aligned with using structs I think.
Structs can be allocated on the callstack and on the heap.

## Explicit Memory Management

How to align the Garbage Collector (GC) with the memory semantics of Z# - which are not fully crystalized yet.

## Zsharp Runtime

I think it could be a good idea to have a runtime assembly that contains common used functionality.

## .NET Interop

Using .NET it becomes possible to intermix (compiled) Zsharp code with other .NET code.

### Using .NET code in Zsharp

> How do we deal with the different semantics?

### Calling Zsharp code from other .NET code

Perhaps add extra checking on the public (exported) functions.

