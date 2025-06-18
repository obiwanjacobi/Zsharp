using System.Collections.Generic;
using Maja.Compiler.Diagnostics;

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
}
