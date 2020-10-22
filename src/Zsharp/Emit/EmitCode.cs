using Zsharp.AST;

namespace Zsharp.Emit
{
    public class EmitCode : AstVisitor
    {
        public EmitCode(string assemblyName)
        {
            Context = EmitContext.Create(assemblyName);
        }

        public EmitContext? Context { get; private set; }

        public override void VisitModule(AstModule module)
        {
            if (Context == null)
            {
                Context = EmitContext.Create(module.Name);
            }

            using var scope = Context.AddModule(module);

            VisitChildren(module);
        }

        public override void VisitFunction(AstFunction function)
        {
            using var scope = Context.AddFunction(function);

            VisitChildren(function);
        }

        public override void VisitVariableDefinition(AstVariableDefinition variable)
        {
            Context.AddVariable(variable);
        }

        public override void VisitAssignment(AstAssignment assign)
        {
            base.VisitAssignment(assign);
        }

        public override void VisitBranchExpression(AstBranchExpression branch)
        {
            if (branch.BranchType == AstBranchType.ExitFunction)
            {
                if (branch.HasExpression)
                {
                    VisitChildren(branch);
                    // TODO: ld expression result
                    return;
                }

                var instruction = Context.InstructionFactory.Return();
                Context.CodeBuilder.CodeBlock.Add(instruction);
            }
        }

        public override void VisitExpression(AstExpression expression)
        {
            var emitExpr = new EmitExpression(Context);
            emitExpr.VisitExpression(expression);
        }
    }
}
