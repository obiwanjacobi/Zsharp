using Maja.Compiler.Eval;

namespace Maja.Repl;

// maja specific repl additions
internal class MajaController : ReplController
{
    private const char CommandChar = ';';
    private readonly Evaluator _evaluator = new();

    // overrides Repl virtuals to implement specifics

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
                case "cls":
                    document.Clear();
                    view.Reset();
                    Console.Clear();
                    break;
                case "rst":
                    _evaluator.Reset();
                    break;
                default:
                    break;
            }
            return true;
        }
        else
        {
            return base.HandleEnter(document, view);
        }
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
}
