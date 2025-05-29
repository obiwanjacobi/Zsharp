using System.Collections.Specialized;

namespace Maja.Repl;

internal sealed class ReplView
{
    private readonly ReplDocument _document;
    private int _cursorTop;

    public ReplView(ReplDocument document)
    {
        _document = document;
        _document.CollectionChanged += Document_CollectionChanged;
        _cursorTop = Console.CursorTop;

        Display();
    }

    private void Document_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => Display();

    private int _currentCharIndex;
    public int CurrentCharIndex
    {
        get => _currentCharIndex;
        set
        {
            if (_currentCharIndex != value)
            {
                _currentCharIndex = value;
                SetCursorPos();
            }
        }
    }

    private int _currentLineIndex;
    public int CurrentLineIndex
    {
        get => _currentLineIndex;
        set
        {
            if (_currentLineIndex != value)
            {
                _currentLineIndex = value;
                SetCursorPos();
            }
        }
    }

    private void Display()
    {
        Console.CursorVisible = false;

        var lineCount = 0;
        foreach (var line in _document)
        {
            if (_cursorTop + lineCount >= Console.WindowHeight)
            {
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                Console.WriteLine();
                if (_cursorTop > 0)
                    _cursorTop--;
            }

            Console.SetCursorPosition(0, _cursorTop + lineCount);

            if (lineCount == 0)
                Console.Write("» ");
            else
                Console.Write("· ");

            Console.Write(line);

            // blank out the rest of the line
            Console.Write(new string(' ', Console.WindowWidth - line.Length));
            lineCount++;
        }

        EraseExcessLines(lineCount);
        SetCursorPos();

        Console.ResetColor();
        Console.CursorVisible = true;
    }

    private int _prevLineCount;
    // erase lines when previous display had more lines.
    private void EraseExcessLines(int lineCount)
    {
        if (_prevLineCount > lineCount)
        {
            var blankLine = new string(' ', Console.WindowWidth);
            for (var i = 0; i < _prevLineCount - lineCount; i++)
            {
                Console.SetCursorPosition(0, _cursorTop + lineCount + i);
                Console.WriteLine(blankLine);
            }
        }

        _prevLineCount = lineCount;
    }

    private void SetCursorPos()
    {
        var top = _cursorTop + CurrentLineIndex;
        if (Console.BufferHeight > top)
            Console.CursorTop = top;
        Console.CursorLeft = 2 + CurrentCharIndex;
    }

    public void Reset()
    {
        _cursorTop = 0;
        Clear();
    }

    public void Clear()
    {
        _currentLineIndex = 0;
        _currentCharIndex = 0;
        SetCursorPos();
    }

    public void SkipLines(int lines)
    {
        _cursorTop += lines;
        _currentCharIndex = 0;
        _currentLineIndex = 0;
        SetCursorPos();
    }
}
