using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR.Processor;

internal interface IIrProcessor
{

}

internal abstract class IrProcessor<T> : IIrProcessor
    where T : IrProcess
{
    // processes a specific type of item.

    public abstract IrProcessorState Run(IIrProcessorContext context, T item);
}

internal class IrProcessorState
{
    // result of a Processor action

    private IrProcessorState()
    {
        Diagnostics = new();
    }

    public DiagnosticList Diagnostics { get; }

    // processed items (count?)

    public static IrProcessorState Empty = new IrProcessorState();
}


internal interface IIrProcessorContext
{
    bool Enqueue(IrProcess task);

    bool TryLookupFunction(DeclarationFunctionSyntax syntax, [NotNullWhen(true)] out IrProcessFunction? function);
    bool TryLookupVariable(DeclarationVariableSyntax syntax, [NotNullWhen(true)] out IrProcessVariable? variable);
    bool TryLookupType(DeclarationTypeSyntax syntax, [NotNullWhen(true)] out IrProcessType? type);
    bool TryLookupCodeBlock(CodeBlockSyntax syntax, [NotNullWhen(true)] out IrProcessCodeBlock? codeBlock);
}

internal class IrProcessorContext : IIrProcessorContext
{
    // interaction of Processor with the ProcessManager

    private readonly IrProcess _item;
    internal IrProcessorContext(IrProcess item)
    {
        _item = item;
    }

    private List<IrProcess> _queuedItems = new();
    internal IEnumerable<IrProcess> QueuedItems => _queuedItems;

    public bool Enqueue(IrProcess task)
    {
        if (task == _item)
            throw new MajaException($"Do not enqueue the current item that is being processed: {task}");

        _queuedItems.Add(task);
        return true;
    }

    public bool TryLookupFunction(DeclarationFunctionSyntax syntax, [NotNullWhen(true)] out IrProcessFunction? function)
    {
        function = _item.References.OfType<IrProcessFunction>().Where(p => p.Syntax == syntax).SingleOrDefault();
        return function is not null;
    }
    public bool TryLookupVariable(DeclarationVariableSyntax syntax, [NotNullWhen(true)] out IrProcessVariable? variable)
    {
        variable = _item.References.OfType<IrProcessVariable>().Where(p => p.Syntax == syntax).SingleOrDefault();
        return variable is not null;
    }
    public bool TryLookupType(DeclarationTypeSyntax syntax, [NotNullWhen(true)] out IrProcessType? type)
    {
        type = _item.References.OfType<IrProcessType>().Where(p => p.Syntax == syntax).SingleOrDefault();
        return type is not null;
    }
    public bool TryLookupCodeBlock(CodeBlockSyntax syntax, [NotNullWhen(true)] out IrProcessCodeBlock? codeBlock)
    {
        codeBlock = _item.References.OfType<IrProcessCodeBlock>().Where(p => p.Syntax == syntax).SingleOrDefault();
        return codeBlock is not null;
    }
}
