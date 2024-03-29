﻿using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstTypeDefinitionStruct : AstTypeDefinitionWithFields,
        IAstCodeBlockLine, IAstSymbolTableSite
    {
        internal AstTypeDefinitionStruct(ParserRuleContext context, AstSymbolTable parentTable)
            : base(AstNodeKind.Struct)
        {
            SymbolTable = new AstSymbolTable("", parentTable);
            Context = context;
        }

        public uint Indent { get; set; }

        public override bool IsStruct => true;

        public AstSymbolTable SymbolTable { get; }

        public override bool TrySetIdentifier(AstIdentifier identifier)
        {
            var success = base.TrySetIdentifier(identifier);

            if (success)
                SymbolTable.SetName(identifier.SymbolName.CanonicalName.FullName);

            return success;
        }

        public new IEnumerable<AstTypeDefinitionStructField> Fields
            => base.Fields.Cast<AstTypeDefinitionStructField>();

        public bool TryAddField(AstTypeDefinitionStructField field)
        {
            var success = base.TryAddField(field);

            if (success)
                SymbolTable.Add(field);

            return success;
        }

        public override bool TryAddTemplateParameter(AstTemplateParameterDefinition templateParameter)
        {
            if (base.TryAddTemplateParameter(templateParameter))
            {
                SymbolTable.Add(templateParameter);
                return true;
            }
            return false;
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionStruct(this);
    }
}
