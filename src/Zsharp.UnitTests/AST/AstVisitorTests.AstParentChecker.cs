using FluentAssertions;
using Zsharp.AST;

namespace Zsharp.UnitTests.AST
{
    partial class AstVisitorTests
    {
        private class AstParentChecker : AstVisitor
        {
            public override void VisitAssignment(AstAssignment assign)
            {
                assign.Parent.Should().NotBeNull();
                assign.VisitChildren(this);
            }
            public override void VisitBranch(AstBranch branch)
            {
                branch.Parent.Should().NotBeNull();
                branch.VisitChildren(this);
            }
            public override void VisitBranchExpression(AstBranchExpression branch)
            {
                branch.Parent.Should().NotBeNull();
                branch.VisitChildren(this);
            }
            public override void VisitBranchConditional(AstBranchConditional branch)
            {
                branch.Parent.Should().NotBeNull();
                branch.VisitChildren(this);
            }
            public override void VisitCodeBlock(AstCodeBlock codeBlock)
            {
                codeBlock.Parent.Should().NotBeNull();
                codeBlock.VisitChildren(this);
            }
            public override void VisitExpression(AstExpression expression)
            {
                expression.Parent.Should().NotBeNull();
                expression.VisitChildren(this);
            }
            public override void VisitExpressionOperand(AstExpressionOperand operand)
            {
                operand.Parent.Should().NotBeNull();
                operand.VisitChildren(this);
            }
            public override void VisitFile(AstFile file)
            {
                // file is root in our tests (Parent is null)
                file.VisitChildren(this);
            }
            public override void VisitFunctionDefinition(AstFunctionDefinition function)
            {
                function.Parent.Should().NotBeNull();
                function.VisitChildren(this);
            }
            public override void VisitFunctionReference(AstFunctionReference function)
            {
                function.Parent.Should().NotBeNull();
                function.VisitChildren(this);
            }
            public override void VisitFunctionParameterDefinition(AstFunctionParameterDefinition parameter)
            {
                parameter.Parent.Should().NotBeNull();
                parameter.VisitChildren(this);
            }
            public override void VisitFunctionParameterReference(AstFunctionParameterArgument argument)
            {
                argument.Parent.Should().NotBeNull();
                argument.VisitChildren(this);
            }
            public override void VisitModulePublic(AstModuleImpl module)
            {
                module.Parent.Should().NotBeNull();
                module.VisitChildren(this);
            }
            public override void VisitLiteralNumeric(AstLiteralNumeric numeric)
            {
                numeric.Parent.Should().NotBeNull();
                numeric.VisitChildren(this);
            }
            public override void VisitTypeReferenceType(AstTypeReferenceType type)
            {
                type.Parent.Should().NotBeNull();
                type.VisitChildren(this);
            }
            public override void VisitTypeDefinitionEnum(AstTypeDefinitionEnum enumType)
            {
                enumType.Parent.Should().NotBeNull();
                enumType.VisitChildren(this);
            }
            public override void VisitVariableDefinition(AstVariableDefinition variable)
            {
                variable.Parent.Should().NotBeNull();
                variable.VisitChildren(this);
            }
            public override void VisitVariableReference(AstVariableReference variable)
            {
                variable.Parent.Should().NotBeNull();
                variable.VisitChildren(this);
            }
            public override void VisitLiteralBoolean(AstLiteralBoolean literalBool)
            {
                literalBool.Parent.Should().NotBeNull();
                literalBool.VisitChildren(this);
            }
            public override void VisitLiteralString(AstLiteralString literalString)
            {
                literalString.Parent.Should().NotBeNull();
                literalString.VisitChildren(this);
            }
            public override void VisitModuleExternal(AstModuleExternal module)
            {
                module.Parent.Should().NotBeNull();
                module.VisitChildren(this);
            }
            public override void VisitTemplateInstanceFunction(AstTemplateInstanceFunction templateFunction)
            {
                templateFunction.Parent.Should().NotBeNull();
                templateFunction.VisitChildren(this);
            }
            public override void VisitTemplateInstanceStruct(AstTemplateInstanceStruct structTemplate)
            {
                structTemplate.Parent.Should().NotBeNull();
                structTemplate.VisitChildren(this);
            }
            public override void VisitTemplateInstanceType(AstTemplateInstanceType typeTemplate)
            {
                typeTemplate.Parent.Should().NotBeNull();
                typeTemplate.VisitChildren(this);
            }
            public override void VisitTemplateParameterDefinition(AstTemplateParameterDefinition templateParameter)
            {
                templateParameter.Parent.Should().NotBeNull();
                templateParameter.VisitChildren(this);
            }
            public override void VisitTemplateParameterReference(AstTemplateParameterArgument templateArgument)
            {
                templateArgument.Parent.Should().NotBeNull();
                templateArgument.VisitChildren(this);
            }
            public override void VisitTypeDefinitionEnumOption(AstTypeDefinitionEnumOption enumOption)
            {
                enumOption.Parent.Should().NotBeNull();
                enumOption.VisitChildren(this);
            }
            public override void VisitTypeDefinitionFunction(AstTypeDefinitionFunction function)
            {
                function.Parent.Should().NotBeNull();
                function.VisitChildren(this);
            }
            public override void VisitTypeDefinitionStruct(AstTypeDefinitionStruct structType)
            {
                structType.Parent.Should().NotBeNull();
                structType.VisitChildren(this);
            }
            public override void VisitTypeDefinitionStructField(AstTypeDefinitionStructField structField)
            {
                structField.Parent.Should().NotBeNull();
                structField.VisitChildren(this);
            }
            public override void VisitTypeFieldDefinition(AstTypeFieldDefinition field)
            {
                field.Parent.Should().NotBeNull();
                field.VisitChildren(this);
            }
            public override void VisitTypeFieldInitialization(AstTypeFieldInitialization field)
            {
                field.Parent.Should().NotBeNull();
                field.VisitChildren(this);
            }
            public override void VisitTypeFieldReferenceEnumOption(AstTypeFieldReferenceEnumOption enumOption)
            {
                enumOption.Parent.Should().NotBeNull();
                enumOption.VisitChildren(this);
            }
            public override void VisitTypeFieldReferenceStructField(AstTypeFieldReferenceStructField structField)
            {
                structField.Parent.Should().NotBeNull();
                structField.VisitChildren(this);
            }
            public override void VisitTypeReferenceFunction(AstTypeReferenceFunction function)
            {
                function.Parent.Should().NotBeNull();
                function.VisitChildren(this);
            }
        }
    }
}
