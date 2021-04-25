using Mono.Cecil;
using System;
using System.IO;
using Zsharp.AST;

namespace Zsharp.EmitIL
{
    public class EmitCode : AstVisitor
    {
        public EmitCode(string assemblyName)
        {
            Context = EmitContext.Create(assemblyName);
        }

        public EmitContext? Context { get; private set; }

        public override void VisitModulePublic(AstModulePublic module)
        {
            if (Context == null)
            {
                Context = EmitContext.Create(module.Identifier.Name);
            }

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
            VisitChildren(function);

            var method = Context.GetFunctionReference(function.FunctionDefinition);
            var call = Context.InstructionFactory.Call(method);
            Context.CodeBuilder.CodeBlock.Add(call);
        }

        public override void VisitVariableDefinition(AstVariableDefinition variable)
        {
            var name = variable.Identifier.CanonicalName;

            if (variable.IsTopLevel())
            {
                if (!Context.ModuleClass.HasField(name))
                {
                    Context.ModuleClass.AddField(name, Context.ToTypeReference(variable.TypeReference));
                }
            }
            else
            {
                if (!Context.CodeBuilder.HasVariable(name))
                {
                    Context.CodeBuilder.AddVariable(name, Context.ToTypeReference(variable.TypeReference));
                }
            }
        }

        public override void VisitAssignment(AstAssignment assign)
        {
            var name = assign.Variable.Identifier.CanonicalName;

            if (assign.HasFields)
            {
                Ast.Guard(assign.Variable.TypeReference.TypeDefinition.IsStruct, "Expect Struct.");

                TypeReference tempTypeRef;
                var tempName = CodeBuilder.BuildInitName(name);
                if (!Context.CodeBuilder.HasVariable(tempName))
                {
                    tempTypeRef = Context.ToTypeReference(assign.Variable.TypeReference);
                    Context.CodeBuilder.AddVariable(tempName, tempTypeRef);
                }

                var tempVarDef = Context.CodeBuilder.GetVariable(tempName);

                Context.CodeBuilder.CodeBlock.Add(
                    Context.InstructionFactory.LoadVariableAddress(tempVarDef));
                Context.CodeBuilder.CodeBlock.Add(
                    Context.InstructionFactory.InitObject(tempVarDef.VariableType));

                base.VisitAssignment(assign);

                Context.CodeBuilder.CodeBlock.Add(
                    Context.InstructionFactory.LoadVariable(tempVarDef));
            }
            else
                base.VisitAssignment(assign);

            if (assign.IsTopLevel())
            {
                FieldDefinition field;

                if (Context.ModuleClass.HasField(name))
                    field = Context.ModuleClass.GetField(name);
                else
                {
                    var varDef = assign.Variable as AstVariableDefinition;
                    if (varDef == null)
                    {
                        var varRef = (AstVariableReference)assign.Variable;
                        varDef = varRef.VariableDefinition;
                    }
                    field = Context.ModuleClass.AddField(name,
                        Context.ToTypeReference(varDef.TypeReference));
                }
                Context.CodeBuilder.CodeBlock.Add(
                    Context.InstructionFactory.StoreField(field));
            }
            else
            {
                var varDef = Context.CodeBuilder.GetVariable(name);
                Context.CodeBuilder.CodeBlock.Add(
                    Context.InstructionFactory.StoreVariable(varDef));
            }
        }

        public override void VisitTypeFieldInitialization(AstTypeFieldInitialization field)
        {
            var assign = field.ParentAs<AstAssignment>();
            if (assign != null)
            {
                var varName = CodeBuilder.BuildInitName(assign.Variable.Identifier.CanonicalName);
                var varDef = Context.CodeBuilder.GetVariable(varName);

                Context.CodeBuilder.CodeBlock.Add(
                    Context.InstructionFactory.LoadVariableAddress(varDef));

                VisitChildren(field);

                var typeDef = Context.GetTypeDefinition(assign.Variable.TypeReference);
                var fieldDef = typeDef.Fields.Find(field.Identifier.CanonicalName);

                Context.CodeBuilder.CodeBlock.Add(
                    Context.InstructionFactory.StoreField(fieldDef));
            }
        }

        public override void VisitBranchExpression(AstBranchExpression branch)
        {
            if (branch.BranchType == AstBranchType.ExitFunction)
            {
                if (branch.HasExpression)
                {
                    VisitChildren(branch);
                }

                Context.CodeBuilder.Return();
            }
            else
            {
                throw new NotImplementedException(
                    $"Emit Branch '{branch.BranchType}' not implemented.");
            }
        }

        public override void VisitBranchConditional(AstBranchConditional branch)
        {
            if (branch.HasExpression)
            {
                Visit(branch.Expression!);

                CodeBlock nextBlock;
                var builder = Context.CodeBuilder;
                if (branch.ParentAs<AstBranchConditional>() == null)
                {
                    nextBlock = builder.BranchConditional(
                        builder.NewBlockLabel(), builder.NewBlockLabel());
                }
                else    // sub-branch
                {
                    nextBlock = builder.Branch(builder.NewBlockLabel());
                }

                Visit(branch.CodeBlock!);

                builder.CodeBlock = nextBlock;

                if (branch.HasSubBranch)
                {
                    // TODO: the sub-branch's nextBlock should be the root's nextBlock...
                    Visit(branch.SubBranch!);
                }
            }
            else
                Visit(branch.CodeBlock!);
        }

        public override void VisitExpression(AstExpression expression)
            => new EmitExpression(Context).VisitExpression(expression);

        public override void VisitTypeDefinitionEnum(AstTypeDefinitionEnum enumType)
            => Context.ModuleClass.AddTypeEnum(enumType);

        public override void VisitTypeDefinitionStruct(AstTypeDefinitionStruct structType)
            => Context.ModuleClass.AddTypeStruct(structType);

        public void SaveAs(string filePath)
        {
            Context.Assembly.Name.Name = Path.GetFileNameWithoutExtension(filePath);
            Context.Assembly.Write(filePath);
        }
    }
}
