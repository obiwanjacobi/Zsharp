﻿using System;
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
            IrFunctionDeclaration funDecl => RewriteFunctionDeclaration(funDecl),
            IrTypeDeclaration typeDecl => RewriteTypeDeclaration(typeDecl),
            IrVariableDeclaration varDecl => RewriteVariableDeclaration(varDecl),
            _ => declaration
        };
    }

    protected virtual IrFunctionDeclaration RewriteFunctionDeclaration(IrFunctionDeclaration function)
    {
        var parameters = RewriteParameters(function.Parameters);
        var retType = RewriteType(function.ReturnType);
        var body = RewriteCodeBlock(function.Body);

        if (parameters == function.Parameters &&
            retType == function.ReturnType &&
            body == function.Body)
            return function;

        return new IrFunctionDeclaration(function.Syntax, function.Symbol, parameters, retType, function.Scope, body);
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

    protected virtual IrTypeDeclaration RewriteTypeDeclaration(IrTypeDeclaration type)
    {
        var enums = RewriteEnums(type.Enums);
        var fields = RewriteFields(type.Fields);
        var rules = RewriteRules(type.Rules);

        if (enums == type.Enums &&
            fields == type.Fields &&
            rules == type.Rules)
            return type;

        return new IrTypeDeclaration(type.Syntax, type.Symbol, enums, fields, rules);
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

    protected virtual IrVariableDeclaration RewriteVariableDeclaration(IrVariableDeclaration variable)
    {
        var initializer = RewriteExpression(variable.Initializer);

        if (initializer == variable.Initializer)
            return variable;

        return new IrVariableDeclaration(variable.Syntax, variable.Symbol, variable.TypeSymbol, initializer);
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

    protected virtual IrExpression RewriteExpressionBinary(IrExpressionBinary expression)
    {
        var left = RewriteExpression(expression.Left);
        var right = RewriteExpression(expression.Right);

        if (left == expression.Left &&
            right == expression.Right)
            return expression;

        return new IrExpressionBinary(expression.Syntax, left!, expression.Operator, right!);
    }

    protected virtual IrExpression RewriteExpressionIdentifier(IrExpressionIdentifier expression)
    {
        return expression;
    }

    protected virtual IrExpression RewriteExpressionInvocation(IrExpressionInvocation expression)
    {
        var args = RewriteArguments(expression.Arguments);

        if (args == expression.Arguments)
            return expression;

        return new IrExpressionInvocation(expression.Syntax, expression.Symbol, args, expression.TypeSymbol);
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

    protected virtual IrExpression RewriteExpressionLiteral(IrExpressionLiteral expression)
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