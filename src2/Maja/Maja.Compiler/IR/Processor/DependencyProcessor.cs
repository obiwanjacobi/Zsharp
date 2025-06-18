namespace Maja.Compiler.IR.Processor;

internal sealed class DeclarationFunctionSymbolsProc : IrProcessor<IrProcessFunction>
{
    public override IrProcessorState Run(IIrProcessorContext context, IrProcessFunction item)
    {
        item.Symbol = item.Syntax.ToSymbol(item.Scope);

        var codeBlock = new IrProcessCodeBlock(item.Syntax.CodeBlock, item.Scope);
        IrProcessReference.Connect(item, codeBlock);

        context.Enqueue(codeBlock);

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
            IrProcessReference.Connect(declItem, declItem);

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