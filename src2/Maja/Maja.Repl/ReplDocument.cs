using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Maja.Repl;

internal sealed class ReplDocument : IEnumerable<string>, INotifyCollectionChanged
{
    private readonly ObservableCollection<string> _lines = new();

    public ReplDocument()
        => Clear();

    public int LineCount => _lines.Count;

    public string this[int index]
    {
        get => _lines[index];
        set => _lines[index] = value;
    }

    public void Clear()
    {
        _lines.Clear();
        _lines.Add("");
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add => _lines.CollectionChanged += value;
        remove => _lines.CollectionChanged -= value;
    }

    public IEnumerator<string> GetEnumerator()
        => _lines.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()
        => _lines.GetEnumerator();

    public override string ToString()
        => String.Join(Environment.NewLine, _lines);

    public void InsertCharsAt(int lineIndex, int charIndex, string text)
    {
        var newLine = _lines[lineIndex].Insert(charIndex, text);
        _lines[lineIndex] = newLine;
    }

    public void RemoveCharsAt(int lineIndex, int charIndex, int charCount)
    {
        var line = _lines[lineIndex];
        var before = line[..charIndex];
        var after = line[(charIndex + charCount)..];
        _lines[lineIndex] = before + after;
    }

    public void NewLineAt(int lineIndex, int charIndex)
    {
        var line = _lines[lineIndex];
        var before = line[..charIndex];
        var after = line[charIndex..];

        _lines[lineIndex] = before;

        lineIndex++;
        _lines.Insert(lineIndex, after);
    }

    public void RemoveLineAt(int lineIndex)
    {
        _lines.RemoveAt(lineIndex);

        if (_lines.Count == 0)
            Clear();
    }
}
