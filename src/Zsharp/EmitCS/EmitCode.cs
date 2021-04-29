using System.Collections.Generic;
using System.IO;
using Zsharp.AST;

namespace Zsharp.EmitCS
{
    public class EmitCode : AstVisitor
    {
        public EmitCode(string assemblyName)
        {
            Context = EmitContext.Create(assemblyName);
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

            Context.CsBuilder.Append($"{name}(");

            VisitChildren(function);

            Context.CsBuilder.EndLine(")");
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

        public override void VisitAssignment(AstAssignment assign)
        {
            if (assign.HasFields)
            {
                Ast.Guard(assign.Variable!.TypeReference!.TypeDefinition!.IsStruct, "Expect Struct.");

                base.VisitAssignment(assign);
            }

            if (assign.Expression != null)
            {
                Context.CsBuilder.WriteIndent();
                Context.CsBuilder.Append(assign.Variable!.TypeReference.ToCode());
                Context.CsBuilder.Append(" ");
                Context.CsBuilder.Append(assign.Variable!.Identifier!.CanonicalName);
                Context.CsBuilder.Append(" = ");
                VisitExpression(assign.Expression);
                Context.CsBuilder.EndLine();
            }
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
                Context.CsBuilder.Append("(");

                Visit(branch.Expression!);

                Context.CsBuilder.StartScope(")");

                Visit(branch.CodeBlock!);

                Context.CsBuilder.EndScope();

                if (branch.HasSubBranch)
                {
                    var subHasExpression = branch.SubBranch!.HasExpression;
                    Context.CsBuilder.StartBranch(BranchStatement.Else);

                    if (!subHasExpression)
                        Context.CsBuilder.StartScope();

                    Visit(branch.SubBranch!);

                    if (!subHasExpression)
                        Context.CsBuilder.EndScope();
                }
            }
            else
                Visit(branch.CodeBlock!);
        }

        public override void VisitExpression(AstExpression expression)
            => new EmitExpression(Context).VisitExpression(expression);

        public override void VisitTypeDefinitionEnum(AstTypeDefinitionEnum enumType)
        {
            var access = enumType.Symbol!.SymbolLocality == AstSymbolLocality.Exported
                ? AccessModifiers.Public : AccessModifiers.Private;

            Ast.Guard(enumType.BaseType, "Enum has no base type.");
            Context.CsBuilder.StartEnum(access, enumType.Identifier!.CanonicalName, enumType.BaseType.ToCode());

            VisitChildren(enumType);

            Context.CsBuilder.EndScope();
        }

        public override void VisitTypeDefinitionEnumOption(AstTypeDefinitionEnumOption enumOption)
        {
            Context.CsBuilder.WriteIndent();
            Context.CsBuilder.Append(enumOption.Identifier!.CanonicalName);
            if (enumOption.Expression != null)
            {
                Context.CsBuilder.Append(" = ");
                VisitExpression(enumOption.Expression);
            }
            Context.CsBuilder.AppendLine(",");
        }

        public override void VisitTypeDefinitionStruct(AstTypeDefinitionStruct structType)
        {
            var access = structType.Symbol!.SymbolLocality == AstSymbolLocality.Exported
                ? AccessModifiers.Public : AccessModifiers.Private;

            List<string> baseTypes = new();
            if (structType.BaseType != null)
            {
                baseTypes.Add(structType.BaseType.ToCode());
            }

            Context.CsBuilder.StartRecord(access, structType.Identifier!.CanonicalName, baseTypes.ToArray());

            VisitChildren(structType);

            Context.CsBuilder.EndScope();
        }

        public override void VisitTypeDefinitionStructField(AstTypeDefinitionStructField structField)
        {
            var access = AccessModifiers.Public;
            var typeName = structField.TypeReference.ToCode();
            var fieldName = structField.Identifier!.CanonicalName;
            Context.CsBuilder.Property(access, typeName, fieldName);
        }

        public void SaveAs(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(filePath, Context.CsBuilder.ToString());
        }

        public override string ToString()
        {
            return Context.CsBuilder.ToString();
        }
    }
}
