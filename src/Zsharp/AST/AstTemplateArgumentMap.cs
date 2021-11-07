using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public sealed class AstTemplateArgumentMap 
        : AstParameterArgumentMap<AstTemplateParameterDefinition, AstTemplateParameterReference>
    {
        public AstTemplateArgumentMap(
            IEnumerable<AstTemplateParameterDefinition> templateParameters,
            IEnumerable<AstTemplateParameterReference> templateArguments)
            : base(templateParameters, templateArguments)
        { }

        private static readonly AstTemplateArgumentMap _empty
            = new (Enumerable.Empty<AstTemplateParameterDefinition>(), Enumerable.Empty<AstTemplateParameterReference>());

        public static AstTemplateArgumentMap Empty => _empty;
    }
}
