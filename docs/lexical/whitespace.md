# Whitespace

Unlike most languages, whitespace matters in Z#. Similar to Python and F#.

## Spaces and Tabs

Spaces are used to separate code elements in order to make them distinct.

`iftrue` does not mean anything, while `if true` represents an `if` statement with a condition (`true`). The space separates the parts and makes them meaningful.

## Indentation

Z# uses whitespace -indents- to signal scope. Where a lot of languages use `{}` to declare a scope, Z# attempts to minimize noise and do away with those characters.

**An _`indent`_ is represented by a sequence of spaces.**
Typically 4 spaces are used. The number of spaces must be consistent throughout a single file but can differ between files. The first indent that is encountered is taken as the template for all others to follow.

The start of the line represents the root scope. An `indent` is always placed below a statement or instruction that belongs to a higher scope (one up).

A simple example of a function and its implementation:

```C#
MyFuncion: ()
    implementation_here
    more_code_here
```

The function is at root scope and all of its implementation is one indent removed from that origin.

These scopes can of course be nested.

Here is an example of a function with nested scopes.

```C#
MyFuncion: ()
    if true
        conditional_code_here
    implementation_here
```

The 'conditional code' is only executed if the `if` branch is taken. The 'implementation' is always executed.

## Scope Names

Names for the indentation levels that create scopes:

```C#
global-scope        // exports
<File>
    top-level (file-scope)

    <Function>
        function-scope

    <Type>
        type-scope
```

## Breaking up lines

As a general rule: broken up lines continue on the next line one indent further than a new (child) scope would be: a double indent.

## Merging lines

To merge separate lines onto a single one, use the `,` to separate each 'line'.

```csharp
fn: ()
    if true, doStuff(), else, doNothing()
```

> TBD I may want to introduce a 'single statement' rule that would also make lambda's and `match` easier. It would allow one single statement to be specified on the same line.

Our example would lose a comma or two:

```csharp
fn: ()
    if true doStuff(), else doNothing()
    // or
    if true => doStuff(), else => doNothing()
    // mixup with lambdas?
    if (true) => doStuff(), else => doNothing()
```

> Also may decide to use `;` instead of `,`

```csharp
fn: ()
    if true doStuff(); else doNothing()
```

### Literal Strings

No extra quotes are required. The new-line (EOL) character(s) and the indent white-space(s) will **NOT** become part of the string.

```C#
s = "this is a very long string to demonstrate
        how to break up long literal strings"
```

So in this example there is a trailing space after 'demonstrate' in order to separate that word from the next word 'how'.

### Functions

The function name cannot be split. Parameters can be spread out over multiple lines. The opening parenthesis is always next to the function name. The closing parenthesis can either be directly after the last parameter or at the beginning (same indent/scope as the function name) of a new line. Commas that separate the parameters go directly after its previous parameter and cannot be on the start of a new line (after a double indent).

```C#
my_very_long_function_name_with_lots_of_parameters: (
        p1: U8, p2: U16, p3: U24, p4: U32,
        p5: Str
)
    ...
```

Use new-lines instead of commas.

```C#
function_with_params_but_no_commas: (
        p1: U8
        p2: U16
        p3: U24
        p4: U32
        p5: Str
)
    ...
```
