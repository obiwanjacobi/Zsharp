using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR;

internal abstract class IrRewriter
{
    protected DiagnosticList Diagnostics = new();

    protected virtual IEnumerable<IrProgram> RewriteProgram(IrProgram program)
    {
        var modules = RewriteModule(program.Module);

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

        if (modules.IsExactly(program.Module) &&
            !Diagnostics.HasDiagnostics &&
            compilationIsUnchanged)
            return [program];

        var diagnostics = new DiagnosticList();
        diagnostics.AddRange(program.Diagnostics);
        diagnostics.AddRange(Diagnostics);

        var programs = new List<IrProgram>();
        foreach (var module in modules)
        {
            var prog = new IrProgram(program.Syntax, program.Scope, module,
            compilationIsUnchanged ? program.Root : new IrCompilation(
                program.Root.Syntax, imports, exports, statements, declarations),
            diagnostics);

            programs.Add(prog);
        }

        return programs;
    }

    protected virtual IEnumerable<IrModule> RewriteModule(IrModule module)
    {
        return [module];
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

    protected virtual IEnumerable<IrDeclaration> RewriteDeclaration(IrDeclaration declaration)
    {
        return declaration switch
        {
            IrDeclarationFunction funDecl => RewriteDeclarationFunction(funDecl),
            IrDeclarationType typeDecl => RewriteDeclarationType(typeDecl),
            IrDeclarationVariable varDecl => RewriteDeclarationVariable(varDecl),
            _ => throw new NotSupportedException($"IrRewriter: Declaration of type {declaration.GetType().Name} is not supported.")
        };
    }

    protected virtual IEnumerable<IrDeclarationFunction> RewriteDeclarationFunction(IrDeclarationFunction function)
    {
        var typeParameters = RewriteTypeParameters(function.TypeParameters);
        var parameters = RewriteParameters(function.Parameters);
        var retTypes = RewriteType(function.ReturnType);
        var body = RewriteCodeBlock(function.Body);

        var retType = retTypes.SingleOrDefault(IrType.Void);

        if (typeParameters == function.TypeParameters &&
            parameters == function.Parameters &&
            retType == function.ReturnType &&
            body == function.Body)
            return [function];

        // TODO: needs deeper replacement for templates!
        var functionSymbol = new DeclaredFunctionSymbol(
            function.Symbol.Name,
            typeParameters.Select(tp => tp.Symbol),
            parameters.Select(p => p.Symbol),
            retType.Symbol
        );

        return [new IrDeclarationFunction(function.Syntax, functionSymbol, typeParameters, parameters, retType, function.Scope, body, function.Locality)];
    }

    protected virtual ImmutableArray<IrParameter> RewriteParameters(ImmutableArray<IrParameter> parameters)
    {
        return RewriteArray(parameters, RewriteParameter);
    }

    protected virtual IEnumerable<IrParameter> RewriteParameter(IrParameter parameter)
    {
        var types = RewriteType(parameter.Type);
        var type = types.Single();

        if (type == parameter.Type)
            return [parameter];

        var paramSymbol = new ParameterSymbol(parameter.Symbol.Name, type.Symbol);
        return [new IrParameter(parameter.Syntax, paramSymbol, type!)];
    }

    protected virtual ImmutableArray<IrTypeParameter> RewriteTypeParameters(ImmutableArray<IrTypeParameter> parameters)
    {
        return RewriteArray(parameters, RewriteTypeParameter);
    }

    protected virtual IrTypeParameter RewriteTypeParameter(IrTypeParameter parameter)
    {
        return parameter switch
        {
            IrTypeParameterGeneric tpg => RewriteTypeParameterGeneric(tpg),
            IrTypeParameterTemplate tpt => RewriteTypeParameterTemplate(tpt),
            _ => throw new NotSupportedException($"Ir: TypeParameter {parameter.GetType()} is not supported.")
        };
    }

    protected virtual IrTypeParameter RewriteTypeParameterGeneric(IrTypeParameterGeneric parameter)
    {
        var types = RewriteType(parameter.Type);
        var type = types.SingleOrDefault();

        if (type == parameter.Type)
            return parameter;

        return new IrTypeParameterGeneric(parameter.Syntax, type!, parameter.Symbol);
    }

    protected virtual IrTypeParameter RewriteTypeParameterTemplate(IrTypeParameterTemplate parameter)
    {
        var types = RewriteType(parameter.Type);
        var type = types.SingleOrDefault();

        if (type == parameter.Type)
            return parameter;

        return new IrTypeParameterTemplate(parameter.Syntax, type!, parameter.Symbol);
    }

    protected virtual IEnumerable<IrDeclarationType> RewriteDeclarationType(IrDeclarationType type)
    {
        var typeParams = RewriteTypeParameters(type.TypeParameters);
        var enums = RewriteEnums(type.Enums);
        var fields = RewriteFields(type.Fields);
        var rules = RewriteRules(type.Rules);
        var types = RewriteType(type.BaseType);
        var baseType = types.SingleOrDefault();

        if (typeParams == type.TypeParameters &&
            enums == type.Enums &&
            fields == type.Fields &&
            rules == type.Rules &&
            baseType == type.BaseType)
            return [type];

        // TODO: needs deeper replacement for templates!
        var typeSymbol = new DeclaredTypeSymbol(
            type.Symbol.Name,
            typeParams.Select(tp => tp.Symbol),
            enums.Select(e => e.Symbol),
            fields.Select(f => f.Symbol),
            rules.Select(r => r.Symbol),
            baseType?.Symbol
        );

        return [new IrDeclarationType(type.Syntax, typeSymbol, typeParams, enums, fields, rules, baseType, type.Scope, type.Locality)];
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

    protected virtual IEnumerable<IrDeclarationVariable> RewriteDeclarationVariable(IrDeclarationVariable variable)
    {
        var initializer = RewriteExpression(variable.Initializer);

        if (initializer == variable.Initializer)
            return [variable];

        return [new IrDeclarationVariable(variable.Syntax, variable.Symbol, variable.TypeSymbol, initializer)];
    }

    [return: NotNullIfNotNull(nameof(type))]
    protected virtual IEnumerable<IrType> RewriteType(IrType? type)
    {
        return type is null ? [] : [type];
    }

    protected virtual ImmutableArray<IrStatement> RewriteStatements(ImmutableArray<IrStatement> statements)
    {
        return RewriteArray(statements, RewriteStatement);
    }

    protected virtual IEnumerable<IrStatement> RewriteStatement(IrStatement statement)
    {
        return statement switch
        {
            IrStatementIf statIf => [RewriteStatementIf(statIf)],
            IrStatementExpression statExpr => RewriteStatementExpression(statExpr),
            IrStatementLoop statLoop => RewriteStatementLoop(statLoop),
            IrStatementReturn statRet => RewriteStatementReturn(statRet),
            IrStatementAssignment statAssign => RewriteStatementAssignment(statAssign),
            _ => throw new NotSupportedException($"IrRewriter: Statement of type {statement.GetType().Name} is not supported.")
        };
    }

    protected virtual IEnumerable<IrStatement> RewriteStatementAssignment(IrStatementAssignment statement)
    {
        var expr = RewriteExpression(statement.Expression);

        if (expr == statement.Expression)
            return [statement];

        return [new IrStatementAssignment(statement.Symbol, expr, statement.Locality)];
    }

    protected virtual IEnumerable<IrStatement> RewriteStatementExpression(IrStatementExpression statement)
    {
        var expr = RewriteExpression(statement.Expression);

        if (expr == statement.Expression)
            return [statement];

        return [new IrStatementExpression(statement.Syntax, expr!, statement.Locality)];
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

    [return: NotNullIfNotNull(nameof(elseClause))]
    protected virtual IrElseClause? RewriteElseClause(IrElseClause? elseClause)
    {
        if (elseClause is null) return null;

        var codeBlock = RewriteCodeBlock(elseClause.CodeBlock);

        if (codeBlock == elseClause.CodeBlock)
            return elseClause;

        return new IrElseClause(elseClause.Syntax, codeBlock);
    }

    [return: NotNullIfNotNull(nameof(elseIfClause))]
    protected virtual IrElseIfClause? RewriteElseIfClause(IrElseIfClause? elseIfClause)
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

    protected virtual IEnumerable<IrStatement> RewriteStatementReturn(IrStatementReturn statement)
    {
        var expr = RewriteExpression(statement.Expression);

        if (expr == statement.Expression)
            return [statement];

        return [new IrStatementReturn(statement.Syntax, expr)];
    }

    protected virtual IEnumerable<IrStatement> RewriteStatementLoop(IrStatementLoop statement)
    {
        return [statement];
    }

    [return: NotNullIfNotNull(nameof(expression))]
    protected virtual IrExpression? RewriteExpression(IrExpression? expression)
    {
        return expression switch
        {
            IrExpressionBinary exprBin => RewriteExpressionBinary(exprBin),
            IrExpressionIdentifier exprId => RewriteExpressionIdentifier(exprId),
            IrExpressionInvocation exprInv => RewriteExpressionInvocation(exprInv),
            IrExpressionTypeInitializer exprTi => RewriteExpressionTypeInitializer(exprTi),
            IrExpressionLiteral exprLit => RewriteExpressionLiteral(exprLit),
            IrExpressionMemberAccess exprMemAcc => RewriteExpressionMemberAccess(exprMemAcc),
            _ => expression is null
                ? null
                : throw new NotSupportedException($"IrRewriter: Expression of type {expression.GetType().Name} is not supported.")
        };
    }

    protected virtual IrExpression RewriteExpressionMemberAccess(IrExpressionMemberAccess expression)
    {
        var expr = RewriteExpression(expression.Expression);

        if (expr == expression.Expression)
            return expression;

        return new IrExpressionMemberAccess(expression.Syntax, expression.TypeSymbol, expr, expression.Members);
    }

    protected virtual IrExpression RewriteExpressionBinary(IrExpressionBinary expression)
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

    protected virtual IrExpression RewriteExpressionIdentifier(IrExpressionIdentifier expression)
    {
        return expression;
    }

    protected virtual IrExpression RewriteExpressionInvocation(IrExpressionInvocation expression)
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

    protected virtual IEnumerable<IrArgument> RewriteArgument(IrArgument argument)
    {
        var expr = RewriteExpression(argument.Expression);

        if (expr == argument.Expression)
            return [argument];

        return [new IrArgument(argument.Syntax, expr!, argument.Symbol)];
    }

    protected virtual ImmutableArray<IrTypeArgument> RewriteTypeArguments(ImmutableArray<IrTypeArgument> typeArguments)
    {
        return RewriteArray(typeArguments, RewriteTypeArgument);
    }

    protected virtual IrTypeArgument RewriteTypeArgument(IrTypeArgument typeArgument)
    {
        var types = RewriteType(typeArgument.Type);
        var type = types.Single();

        if (type == typeArgument.Type)
            return typeArgument;

        return new IrTypeArgument(typeArgument.Syntax, type!);
    }

    protected virtual IrExpression RewriteExpressionTypeInitializer(IrExpressionTypeInitializer initializer)
    {
        var typeArgs = RewriteTypeArguments(initializer.TypeArguments);
        var fields = RewriteTypeInitializerFields(initializer.Fields);

        if (typeArgs == initializer.TypeArguments &&
            fields == initializer.Fields)
            return initializer;

        return new IrExpressionTypeInitializer(initializer.Syntax, initializer.TypeSymbol, initializer.TypeArguments, fields);
    }
    protected virtual ImmutableArray<IrTypeInitializerField> RewriteTypeInitializerFields(ImmutableArray<IrTypeInitializerField> fields)
    {
        return RewriteArray(fields, RewriteTypeInitializerField);
    }
    protected virtual IrTypeInitializerField RewriteTypeInitializerField(IrTypeInitializerField initializer)
    {
        var expr = RewriteExpression(initializer.Expression);

        if (expr == initializer.Expression)
            return initializer;

        return new IrTypeInitializerField(initializer.Syntax, initializer.Field, expr!);
    }

    protected virtual IrExpression RewriteExpressionLiteral(IrExpressionLiteral expression)
    {
        return expression;
    }

    private static ImmutableArray<T> RewriteArray<T>(ImmutableArray<T> array, Func<T, T?> itemRewriter)
        where T : class
    {
        var list = new List<T>();

        for (var i = 0; i < array.Length; i++)
        {
            var oldItem = array[i];
            var newItem = itemRewriter(oldItem);
            if (newItem is not null)
                list.Add(newItem);
        }

        return list.ToImmutableArray();
    }

    private static ImmutableArray<T> RewriteArray<T>(ImmutableArray<T> array, Func<T, IEnumerable<T>> itemRewriter)
        where T : class
    {
        var list = new List<T>();
        for (var i = 0; i < array.Length; i++)
        {
            var oldItem = array[i];
            var newItems = itemRewriter(oldItem);
            list.AddRange(newItems);
        }

        return list.ToImmutableArray();
    }
}

// ----------------------------------------------------------------------------

internal abstract class IrCopyRewriter : IrRewriter
{
    protected override IEnumerable<IrArgument> RewriteArgument(IrArgument argument)
    {
        var expression = RewriteExpression(argument.Expression)!;
        return [new IrArgument(argument.Syntax, expression, argument.Symbol)];
    }

    protected override IrBinaryOperator RewriteBinaryOperator(IrBinaryOperator @operator)
        => new IrBinaryOperator(@operator.Kind, @operator.OperandType);

    protected override IrCodeBlock RewriteCodeBlock(IrCodeBlock codeBlock)
    {
        var statements = RewriteStatements(codeBlock.Statements);
        var declarations = RewriteDeclarations(codeBlock.Declarations);
        return new IrCodeBlock(codeBlock.Syntax, statements, declarations);
    }

    protected override IEnumerable<IrDeclarationFunction> RewriteDeclarationFunction(IrDeclarationFunction function)
    {
        var typeParameters = RewriteTypeParameters(function.TypeParameters);
        var parameters = RewriteParameters(function.Parameters);
        var types = RewriteType(function.ReturnType);
        var returnType = types.Single();

        // TODO: the scope parent points to the original scope instance, not the duplicated one!
        var scope = new IrFunctionScope(function.Scope.Name, function.Scope.Parent);
        var codeBlock = RewriteCodeBlock(function.Body);

        return [new IrDeclarationFunction(function.Syntax, function.Symbol, typeParameters, parameters, returnType, scope, codeBlock, function.Locality)];
    }

    protected override IEnumerable<IrDeclarationType> RewriteDeclarationType(IrDeclarationType type)
    {
        var typeParameters = RewriteTypeParameters(type.TypeParameters);
        var enums = RewriteEnums(type.Enums);
        var fields = RewriteFields(type.Fields);
        var rules = RewriteRules(type.Rules);
        // TODO: the scope parent points to the original scope instance, not the duplicated one!
        var scope = new IrTypeScope(type.Scope.Name, type.Scope.Parent);

        return [new IrDeclarationType(type.Syntax, type.Symbol, typeParameters, enums, fields, rules, type.BaseType, scope, type.Locality)];
    }

    protected override IEnumerable<IrDeclarationVariable> RewriteDeclarationVariable(IrDeclarationVariable variable)
    {
        var expression = RewriteExpression(variable.Initializer);
        return [new IrDeclarationVariable(variable.Syntax, variable.Symbol, variable.TypeSymbol, expression)];
    }

    protected override IrExpressionBinary RewriteExpressionBinary(IrExpressionBinary expression)
    {
        var left = RewriteExpression(expression.Left)!;
        var op = RewriteBinaryOperator(expression.Operator);
        var right = RewriteExpression(expression.Right)!;
        return new IrExpressionBinary(left, op, right);
    }

    protected override IrExpressionInvocation RewriteExpressionInvocation(IrExpressionInvocation expression)
    {
        var typeArguments = RewriteTypeArguments(expression.TypeArguments);
        var arguments = RewriteArguments(expression.Arguments);
        return new IrExpressionInvocation(expression.Syntax, expression.Symbol, typeArguments, arguments, expression.TypeSymbol);
    }

    protected override IEnumerable<IrParameter> RewriteParameter(IrParameter parameter)
    {
        var types = RewriteType(parameter.Type);
        var type = types.Single();
        return [new IrParameter(parameter.Syntax, parameter.Symbol, type)];
    }

    protected override IrTypeMemberField RewriteField(IrTypeMemberField memberField)
    {
        var types = RewriteType(memberField.Type);
        var type = types.Single();
        var expression = RewriteExpression(memberField.DefaultValue);
        return new IrTypeMemberField(memberField.Syntax, memberField.Symbol, type, expression);
    }

    protected override IrTypeMemberRule RewriteRule(IrTypeMemberRule memberRule)
    {
        var expression = RewriteExpression(memberRule.Expression);
        return new IrTypeMemberRule(memberRule.Syntax, memberRule.Symbol, expression);
    }

    protected override IEnumerable<IrStatement> RewriteStatementExpression(IrStatementExpression statement)
    {
        var expression = RewriteExpression(statement.Expression);
        return [new IrStatementExpression(statement.Syntax, expression, statement.Locality)];
    }

    protected override IrStatement RewriteStatementIf(IrStatementIf statement)
    {
        var expression = RewriteExpression(statement.Condition);
        var codeBlock = RewriteCodeBlock(statement.CodeBlock);
        var elseClause = RewriteElseClause(statement.ElseClause);
        var elifClause = RewriteElseIfClause(statement.ElseIfClause);

        return new IrStatementIf(statement.Syntax, expression, codeBlock, elseClause, elifClause);
    }

    [return: NotNullIfNotNull(nameof(elseClause))]
    protected override IrElseClause? RewriteElseClause(IrElseClause? elseClause)
    {
        if (elseClause is null) return null;
        var codeBlock = RewriteCodeBlock(elseClause.CodeBlock);
        return new IrElseClause(elseClause.Syntax, codeBlock);
    }

    [return: NotNullIfNotNull(nameof(elseIfClause))]
    protected override IrElseIfClause? RewriteElseIfClause(IrElseIfClause? elseIfClause)
    {
        if (elseIfClause is null) return null;
        var expression = RewriteExpression(elseIfClause.Condition);
        var codeBlock = RewriteCodeBlock(elseIfClause.CodeBlock);
        var elseClause = RewriteElseClause(elseIfClause.ElseClause);
        var elifClause = RewriteElseIfClause(elseIfClause.ElseIfClause);
        return new IrElseIfClause(elseIfClause.Syntax, expression, codeBlock, elseClause, elifClause);
    }

    protected override IEnumerable<IrStatement> RewriteStatementLoop(IrStatementLoop statement)
    {
        var expression = RewriteExpression(statement.Expression);
        var codeBlock = RewriteCodeBlock(statement.CodeBlock);
        return [new IrStatementLoop(statement.Syntax, expression, codeBlock)];
    }

    protected override IEnumerable<IrStatement> RewriteStatementReturn(IrStatementReturn statement)
    {
        var expression = RewriteExpression(statement.Expression);
        return [new IrStatementReturn(statement.Syntax, expression)];
    }

    [return: NotNullIfNotNull("type")]
    protected override IEnumerable<IrType> RewriteType(IrType? type)
    {
        if (type is null) return [];
        var symbol = type.Symbol;
        return [new IrType(type.Syntax, symbol)];
    }

    protected override IrTypeArgument RewriteTypeArgument(IrTypeArgument typeArgument)
    {
        var types = RewriteType(typeArgument.Type);
        var type = types.Single();

        return new IrTypeArgument(typeArgument.Syntax, type);
    }

    protected override IrExpressionTypeInitializer RewriteExpressionTypeInitializer(IrExpressionTypeInitializer initializer)
    {
        var symbol = initializer.TypeSymbol;
        var typeArguments = RewriteTypeArguments(initializer.TypeArguments);
        var fields = RewriteTypeInitializerFields(initializer.Fields);
        return new IrExpressionTypeInitializer(initializer.Syntax, symbol, typeArguments, fields);
    }

    protected override IrTypeInitializerField RewriteTypeInitializerField(IrTypeInitializerField initializer)
    {
        var expression = RewriteExpression(initializer.Expression);
        return new IrTypeInitializerField(initializer.Syntax, initializer.Field, expression);
    }
}
