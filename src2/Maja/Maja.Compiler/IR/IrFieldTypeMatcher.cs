using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR;

internal class IrFieldTypeMatcher
{
    private readonly List<IrFieldTypeMatchInfo> _infos;
    private readonly DiagnosticList _diagnostics = new();

    public IrFieldTypeMatcher(IEnumerable<TypeParameterSymbol> typeParameters, IEnumerable<IrTypeArgument> typeArguments, IEnumerable<FieldSymbol> fields, IEnumerable<IrTypeInitializerField> initializers)
    {
        _infos = CreateFieldTypeInfos(typeParameters, typeArguments, fields, initializers);
    }

    public List<IrTypeInitializerField> RewriteFieldInitializers()
    {
        var fieldInits = new List<IrTypeInitializerField>();

        foreach (var info in _infos)
        {
            if (info.Initializer.Expression.TypeInferredSymbol is not null)
            {
                var type = info.Field.Type;

                if (info.TypeArgument is not null)
                    type = info.TypeArgument.Type.Symbol;

                // rewrite field init expression with type-argument type
                fieldInits.Add(ReWriteInitializerExpression(type, info.Initializer));
            }
            else if (info.Initializer.Expression.TypeSymbol.Name != info.Field.Type.Name)
            {
                if (!TryReWriteInitializerExpression(info.Field.Type, info.Initializer, out var newInit))
                    _diagnostics.TypeMismatch(info.Initializer.Syntax.Location,
                        info.Initializer.Expression.TypeSymbol.Name.FullName,
                        info.Field.Type.Name.FullName);

                fieldInits.Add(newInit);
            }
            else
            {
                fieldInits.Add(info.Initializer);
            }
        }

        return fieldInits;

        static IrTypeInitializerField ReWriteInitializerExpression(TypeSymbol type, IrTypeInitializerField initializer)
        {
            var rewriter = new IrExpressionTypeRewriter(type);
            var expr = rewriter.RewriteExpression(initializer.Expression);

            if (expr == initializer.Expression)
                return initializer;

            var field = new FieldSymbol(initializer.Field.Name, type);
            return new IrTypeInitializerField(initializer.Syntax, field, expr);
        }

        static bool TryReWriteInitializerExpression(TypeSymbol type, IrTypeInitializerField initializer, out IrTypeInitializerField rewrittenInitializer)
        {
            rewrittenInitializer = ReWriteInitializerExpression(type, initializer);
            return rewrittenInitializer != initializer;
        }
    }

    private List<IrFieldTypeMatchInfo> CreateFieldTypeInfos(IEnumerable<TypeParameterSymbol> typeParameters, IEnumerable<IrTypeArgument> typeArguments, IEnumerable<FieldSymbol> fields, IEnumerable<IrTypeInitializerField> initializers)
    {
        var typeParamArgs = typeParameters.Zip(typeArguments);
        var lookupTypeParameters = typeParamArgs.ToDictionary(pa => pa.First.Name, p => p);

        var infos = new List<IrFieldTypeMatchInfo>();

        var fieldInits = fields.Zip(initializers);

        foreach (var fldInit in fieldInits)
        {
            var info = new IrFieldTypeMatchInfo(fldInit.First, fldInit.Second);
            if (lookupTypeParameters.TryGetValue(fldInit.First.Type.Name, out var typeParamArg))
            {
                info.TypeParameter = typeParamArg.First;
                info.TypeArgument = typeParamArg.Second;
            }
            infos.Add(info);
        }

        return infos;
    }
}

internal sealed class IrFieldTypeMatchInfo
{
    public IrFieldTypeMatchInfo(FieldSymbol field, IrTypeInitializerField initializer)
    {
        Field = field;
        Initializer = initializer;
    }

    public FieldSymbol Field { get; }
    public IrTypeInitializerField Initializer { get; }
    public TypeParameterSymbol? TypeParameter { get; set; }
    public IrTypeArgument? TypeArgument { get; set; }
}
