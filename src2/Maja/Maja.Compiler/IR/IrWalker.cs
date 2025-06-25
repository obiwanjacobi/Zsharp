using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.IR;

internal abstract class IrWalker<R>
{
    public virtual R Default => default;

    public virtual R AggregateResult(R aggregate, R newResult)
        => newResult;

    private readonly Stack<IrScope> _scopes = new();
    protected IrScope PeekScope()
        => _scopes.Peek();
    protected void PushScope(IrScope scope)
        => _scopes.Push(scope);
    protected IrScope PopScope()
        => _scopes.Pop();

    public virtual R OnProgram(IrProgram program)
    {
        var result = OnModule(program.Module);
        return result;
    }
    public virtual R OnCodeBlock(IrCodeBlock codeBlock)
    {
        //var result = OnDeclarations(codeBlock.Declarations);
        //result = AggregateResult(result, OnStatements(codeBlock.Statements));
        //return result;
        var result = OnNodes(codeBlock.Nodes);
        return result;
    }

    public virtual R OnModule(IrModule module)
    {
        PushScope(module.Scope);
        var result = OnImports(module.Imports);
        result = AggregateResult(result, OnExports(module.Exports));
        result = AggregateResult(result, OnDeclarations(module.Declarations));
        result = AggregateResult(result, OnStatements(module.Statements));
        PopScope();
        return result;
    }

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

    public virtual R OnNodes(IEnumerable<IrNode> nodes)
        => nodes.Select(OnNode).Aggregate(Default, AggregateResult);

    public virtual R OnNode(IrNode node)
    {
        return node switch
        {
            IrStatement statement => OnStatement(statement),
            IrDeclaration declaration => OnDeclaration(declaration),
            _ => Default
        };
    }

    public virtual R OnDeclarations(IEnumerable<IrDeclaration> declarations)
        => declarations.Select(OnDeclaration).Aggregate(Default, AggregateResult);

    public virtual R OnDeclaration(IrDeclaration declaration)
    {
        return declaration switch
        {
            IrDeclarationFunction fd => OnDeclarationFunction(fd),
            IrDeclarationType td => OnDeclarationType(td),
            IrDeclarationVariable vd => OnDeclarationVariable(vd),
            _ => Default
        };
    }

    public virtual R OnDeclarationFunction(IrDeclarationFunction function)
    {
        var result = OnTypeParametersGeneric(function.TypeParameters.OfType<IrTypeParameterGeneric>());
        PushScope(function.Scope);
        result = AggregateResult(result, OnTypeParametersTemplate(function.TypeParameters.OfType<IrTypeParameterTemplate>()));
        result = AggregateResult(result, OnParameters(function.Parameters));
        result = AggregateResult(result, OnOptionalType(result, function.ReturnType));
        result = AggregateResult(result, OnCodeBlock(function.Body));
        PopScope();
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
        PushScope(type.Scope);
        var result = OnTypeMemberEnums(type.Enums);
        result = AggregateResult(result, OnTypeMemberFields(type.Fields));
        result = AggregateResult(result, OnTypeMemberRules(type.Rules));
        PopScope();
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
    public virtual R OnOperatorBinary(IrOperatorBinary op)
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
        => statements.Select(OnStatement).Aggregate(Default, AggregateResult);

    private R OnStatement(IrStatement statement)
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
    }

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
