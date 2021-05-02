using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstAssignment : AstNode,
        IAstCodeBlockItem, IAstExpressionSite, IAstTypeInitializeSite
    {
        public AstAssignment(Variable_def_typedContext context)
            : base(AstNodeType.Assignment)
        {
            Context = context;
        }

        public AstAssignment(Variable_assign_structContext context)
            : base(AstNodeType.Assignment)
        {
            Context = context;
        }

        public AstAssignment(Variable_assign_valueContext context)
            : base(AstNodeType.Assignment)
        {
            Context = context;
        }

        internal AstAssignment(ParserRuleContext context)
            : base(AstNodeType.Assignment)
        {
            Context = context;
        }

        public ParserRuleContext Context { get; }

        public uint Indent { get; set; }

        private AstExpression? _expression;
        public AstExpression? Expression => _expression;

        public bool TrySetExpression(AstExpression? expression)
            => this.SafeSetParent(ref _expression, expression);

        public void SetExpression(AstExpression expression)
        {
            if (!TrySetExpression(expression))
                throw new InvalidOperationException(
                    "Expression is already set or null.");
        }

        private AstVariable? _variable;

        public AstVariable? Variable => _variable;

        public bool TrySetVariable(AstVariable? variable)
            => this.SafeSetParent(ref _variable, variable);

        public void SetVariable(AstVariable variable)
        {
            if (!TrySetVariable(variable))
                throw new InvalidOperationException(
                    "Variable is already set or null.");
        }

        // override variable reference with definition
        public void SetVariableDefinition(AstVariableDefinition variableDefinition)
        {
            Ast.Guard(_variable is AstVariableReference, "Unexpected Variable Type on Assign.");

            _variable = null;
            if (!this.SafeSetParent(ref _variable, variableDefinition))
            {
                throw new InvalidOperationException(
                    "SetParent failed in AstAssignment.SetVariableDefinition.");
            }
        }

        private readonly List<AstTypeFieldInitialization> _fields = new List<AstTypeFieldInitialization>();
        public IEnumerable<AstTypeFieldInitialization> Fields => _fields;

        public bool HasFields => _fields.Count > 0;

        public bool TryAddFieldInit(AstTypeFieldInitialization? field)
        {
            if (field != null &&
                field.TrySetParent(this))
            {
                _fields.Add(field);
                return true;
            }
            return false;
        }

        public void AddFieldInit(AstTypeFieldInitialization field)
        {
            if (!TryAddFieldInit(field))
                throw new InvalidOperationException(
                    "TypeField is already set or null.");
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitAssignment(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            Expression?.Accept(visitor);
            Variable?.Accept(visitor);
            foreach (var field in _fields)
            {
                field.Accept(visitor);
            }
        }
    }
}