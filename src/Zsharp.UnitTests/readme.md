# Tested

Keeping track of the progress of Z# features.

Columns:

- Feature: the language feature (syntax)
- Grammar: does it parse and show up in the parse tree
- Ast: is there an Ast node to represent the feature
- Semantics: are the related semantics implemented
- Emit: is code emitted to represent the feature
- Runtime: does the feature work at runtime
- Description: example, description, notes

Values:

- (blank) = TODO
- \- = not applicable
- x = supported
- ? = unknown
- R = Resolve Definitions (Semantics)

---

|Feature      |   |Grammar|Ast|Semantics|Emit|Runtime|Description|
|-------------|--------------|:-:|:-:|:-:|:-:|---|---------------|
| comments    |              | x |   | - |   |   | `// comment`  |

---

## Module

- decl
- import
- export

|Feature         |   |Grammar|Ast|Semantics|Emit|Runtime|Description|
|----------------|--------------|:-:|:-:|:-:|:-:|---|---------------------|
| module | decl                 | x |   | R |   |   | `module myModule`   |
| module | import               | x |   | R |   |   | `use yourModule` |
| export | func                 | x | x | R | x |   | `pub myFunc`     |
| export | enum                 | x | x | R | x |   | `pub myEnum`     |
| export | struct               | x | x | R | x |   | `pub myStruct`   |
| export | in-place             | x |   | R |   |   | `pub fn: ()`...  |

---

## File

- top level vars
- top level types
- top level funcs - see Function

|Feature      |   |Grammar|Ast|Semantics|Emit|Runtime|Description|
|-------------|--------------|:-:|:-:|:-:|:-:|---|---------------|
| var         | decl         | x | x | R | x |   | ::`v: U8`     |
|             | assign       | x | x | R | x |   | ::`v = 42`    |
|             | typed assign | x | x | R | x |   | ::`v: U8 = 42` |
| func        | ref          | x |   | R |   |   | ::`fn()`      |
| func        | ref self     | x |   | R |   |   | ::`x.fn()`    |

---

## Function

- decl
- ref
- parameters / retval
- bound
- template
- generic
- alias

|Feature  |   |Grammar|Ast|Semantics|Emit|Runtime|Description|
|---------|-----------|:-:|:-:|:-:|:-:|---|---------------|
| decl    | Void      | x | x | R | x |   | `fn: ()`...   |
| decl    | param1    | x | x | R | x |   | `fn: (p: U8)`... |
| decl    | param2    | x | x | R |   |   | `fn: (p: U8, s: Str)`... |
| decl    | self      | x | x | R |   |   | `fn: (self: U8)`... |
| decl    | retval    | x | x | R | x |   | `fn: (): Bool`... |
| decl    | template  | x | x | R |   |   | `fn: <#T>()`        |
| decl    | generic   | x | x | R |   |   | `fn: <T>()`... |
| ref     | Void      | x | x | R |   |   | `fn()`        |
| ref     | retval    | x | x | R | x |   | `x = fn()`    |
| ref     | self      | x | x | R |   |   | `x.fn()`      |
| ref     | template  | x | x | R |   |   | `fn<U8>()`    |
| ref     | external  | x | x | R | x |   | `import`/`extern()` |
| use     | template  | x | x | R |   |   | `T()`         |
| use     | convert   | x | x | R |   |   | `U16(x)`      |

---

## Variable

- decl
- ref => see expressions

|Feature      |   |Grammar|Ast|Semantics|Emit|Runtime|Description|
|-------------|--------------|:-:|:-:|:-:|:-:|---|---------------|
| decl        |              | x | x | R |   |   | `v: U8`       |
| decl        | assign       | x | x | R | x |   | `v = 42`      |
| decl        | expr         | x | x | R | x |   | `v = 101 + 42` |
| decl        | typed assign | x | x | R |   |   | `v: U8 = 42`  |
| ref         | (v)          | x | x | R | x |   | `a = v + 42`  |
| nav         | field        | x | x | R |   |   | `s.fld1`      |

---

## Types

|Feature      |   |Grammar|Ast|Semantics|Emit|Runtime|Description|
|-------------|--------------|:-:|:-:|:-:|:-:|---|---------------|
| Str         | ref         | x | x | R |   |   | `b: Str`       |
| Bit\<n>     | ref         | x | x | R |   |   | `b: Bit<4>`    |
| Ptr\<T>     | ref         | x | x | R |   |   | `p: Ptr<U8>`   |
| Array\<T>   | ref         | x | x | R |   |   | `a: Array<U8>` |

### Struct

|Feature      |   |Grammar|Ast|Semantics|Emit|Runtime|Description|
|-------------|--------------|:-:|:-:|:-:|:-:|---|---------------|
| decl        |              | x | x | R | x |   | `MyStruct`... |
| decl        | base         | x | x | R |   |   | `MyStruct: Base`... |
| decl        | template     | x | x | R |   |   | `MyStruct<#T>`... |
| decl        | generic      | x | x | R |   |   | `MyStruct<T>`... |
| ref         | fld init     | x | x | R | x |   | `a = MyStruct`... |
| ref         | template     | x | x | R |   |   | `a = MyStruct<U8>`... |

---

### Enum

|Feature      |   |Grammar|Ast|Semantics|Emit|Runtime|Description|
|-------------|--------------|:-:|:-:|:-:|:-:|---|---------------|
| enum        | decl - comma | x | x | R |   |   | `opt1, opt2`  |
| enum        | decl - list  | x | x | R |   |   | `opt1\n\topt2`|
| enum        | decl - val   | x | x | R | x |   | `opt0 = 0`    |
| enum        | decl - str   | x | x | R | x |   | `MyEnum: Str` |
| enum      | decl - str-val | x | x | R |   |   | `opt1 = "1"`  |

---

## Flow

|Feature      |   |Grammar|Ast|Semantics|Emit|Runtime|Description|
|-------------|----------|:-:|:-:|:-:|:-:|---|---------------|
| if          |          | x | x |   | x |   | `if true`     |
| if          | nested   | x | x |   | x |   | `if true`...  |
| if-else     |          | x | x |   | x |   | `if true - else` |
| if-else     | if-else  | x | x |   | x |   | `if true - else if false - else` |
| loop        |          | x | x |   |   |   | `loop`        |
| loop        | expr     | x | x |   |   |   | `loop x < 42` |
| loop        | iter     | x | x |   |   |   | `loop x in [0..42]` |
| return      |          | x | x |   | x |   | `return`      |
| return      | expr     | x | x |   |   |   | `return 42`   |

---

## Expressions

|Feature      |   |Grammar|Ast|Semantics|Emit|Runtime|Description|
|-------------|----------|:-:|:-:|:-:|:-:|---|---------------|
| precedence  |          | x | x |   |   |   | `(...) * (...)` |
| arithmetic  | literals | x | x |   | x |   | `42 + 101`    |
| arithmetic  | var      | x | x | R |   |   | `x + y`       |
| comparison  | var      | x | x | R | x |   | `x < 42`      |
| logic       | literal  | x | x |   | x |   | `not (42 > 101)` |
| logic       | var      | x | x | R |   |   | `42 > x and 101 < y` |
| bitwise     | shift    | x | x | R |   |   | `x >> 4` |

---
