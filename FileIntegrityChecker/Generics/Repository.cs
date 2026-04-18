namespace FileIntegrityChecker.Generics;

/// <summary>
/// A generic in-memory repository for storing and retrieving any entity type.
/// </summary>
/// <typeparam name="T">The entity type – must be a class.</typeparam>
// OOP: Generics (Type-safe, reusable collection without casting)
// OOP: Encapsulation (private storage, public access methods)
public class Repository<T> where T : class
{
    // OOP: private field
    private readonly List<T> _store = new();

    // OOP: Static field — tracks total repos created
    private static int _instanceCount = 0;

    // OOP: Read-only computed property
    public int Count => _store.Count;

    // OOP: Static Constructor
    static Repository()
    {
        // Runs once for the type; initialize any static state
        Console.WriteLine($"[Repository<{typeof(T).Name}>] Repository type initialized.");
    }

    // OOP: Default Constructor
    public Repository()
    {
        _instanceCount++;
    }

    /// <summary>Adds an item to the repository.</summary>
    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _store.Add(item);
    }

    /// <summary>Removes an item from the repository.</summary>
    public bool Remove(T item) => _store.Remove(item);

    /// <summary>Returns all stored items as a read-only list.</summary>
    public IReadOnlyList<T> GetAll() => _store.AsReadOnly();

    // OOP: Generic Method with predicate
    /// <summary>Finds the first item matching the given predicate.</summary>
    public T? Find(Func<T, bool> predicate) => _store.FirstOrDefault(predicate);

    /// <summary>Filters items using a predicate — demonstrates LINQ + Generics.</summary>
    public IEnumerable<T> Where(Func<T, bool> predicate) => _store.Where(predicate);

    /// <summary>Clears all stored items.</summary>
    public void Clear() => _store.Clear();

    // OOP: Static Method
    public static int GetInstanceCount() => _instanceCount;
}
