# Emit Code

## CodeBlock

A code block is a single flow of execution that either ends with a (condition) branch, return or fall through.

---

```
<code> // __entry
if <condition>  // expression + conditional branch
    <code>      // alt-next
<code>          // next
```

becomes (CB=CodeBlock)

```
CB: '__entry'
    ...
    Expr: <condition>
    br-true 'if_alt' : 'cb2'

CB: 'if_alt'
    ...
    br 'cb2'    // fall through

CB: 'cb2'
    ...
    ret
```

---

```
<code> // __entry
loop <condition>    // expression + conditional branch
    continue        // br to loop
    break           // br to next
                    // br back to loop
<code>              // next
```

becomes (CB=CodeBlock)

```
CB: '__entry'
    ...

CB: 'loop'
    Expr: <condition>
    br-true 'alt_next' : 'cb2'

CB: 'alt_next'  // loop code
    ...
    br 'loop'

CB: 'cb2'
    ...
    ret
```
