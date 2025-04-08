using System;
using System.Collections.Concurrent;

namespace Indigosoft.User.Ip.Tracker.Database;
public class EventQueue<T>()
{
    private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<EventQueue<T>>();
    private readonly ConcurrentQueue<T> _queue = new();

    public void Enqueue(T item)
    {
        if(item is null)
        {
            Log.Error($"{nameof(item)} cannot be null.");
            throw new ArgumentNullException(nameof(item));
        }

        _queue.Enqueue(item);
        Log.Information($"{nameof(item)} added in queue.");
    }

    public bool TryDequeue(out T item)
    {
        return _queue.TryDequeue(out item);
    }

    public bool IsEmpty => _queue.IsEmpty;
}
