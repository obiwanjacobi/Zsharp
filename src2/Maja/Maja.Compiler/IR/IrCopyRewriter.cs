using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Maja.Compiler.IR;

internal abstract class IrCopyRewriter : IrRewriter
{
    protected override IEnumerable<IrArgument> RewriteArgument(IrArgument argument)
    {
        var expression = RewriteExpression(argument.Expression)!;
        return [new IrArgument(argument.Syntax, expression, argument.Symbol)];
    }

    protected override IrOperatorBinary RewriteOperatorBinary(IrOperatorBinary @operator)
        => new IrOperatorBinary(@operator.Kind, @operator.OperandType);

    protected override IrCodeBlock RewriteCodeBlock(IrCodeBlock codeBlock)
    {
        var nodes = new List<IrNode>();
        foreach (var irNode in codeBlock.Nodes)
        {
            IEnumerable<IrNode> newNodes = irNode switch
            {
                IrStatement statement => RewriteStatement(statement),
                IrDeclaration declaration => RewriteDeclaration(declaration),
                _ => throw new MajaException($"IR: Invalid code block root object: {irNode.GetType().FullName}.")
            };

            nodes.AddRange(newNodes);
        }
        //var statements = RewriteStatements(codeBlock.Statements);
        //var declarations = RewriteDeclarations(codeBlock.Declarations);
        //return new IrCodeBlock(codeBlock.Syntax, statements, declarations);
        return new IrCodeBlock(codeBlock.Syntax, nodes);
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
        var assignment = RewriteAssignmentOperator(variable.AssignmentOperator);
        return [new IrDeclarationVariable(variable.Syntax, variable.Symbol, variable.TypeSymbol, assignment, expression)];
    }

    protected override IrExpressionBinary RewriteExpressionBinary(IrExpressionBinary expression)
    {
        var left = RewriteExpression(expression.Left)!;
        var op = RewriteOperatorBinary(expression.Operator);
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
