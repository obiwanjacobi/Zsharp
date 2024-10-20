﻿using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.IR;

internal abstract class IrWalker<R>
{
    public virtual R Default => default;

    public virtual R AggregateResult(R aggregate, R newResult)
        => newResult;

    public virtual R OnProgram(IrProgram program)
    {
        var result = OnModule(program.Module);
        result = AggregateResult(result, OnCompilation(program.Root));
        return result;
    }
    public virtual R OnCompilation(IrCompilation compilation)
    {
        var result = OnImports(compilation.Imports);
        result = AggregateResult(result, OnExports(compilation.Exports));
        result = AggregateResult(result, OnDeclarations(compilation.Declarations));
        result = AggregateResult(result, OnStatements(compilation.Statements));
        return result;
    }
    public virtual R OnCodeBlock(IrCodeBlock codeBlock)
    {
        var result = OnDeclarations(codeBlock.Declarations);
        result = AggregateResult(result, OnStatements(codeBlock.Statements));
        return result;
    }

    public virtual R OnModule(IrModule module)
        => Default;

    public virtual R OnExports(IEnumerable<IrExport> exports)
        => exports.Select(OnExport)
            .Aggregate(Default, AggregateResult);
    public virtual R OnExport(IrExport export)
        => Default;

    public virtual R OnImports(IEnumerable<IrImport> imports)
    => imports.Select(OnImport)
            .Aggregate(Default, AggregateResult);
    public virtual R OnImport(IrImport import)
        => Default;

    public virtual R OnDeclarations(IEnumerable<IrDeclaration> declarations)
        => declarations.Select(declaration =>
        {
            return declaration switch
            {
                IrDeclarationFunction fd => OnDeclarationFunction(fd),
                IrDeclarationType td => OnDeclarationType(td),
                IrDeclarationVariable vd => OnDeclarationVariable(vd),
                _ => Default
            };
        })
            .Aggregate(Default, AggregateResult);

    public virtual R OnDeclarationFunction(IrDeclarationFunction function)
    {
        var result = OnTypeParametersGeneric(function.TypeParameters.OfType<IrTypeParameterGeneric>());
        result = AggregateResult(result, OnTypeParametersTemplate(function.TypeParameters.OfType<IrTypeParameterTemplate>()));
        result = AggregateResult(result, OnParameters(function.Parameters));
        result = AggregateResult(result, OnOptionalType(result, function.ReturnType));
        result = AggregateResult(result, OnCodeBlock(function.Body));
        return result;
    }
    public virtual R OnParameters(IEnumerable<IrParameter> parameters)
        => parameters.Select(OnParameter)
            .Aggregate(Default, AggregateResult);
    public virtual R OnParameter(IrParameter parameter)
        => OnType(parameter.Type);
    public virtual R OnTypeParametersGeneric(IEnumerable<IrTypeParameterGeneric> parameters)
        => parameters.Select(OnTypeParameterGeneric)
            .Aggregate(Default, AggregateResult);
    public virtual R OnTypeParameterGeneric(IrTypeParameterGeneric parameter)
    {
        if (parameter.Type is not null)
            return OnType(parameter.Type);

        return Default;
    }
    public virtual R OnTypeParametersTemplate(IEnumerable<IrTypeParameterTemplate> parameters)
        => parameters.Select(OnTypeParameterTemplate)
            .Aggregate(Default, AggregateResult);
    public virtual R OnTypeParameterTemplate(IrTypeParameterTemplate parameter)
    {
        if (parameter.Type is not null)
            return OnType(parameter.Type);

        return Default;
    }

    public virtual R OnDeclarationType(IrDeclarationType type)
    {
        var result = OnTypeMemberEnums(type.Enums);
        result = AggregateResult(result, OnTypeMemberFields(type.Fields));
        result = AggregateResult(result, OnTypeMemberRules(type.Rules));
        return result;
    }
    public virtual R OnTypeMemberEnums(IEnumerable<IrTypeMemberEnum> memberEnums)
        => memberEnums.Select(OnTypeMemberEnum)
            .Aggregate(Default, AggregateResult);
    public virtual R OnTypeMemberEnum(IrTypeMemberEnum memberEnum)
        => OnOptionalExpression(Default, memberEnum.ValueExpression);
    public virtual R OnTypeMemberFields(IEnumerable<IrTypeMemberField> memberFields)
        => memberFields.Select(OnTypeMemberField)
            .Aggregate(Default, AggregateResult);
    public virtual R OnTypeMemberField(IrTypeMemberField memberField)
    {
        var result = OnType(memberField.Type);
        result = OnOptionalExpression(result, memberField.DefaultValue);
        return result;
    }
    public virtual R OnTypeMemberRules(IEnumerable<IrTypeMemberRule> memberRules)
        => memberRules.Select(OnTypeMemberRule)
            .Aggregate(Default, AggregateResult);
    public virtual R OnTypeMemberRule(IrTypeMemberRule memberRule)
        => Default;

    public virtual R OnDeclarationVariable(IrDeclarationVariable variable)
        => OnOptionalExpression(Default, variable.Initializer);

    public virtual R OnType(IrType type)
        => Default;
    public virtual R OnExpression(IrExpression expression)
        => expression switch
        {
            IrExpressionBinary be => OnExpressionBinary(be),
            IrExpressionIdentifier ie => OnExpressionIdentifier(ie),
            IrExpressionInvocation ei => OnExpressionInvocation(ei),
            IrExpressionTypeInitializer eti => OnExpressionTypeInitializer(eti),
            IrExpressionMemberAccess ema => OnExpressionMemberAccess(ema),
            IrExpressionLiteral le => OnExpressionLiteral(le),
            _ => Default
        };
    public virtual R OnExpressionBinary(IrExpressionBinary expression)
    {
        var result = OnExpression(expression.Left);
        result = AggregateResult(result, OnOperatorBinary(expression.Operator));
        result = AggregateResult(result, OnExpression(expression.Right));
        return result;
    }
    public virtual R OnOperatorBinary(IrBinaryOperator op)
        => Default;
    public virtual R OnExpressionLiteral(IrExpressionLiteral expression)
        => Default;
    public virtual R OnExpressionInvocation(IrExpressionInvocation invocation)
    {
        var result = OnInvocationTypeArguments(invocation.TypeArguments);
        result = AggregateResult(result, OnInvocationArguments(invocation.Arguments));
        return result;
    }
    public virtual R OnInvocationTypeArguments(IEnumerable<IrTypeArgument> arguments)
        => arguments.Select(OnInvocationTypeArgument)
            .Aggregate(Default, AggregateResult);
    public virtual R OnInvocationTypeArgument(IrTypeArgument argument)
        => OnType(argument.Type);
    public virtual R OnInvocationArguments(IEnumerable<IrArgument> arguments)
        => arguments.Select(OnInvocationArgument)
            .Aggregate(Default, AggregateResult);
    public virtual R OnInvocationArgument(IrArgument argument)
        => OnExpression(argument.Expression);
    public virtual R OnExpressionTypeInitializer(IrExpressionTypeInitializer expression)
        => expression.Fields.Select(f => OnTypeInitializerField(f)).Aggregate(Default, AggregateResult);
    public virtual R OnTypeInitializerField(IrTypeInitializerField initializer)
        => OnExpression(initializer.Expression);
    public virtual R OnExpressionIdentifier(IrExpressionIdentifier identifier)
        => Default;
    public virtual R OnExpressionMemberAccess(IrExpressionMemberAccess memberAccess)
        => OnExpression(memberAccess.Expression);
    public virtual R OnStatements(IEnumerable<IrStatement> statements)
        => statements.Select(statement =>
        {
            return statement switch
            {
                IrStatementAssignment statAss => OnStatementAssignment(statAss),
                IrStatementExpression statExpr => OnStatementExpression(statExpr),
                IrStatementIf statIf => OnStatementIf(statIf),
                IrStatementLoop statLoop => OnStatementLoop(statLoop),
                IrStatementReturn statRet => OnStatementReturn(statRet),
                _ => Default
            };
        })
            .Aggregate(Default, AggregateResult);

    public virtual R OnStatementAssignment(IrStatementAssignment statement)
        => OnExpression(statement.Expression);

    public virtual R OnStatementIf(IrStatementIf statement)
    {
        var result = OnExpression(statement.Condition);
        result = AggregateResult(result, OnCodeBlock(statement.CodeBlock));
        if (statement.ElseClause is IrElseClause elseClause)
        {
            result = AggregateResult(result, OnStatementIf_ElseClause(elseClause));
        }
        else if (statement.ElseIfClause is IrElseIfClause elseIfClause)
        {
            result = AggregateResult(result, OnStatementIf_ElseIfClause(elseIfClause));
        }
        return result;
    }
    public virtual R OnStatementIf_ElseIfClause(IrElseIfClause elseIfClause)
    {
        var result = OnExpression(elseIfClause.Condition);
        result = AggregateResult(result, OnCodeBlock(elseIfClause.CodeBlock));

        if (elseIfClause.ElseClause is IrElseClause nestedElse)
        {
            result = AggregateResult(result, OnStatementIf_ElseClause(nestedElse));
        }
        else if (elseIfClause.ElseIfClause is IrElseIfClause nestedElseIf)
        {
            result = AggregateResult(result, OnStatementIf_ElseIfClause(nestedElseIf));
        }
        return result;
    }
    public virtual R OnStatementIf_ElseClause(IrElseClause elseClause)
        => OnCodeBlock(elseClause.CodeBlock);

    public virtual R OnStatementExpression(IrStatementExpression statement)
        => OnExpression(statement.Expression);
    public virtual R OnStatementReturn(IrStatementReturn statement)
        => OnOptionalExpression(Default, statement.Expression);
    public virtual R OnStatementLoop(IrStatementLoop statement)
        => Default;

    protected R OnOptionalType(R result, IrType? type)
        => type is not null
            ? OnType(type)
            : result;
    protected R OnOptionalExpression(R result, IrExpression? expression)
        => expression is not null
            ? OnExpression(expression)
            : result;
}
