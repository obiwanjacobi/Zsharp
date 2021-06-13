namespace Zsharp.AST
{
    public sealed class AstSymbolReferenceRemover : AstVisitor
    {
        public static void RemoveReference(AstNode node)
            => new AstSymbolReferenceRemover().Visit(node);

        public void Remove(AstNode node)
            => Visit(node);

        public override void VisitFunctionParameterReference(AstFunctionParameterReference parameter)
        {
            parameter.Symbol?.RemoveReference(parameter);
            base.VisitFunctionParameterReference(parameter);
        }

        public override void VisitFunctionReference(AstFunctionReference function)
        {
            function.Symbol?.RemoveReference(function);
            base.VisitFunctionReference(function);
        }

        public override void VisitTemplateParameterReference(AstTemplateParameterReference templateParameter)
        {
            templateParameter.Symbol?.RemoveReference(templateParameter);
            base.VisitTemplateParameterReference(templateParameter);
        }

        public override void VisitTypeFieldReferenceEnumOption(AstTypeFieldReferenceEnumOption enumOption)
        {
            enumOption.Symbol?.RemoveReference(enumOption);
            base.VisitTypeFieldReferenceEnumOption(enumOption);
        }

        public override void VisitTypeFieldReferenceStructField(AstTypeFieldReferenceStructField structField)
        {
            structField.Symbol?.RemoveReference(structField);
            base.VisitTypeFieldReferenceStructField(structField);
        }

        public override void VisitTypeReferenceFunction(AstTypeReferenceFunction function)
        {
            function.Symbol?.RemoveReference(function);
            base.VisitTypeReferenceFunction(function);
        }

        public override void VisitTypeReferenceType(AstTypeReferenceType type)
        {
            type.Symbol?.RemoveReference(type);
            base.VisitTypeReferenceType(type);
        }

        public override void VisitVariableReference(AstVariableReference variable)
        {
            variable.Symbol?.RemoveReference(variable);
            base.VisitVariableReference(variable);
        }
    }
}
