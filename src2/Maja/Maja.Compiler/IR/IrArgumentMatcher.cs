using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR;

/// <summary>
/// Matches up (named) function arguments with the declared parameters
/// with optional type arguments and type parameters.
/// </summary>
internal sealed class IrArgumentMatcher
{
    private readonly List<IrArgumentMatchInfo> _argumentInfos;
    private readonly Dictionary<TypeSymbol, TypeSymbol> _typeMap;
    private readonly DiagnosticList _diagnostics = new();

    public IrArgumentMatcher(
        IEnumerable<TypeParameterSymbol> typeParameters, IEnumerable<IrTypeArgument> typeArguments,
        IEnumerable<ParameterSymbol> parameters, IEnumerable<IrArgument> arguments)
    {
        _argumentInfos = CreateArgumentInfos(typeParameters, typeArguments, parameters, arguments);

        _typeMap = _argumentInfos
            .Where(a => a.TypeParameter is not null)
            .ToDictionary(a => new TypeSymbol(a.TypeParameter!.Name), a => a.TypeArgument!.Type.Symbol);
    }

    public DiagnosticList Diagnostics => _diagnostics;

    public List<IrArgument> RewriteArgumentTypes()
    {
        var args = new List<IrArgument>();

        foreach (var info in _argumentInfos)
        {
            if (info.Argument.Expression.TypeInferredSymbol is not null)
            {
                var type = info.Parameter.Type;

                if (info.TypeArgument is not null)
                    type = info.TypeArgument.Type.Symbol;

                // rewrite argument expression with type-argument type
                args.Add(ReWriterArgumentExpression(type, info.Argument));
            }
            else if (info.Argument.Expression.TypeSymbol.Name != info.Parameter.Type.Name)
            {
                _diagnostics.TypeMismatch(info.Argument.Syntax.Location,
                    info.Argument.Expression.TypeSymbol.Name.FullName,
                    info.Parameter.Type.Name.FullName);

                args.Add(info.Argument);
            }
            else
            {
                args.Add(info.Argument);
            }
        }

        return args;

        static IrArgument ReWriterArgumentExpression(TypeSymbol type, IrArgument argument)
        {
            var rewriter = new IrExpressionTypeRewriter(type);
            var expr = rewriter.RewriteExpression(argument.Expression);

            if (expr == argument.Expression)
                return argument;

            return new IrArgument(argument.Syntax, expr, argument.Symbol);
        }
    }

    public bool TryMapSymbol(TypeSymbol keySymbol, [NotNullWhen(true)] out TypeSymbol? valueSymbol)
        => _typeMap.TryGetValue(keySymbol, out valueSymbol);

    private List<IrArgumentMatchInfo> CreateArgumentInfos(
        IEnumerable<TypeParameterSymbol> typeParameters, IEnumerable<IrTypeArgument> typeArguments,
        IEnumerable<ParameterSymbol> parameters, IEnumerable<IrArgument> arguments)
    {
        // We do not support named (out-of-order) type arguments yet.
        var typeParamArgs = typeParameters.Zip(typeArguments);
        var lookupTypeParameters = typeParamArgs.ToDictionary(pa => pa.First.Name, p => p);
        var lookupParams = parameters.ToDictionary(p => p.Name, p => p);
        var namedArgs = arguments.Where(a => a.Symbol is not null);

        var infos = new List<IrArgumentMatchInfo>();

        if (namedArgs.Any())
        {
            foreach (var arg in namedArgs)
            {
                if (!lookupParams.TryGetValue(arg.Symbol!.Name, out var parameter))
                {
                    _diagnostics.NoParameterForNamedArgument(arg.Syntax.Location, arg.Symbol!.Name.Value);
                    continue;
                }

                var info = new IrArgumentMatchInfo(parameter, arg);
                if (lookupTypeParameters.TryGetValue(parameter.Type.Name, out var typeParamArg))
                {
                    info.TypeParameter = typeParamArg.First;
                    info.TypeArgument = typeParamArg.Second;
                }
                infos.Add(info);
            }
        }
        else // args in order
        {
            var paramArgs = parameters.Zip(arguments);

            foreach (var paramArg in paramArgs)
            {
                var info = new IrArgumentMatchInfo(paramArg.First, paramArg.Second);
                if (lookupTypeParameters.TryGetValue(paramArg.First.Type.Name, out var typeParamArg))
                {
                    info.TypeParameter = typeParamArg.First;
                    info.TypeArgument = typeParamArg.Second;
                }
                infos.Add(info);
            }
        }

        return infos;
    }
}

internal sealed class IrArgumentMatchInfo
{
    public IrArgumentMatchInfo(ParameterSymbol parameter, IrArgument argument)
    {
        Parameter = parameter;
        Argument = argument;
    }

    public ParameterSymbol Parameter { get; }
    public IrArgument Argument { get; }
    public TypeParameterSymbol? TypeParameter { get; set; }
    public IrTypeArgument? TypeArgument { get; set; }
}