﻿using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeDefinitionStruct : AstTypeDefinition, IAstCodeBlockItem
    {
        public AstTypeDefinitionStruct(Struct_defContext context)
            : base(AstNodeType.Struct)
        {
            Context = context;
        }

        public int Indent { get; set; }

        public new IEnumerable<AstTypeDefinitionStructField> Fields
            => base.Fields.Cast<AstTypeDefinitionStructField>();

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionStruct(this);
    }
}
