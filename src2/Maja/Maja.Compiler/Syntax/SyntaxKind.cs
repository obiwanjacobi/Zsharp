namespace Maja.Compiler.Syntax;

public enum SyntaxKind
{
    Unknown = 0,

    CompilationUnit = 10,
    CodeBlock = 11,

    ModuleDirective = 100,
    PublicExportDirective = 101,
    UseImportDirective = 102,

    FunctionDeclaration = 1000,
    FunctionParameter = 1001,
    FunctionArgument = 1002,

    TypedVariableDeclaration = 1500,
    InferredVariableDeclaration = 1501,

    TypeDeclaration = 2000,
    GenericTypeParameter = 2001,
    TemplateTypeParameter = 2002,
    ValueTypeParameter = 2003,
    Type = 2004,
    TypeArgument = 2005,

    MemberList = 2500,
    MemberField = 2501,
    MemberEnum = 2502,
    MemberRule = 2503,

    ExpressionOperator = 3000,
    UnaryExpression = 3001,
    BinaryExpression = 3002,
    TernaryExpression = 3003,
    InvocationExpression = 3004,
    IdentifierExpression = 3005,
    LiteralExpression = 3006,
    LiteralBoolExpression = 3007,
    LiteralNumber = 3008,
    LiteralString = 3009,

    StatementExpression = 4000,
    StatementIf = 4001,
    StatementElse = 4002,
    StatementElseIf = 4003,
    StatementReturn = 4004,
    StatementAssignment = 4005,

    NameIdentifier = 5000,
    QualifiedNameIdentifier = 5001,
}
