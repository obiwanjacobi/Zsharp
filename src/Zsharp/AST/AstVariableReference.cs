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

        private AstVariableDefinition? _varDef;
        public AstVariableDefinition? VariableDefinition => _varDef;

        private AstFunctionParameter? _paramDef;
        public AstFunctionParameter? ParameterDefinition => _paramDef;

        public bool SetVariableDefinition(AstVariableDefinition variableDefinition) => Ast.SafeSet(ref _varDef, variableDefinition);

        public bool SetVariableDefinition(AstFunctionParameter paramDefinition)
        {
            return Ast.SafeSet(ref _paramDef, paramDefinition);
        }

        public override void Accept(AstVisitor visitor) => visitor.VisitVariableReference(this);

        public override void VisitChildren(AstVisitor visitor) => Identifier?.Accept(visitor);
    }
}