using System;
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
            IDisposable? scope = null;
            if (module is AstModulePublic publicModule)
            {
                if (Context == null)
                {
                    Context = EmitContext.Create(module.Name);
                }

                scope = Context.AddModule(publicModule);
            }

            try
            {
                VisitChildren(module);
            }
            finally
            {
                if (scope != null)
                    scope.Dispose();
            }
        }

        public override void VisitFunction(AstFunction function)
        {
            using var scope = Context.AddFunction(function);

            VisitChildren(function);
        }

        public override void VisitVariableDefinition(AstVariableDefinition variable)
        {
            if (!Context.HasVariable(variable.Identifier.Name))
            {
                Context.AddVariable(variable);
            }
        }

        public override void VisitAssignment(AstAssignment assign)
        {
            base.VisitAssignment(assign);

            var varDef = Context.CodeBuilder.GetVariable(assign.Variable.Identifier.Name);
            var store = Context.InstructionFactory.StoreVariable(varDef);
            Context.CodeBuilder.CodeBlock.Add(store);
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

                Context.CodeBuilder.Return();
            }
        }

        public override void VisitExpression(AstExpression expression)
            => new EmitExpression(Context).VisitExpression(expression);

        public void SaveAs(string filePath) => Context.Assembly.Write(filePath);
    }
}
