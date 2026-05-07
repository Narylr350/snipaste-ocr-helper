namespace SnipasteOcrHelper.History;

public sealed class OcrHistoryStore
{
    private readonly object gate = new();
    private readonly int capacity;
    private readonly Queue<OcrHistoryEntry> entries = new();

    public event EventHandler? Changed;

    public OcrHistoryStore(int capacity = 100)
    {
        this.capacity = capacity;
    }

    public void Add(OcrHistoryEntry entry)
    {
        lock (gate)
        {
            entries.Enqueue(entry);
            while (entries.Count > capacity)
            {
                entries.Dequeue();
            }
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }

    public IReadOnlyList<OcrHistoryEntry> Snapshot()
    {
        lock (gate)
        {
            return entries.Reverse().ToArray();
        }
    }
}
