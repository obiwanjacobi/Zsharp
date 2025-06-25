namespace Maja.Compiler.IR.Processor;

internal sealed class DeclarationFunctionSymbolsProc : IrProcessor<IrProcessFunction>
{
    public override IrProcessorState Run(IIrProcessorContext context, IrProcessFunction item)
    {
        if (item.Symbol is null)
        {
            item.Symbol = item.Syntax.ToSymbol(item.Scope);
            var codeBlock = new IrProcessCodeBlock(item.Syntax.CodeBlock, item.Scope);
            item.AddReference(codeBlock);
            context.Enqueue(codeBlock);
        }
        else if (!item.Symbol.IsUnresolved)
        {
            // TODO: this only needs to happen once
            if (!item.Scope.TryDeclareFunction(item.Symbol))
            {
                // diagnostic
            }

            if (item.Dependencies.IsDone)
            {
                var factory = new IrFactory(context, item);
                item.Model = factory.DeclarationFunction(item.Syntax, item.Symbol, item.Scope);

            }
        }
        return IrProcessorState.Empty;
    }
}

internal sealed class DeclarationTypeSymbolsProc : IrProcessor<IrProcessType>
{
    public override IrProcessorState Run(IIrProcessorContext context, IrProcessType item)
    {
        item.Symbol = item.Syntax.ToSymbol(item.Scope);
        return IrProcessorState.Empty;
    }
}

internal sealed class DeclarationVariableSymbolsProc : IrProcessor<IrProcessVariable>
{
    public override IrProcessorState Run(IIrProcessorContext context, IrProcessVariable item)
    {
        item.Symbol = item.Syntax.ToSymbol(item.Scope);
        return IrProcessorState.Empty;
    }
}

internal sealed class CodeBlockSymbolsProc : IrProcessor<IrProcessCodeBlock>
{
    public override IrProcessorState Run(IIrProcessorContext context, IrProcessCodeBlock item)
    {
        foreach (var decl in item.Syntax.Members)
        {
            var declItem = IrProcessBuilder.Declaration(decl, item.Scope);
            item.AddReference(declItem);

            context.Enqueue(declItem);
        }

        //foreach (var stat in item.Syntax.Statements)
        //{
        //    IrProcessBuilder.Statement(stat, item.Scope);
        //}

        var props = item.ProcessorProperties.Get(GetType(), () => new MakeSymbolsProps());
        props.IsDone = true;

        return IrProcessorState.Empty;
    }
}


internal sealed class MakeSymbolsProps : IrProcessorProperties
{
}