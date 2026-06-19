namespace Tests;

internal class SharedWriter
{
    private readonly HashSet<Action<string>> _writers = [];
    private readonly Lock _writersLock = new();

    internal void Add(Action<string> writer)
    {
        lock (_writersLock)
        {
            if (!_writers.Add(writer))
                throw new InvalidOperationException("Add");
        }
    }

    internal void Remove(Action<string> writer)
    {
        lock (_writersLock)
        {
            if (!_writers.Remove(writer))
                throw new InvalidOperationException("Remove");
        }
    }

    internal void Write(string text)
    {
        lock (_writersLock)
        {
            foreach (Action<string> write in _writers)
                write(text);
        }
    }
}
