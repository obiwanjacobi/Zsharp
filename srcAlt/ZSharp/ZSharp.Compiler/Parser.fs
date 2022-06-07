module ZSharp.Parser

open FParsec
open Ast
open Tokens



let getStreamName (stream: CharStream<ParserState>) = Reply stream.Name

let indent (stream: CharStream<ParserState>) = 
    let pindentSize = manySatisfy (fun c -> c = ' ') |>> fun tab -> tab.Length
    let tabSize = pindentSize stream
    if tabSize.Result = 0 then
        Reply 0
    elif stream.UserState.IndentCharCount = 0 then
        // TODO: do we need to fix this ignore?
        updateUserState (fun us -> {us with IndentCharCount = tabSize.Result}) |> ignore
        Reply 1
    else
        let remainder = tabSize.Result % stream.UserState.IndentCharCount
        if remainder > 0 then
            Reply (Error, ErrorMessageList (ErrorMessage.Message "Invalid Indentation") )
        else
            Reply (tabSize.Result / stream.UserState.IndentCharCount)


let makeComment (pos, comment) = { Location = pos; Comment = comment }
let comment = getPosition .>>. (commentStart >>. (restOfLine true)) |>> makeComment

let endOfLine = optional comment .>> skipNewline
let emptyLine = many space .>> skipNewline

let identifier = many1Chars (letter <|> digit) |>> AstIdentifier
let identifier_module = identifier
let identifier_variable = identifier

let indentPosition = indent .>>. getPosition;

// expression

let literalNumber = 
    getPosition .>>. numberLiteral (NumberLiteralOptions.DefaultFloat ||| NumberLiteralOptions.DefaultInteger) "number"
    |>> fun (pos, n) ->
        if n.IsInteger then AstExpression.IntLiteral { Location = pos; Value = int n.String }
        else AstExpression.FloatLiteral { Location = pos; Value = float n.String }
let literalBool = 
    getPosition .>>. (stringReturn "false" false <|> stringReturn "true" true) 
    |>> (fun (pos, b) -> AstExpression.BoolLiteral { AstBoolLiteral.Location = pos; AstBoolLiteral.Value = b })
let pstringLiteral =
    let normalChar = satisfy (fun c -> c <> '\\' && c <> '"')
    // TODO: different escape sequences
    let unescape c = match c with
                     | 'n' -> '\n'
                     | 'r' -> '\r'
                     | 't' -> '\t'
                     | c   -> c
    let escapedChar = pstring "\\" >>. (anyOf "\\nrt\"" |>> unescape)
    between (pstring "\"") (pstring "\"") (manyChars (normalChar <|> escapedChar))
let literalString = 
    getPosition .>>. pstringLiteral 
    |>> (fun (pos, str) -> AstExpression.StringLiteral { Location = pos; Value = str } )

let variableRef =
    getPosition .>>. identifier_variable
    |>> fun (pos, identifier) -> AstExpression.Identifier { Location = pos; Identifier = identifier; }

let opp = new OperatorPrecedenceParser<AstExpression, unit, ParserState>()
let expression = opp.ExpressionParser
opp.TermParser <- 
    choice [
        literalNumber;
        literalBool;
        literalString;
        variableRef .>> optSpace;
        between (pstring "(") (pstring ")" >>. optSpace) expression;
    ]

opp.AddOperator(InfixOperator("+", space, 11, Associativity.Left, fun lhs rhs -> AstExpression.Infix (lhs, rhs, AstExpressionOperator.ArithmeticAdd)))
opp.AddOperator(InfixOperator("-", space, 11, Associativity.Left, fun lhs rhs -> AstExpression.Infix (lhs, rhs, AstExpressionOperator.ArithmeticSubtract)))
opp.AddOperator(InfixOperator("*", space, 12, Associativity.Left, fun lhs rhs -> AstExpression.Infix (lhs, rhs, AstExpressionOperator.ArithmeticMultiply)))
opp.AddOperator(InfixOperator("/", space, 12, Associativity.Left, fun lhs rhs -> AstExpression.Infix (lhs, rhs, AstExpressionOperator.ArithmeticDivide)))
opp.AddOperator(InfixOperator("**", space, 13, Associativity.Left, fun lhs rhs -> AstExpression.Infix (lhs, rhs, AstExpressionOperator.ArithmeticPower)))
opp.AddOperator(InfixOperator("//", space, 13, Associativity.Left, fun lhs rhs -> AstExpression.Infix (lhs, rhs, AstExpressionOperator.ArithmeticRoot)))
opp.AddOperator(PrefixOperator("-", space, 15, false, fun expr -> AstExpression.Prefix (expr, AstExpressionOperator.ArithmeticNegate)))

opp.AddOperator(InfixOperator("=", space, 1, Associativity.None, fun lhs rhs -> AstExpression.Infix (lhs, rhs, AstExpressionOperator.CompareEquals)))
opp.AddOperator(InfixOperator("<", space, 1, Associativity.None, fun lhs rhs -> AstExpression.Infix (lhs, rhs, AstExpressionOperator.CompareSmaller)))
opp.AddOperator(InfixOperator(">", space, 1, Associativity.None, fun lhs rhs -> AstExpression.Infix (lhs, rhs, AstExpressionOperator.CompareGreater)))
opp.AddOperator(InfixOperator(">=", space, 1, Associativity.None, fun lhs rhs -> AstExpression.Infix (lhs, rhs, AstExpressionOperator.CompareGreaterEquals)))
opp.AddOperator(InfixOperator("=<", space, 1, Associativity.None, fun lhs rhs -> AstExpression.Infix (lhs, rhs, AstExpressionOperator.CompareSmallerEquals)))

opp.AddOperator(InfixOperator("and", space, 1, Associativity.None, fun lhs rhs -> AstExpression.Infix (lhs, rhs, AstExpressionOperator.LogicAnd)))
opp.AddOperator(InfixOperator("or", space, 1, Associativity.None, fun lhs rhs -> AstExpression.Infix (lhs, rhs, AstExpressionOperator.LogicAnd)))
opp.AddOperator(PrefixOperator("not", space, 1, false, fun expr -> AstExpression.Prefix (expr, AstExpressionOperator.LogicAnd)))

// module

let pmodule = 
    (getPosition .>> kwModule) .>>. identifier_module 
    |>> (fun (pos, identifier) -> { Location = pos; ModuleName = identifier })

// statements

let statement, statementImpl = createParserForwardedToRef()

let variableList = 
    getPosition .>>. sepBy1 identifier_variable (symComma .>> optional space)
    |>> (fun (pos, vars) -> { Location = pos; Variables = vars })

let assignment = 
    pipe4 indentPosition (variableList .>> space) (pchar symbolEquals) (space >>. expression) 
        (fun (indent, pos) identifiers _ expression ->
            {   AstAssignment.Indent = indent;
                AstAssignment.Location = pos;
                AstAssignment.Variables = identifiers;
                AstAssignment.Expression = expression;
            } )

let loopConditional =
    pipe3 indentPosition kwLoop (opt (space >>. expression))
        (fun (indent, pos) _ expression -> { Indent = indent; Location = pos; Expression = expression })

let loopRange =
    indentPosition .>>. ((kwLoop .>> space) >>. (variableList .>> ((space >>. kwIn) .>> space))) .>>. expression
    |>> (fun (((indent, pos), vars), expression) ->
        { 
            AstLoopRange.Indent = indent;
            AstLoopRange.Location = pos;
            AstLoopRange.Variables = vars;
            AstLoopRange.Expression = expression;
        })

let loop = choice [
    attempt loopRange |>> RangeLoop;
    loopConditional |>> ConditionalLoop;
]

let codeblock =
    // TODO: how to parse empty lines with spaces (and return an Ast type?)
    // many emptyLine
    many (spaces >>. comment)
    .>>. many statement
    |>> (fun (comments, statements) -> { Comments = comments; Statements = statements })


statementImpl.Value <- 
    choice [
        attempt (assignment .>> endOfLine) |>> AstStatement.Assignment;
        attempt (loop .>> endOfLine) |>> AstStatement.Loop;
    ]

let file = 
    getStreamName .>>. codeblock .>> eof
    |>> (fun (name, codeblock) -> { Name = name; Module = None; CodeBlock = codeblock } )

// entry points

let runParser parser (code: string) (name: string option) =
    runParserOnString parser ParserState.Default (if name.IsSome then name.Value else "") code

let parseFile (code: string) (name: string option) =
    runParser file code name
