using System.Collections.Concurrent;
using HumanHands.Domain.Entities;

namespace HumanHands.Infrastructure.Persistence;

/// <summary>
/// Thread-safe in-memory store for <see cref="TaskItem"/> records.
/// Registered as a singleton; no persistence between restarts.
/// </summary>
public sealed class InMemoryTaskStore
{
    private readonly ConcurrentDictionary<Guid, TaskItem> _store = new();

    public void Add(TaskItem task) => _store[task.Id] = task;

    public TaskItem? FindById(Guid id) =>
        _store.TryGetValue(id, out var task) ? task : null;
}
