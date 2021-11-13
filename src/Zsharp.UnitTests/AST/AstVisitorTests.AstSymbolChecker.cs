using FluentAssertions;
using Zsharp.AST;

namespace Zsharp.UnitTests.AST
{
    partial class AstVisitorTests
    {
        private class AstSymbolChecker : AstVisitor
        {
            public override void VisitModulePublic(AstModuleImpl module)
            {
                module.Symbol.Should().NotBeNull();
                base.VisitModulePublic(module);
            }
            public override void VisitTemplateParameterDefinition(AstTemplateParameterDefinition templateParameter)
            {
                templateParameter.Symbol.Should().NotBeNull();
                base.VisitTemplateParameterDefinition(templateParameter);
            }
            public override void VisitTemplateParameterArgument(AstTemplateParameterArgument templateArgument)
            {
                templateArgument.Symbol.Should().NotBeNull();
                base.VisitTemplateParameterArgument(templateArgument);
            }
            public override void VisitFunctionDefinition(AstFunctionDefinition function)
            {
                function.Symbol.Should().NotBeNull();
                function.VisitChildren(this);
            }
            public override void VisitFunctionReference(AstFunctionReference function)
            {
                function.Symbol.Should().NotBeNull();
                function.VisitChildren(this);
            }
            public override void VisitFunctionParameterDefinition(AstFunctionParameterDefinition parameter)
            {
                parameter.Symbol.Should().NotBeNull();
                parameter.VisitChildren(this);
            }
            public override void VisitFunctionParameterArgument(AstFunctionParameterArgument argument)
            {
                // only named parameters have an identifer and therefor a symbol
                //parameter.Symbol.Should().NotBeNull();
                argument.VisitChildren(this);
            }
            public override void VisitTypeDefinitionEnum(AstTypeDefinitionEnum enumType)
            {
                enumType.Symbol.Should().NotBeNull();
                base.VisitTypeDefinitionEnum(enumType);
            }
            public override void VisitTypeDefinitionEnumOption(AstTypeDefinitionEnumOption enumOption)
            {
                enumOption.Symbol.Should().NotBeNull();
                base.VisitTypeDefinitionEnumOption(enumOption);
            }
            public override void VisitTypeDefinitionStruct(AstTypeDefinitionStruct structType)
            {
                structType.Symbol.Should().NotBeNull();
                base.VisitTypeDefinitionStruct(structType);
            }
            public override void VisitTypeDefinitionStructField(AstTypeDefinitionStructField structField)
            {
                structField.Symbol.Should().NotBeNull();
                base.VisitTypeDefinitionStructField(structField);
            }
            public override void VisitTypeFieldDefinition(AstTypeFieldDefinition field)
            {
                field.Symbol.Should().NotBeNull();
                base.VisitTypeFieldDefinition(field);
            }
            public override void VisitTypeReferenceType(AstTypeReferenceType type)
            {
                type.Symbol.Should().NotBeNull();
                type.VisitChildren(this);
            }
            public override void VisitVariableDefinition(AstVariableDefinition variable)
            {
                variable.Symbol.Should().NotBeNull();
                variable.VisitChildren(this);
            }
            public override void VisitVariableReference(AstVariableReference variable)
            {
                variable.Symbol.Should().NotBeNull();
                base.VisitVariableReference(variable);
            }
        }
    }
}
