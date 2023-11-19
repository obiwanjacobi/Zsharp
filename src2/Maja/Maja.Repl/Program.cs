namespace Maja.Repl;

internal static class Program
{
    static void Main(string[] args)
    {
        var repl = new MajaController();
        repl.Run();
    }
}
