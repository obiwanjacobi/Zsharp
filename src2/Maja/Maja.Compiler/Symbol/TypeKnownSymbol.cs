namespace Maja.Compiler.Symbol;

public static class TypeKnownSymbol
{
    public static readonly TypeSymbol OptOfT = new TypeSymbol(new SymbolName("Opt`1"));
    public static readonly TypeSymbol ErrOfT = new TypeSymbol(new SymbolName("Err`1"));
    public static readonly TypeSymbol MutOfT = new TypeSymbol(new SymbolName("Mut`1"));
    public static readonly TypeSymbol RefOfT = new TypeSymbol(new SymbolName("Ref`1"));
    public static readonly TypeSymbol PtrOfT = new TypeSymbol(new SymbolName("Ptr`1"));
    public static readonly TypeSymbol AtomOfT = new TypeSymbol(new SymbolName("Atom`1"));
}
