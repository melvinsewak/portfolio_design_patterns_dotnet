using System.Collections.Concurrent;
using System.Text;

namespace DesignPatterns.CreationalPatterns;

/// <summary>
/// Generic Object Pool
/// Manages a pool of reusable objects to avoid expensive creation/destruction
/// </summary>
public class ObjectPool<T> where T : class
{
    private readonly ConcurrentBag<T> _objects;
    private readonly Func<T> _objectGenerator;
    private readonly Action<T>? _resetAction;
    private readonly int _maxSize;
    
    public ObjectPool(Func<T> objectGenerator, Action<T>? resetAction = null, int maxSize = 100)
    {
        _objects = new ConcurrentBag<T>();
        _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
        _resetAction = resetAction;
        _maxSize = maxSize;
    }
    
    public T Get()
    {
        if (_objects.TryTake(out T? item))
        {
            Console.WriteLine($"Reusing object from pool. Pool size: {_objects.Count}");
            return item;
        }
        
        Console.WriteLine("Creating new object. Pool was empty.");
        return _objectGenerator();
    }
    
    public void Return(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        
        if (_objects.Count < _maxSize)
        {
            _resetAction?.Invoke(item);
            _objects.Add(item);
            Console.WriteLine($"Returned object to pool. Pool size: {_objects.Count}");
        }
        else
        {
            Console.WriteLine("Pool is full. Discarding object.");
            if (item is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
    
    public int Count => _objects.Count;
}

/// <summary>
/// Example: Database Connection Pool
/// </summary>
public class DatabaseConnection : IDisposable
{
    public Guid ConnectionId { get; }
    public DateTime CreatedAt { get; }
    public DateTime LastUsed { get; private set; }
    public bool IsOpen { get; private set; }
    
    public DatabaseConnection()
    {
        ConnectionId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        LastUsed = DateTime.UtcNow;
        Console.WriteLine($"[EXPENSIVE] Creating new connection: {ConnectionId}");
    }
    
    public void Open()
    {
        IsOpen = true;
        Console.WriteLine($"Connection {ConnectionId} opened");
    }
    
    public void Close()
    {
        IsOpen = false;
        Console.WriteLine($"Connection {ConnectionId} closed");
    }
    
    public void ExecuteQuery(string query)
    {
        if (!IsOpen)
            throw new InvalidOperationException("Connection is not open");
        
        LastUsed = DateTime.UtcNow;
        Console.WriteLine($"Executing query on connection {ConnectionId}: {query}");
    }
    
    public void Reset()
    {
        Close();
        Console.WriteLine($"Connection {ConnectionId} reset");
    }
    
    public void Dispose()
    {
        Close();
        Console.WriteLine($"[CLEANUP] Connection {ConnectionId} disposed");
    }
}

/// <summary>
/// Connection Pool Manager
/// </summary>
public class DatabaseConnectionPool
{
    private readonly ObjectPool<DatabaseConnection> _pool;
    
    public DatabaseConnectionPool(int maxSize = 10)
    {
        _pool = new ObjectPool<DatabaseConnection>(
            objectGenerator: () => new DatabaseConnection(),
            resetAction: conn => conn.Reset(),
            maxSize: maxSize
        );
    }
    
    public DatabaseConnection AcquireConnection()
    {
        var connection = _pool.Get();
        connection.Open();
        return connection;
    }
    
    public void ReleaseConnection(DatabaseConnection connection)
    {
        if (connection == null) return;
        _pool.Return(connection);
    }
    
    public int AvailableConnections => _pool.Count;
}

/// <summary>
/// Example: Thread Pool (simplified simulation)
/// </summary>
public class WorkerThread
{
    public int ThreadId { get; }
    public bool IsBusy { get; set; }
    
    public WorkerThread(int threadId)
    {
        ThreadId = threadId;
        Console.WriteLine($"[EXPENSIVE] Creating worker thread: {ThreadId}");
    }
    
    public void ExecuteWork(Action work)
    {
        IsBusy = true;
        Console.WriteLine($"Thread {ThreadId} executing work...");
        work();
        IsBusy = false;
        Console.WriteLine($"Thread {ThreadId} completed work");
    }
    
    public void Reset()
    {
        IsBusy = false;
        Console.WriteLine($"Thread {ThreadId} reset to idle state");
    }
}

public class ThreadPool
{
    private readonly ObjectPool<WorkerThread> _pool;
    private static int _nextThreadId = 1;
    
    public ThreadPool(int maxThreads = 5)
    {
        _pool = new ObjectPool<WorkerThread>(
            objectGenerator: () => new WorkerThread(_nextThreadId++),
            resetAction: thread => thread.Reset(),
            maxSize: maxThreads
        );
    }
    
    public void QueueWork(Action work)
    {
        var thread = _pool.Get();
        thread.ExecuteWork(work);
        _pool.Return(thread);
    }
}

/// <summary>
/// Example: String Builder Pool
/// </summary>
public class StringBuilderPool
{
    private static readonly ObjectPool<StringBuilder> _pool = new(
        objectGenerator: () => new StringBuilder(capacity: 256),
        resetAction: sb => sb.Clear(),
        maxSize: 20
    );
    
    public static StringBuilder Get() => _pool.Get();
    
    public static void Return(StringBuilder sb) => _pool.Return(sb);
    
    public static string BuildString(Action<StringBuilder> buildAction)
    {
        var sb = Get();
        try
        {
            buildAction(sb);
            return sb.ToString();
        }
        finally
        {
            Return(sb);
        }
    }
}

/// <summary>
/// Example: Disposable Pattern with Object Pool
/// </summary>
public class PooledObject<T> : IDisposable where T : class
{
    private readonly T _obj;
    private readonly Action<T> _returnAction;
    private bool _disposed;
    
    public PooledObject(T obj, Action<T> returnAction)
    {
        _obj = obj ?? throw new ArgumentNullException(nameof(obj));
        _returnAction = returnAction ?? throw new ArgumentNullException(nameof(returnAction));
    }
    
    public T Object => _disposed ? throw new ObjectDisposedException(nameof(PooledObject<T>)) : _obj;
    
    public void Dispose()
    {
        if (!_disposed)
        {
            _returnAction(_obj);
            _disposed = true;
        }
    }
}

public class PooledObjectFactory<T> where T : class
{
    private readonly ObjectPool<T> _pool;
    
    public PooledObjectFactory(Func<T> objectGenerator, Action<T>? resetAction = null)
    {
        _pool = new ObjectPool<T>(objectGenerator, resetAction);
    }
    
    public PooledObject<T> GetPooledObject()
    {
        var obj = _pool.Get();
        return new PooledObject<T>(obj, _pool.Return);
    }
}

public static class ObjectPoolDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Object Pool Pattern Demo ===\n");
        
        // Database Connection Pool
        Console.WriteLine("1. Database Connection Pool:");
        var dbPool = new DatabaseConnectionPool(maxSize: 3);
        
        var conn1 = dbPool.AcquireConnection();
        conn1.ExecuteQuery("SELECT * FROM Users");
        dbPool.ReleaseConnection(conn1);
        
        var conn2 = dbPool.AcquireConnection(); // Should reuse conn1
        conn2.ExecuteQuery("SELECT * FROM Orders");
        
        var conn3 = dbPool.AcquireConnection();
        conn3.ExecuteQuery("SELECT * FROM Products");
        
        dbPool.ReleaseConnection(conn2);
        dbPool.ReleaseConnection(conn3);
        
        // Thread Pool
        Console.WriteLine("\n2. Thread Pool:");
        var threadPool = new ThreadPool(maxThreads: 2);
        
        threadPool.QueueWork(() => Console.WriteLine("Task 1 running"));
        threadPool.QueueWork(() => Console.WriteLine("Task 2 running"));
        threadPool.QueueWork(() => Console.WriteLine("Task 3 running"));
        
        // StringBuilder Pool
        Console.WriteLine("\n3. StringBuilder Pool:");
        var result = StringBuilderPool.BuildString(sb =>
        {
            sb.Append("Hello");
            sb.Append(" ");
            sb.Append("from");
            sb.Append(" ");
            sb.Append("pooled");
            sb.Append(" ");
            sb.Append("StringBuilder!");
        });
        Console.WriteLine($"Built string: {result}");
        
        // Using disposable pattern
        Console.WriteLine("\n4. Disposable Pooled Objects:");
        var factory = new PooledObjectFactory<DatabaseConnection>(
            objectGenerator: () => new DatabaseConnection(),
            resetAction: conn => conn.Reset()
        );
        
        using (var pooled = factory.GetPooledObject())
        {
            var connection = pooled.Object;
            connection.Open();
            connection.ExecuteQuery("SELECT * FROM Settings");
        } // Automatically returned to pool on disposal
        
        Console.WriteLine("\nPool Demo Complete");
    }
}
