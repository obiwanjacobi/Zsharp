using System;
using System.Collections.Immutable;

namespace Maja.Compiler.IR.Lower;

internal abstract class IrRewriter
{
    protected virtual IrProgram RewriteProgram(IrProgram program)
    {
        var module = RewriteModule(program.Module);

        // compilation
        var exports = RewriteExports(program.Root.Exports);
        var imports = RewriteImports(program.Root.Imports);
        var declarations = RewriteDeclarations(program.Root.Declarations);
        var statements = RewriteStatements(program.Root.Statements);

        var compilationIsUnchanged =
            exports == program.Root.Exports &&
            imports == program.Root.Imports &&
            declarations == program.Root.Declarations &&
            statements == program.Root.Statements;

        if (module == program.Module &&
            compilationIsUnchanged)
            return program;

        return new IrProgram(program.Syntax, program.Scope, module,
            compilationIsUnchanged ? program.Root : new IrCompilation(
                program.Root.Syntax, imports, exports, statements, declarations),
            program.Diagnostics);
    }

    protected virtual IrModule RewriteModule(IrModule module)
    {
        return module;
    }

    protected virtual ImmutableArray<IrExport> RewriteExports(ImmutableArray<IrExport> exports)
    {
        return RewriteArray(exports, RewriteExport);
    }

    protected virtual IrExport RewriteExport(IrExport export)
    {
        return export;
    }

    protected virtual ImmutableArray<IrImport> RewriteImports(ImmutableArray<IrImport> imports)
    {
        return RewriteArray(imports, RewriteImport);
    }

    protected virtual IrImport RewriteImport(IrImport import)
    {
        return import;
    }

    protected virtual IrCodeBlock RewriteCodeBlock(IrCodeBlock codeBlock)
    {
        var declarations = RewriteDeclarations(codeBlock.Declarations);
        var statement = RewriteStatements(codeBlock.Statements);

        if (declarations == codeBlock.Declarations &&
            statement == codeBlock.Statements)
            return codeBlock;

        return new IrCodeBlock(codeBlock.Syntax, statement, declarations);
    }

    protected virtual ImmutableArray<IrDeclaration> RewriteDeclarations(ImmutableArray<IrDeclaration> declarations)
    {
        return RewriteArray(declarations, RewriteDeclaration);
    }

    protected virtual IrDeclaration RewriteDeclaration(IrDeclaration declaration)
    {
        return declaration switch
        {
            IrDeclarationFunction funDecl => RewriteDeclarationFunction(funDecl),
            IrDeclarationType typeDecl => RewriteDeclarationType(typeDecl),
            IrDeclarationVariable varDecl => RewriteDeclarationVariable(varDecl),
            _ => declaration
        };
    }

    protected virtual IrDeclarationFunction RewriteDeclarationFunction(IrDeclarationFunction function)
    {
        var typeParameters = RewriteTypeParameters(function.TypeParameters);
        var parameters = RewriteParameters(function.Parameters);
        var retType = RewriteType(function.ReturnType) ?? IrType.Void;
        var body = RewriteCodeBlock(function.Body);

        if (typeParameters == function.TypeParameters &&
            parameters == function.Parameters &&
            retType == function.ReturnType &&
            body == function.Body)
            return function;

        return new IrDeclarationFunction(function.Syntax, function.Symbol, typeParameters, parameters, retType, function.Scope, body);
    }

    protected virtual ImmutableArray<IrParameter> RewriteParameters(ImmutableArray<IrParameter> parameters)
    {
        return RewriteArray(parameters, RewriteParameter);
    }

    protected virtual IrParameter RewriteParameter(IrParameter parameter)
    {
        var type = RewriteType(parameter.Type);

        if (type == parameter.Type)
            return parameter;

        return new IrParameter(parameter.Syntax, parameter.Symbol, type!);
    }

    protected virtual ImmutableArray<IrTypeParameter> RewriteTypeParameters(ImmutableArray<IrTypeParameter> parameters)
    {
        return RewriteArray(parameters, RewriteTypeParameter);
    }

    protected virtual IrTypeParameter RewriteTypeParameter(IrTypeParameter parameter)
    {
        return parameter switch
        {
            IrTypeParameterGeneric tpg => RewriteTypeParameterGeneric((IrTypeParameterGeneric)parameter),
            _ => throw new NotSupportedException($"Ir: TypeParameter {parameter} is not supported.")
        };
    }

    protected virtual IrTypeParameterGeneric RewriteTypeParameterGeneric(IrTypeParameterGeneric parameter)
    {
        var type = RewriteType(parameter.Type);

        if (type == parameter.Type)
            return parameter;

        return new IrTypeParameterGeneric(parameter.Syntax, type!, parameter.Symbol);
    }

    protected virtual IrDeclarationType RewriteDeclarationType(IrDeclarationType type)
    {
        var enums = RewriteEnums(type.Enums);
        var fields = RewriteFields(type.Fields);
        var rules = RewriteRules(type.Rules);

        if (enums == type.Enums &&
            fields == type.Fields &&
            rules == type.Rules)
            return type;

        return new IrDeclarationType(type.Syntax, type.Symbol, enums, fields, rules);
    }

    protected virtual ImmutableArray<IrTypeMemberEnum> RewriteEnums(ImmutableArray<IrTypeMemberEnum> memberEnums)
    {
        return RewriteArray(memberEnums, RewriteEnum);
    }

    protected virtual IrTypeMemberEnum RewriteEnum(IrTypeMemberEnum memberEnum)
    {
        return memberEnum;
    }

    protected virtual ImmutableArray<IrTypeMemberField> RewriteFields(ImmutableArray<IrTypeMemberField> memberFields)
    {
        return RewriteArray(memberFields, RewriteField);
    }

    protected virtual IrTypeMemberField RewriteField(IrTypeMemberField memberField)
    {
        return memberField;
    }

    protected virtual ImmutableArray<IrTypeMemberRule> RewriteRules(ImmutableArray<IrTypeMemberRule> memberRules)
    {
        return RewriteArray(memberRules, RewriteRule);
    }

    protected virtual IrTypeMemberRule RewriteRule(IrTypeMemberRule memberRule)
    {
        return memberRule;
    }

    protected virtual IrDeclarationVariable RewriteDeclarationVariable(IrDeclarationVariable variable)
    {
        var initializer = RewriteExpression(variable.Initializer);

        if (initializer == variable.Initializer)
            return variable;

        return new IrDeclarationVariable(variable.Syntax, variable.Symbol, variable.TypeSymbol, initializer);
    }

    protected virtual IrType? RewriteType(IrType? type)
    {
        return type;
    }

    protected virtual ImmutableArray<IrStatement> RewriteStatements(ImmutableArray<IrStatement> statements)
    {
        return RewriteArray(statements, RewriteStatement);
    }

    protected virtual IrStatement RewriteStatement(IrStatement statement)
    {
        return statement switch
        {
            IrStatementIf statIf => RewriteStatementIf(statIf),
            IrStatementExpression statExpr => RewriteStatementExpression(statExpr),
            IrStatementLoop statLoop => RewriteStatementLoop(statLoop),
            IrStatementReturn statRet => RewriteStatementReturn(statRet),
            _ => statement
        };
    }

    protected virtual IrStatement RewriteStatementExpression(IrStatementExpression statement)
    {
        var expr = RewriteExpression(statement.Expression);

        if (expr == statement.Expression)
            return statement;

        return new IrStatementExpression(statement.Syntax, expr!);
    }

    protected virtual IrStatement RewriteStatementIf(IrStatementIf statement)
    {
        var condition = RewriteExpression(statement.Condition);
        var codeBlock = RewriteCodeBlock(statement.CodeBlock);
        var elseClause = RewriteElseClause(statement.ElseClause);
        var elifClause = RewriteElseIfClause(statement.ElseIfClause);

        if (condition == statement.Condition &&
            codeBlock == statement.CodeBlock &&
            elseClause == statement.ElseClause &&
            elifClause == statement.ElseIfClause)
            return statement;

        return new IrStatementIf(statement.Syntax, condition!, codeBlock, elseClause, elifClause);
    }

    private IrElseClause? RewriteElseClause(IrElseClause? elseClause)
    {
        if (elseClause is null) return null;

        var codeBlock = RewriteCodeBlock(elseClause.CodeBlock);

        if (codeBlock == elseClause.CodeBlock)
            return elseClause;

        return new IrElseClause(elseClause.Syntax, codeBlock);
    }

    private IrElseIfClause? RewriteElseIfClause(IrElseIfClause? elseIfClause)
    {
        if (elseIfClause is null) return null;

        var condition = RewriteExpression(elseIfClause.Condition);
        var codeBlock = RewriteCodeBlock(elseIfClause.CodeBlock);
        var elseClause = RewriteElseClause(elseIfClause.ElseClause);
        var elifClause = RewriteElseIfClause(elseIfClause.ElseIfClause);

        if (condition == elseIfClause.Condition &&
            codeBlock == elseIfClause.CodeBlock &&
            elseClause == elseIfClause.ElseClause &&
            elifClause == elseIfClause.ElseIfClause)
            return elseIfClause;

        return new IrElseIfClause(elseIfClause.Syntax, condition!, codeBlock, elseClause, elifClause);
    }

    protected virtual IrStatement RewriteStatementReturn(IrStatementReturn statement)
    {
        var expr = RewriteExpression(statement.Expression);

        if (expr == statement.Expression)
            return statement;

        return new IrStatementReturn(statement.Syntax, expr);
    }

    protected virtual IrStatement RewriteStatementLoop(IrStatementLoop statement)
    {
        return statement;
    }

    protected virtual IrExpression? RewriteExpression(IrExpression? expression)
    {
        return expression switch
        {
            IrExpressionBinary exprBin => RewriteExpressionBinary(exprBin),
            IrExpressionIdentifier exprId => RewriteExpressionIdentifier(exprId),
            IrExpressionInvocation exprInv => RewriteExpressionInvocation(exprInv),
            IrExpressionLiteral exprLit => RewriteExpressionLiteral(exprLit),
            _ => expression
        };
    }

    protected virtual IrExpressionBinary RewriteExpressionBinary(IrExpressionBinary expression)
    {
        var left = RewriteExpression(expression.Left);
        var right = RewriteExpression(expression.Right);
        var op = RewriteBinaryOperator(expression.Operator);

        if (left == expression.Left &&
            right == expression.Right &&
            op == expression.Operator)
            return expression;

        return new IrExpressionBinary(expression.Syntax, left!, expression.Operator, right!);
    }

    protected virtual IrBinaryOperator RewriteBinaryOperator(IrBinaryOperator @operator)
    {
        return @operator;
    }

    protected virtual IrExpressionIdentifier RewriteExpressionIdentifier(IrExpressionIdentifier expression)
    {
        return expression;
    }

    protected virtual IrExpressionInvocation RewriteExpressionInvocation(IrExpressionInvocation expression)
    {
        var typeArgs = RewriteTypeArguments(expression.TypeArguments);
        var args = RewriteArguments(expression.Arguments);

        if (args == expression.Arguments && typeArgs == expression.TypeArguments)
            return expression;

        return new IrExpressionInvocation(expression.Syntax, expression.Symbol, typeArgs, args, expression.TypeSymbol);
    }

    protected virtual ImmutableArray<IrArgument> RewriteArguments(ImmutableArray<IrArgument> arguments)
    {
        return RewriteArray(arguments, RewriteArgument);
    }

    protected virtual IrArgument RewriteArgument(IrArgument argument)
    {
        var expr = RewriteExpression(argument.Expression);

        if (expr == argument.Expression)
            return argument;

        return new IrArgument(argument.Syntax, expr!, argument.Symbol);
    }

    protected virtual ImmutableArray<IrTypeArgument> RewriteTypeArguments(ImmutableArray<IrTypeArgument> typeArguments)
    {
        return RewriteArray(typeArguments, RewriteTypeArgument);
    }

    protected virtual IrTypeArgument RewriteTypeArgument(IrTypeArgument typeArgument)
    {
        var type = RewriteType(typeArgument.Type);

        if (type == typeArgument.Type)
            return typeArgument;

        return new IrTypeArgument(typeArgument.Syntax, type!);
    }

    protected virtual IrExpressionLiteral RewriteExpressionLiteral(IrExpressionLiteral expression)
    {
        return expression;
    }

    private static ImmutableArray<T> RewriteArray<T>(ImmutableArray<T> array, Func<T, T?> itemRewriter)
        where T : class
    {
        ImmutableArray<T>.Builder? builder = null;

        for (var i = 0; i < array.Length; i++)
        {
            var oldItem = array[i];
            var newItem = itemRewriter(oldItem);

            if (oldItem != newItem &&
                builder is null)
            {
                builder = ImmutableArray.CreateBuilder<T>(array.Length);

                // add items before first new one
                for (var j = 0; j < i; j++)
                    builder.Add(array[j]);
            }

            if (builder is not null &&
                newItem is not null)
                builder.Add(newItem);
        }

        return builder is null
            ? array
            : builder.MoveToImmutable();
    }
}
