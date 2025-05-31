# Maja

A new implementation using a immutable models.

## Syntax Model

The source code is parsed and represented with the Syntax model. This model can be round-tripped to reproduce the original source code text.

## Intermediate Representation Model

After the source code is parsed into a Syntax model, it is passed to the Intermediate Representation (IR) model builder.
It creates the IR model with a reference to the Syntax and the Symbols of types and functions declared and referenced.

## Repl / Evaluator

A console application that can be used to enter source code in small steps and execute it to see the result.
