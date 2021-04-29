using System.IO;
using Zsharp.AST;

namespace Zsharp.EmitCS
{
    public class EmitCode : AstVisitor
    {
        public EmitCode(string assemblyName)
        {
            Context = new EmitContext(assemblyName, null);
        }

        public EmitContext Context { get; private set; }

        public override void VisitModulePublic(AstModulePublic module)
        {
            // usings / imports
            Context.Imports(module.Symbol!.SymbolTable);

            using var scope = Context.AddModule(module);

            VisitChildren(module);
        }

        public override void VisitFunctionDefinition(AstFunctionDefinition function)
        {
            using var scope = Context.AddFunction(function);

            VisitChildren(function);
        }

        public override void VisitFunctionReference(AstFunctionReference function)
        {
            var functionDef = function.FunctionDefinition!;
            var name = functionDef.Identifier!.CanonicalName;

            if (functionDef.IsExternal)
            {
                name = ((AstFunctionDefinitionExternal)functionDef).ExternalName.FullName;
            }

            Context.CodeBuilder.CsBuilder.Append($"{name}(");

            VisitChildren(function);

            Context.CodeBuilder.CsBuilder.EndLine(")");
        }

        public override void VisitVariableDefinition(AstVariableDefinition variable)
        {
            if (variable.IsTopLevel())
            {
                Context.ModuleClass.AddField(variable);
            }
            else
            {
                Context.CodeBuilder.AddVariable(variable);
            }
        }

        public override void VisitVariableReference(AstVariableReference variable)
        {
            var expressionVisitor = new EmitExpression(Context.CodeBuilder.CsBuilder);
            expressionVisitor.VisitVariableReference(variable);
        }

        public override void VisitAssignment(AstAssignment assign)
        {
            if (assign.Variable is AstVariableDefinition varDef)
            {
                VisitVariableDefinition(varDef);
            }
            else
            {
                Context.CodeBuilder.CsBuilder.WriteIndent();
                VisitVariableReference((AstVariableReference)assign.Variable!);
            }

            if (assign.HasFields)
            {
                Ast.Guard(assign.Variable!.TypeReference!.TypeDefinition!.IsStruct, "Expect Struct.");

                base.VisitAssignment(assign);
            }

            if (assign.Expression != null)
            {
                Context.CodeBuilder.CsBuilder.Append(" = ");
                VisitExpression(assign.Expression);
            }

            Context.CodeBuilder.CsBuilder.EndLine();
        }

        public override void VisitTypeFieldInitialization(AstTypeFieldInitialization field)
        {
            //var assign = field.ParentAs<AstAssignment>();
            //if (assign != null)
            //{
            //    var varName = CodeBuilder.BuildInitName(assign.Variable.Identifier.CanonicalName);
            //    var varDef = Context.CodeBuilder.GetVariable(varName);

            //    Context.CodeBuilder.CodeBlock.Add(
            //        Context.InstructionFactory.LoadVariableAddress(varDef));

            //    VisitChildren(field);

            //    var typeDef = Context.GetTypeDefinition(assign.Variable.TypeReference);
            //    var fieldDef = typeDef.Fields.Find(field.Identifier.CanonicalName);

            //    Context.CodeBuilder.CodeBlock.Add(
            //        Context.InstructionFactory.StoreField(fieldDef));
            //}
        }

        public override void VisitBranchExpression(AstBranchExpression branch)
        {
            Context.CodeBuilder.StartBranch(branch);

            if (branch.HasExpression)
            {
                VisitChildren(branch);
            }

            Context.CodeBuilder.CsBuilder.EndLine();
        }

        public override void VisitBranchConditional(AstBranchConditional branch)
        {
            if (branch.HasExpression)
            {
                Context.CodeBuilder.StartBranch(branch);
                Context.CodeBuilder.CsBuilder.Append("(");

                Visit(branch.Expression!);

                Context.CodeBuilder.CsBuilder.StartScope(")");

                Visit(branch.CodeBlock!);

                Context.CodeBuilder.CsBuilder.EndScope();

                if (branch.HasSubBranch)
                {
                    var subHasExpression = branch.SubBranch!.HasExpression;
                    Context.CodeBuilder.CsBuilder.StartBranch(BranchStatement.Else);

                    if (!subHasExpression)
                        Context.CodeBuilder.CsBuilder.StartScope();

                    Visit(branch.SubBranch!);

                    if (!subHasExpression)
                        Context.CodeBuilder.CsBuilder.EndScope();
                }
            }
            else
                Visit(branch.CodeBlock!);
        }

        public override void VisitExpression(AstExpression expression)
            => new EmitExpression(Context.CodeBuilder.CsBuilder).VisitExpression(expression);

        public override void VisitTypeDefinitionEnum(AstTypeDefinitionEnum enumType)
            => Context.ModuleClass.AddEnum(enumType);

        public override void VisitTypeDefinitionStruct(AstTypeDefinitionStruct structType)
            => Context.ModuleClass.AddStruct(structType);

        public void SaveAs(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(filePath, ToString());
        }

        public override string ToString()
        {
            return Context.Namespace.ToCode();
        }
    }
}
