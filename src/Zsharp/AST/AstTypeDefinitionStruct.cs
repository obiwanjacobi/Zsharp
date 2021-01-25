using System;
using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeDefinitionStruct : AstTypeDefinition,
        IAstCodeBlockItem, IAstTemplateSite, IAstSymbolTableSite
    {
        public AstTypeDefinitionStruct(Struct_defContext context, AstSymbolTable parentTable)
            : base(AstNodeType.Struct)
        {
            Symbols = new AstSymbolTable("", parentTable);
            Context = context;
        }

        public int Indent { get; set; }

        public AstSymbolTable Symbols { get; }

        public override bool TrySetIdentifier(AstIdentifier identifier)
        {
            var success = base.TrySetIdentifier(identifier);

            if (success)
                Symbols.SetName(identifier.CanonicalName);

            return success;
        }

        public new IEnumerable<AstTypeDefinitionStructField> Fields
            => base.Fields.Cast<AstTypeDefinitionStructField>();

        public bool TryAddField(AstTypeDefinitionStructField field)
        {
            var success = base.TryAddField(field);

            if (success)
                Symbols.Add(field);

            return success;
        }

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

            Identifier.TemplateParameterCount = _parameters.Count;
            return true;
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionStruct(this);
    }
}
