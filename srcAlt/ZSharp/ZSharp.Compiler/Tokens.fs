module ZSharp.Tokens

open FParsec
open Ast

let symbolEquals = '='

// whitespace

let space: VoidParser = skipChar ' ' >>% ()
let optSpace: VoidParser = optional (skipChar ' ') >>% ()
let eol: VoidParser = skipNewline


// keywords

let kwModule: VoidParser = pstring "module" >>% ()
let kwUse: VoidParser = pstring "use" >>% ()
let kwPub: VoidParser = pstring "pub" >>% ()
let kwLoop: VoidParser = pstring "loop" >>% ()
let kwDo: VoidParser = pstring "do" >>% ()
let kwIn: VoidParser = pstring "in" >>% ()
let kwIf: VoidParser = pstring "if" >>% ()
let kwElse: VoidParser = pstring "else" >>% ()
let kwSelf: VoidParser = pstring "self" >>% ()
let kwTrue: VoidParser = pstring "true" >>% ()
let kwFalse: VoidParser = pstring "false" >>% ()

// operators

let opAnd: VoidParser = pstring "and" >>% ()
let opOr: VoidParser = pstring "or" >>% ()
let opNot: VoidParser = pstring "not" >>% ()

// tokens

let tokColon: VoidParser = pstring ": " >>% ()
let tokDot: VoidParser = pstring "." >>% ()
let tokRange: VoidParser = pstring ".." >>% ()
let tokSpread: VoidParser = pstring "..." >>% ()
let tokCompTime: VoidParser = pstring "#" >>% ()

// intrinsic symbols

let symDiscard: VoidParser = pstring "_ " >>% ()
let symComma: VoidParser = pstring ", " >>% ()

// others

let commentStart: VoidParser = pstring "__" >>% ()
