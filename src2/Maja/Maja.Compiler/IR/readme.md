# Intermediate Representation

This model represents the analyzed and type-checked code.
It is also used to lower the code constructs into simpler forms.

## Type Checking and Inferrence

### Expressions

Rules for Type inferrence in expressions.

- There is no implicit type conversion, except for literals.
- Operand overloads (on operators) exist to work with different type (add I32 + F32).
- Operator's result-types are bigger than the operand types up until 64 bits.

Desicion: `IrExpressionLiteral` are not rewritten and will always keep their `TypeInferredSymbol`.

#### Literals

Only the types of literals are coerced to the fit the context in which they are used.

- Prefer the smallest type size to fit the literal (Currently defaults to I64).
- prefer signed over unsigned.
- Upgrade literal types to fit the largest in the expression

IrExpressionLiteral Type candidates are based on integer or floating point number or string or boolean.

```csharp
// for 42 with TypeSymbol: I8-I64 and U8-U64.
// without further forces it results in the smallest size: I8
v := 42


// for 42 is upgraded to I16-I64 and U16-U64 because of 2112 => I16
v := 42 + 2112


// for 42 is upgraded to F32-F96 because 3.14 is a floating point number => F32
v := 42 + 3.14


// literal 42 is upgraded to I64 to match variable size
v : I64 = 42


fn: (p: U32)
  ...
// for 42 with TypeSymbol U32 based on function parameter type.
fn(42)
```

`v := 42 + 2112`

IrDeclarationVariable
  .Name = v
  .TypeSymbol = TypeSymbol.I64
  .Initializer = IrExpressionBinary
    .TypeSymbol.I64
    .Left = IrExpressionLiteral.TypeInferredSymbol (candidates)
    .Right = IrExpressionLiteral.TypeInferredSymbol (candidates)
