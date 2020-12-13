using System;
using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeDefinitionStruct : AstTypeDefinition, IAstCodeBlockItem,
        IAstTemplateSite, IAstSymbolTableSite
    {
        public AstTypeDefinitionStruct(Struct_defContext context, AstSymbolTable parentTable)
            : base(AstNodeType.Struct)
        {
            Symbols = new AstSymbolTable("", parentTable);
            Context = context;
        }

        public int Indent { get; set; }

        public new IEnumerable<AstTypeDefinitionStructField> Fields
            => base.Fields.Cast<AstTypeDefinitionStructField>();

        public AstSymbolTable Symbols { get; }

        private readonly List<AstTemplateParameter> _parameters = new List<AstTemplateParameter>();
        public IEnumerable<AstTemplateParameter> Parameters => _parameters;

        public void AddTemplateParameter(AstTemplateParameter templateParameter)
        {
            if (!TryAddTemplateParameter(templateParameter))
                throw new InvalidOperationException(
                    "TemplateParameter is already set or null.");
        }

        public bool TryAddTemplateParameter(AstTemplateParameter templateParameter)
        {
            if (templateParameter == null)
                return false;

            Symbols.Add(templateParameter);
            _parameters.Add(templateParameter);
            return true;
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionStruct(this);
    }
}
