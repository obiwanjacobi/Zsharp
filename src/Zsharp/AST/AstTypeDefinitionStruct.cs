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

        public uint Indent { get; set; }

        public override bool IsStruct => true;

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

        // true when type is a template definition
        public bool IsTemplate => _templateParameters.Count > 0;

        private readonly List<AstTemplateParameter> _templateParameters = new();
        public IEnumerable<AstTemplateParameter> TemplateParameters => _templateParameters;

        public void AddTemplateParameter(AstTemplateParameter templateParameter)
        {
            if (!TryAddTemplateParameter(templateParameter))
                throw new InvalidOperationException(
                    "TemplateParameter is already set or null.");
        }

        public bool TryAddTemplateParameter(AstTemplateParameter templateParameter)
        {
            if (templateParameter == null ||
                templateParameter is not AstTemplateParameterDefinition)
                return false;

            Symbols.Add((AstTemplateParameterDefinition)templateParameter);
            _templateParameters.Add(templateParameter);

            Identifier.TemplateParameterCount = _templateParameters.Count;
            return true;
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionStruct(this);
    }
}
