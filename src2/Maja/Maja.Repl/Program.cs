namespace Maja.Repl;

internal static class Program
{
    static void Main(string[] args)
    {
        var repl = new MajaController();

        foreach(var file in args)
        {
            var content = File.ReadAllText(file);
            repl.Load(content);
        }
        
        repl.Run();
    }
}
