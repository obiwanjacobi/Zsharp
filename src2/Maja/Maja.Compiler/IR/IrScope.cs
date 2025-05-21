using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Maja.Compiler.External;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR;

internal abstract class IrScope
{
    protected IrScope(string originalName, IrScope? parent)
    {
        Name = originalName;
        Parent = parent;
    }

    public string Name { get; }
    public virtual string FullName
        => Parent is null
            ? Name
            : Parent.FullName + "." + Name;

    public IrScope? Parent { get; }

    public virtual bool IsExport(SymbolName name)
        => Parent?.IsExport(name) ?? false;

    //
    // Symbol Declarations
    //

    protected SymbolTable SymbolTable { get; } = new();

    public IReadOnlyCollection<Symbol.Symbol> Symbols
        => SymbolTable.Symbols;

    public bool TryDeclareVariable(DeclaredVariableSymbol symbol)
        => SymbolTable.TryDeclareSymbol(symbol);

    public bool TryDeclareType(TypeSymbol symbol)
        => SymbolTable.TryDeclareSymbol(symbol);

    public bool TryDeclareFunction(DeclaredFunctionSymbol symbol)
        => SymbolTable.TryDeclareSymbol(symbol);

    public bool TryLookupSymbol(string name, [NotNullWhen(true)] out Symbol.Symbol? symbol)
        => SymbolTable.TryLookupSymbol<Symbol.Symbol>(name, out symbol);

    public virtual bool TryLookupSymbol<T>(SymbolName name, [NotNullWhen(true)] out T? symbol)
        where T : Symbol.Symbol
    {
        if (SymbolTable.TryLookupSymbol<T>(name.FullName, out symbol))
        {
            return true;
        }

        if (SymbolTable.TryLookupSymbol<T>(name.Value, out symbol) &&
            symbol.Name.Namespace.Value == FullName)
        {
            return true;
        }

        var result = SymbolTable.Symbols.Where(s => name.MatchesWith(s.Name) >= 0);
        if (result.Any())
        {
            Debug.Assert(result.Count() == 1);

            symbol = (T)result.First();
            return true;
        }

        if (Parent is not null)
        {
            return Parent.TryLookupSymbol<T>(name, out symbol);
        }

        symbol = null;
        return false;
    }

    public virtual bool TryLookupFunctionSymbol(SymbolName name, IEnumerable<TypeSymbol> argumentTypes, [NotNullWhen(true)] out DeclaredFunctionSymbol? symbol)
    {
        if (SymbolTable.TryLookupSymbol(name.FullName, out symbol))
        {
            return true;
        }

        if (SymbolTable.TryLookupSymbol(name.Value, out symbol) &&
            symbol.Name.Namespace.Value == FullName)
        {
            return true;
        }

        if (FunctionOverloadPicker.TryPickFunctionSymbol(
            SymbolTable.Symbols.OfType<DeclaredFunctionSymbol>(), name, argumentTypes, out symbol))
            return true;

        if (Parent is not null)
        {
            return Parent.TryLookupFunctionSymbol(name, argumentTypes, out symbol);
        }

        symbol = null;
        return false;
    }

    // find type and locate member on type, then return the type of the member
    public bool TryLookupMemberType(SymbolName type, SymbolName member, [NotNullWhen(true)] out TypeSymbol? memberType)
    {
        if (TryLookupSymbol<DeclaredTypeSymbol>(type, out var typeDecl))
        {
            var field = typeDecl.Fields.SingleOrDefault(f => f.Name == member);
            memberType = field?.Type;
            return memberType is not null;
        }

        memberType = null;
        return false;
    }

    public int TryDeclareTypes(IEnumerable<TypeParameterSymbol> typeParameters)
    {
        var index = 0;
        foreach (var param in typeParameters)
        {
            var name = new SymbolName(param.Name.FullName);
            var symbol = new TypeSymbol(name);
            if (!TryDeclareType(symbol))
                return index;

            index++;
        }

        return -1;
    }

    public int TryDeclareVariables(IEnumerable<ParameterSymbol> parameters)
    {
        var index = 0;
        foreach (var param in parameters)
        {
            var name = new SymbolName(param.Name.FullName);
            var variable = new DeclaredVariableSymbol(name, param.Type);
            if (!TryDeclareVariable(variable))
                return index;

            index++;
        }

        return -1;
    }

    // looks up fields on type and its base-types.
    public bool TryLookupField(DeclaredTypeSymbol typeDecl, string name, [NotNullWhen(true)] out FieldSymbol? field)
    {
        var type = typeDecl;
        while (type != null)
        {
            if (type.Fields.TryLookup(name, out field))
                return true;

            if (type.BaseType is not TypeSymbol baseType ||
                !SymbolTable.TryLookupSymbol<DeclaredTypeSymbol>(baseType.Name, out type))
            {
                break;
            }
        }

        field = null;
        return false;
    }

    //
    // Template Declarations
    //

    private Dictionary<string, IrDeclaration> _templateDecls = new();

    public bool TryRegisterTemplateFunction(IrDeclarationFunction templateFunction)
    {
        Debug.Assert(templateFunction.IsTemplate);
        // TODO: should this be the function-name and type-name combined?
        var name = templateFunction.Symbol.Name.Value;

        if (!_templateDecls.ContainsKey(name))
        {
            _templateDecls.Add(name, templateFunction);
            return true;
        }

        return false;
    }

    public bool TryLookupTemplateFunction(string name, [NotNullWhen(true)] out IrDeclarationFunction? templateFunction)
    {
        templateFunction = null;

        if (_templateDecls.TryGetValue(name, out var templDecl))
        {
            templateFunction = templDecl as IrDeclarationFunction;
            return templateFunction is not null;
        }

        if (Parent is not null)
        {
            return Parent.TryLookupTemplateFunction(name, out templateFunction);
        }

        return false;
    }

    public virtual bool TryRegisterTemplateFunctionInstantiation(string name, IrDeclarationFunction functionDecl)
    {
        if (_templateDecls.ContainsKey(name))
        {
            var moduleScope = this.OfType<IrModuleScope>();
            return moduleScope.TryRegisterTemplateFunctionInstantiation(name, functionDecl);
        }

        return Parent?.TryRegisterTemplateFunctionInstantiation(name, functionDecl) ?? false;
    }

    public bool TryRegisterTemplateType(IrDeclarationType templateType)
    {
        Debug.Assert(templateType.IsTemplate);
        // TODO: should this be the function-name and type-name combined?
        var name = templateType.Symbol.Name.Value;

        if (!_templateDecls.ContainsKey(name))
        {
            _templateDecls.Add(name, templateType);
            return true;
        }

        return false;
    }

    public bool TryLookupTemplateType(string name, [NotNullWhen(true)] out IrDeclarationType? templateType)
    {
        templateType = null;

        if (_templateDecls.TryGetValue(name, out var templDecl))
        {
            templateType = templDecl as IrDeclarationType;
            return templateType is not null;
        }

        if (Parent is not null)
        {
            return Parent.TryLookupTemplateType(name, out templateType);
        }

        return false;
    }

    public virtual bool TryRegisterTemplateTypeInstantiation(string name, IrDeclarationType typeDecl)
    {
        if (_templateDecls.ContainsKey(name))
        {
            var moduleScope = this.OfType<IrModuleScope>();
            return moduleScope.TryRegisterTemplateTypeInstantiation(name, typeDecl);
        }

        return Parent?.TryRegisterTemplateTypeInstantiation(name, typeDecl) ?? false;
    }
}

internal sealed class IrFunctionScope : IrScope
{
    public IrFunctionScope(string name, IrScope? parent)
        : base(name, parent)
    { }
}

internal sealed class IrTypeScope : IrScope
{
    public IrTypeScope(string name, IrScope? parent)
        : base(name, parent)
    { }
}

internal sealed class IrModuleScope : IrScope
{
    private readonly Dictionary<string, ExternalModule> _modules = new();

    public IrModuleScope(string originalName, IrScope parent)
        : base(originalName, parent)
    { }

    public override string FullName => Name;

    private List<SymbolName>? _exports;
    internal void SetExports(IEnumerable<IrExport> exports)
    {
        _exports = exports.Select(exp => exp.Name).ToList();
    }

    public override bool IsExport(SymbolName name)
        => _exports?.Exists(exp => exp.MatchesWith(name) == 0) == true;

    public override bool TryLookupSymbol<T>(SymbolName name, [NotNullWhen(true)] out T? symbol)
        where T : class //Symbol.Symbol
    {
        if (base.TryLookupSymbol(name, out symbol!))
            return true;

        var matches = new List<Symbol.Symbol>();
        foreach (var module in _modules.Values)
        {
            var tps = module.LookupTypes(name);
            matches.AddRange(tps);
        }

        // TODO: select a match - if any

        symbol = (T?)matches.FirstOrDefault();
        return symbol is not null;
    }

    public override bool TryLookupFunctionSymbol(SymbolName name, IEnumerable<TypeSymbol> argumentTypes, [NotNullWhen(true)] out DeclaredFunctionSymbol? symbol)
    {
        if (base.TryLookupFunctionSymbol(name, argumentTypes, out symbol))
            return true;

        var matches = new List<DeclaredFunctionSymbol>();
        foreach (var module in _modules.Values)
        {
            var fns = module.LookupFunctions(name, argumentTypes);
            matches.AddRange(fns);
        }

        Debug.Assert(matches.Count <= 1);

        symbol = matches.FirstOrDefault();
        return symbol is not null;
    }

    private Dictionary<string, IrDeclaration> _templateInstanceDecls = new();
    public IEnumerable<IrDeclaration> TemplateInstantiations
        => _templateInstanceDecls.Values;

    public override bool TryRegisterTemplateFunctionInstantiation(string name, IrDeclarationFunction functionDecl)
    {
        if (!_templateInstanceDecls.ContainsKey(name))
        {
            _templateInstanceDecls.Add(name, functionDecl);
            return true;
        }

        return false;
    }
    public override bool TryRegisterTemplateTypeInstantiation(string name, IrDeclarationType typeDecl)
    {
        if (!_templateInstanceDecls.ContainsKey(name))
        {
            _templateInstanceDecls.Add(name, typeDecl);
            return true;
        }

        return false;
    }

    public bool TryDeclareModule(ExternalModule module)
    {
        var name = module.SymbolName.FullName;

        if (!_modules.ContainsKey(name))
        {
            _modules.Add(name, module);
            return true;
        }

        return false;
    }

    public bool TryLookupModule(SymbolName partialName,
        [NotNullWhen(true)] out ExternalModule? module)
    {
        module = _modules.Values.SingleOrDefault(m => partialName.MatchesWith(m.SymbolName) != -1);
        return module is not null;
    }
}

internal sealed class IrGlobalScope : IrScope
{
    public IrGlobalScope()
        : base("global", null)
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
            throw new InvalidOperationException($"TypeSymbol {symbol.Name} could not be declared in the global scope! Duplicates?");
    }

    public bool TryDeclareModule(ModuleSymbol symbol)
        => SymbolTable.TryDeclareSymbol(symbol);

    public override string FullName
        => String.Empty;
}

internal static class IrScopeExtensions
{
    public static T OfType<T>(this IrScope scope)
        where T : IrScope
    {
        return scope is T
            ? (T)scope
            : scope.Parent?.OfType<T>() ??
                throw new MajaException($"Reached the scope root looking for {typeof(T).Name}.");
        ;
    }
}
