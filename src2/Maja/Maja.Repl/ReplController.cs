using System.Diagnostics;

namespace Maja.Repl;

// typical workflow for repl
internal abstract class ReplController
{
    // gather input
    // edit previous entries
    // list state
    // meta commands
    // evaluate/execute

    private bool _inputSubmitted;
    private int _tabWidth = 4;

    public void Load(string file)
    {
        var lines = file.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var line in lines)
        {
            Console.Write(line);

            if (!ProcessInput(line))
                break;
        }
    }

    public void Run()
    {
        while (true)
        {
            var inputText = GetInput();

            if (!ProcessInput(inputText))
                break;
        }
    }

    protected abstract bool ProcessInput(string inputText);

    protected void SubmitInput()
        => _inputSubmitted = true;

    private string GetInput()
    {
        var document = new ReplDocument();
        var view = new ReplView(document);

        _inputSubmitted = false;
        while (!_inputSubmitted)
        {
            var keyInfo = Console.ReadKey(true);
            var handled = HandleKey(keyInfo, document, view);
            Debug.WriteLineIf(!handled, $"{nameof(GetInput)}: '{keyInfo.Key}' was unhandled.");
        }
        return document.ToString()!;
    }

    protected virtual bool HandleKey(ConsoleKeyInfo keyInfo, ReplDocument document, ReplView view)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Backspace:
                return HandleBackspace(document, view);
            case ConsoleKey.Delete:
                return HandleDelete(document, view);
            case ConsoleKey.DownArrow:
                return HandleDownArrow(document, view);
            case ConsoleKey.End:
                return HandleEnd(document, view);
            case ConsoleKey.Enter:
                return HandleEnter(document, view);
            case ConsoleKey.Escape:
                return HandleEscape(document, view);
            case ConsoleKey.Home:
                return HandleHome(document, view);
            case ConsoleKey.LeftArrow:
                return HandleLeftArrow(document, view);
            case ConsoleKey.RightArrow:
                return HandleRightArrow(document, view);
            case ConsoleKey.Tab:
                return HandleTab(document, view);
            case ConsoleKey.UpArrow:
                return HandleUpArrow(document, view);
            default:
                break;
        }

        if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.KeyChar >= ' ')
            return HandleInput(document, view, keyInfo.KeyChar.ToString());

        return false;
    }

    // typing chars that will be part of the input
    protected virtual bool HandleInput(ReplDocument document, ReplView view, string text)
    {
        var charIndex = view.CurrentCharIndex;
        view.CurrentCharIndex += text.Length;
        // fires changed event
        document.InsertCharsAt(view.CurrentLineIndex, charIndex, text);
        return true;
    }

    // deleting the current char (backward)
    protected virtual bool HandleBackspace(ReplDocument document, ReplView view)
    {
        var charIndex = view.CurrentCharIndex;
        if (charIndex == 0)
        {
            if (view.CurrentLineIndex == 0)
                return false;

            var lineIndex = view.CurrentLineIndex;
            view.CurrentLineIndex--;

            var currentLine = document[lineIndex];
            var previousLine = document[view.CurrentLineIndex];
            document.RemoveLineAt(lineIndex);
            // document method?
            document[view.CurrentLineIndex] = previousLine + currentLine;
            view.CurrentCharIndex = previousLine.Length;
        }
        else
        {
            view.CurrentCharIndex--;
            document.RemoveCharsAt(view.CurrentLineIndex, charIndex - 1, 1);
        }

        return true;
    }

    // deleting after the current char (forward)
    protected virtual bool HandleDelete(ReplDocument document, ReplView view)
    {
        var line = document[view.CurrentLineIndex];
        if (view.CurrentCharIndex >= line.Length)
        {
            if (view.CurrentLineIndex == document.LineCount - 1)
                return false;

            var nextLine = document[view.CurrentLineIndex + 1];
            document[view.CurrentLineIndex] += nextLine;
            document.RemoveLineAt(view.CurrentLineIndex + 1);
        }
        document.RemoveCharsAt(view.CurrentLineIndex, view.CurrentCharIndex, 1);
        return true;
    }

    // start a new line at cursor
    protected virtual bool HandleEnter(ReplDocument document, ReplView view)
    {
        var lineIndex = view.CurrentLineIndex;
        var charIndex = view.CurrentCharIndex;

        view.CurrentLineIndex++;
        view.CurrentCharIndex = 0;

        document.NewLineAt(lineIndex, charIndex);
        return true;
    }

    protected virtual bool HandleEscape(ReplDocument document, ReplView view)
    {
        document.Clear();
        view.Reset();
        return true;
    }

    // put cursor at end of line
    protected virtual bool HandleEnd(ReplDocument document, ReplView view)
    {
        view.CurrentCharIndex = document[view.CurrentLineIndex].Length;
        return true;
    }

    // put cursor at beginning of line
    protected virtual bool HandleHome(ReplDocument document, ReplView view)
    {
        view.CurrentCharIndex = 0;
        return true;
    }

    // insert spaces to come to multiple of _tabWidth
    protected virtual bool HandleTab(ReplDocument document, ReplView view)
    {
        var charIndex = view.CurrentCharIndex;
        var roundedChars = _tabWidth - charIndex % _tabWidth;
        view.CurrentCharIndex += roundedChars;
        document.InsertCharsAt(view.CurrentLineIndex, charIndex, new string(' ', roundedChars));
        return true;
    }

    // move the cursor to the left
    protected virtual bool HandleLeftArrow(ReplDocument document, ReplView view)
    {
        if (view.CurrentCharIndex > 0)
        {
            view.CurrentCharIndex--;
            return true;
        }
        if (view.CurrentLineIndex > 0)
        {
            view.CurrentLineIndex--;
            view.CurrentCharIndex = document[view.CurrentLineIndex].Length;
            return true;
        }
        return false;
    }

    // move the cursor to the right
    protected virtual bool HandleRightArrow(ReplDocument document, ReplView view)
    {
        if (view.CurrentCharIndex <= document[view.CurrentLineIndex].Length - 1)
        {
            view.CurrentCharIndex++;
            return true;
        }
        if (view.CurrentLineIndex < document.LineCount - 1)
        {
            view.CurrentLineIndex++;
            view.CurrentCharIndex = 0;
            return true;
        }
        return false;
    }

    // go forward in histroy
    protected virtual bool HandleDownArrow(ReplDocument document, ReplView view)
    {
        if (view.CurrentLineIndex < document.LineCount - 1)
        {
            view.CurrentLineIndex++;
            var line = document[view.CurrentLineIndex];
            if (view.CurrentCharIndex >= line.Length)
                view.CurrentCharIndex = line.Length;
            return true;
        }
        return false;
    }

    // go backward in history
    protected virtual bool HandleUpArrow(ReplDocument document, ReplView view)
    {
        if (view.CurrentLineIndex > 0)
        {
            view.CurrentLineIndex--;
            var line = document[view.CurrentLineIndex];
            if (view.CurrentCharIndex >= line.Length)
                view.CurrentCharIndex = line.Length;
            return true;
        }
        return false;
    }
}
