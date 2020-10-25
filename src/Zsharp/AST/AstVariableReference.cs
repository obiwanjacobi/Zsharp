using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstVariableReference : AstVariable
    {
        private readonly Variable_refContext? _refCtx;
        private readonly Variable_assign_autoContext? _assignCtx;

        public AstVariableReference(Variable_refContext context)
        {
            _refCtx = context;
        }
        public AstVariableReference(Variable_assign_autoContext context)
        {
            _assignCtx = context;
        }

        public bool HasDefinition => _varDef != null || _paramDef != null;

        private AstVariableDefinition? _varDef;
        public AstVariableDefinition? VariableDefinition => _varDef;

        private AstFunctionParameter? _paramDef;
        public AstFunctionParameter? ParameterDefinition => _paramDef;

        public bool TrySetVariableDefinition(AstVariableDefinition variableDefinition) => Ast.SafeSet(ref _varDef, variableDefinition);

        public bool TrySetVariableDefinition(AstFunctionParameter paramDefinition) => Ast.SafeSet(ref _paramDef, paramDefinition);

        public void SetVariableDefinition(AstVariableDefinition variableDefinition)
        {
            if (!TrySetVariableDefinition(variableDefinition))
                throw new InvalidOperationException(
                    "VariableDefinition was already set or null.");
        }

        public void SetVariableDefinition(AstFunctionParameter paramDefinition)
        {
            if (!TrySetVariableDefinition(paramDefinition))
                throw new InvalidOperationException(
                    "FunctionParameter definition was already set or null.");
        }

        public override void Accept(AstVisitor visitor) => visitor.VisitVariableReference(this);
    }
}