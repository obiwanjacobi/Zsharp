using System;
using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR;

internal class IrScope
{
    private Dictionary<string, Symbol.Symbol>? _symbols;

    public IrScope(IrScope? parent)
    {
        Parent = parent;
    }

    public IrScope? Parent { get; }

    public IEnumerable<Symbol.Symbol> Symbols
        => _symbols?.Values ?? Enumerable.Empty<Symbol.Symbol>();

    public bool TryDeclareVariable(VariableSymbol symbol)
        => SymbolTable.TryDeclareSymbol(ref _symbols, symbol);

    public bool TryDeclareType(TypeSymbol symbol)
        => SymbolTable.TryDeclareSymbol(ref _symbols, symbol);

    public bool TryDeclareFunction(FunctionSymbol symbol)
        => SymbolTable.TryDeclareSymbol(ref _symbols, symbol);

    public bool TryLookupSymbol(string name, out Symbol.Symbol? symbol)
    {
        if (SymbolTable.TryLookupSymbol(ref _symbols, name, out symbol))
        {
            return true;
        }

        if (Parent is not null)
        {
            return Parent.TryLookupSymbol(name, out symbol);
        }

        symbol = null;
        return false;
    }

    public bool TryLookupSymbol<T>(string name, out T? symbol)
        where T : Symbol.Symbol
    {
        if (SymbolTable.TryLookupSymbol<T>(ref _symbols, name, out symbol))
        {
            return true;
        }

        if (Parent is not null)
        {
            return Parent.TryLookupSymbol<T>(name, out symbol);
        }

        symbol = null;
        return false;
    }

    public int TryDeclareVariables(IEnumerable<ParameterSymbol> parameters)
    {
        int index = 0;
        foreach (var param in parameters)
        {
            var variable = new VariableSymbol(param.Name, param.Type);
            if (!TryDeclareVariable(variable))
                return index;

            index++;
        }

        return -1;
    }
}

internal sealed class IrModuleScope : IrScope
{
    public IrModuleScope(IrScope parent)
        : base(parent)
    { }
}

internal sealed class IrGlobalScope : IrScope
{
    public IrGlobalScope()
        : base(null)
    {
        // register all built-in types
        DeclareType(TypeSymbol.Bool);
        DeclareType(TypeSymbol.C16);
        DeclareType(TypeSymbol.F16);
        DeclareType(TypeSymbol.F32);
        DeclareType(TypeSymbol.F64);
        DeclareType(TypeSymbol.F96);
        DeclareType(TypeSymbol.I16);
        DeclareType(TypeSymbol.I32);
        DeclareType(TypeSymbol.I64);
        DeclareType(TypeSymbol.I8);
        DeclareType(TypeSymbol.Str);
        DeclareType(TypeSymbol.U16);
        DeclareType(TypeSymbol.U32);
        DeclareType(TypeSymbol.U64);
        DeclareType(TypeSymbol.U8);
        DeclareType(TypeSymbol.Void);
    }

    private void DeclareType(TypeSymbol symbol)
    {
        if (!TryDeclareType(symbol))
            throw new InvalidOperationException($"TypeSymbol {symbol.Name} could not be declared in the global scope!");
    }
}