using System;
using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR.Processor;

internal abstract class IrProcess
{
    // base class for specializations
    protected IrProcess()
    {
        Id = Guid.NewGuid();
        References = new();
        Dependencies = new();
        ProcessorProperties = new();
    }

    public Guid Id { get; }

    // out standing work
    public IrProcessDependencies Dependencies { get; }

    // dependencies that are done
    public List<IrProcess> References { get; }

    // processor states (each processor can store state in item)
    public IrProcessorBag ProcessorProperties { get; }

    public void AddReference(IrProcess child)
        => Dependencies.UnprocessedReferences.Add(child);

    public IrProcessFunction FindReference(DeclarationFunctionSyntax syntax)
        => References.OfType<IrProcessFunction>().Single(r => r.Syntax == syntax);
    public IrProcessType FindReference(DeclarationTypeSyntax syntax)
        => References.OfType<IrProcessType>().Single(r => r.Syntax == syntax);
    public IrProcessVariable FindReference(DeclarationVariableSyntax syntax)
        => References.OfType<IrProcessVariable>().Single(r => r.Syntax == syntax);
    public IrProcessCodeBlock FindReference(CodeBlockSyntax syntax)
        => References.OfType<IrProcessCodeBlock>().Single(r => r.Syntax == syntax);
}


internal class IrProcessType : IrProcess
{
    // processes a type declaration
    public IrProcessType(DeclarationTypeSyntax syntax, IrTypeScope scope)
    {
        Scope = scope;
        Syntax = syntax;
    }

    public IrTypeScope Scope { get; }

    public DeclarationTypeSyntax Syntax { get; }

    public DeclaredTypeSymbol? Symbol { get; internal set; }

    public IrDeclarationType? Model { get; internal set; }
}


internal class IrProcessFunction : IrProcess
{
    // processes a function declaration

    public IrProcessFunction(DeclarationFunctionSyntax syntax, IrFunctionScope scope)
    {
        Scope = scope;
        Syntax = syntax;
    }

    public IrFunctionScope Scope { get; }

    public DeclarationFunctionSyntax Syntax { get; }

    public DeclaredFunctionSymbol? Symbol { get; internal set; }

    public IrDeclarationFunction? Model { get; internal set; }
}

internal class IrProcessVariable : IrProcess
{
    // processes a variable declaration

    public IrProcessVariable(DeclarationVariableSyntax syntax, IrScope scope)
    {
        Scope = scope;
        Syntax = syntax;
    }

    public IrScope Scope { get; }

    public DeclarationVariableSyntax Syntax { get; }

    public DeclaredVariableSymbol? Symbol { get; internal set; }

    public IrDeclarationVariable? Model { get; internal set; }
}

// Only global-init, function, if(-else)-statement and loop-statement have codeblocks
// based on the passed syntax object (ref-equals) the exact location in the parent can be found.
internal class IrProcessCodeBlock : IrProcess
{
    // processes a code block (resolve dependencies)
    public IrProcessCodeBlock(CodeBlockSyntax syntax, IrScope scope)
    {
        Scope = scope;
        Syntax = syntax;
        Symbols = [];
    }

    public IrScope Scope { get; }

    public CodeBlockSyntax Syntax { get; }

    public IrCodeBlock? Model { get; internal set; }

    public IEnumerable<Symbol.Symbol> Symbols { get; internal set; }
}

internal class IrProcessDependencies
{
    // when all are resolved/processed, the item is done
    // when a reference is done processing, it is moved to the main IrProcessXxx references collection
    public IrProcessDependencies(IEnumerable<Symbol.Symbol> symbols, IEnumerable<IrProcess> references)
    {
        UnresolvedSymbols = symbols.ToList();
        UnprocessedReferences = references.ToList();
    }
    public IrProcessDependencies()
    {
        UnresolvedSymbols = new();
        UnprocessedReferences = new();
    }

    // only symbols referenced by the item itself.
    public List<Symbol.Symbol> UnresolvedSymbols { get; }
    // sub-items that this items depends on.
    public List<IrProcess> UnprocessedReferences { get; }

    public bool IsDone
        => UnprocessedReferences.Count == 0 && UnresolvedSymbols.Count == 0;
}

internal class IrProcessorBag
{
    private readonly Dictionary<Type, IrProcessorProperties> _bag = new();

    public T Get<T>(Type type, Func<T> createNew)
        where T : IrProcessorProperties
    {
        if (_bag.ContainsKey(type))
            return (T)_bag[type];

        var props = createNew();
        _bag[type] = props;
        return props;
    }

    public bool IsDone(Type type)
    {
        if (_bag.ContainsKey(type))
            return _bag[type].IsDone;

        return false;
    }

    public bool IsAllDone()
        => _bag.Values.All(prop => prop.IsDone);
}
