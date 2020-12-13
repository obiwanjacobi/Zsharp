# Flow Control

Constructs to direct the flow of program execution.

## Conditional branching

The basic if statement works as expected, but the condition is not enclosed in `()`. There can only be a conditional expression following the `if`, so parsing of the condition stops when a newline is encountered or a comment.

All the `if` options in one example.

```csharp
if true               // is it true?
    conditional_code
else if false         // or is it false?
    never_get_here
else                  // or is it neither?
    other_code
```

> No assignment is permitted inside the `if` conditional expression.

Alternate / inline `if`:

```csharp
if true     // conventional if statement
    ...
else
    ...

true ? ... : ...    // ternary operator

true ? ...          // ternary without 'else'
```

Allow `in` keyword in `if` statement?

```csharp
arr = (1, 2, 3, 4, 5)
x = 2
if x in arr // can x be found in arr?
    ...     // true
```

### Parenthesis

When parenthesis are used they are always part of the expression, not of the `if` statement. There is always a space after the `if` statement - this is what sets it apart from a function call.

```csharp
if (42 = 42)    // if statement (+ space)
   ...

if(42)          // function call (no space)
```

Note that `if` is a reserved keyword and no named language element can have the same name as a reserved keyword.

---

## Loops

A loop is an essential control mechanism in directing the execution flow of the code. There is only one keyword for making a loop: `loop`.

The simplest form is an endless loop:

```C#
loop
    endless_loop
```

A 'while loop' just adds a condition to the statement:

```C#
loop false
    never_get_here
```

A 'for' or 'for-each' loop is constructed using a `Range`.

```C#
loop n in [0..10]
    foreach_n_1_to_9
```

Loop a number of times

```C#
loop 42
    do_this_42_times

c = 42
loop c
    do_this_42_times    // is c available - and what value?
```

> TODO: loop with more than one range? Hard to control behavior.

```csharp
loop w in [5..0], h in [0..10]
    w_makes_two_rounds_and_h_one
```

Nested loops

```csharp
loop h in [0..10]
    loop w in [0..5]
        w_times_h
```

Loop with lambda?
Loop with Function?

```csharp
loop [0..10]
    (n) => log("Now at {n}.`n")

loop [0..10]
    LogInt
```

---

## Break

It is possible to break out of the execution of a loop at any time with the `break` keyword.

> Perhaps call it `leave` instead of `break` as it sounds less dreadful. Or `exit loop` and `exit if` and `exit fn`? `up` (as in up one scope/indent)?

Here's an example of an endless loop that uses a conditional branch `if` to break out of the loop:

```C#
loop
    do_stuff_here
    if true
        break     // this will exit the loop
```

The `break` keyword only works on its immediate parent loop. In the case of nested loops, it will look like this:

```C#
loop                  // loop #1
    do_stuff_here
    loop              // loop #2
        do_other_stuff_here
        if true
            break     // this will exit loop #2
```

---

## Continue

The opposite of `break`, the `continue` keyword will allow you to skip one iteration of the loop:

```C#
loop
    if true
        continue    // this will skip 'do_stuff_here'
    do_stuff_here
```

As with `break`, `continue` only works on the immediate parent loop.

---

## Exit

> TBD: use `exit` as a keyword to replace `break`, `continue`, `yield` and `return`?

```csharp
exit()              // exits program
exit(fn)            // exits function (return)
exit(co)            // exits coroutine (yield)
exit(iter)          // exits current iteration (continue)
exit(loop)          // exits loop (break)
exit(..)            // exits current scope (1 up)
```

Or this syntax? (_ is discard...)

```csharp
exit_app
exit_fn
exit_co
exit_iter
exit_loop
exit
```

```csharp
fn: (p: U8): U8
    if p == 0 => exit()     // abort program
    c: U8
    loop n in [0..p]
        if p == 42 => exit(iter)    // continue
        if p == 101 => exit(loop)   // break
        c += 1

    exit(fn) c  // return c
```
