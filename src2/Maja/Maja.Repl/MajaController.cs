using Maja.Compiler.Eval;

namespace Maja.Repl;

// maja specific repl additions
internal class MajaController : ReplController
{
    private const char CommandChar = ';';
    private readonly Evaluator _evaluator = new();

    // overrides ReplController virtuals to implement specifics

    protected override bool HandleEnter(ReplDocument document, ReplView view)
    {
        var line = document[view.CurrentLineIndex];

        if (line.EndsWith(CommandChar))
        {
            document.RemoveCharsAt(view.CurrentLineIndex, view.CurrentCharIndex - 1, 1);
            view.CurrentCharIndex--;

            if (view.CurrentCharIndex > 0)
                _ = base.HandleEnter(document, view);

            SubmitInput();
            return true;
        }
        else if (line.StartsWith(CommandChar))
        {
            switch (line[1..])
            {
                case "help":
                    PrintHelpMessage();
                    view.SkipLines(8);
                    document.Clear();
                    break;
                case "cls":
                    Console.Clear();
                    view.Reset();
                    document.Clear();
                    break;
                case "rst":
                    _evaluator.Reset();
                    Console.Clear();
                    view.Reset();
                    document.Clear();
                    break;
                default:
                    break;
            }
            
            return true;
        }

        return base.HandleEnter(document, view);
    }

    protected override bool ProcessInput(string inputText)
    {
        Console.WriteLine();

        var result = _evaluator.Eval(inputText);
        Console.WriteLine();
        if (result.Diagnostics.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var diag in result.Diagnostics)
            {
                Console.WriteLine(diag);
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(result.Value);
        }

        Console.ResetColor();
        return true;
    }

    protected override void DisplayTextLine(ReplDocument document, ReplView view, string line)
    {
        Console.Write(line);
    }

    public static void PrintHelpMessage()
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        
        Console.WriteLine();
        Console.WriteLine("Maja Read-Evaluate-Print Loop (REPL) version 1.0-alpha");
        Console.WriteLine("Write code and end with a ';' to execute.");
        Console.WriteLine("Repl commands:");
        Console.WriteLine("    ;help - prints this message.");
        Console.WriteLine("    ;cls  - clears the input and the screen.");
        Console.WriteLine("    ;rst  - resets the Repl state.");
        Console.WriteLine();

        Console.ResetColor();
    }
}
