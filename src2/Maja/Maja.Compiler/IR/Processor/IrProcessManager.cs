using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using Maja.Compiler.Diagnostics;

namespace Maja.Compiler.IR.Processor;

internal class IrProcessManager
{
    // uses Threading.Channels to do multi-threaded processing

    private readonly Channel<IrProcess> _channel = Channel.CreateUnbounded<IrProcess>();
    private readonly Channel<DiagnosticList> _diagChannel = Channel.CreateUnbounded<DiagnosticList>();

    public IrProcessManager()
    {
        Add(new DeclarationFunctionSymbolsProc());
        Add(new DeclarationTypeSymbolsProc());
        Add(new DeclarationVariableSymbolsProc());
        Add(new CodeBlockSymbolsProc());
    }

    private ConcurrentDictionary<Guid, IrProcess> _resolvedItems = new();

    public bool Enqueue(IrProcess task)
    {
        //if (!_processors.TryGetValue(task.GetType(), out _))
        //    throw new NotSupportedException();

        return _channel.Writer.TryWrite(task);
    }

    // retrieve finished items...how?
    // blocks until ready
    public IrProcessResult GetResult()
    {
        var diagnostics = new DiagnosticList();

        // collect diagnostics from diag-channel
        var notDone = true;
        while (notDone)
        {
            var diag = _diagChannel.Reader.ReadAsync().AsTask().Result;
            diagnostics.AddRange(diag);

            notDone = diag.HasDiagnostics;
        }

        // TODO: I don't think we have enough info to create the IrModule.
        IrModule module = null!;
        return new(module, diagnostics);
    }

    private readonly Dictionary<Type, List<IIrProcessor>> _processors = new();

    public void Add<T>(IrProcessor<T> processor)
        where T : IrProcess
    {
        var type = typeof(T);
        if (_processors.ContainsKey(type))
            _processors[type].Add(processor);
        else
            _processors.Add(type, [processor]);
    }

    private readonly List<Thread> _workers = new();
    private CancellationTokenSource? _cts;

    public void Start()
    {
        _cts = new CancellationTokenSource();

        for (var i = 0; i < Environment.ProcessorCount; i++)
        {
            var thread = new Thread(() => WorkerLoop(_cts.Token))
            {
                IsBackground = true
            };

            _workers.Add(thread);
            thread.Start();
        }
    }

    public void Stop()
    {
        if (_cts is null) return;

        _cts.Cancel();

        foreach (var thread in _workers)
        {
            thread.Join();
        }

        _workers.Clear();
        _cts = null;
    }

    private void WorkerLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var item = _channel.Reader.ReadAsync(token).AsTask().Result;
            if (item == null) return;

            ProcessResolvedReferences(item);

            var type = item.GetType();
            if (_processors.TryGetValue(type, out var processors))
            {
                var context = new IrProcessorContext(item);
                var handled = false;

                foreach (var processor in processors)
                {
                    // don't call processors that are done with this item.
                    if (item.ProcessorProperties.IsDone(processor.GetType())) continue;

                    IrProcessorState? state = null;

                    if (type == typeof(IrProcessFunction))
                        state = ((IrProcessor<IrProcessFunction>)processor).Run(context, (IrProcessFunction)item);

                    if (type == typeof(IrProcessType))
                        state = ((IrProcessor<IrProcessType>)processor).Run(context, (IrProcessType)item);

                    if (type == typeof(IrProcessVariable))
                        state = ((IrProcessor<IrProcessVariable>)processor).Run(context, (IrProcessVariable)item);

                    if (type == typeof(IrProcessCodeBlock))
                        state = ((IrProcessor<IrProcessCodeBlock>)processor).Run(context, (IrProcessCodeBlock)item);

                    if (state is null)
                    {
                        throw new MajaException($"Unsupported IrProcess class: {type.FullName}.");
                    }
                    else if (state.Diagnostics.HasDiagnostics)
                    {
                        _diagChannel.Writer.TryWrite(state.Diagnostics);
                    }
                }

                if (!handled)
                {
                    // TODO:
                    // item is completely done
                    // check it off the list from other items...
                    // How do I access other items?
                    if (item.ProcessorProperties.IsAllDone())
                    {
                        _resolvedItems.AddOrUpdate(item.Id, item, (id, item) => item);
                    }
                    else if (!Enqueue(item))
                    {
                        // TODO: log diagnostic
                    }
                }

                foreach (var task in context.QueuedItems)
                {
                    if (!Enqueue(task))
                    {
                        // TODO: log diagnostic
                    }
                }
            }
            else
            {
                // no processors registered for this type
                // mark as item as done?
            }
        }
    }

    private void ProcessResolvedReferences(IrProcess item)
    {
        var refsToRemove = new List<IrProcess>();

        foreach (var unprocRef in item.Dependencies.UnprocessedReferences)
        {
            if (_resolvedItems.Remove(unprocRef.Id, out var childItem))
            {
                Debug.Assert(unprocRef == childItem);
                refsToRemove.Add(unprocRef);
                item.References.Add(childItem);
            }
        }

        foreach (var itemRef in refsToRemove)
        {
            item.Dependencies.UnprocessedReferences.Remove(itemRef);
        }
    }
}


internal class IrProcessResult
{
    public IrProcessResult(IrModule module, DiagnosticList diagnostics)
    {
        Module = module;
        Diagnostics = diagnostics;
    }

    public DiagnosticList Diagnostics { get; }
    public IrModule Module { get; }
}

internal abstract class IrProcessorProperties
{
    public bool IsDone { get; set; }
}
