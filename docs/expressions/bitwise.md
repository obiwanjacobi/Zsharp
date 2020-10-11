# Bitwise Expressions

Bitwise operators result in a value of the same type as the left operand manipulated at the bit level.

|Operator|Function
|--------|------
| & | And
| \| | Or
| ^ | Xor
| ~ | Negate / Invert
| >> | Shift Right*
| << | Shift Left
| >\| | Rotate Right
| \|< | Rotate Left

> *) For signed data types an arithmetic shift is used. For unsigned data types a logical shift is used.

An arithmetic shift sign-extends the result, which means that positive numbers remain positive and negative numbers remain negative.

A rotate shift takes the bit that is shifted out and puts that into the bit position now freed up by the shift operation.

```C#
a = 0b10101010 & 0b10010011    // 0b10000010
b = ~0b11110000                // 0b00001111
c = 0b11100011 >> 1            // 0b01110001
d = 0b00110011 >| 1            // 0b10011001
```

Note that the negate `~` operator is a unary operator and takes only one operand - the value it acts upon.
