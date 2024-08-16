namespace SystemTests;

internal class SharedWriter
{
    private readonly HashSet<Action<string>> Writers = [];

    internal void Add(Action<string> writer)
    {
        lock (Writers)
        {
            if (!Writers.Add(writer))
                throw new InvalidOperationException("Add");
        }
    }

    internal void Remove(Action<string> writer)
    {
        lock (Writers)
        {
            if (!Writers.Remove(writer))
                throw new InvalidOperationException("Remove");
        }
    }

    internal void Write(string text)
    {
        lock (Writers)
        {
            foreach (Action<string> write in Writers)
                write(text);
        }
    }
}
