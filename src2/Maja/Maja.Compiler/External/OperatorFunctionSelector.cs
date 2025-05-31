using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.External.Metadata;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.External;

internal sealed class OperatorFunctionSelector
{
    private readonly string _opSymbol;
    private readonly string _retType;
    private readonly List<string> _paramTypes;

    public OperatorFunctionSelector(string operatorSymbol, TypeSymbol returnType, IEnumerable<TypeSymbol> parameterTypes)
    {
        _opSymbol = operatorSymbol;
        _retType = MajaTypeMapper.MapToDotNetType(returnType);
        _paramTypes = parameterTypes.Select(MajaTypeMapper.MapToDotNetType).ToList();
    }

    public bool IsMatch(OperatorFunctionMetadata operatorFunction)
    {
        if (operatorFunction.Attribute.Symbol != _opSymbol) return false;
        if (operatorFunction.ReturnType.FullName != _retType) return false;
        return operatorFunction.Parameters.Select(p => p.ParameterType.FullName).SequenceEqual(_paramTypes);
    }
}
