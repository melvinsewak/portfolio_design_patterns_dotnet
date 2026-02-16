# Object Pool Pattern

## Intent
Manage a pool of reusable objects to improve performance and resource management by avoiding expensive object creation and destruction.

## Also Known As
Resource Pool

## Problem
You need to manage expensive-to-create objects, and:
- Object creation/destruction is costly (database connections, threads, network connections)
- You need many short-lived instances
- Memory allocation/deallocation causes performance issues
- External resources are limited (connection limits, licenses)

Creating and destroying objects repeatedly wastes resources and degrades performance.

## Solution
The Object Pool pattern:
1. Maintains a pool of reusable objects
2. Provides objects from the pool instead of creating new ones
3. Returns objects to the pool after use for reuse
4. Manages pool size and lifecycle

## Structure
```
┌──────────────┐
│    Client    │
└──────────────┘
       │
       │ acquires/releases
       ▼
┌──────────────┐       manages      ┌──────────────┐
│ ObjectPool   │◆──────────────────>│ PooledObject │
├──────────────┤                    ├──────────────┤
│ - pool       │                    │ + reset()    │
├──────────────┤                    │ + use()      │
│ + acquire()  │                    └──────────────┘
│ + release()  │
└──────────────┘
```

## Participants
1. **ObjectPool** - Manages pool of reusable objects
2. **PooledObject** - Object that can be reused
3. **Client** - Acquires objects from pool, returns them after use

## Implementation in C#

### Basic Generic Pool
```csharp
public class ObjectPool<T> where T : class
{
    private readonly ConcurrentBag<T> _objects;
    private readonly Func<T> _objectGenerator;
    private readonly Action<T>? _resetAction;
    
    public ObjectPool(Func<T> objectGenerator, Action<T>? resetAction = null)
    {
        _objects = new ConcurrentBag<T>();
        _objectGenerator = objectGenerator;
        _resetAction = resetAction;
    }
    
    public T Get()
    {
        return _objects.TryTake(out T? item) 
            ? item 
            : _objectGenerator();
    }
    
    public void Return(T item)
    {
        _resetAction?.Invoke(item);
        _objects.Add(item);
    }
}
```

### Usage Example
```csharp
// Create pool
var connectionPool = new ObjectPool<DatabaseConnection>(
    objectGenerator: () => new DatabaseConnection(),
    resetAction: conn => conn.Reset()
);

// Use object from pool
var connection = connectionPool.Get();
connection.ExecuteQuery("SELECT * FROM Users");
connectionPool.Return(connection);
```

### With Disposable Pattern
```csharp
public class PooledObject<T> : IDisposable where T : class
{
    private readonly T _obj;
    private readonly Action<T> _returnAction;
    
    public PooledObject(T obj, Action<T> returnAction)
    {
        _obj = obj;
        _returnAction = returnAction;
    }
    
    public T Object => _obj;
    
    public void Dispose()
    {
        _returnAction(_obj);
    }
}

// Usage with using statement
using (var pooled = pool.GetPooledObject())
{
    var obj = pooled.Object;
    // Use object
} // Automatically returned to pool
```

## When to Use
✅ **Use when:**
- Object creation is expensive (time or resources)
- You need many short-lived objects
- Objects can be reused after reset
- Resource limits exist (connection pools, thread pools)
- Memory allocation causes performance issues

❌ **Avoid when:**
- Objects are cheap to create
- Objects can't be effectively reused
- Pool management overhead exceeds benefits
- Objects maintain state that can't be reset

## Real-World Examples
1. **Database Connection Pools** - ADO.NET, Entity Framework
2. **Thread Pools** - .NET ThreadPool, Task Parallel Library
3. **StringBuilder Pools** - Reduce allocations
4. **HTTP Client Pools** - HttpClientFactory
5. **Buffer Pools** - ArrayPool<T>, MemoryPool<T>

## Advantages
✅ Improves performance (avoid creation overhead)
✅ Reduces memory allocation/garbage collection
✅ Controls resource usage
✅ Predictable resource consumption
✅ Better resource management

## Disadvantages
❌ Adds complexity to codebase
❌ Requires careful state management (reset objects)
❌ Can waste memory if pool too large
❌ Thread-safety considerations
❌ Can mask resource leaks

## Built-in .NET Pool Types

### 1. ArrayPool<T>
```csharp
// Rent array from pool
var buffer = ArrayPool<byte>.Shared.Rent(1024);
try
{
    // Use buffer
}
finally
{
    ArrayPool<byte>.Shared.Return(buffer);
}
```

### 2. MemoryPool<T>
```csharp
using var owner = MemoryPool<byte>.Shared.Rent(1024);
Memory<byte> memory = owner.Memory;
// Use memory
```

### 3. ObjectPool (Microsoft.Extensions.ObjectPool)
```csharp
using Microsoft.Extensions.ObjectPool;

var policy = new DefaultPooledObjectPolicy<StringBuilder>();
var pool = new DefaultObjectPool<StringBuilder>(policy);

var sb = pool.Get();
try
{
    sb.Append("Hello");
}
finally
{
    pool.Return(sb);
}
```

## Implementation Variations

### 1. Bounded Pool (Max Size)
```csharp
public class BoundedObjectPool<T>
{
    private readonly ConcurrentBag<T> _objects;
    private readonly int _maxSize;
    
    public void Return(T item)
    {
        if (_objects.Count < _maxSize)
        {
            _objects.Add(item);
        }
        else
        {
            (item as IDisposable)?.Dispose();
        }
    }
}
```

### 2. Lazy Pool (Grow on Demand)
```csharp
public class LazyObjectPool<T>
{
    private readonly Lazy<ObjectPool<T>> _pool;
    
    public LazyObjectPool(Func<T> factory)
    {
        _pool = new Lazy<ObjectPool<T>>(() => 
            new ObjectPool<T>(factory));
    }
    
    public T Get() => _pool.Value.Get();
}
```

### 3. Pool with Validation
```csharp
public class ValidatingObjectPool<T>
{
    private readonly Func<T, bool> _validator;
    
    public T Get()
    {
        T item;
        do
        {
            item = _pool.Get();
        } while (!_validator(item));
        
        return item;
    }
}
```

## Best Practices

1. **Always reset objects** - Clear state before returning to pool
```csharp
resetAction: sb => sb.Clear()
```

2. **Set max pool size** - Prevent unbounded memory growth
```csharp
var pool = new ObjectPool<T>(factory, reset, maxSize: 100);
```

3. **Use thread-safe collections** - ConcurrentBag, ConcurrentQueue
```csharp
private readonly ConcurrentBag<T> _objects;
```

4. **Dispose pattern** - Auto-return to pool
```csharp
using (var pooled = pool.Rent())
{
    // Use pooled.Object
} // Auto-returned
```

5. **Monitor pool metrics** - Track usage, misses, size
```csharp
public int TotalCreated { get; private set; }
public int CurrentPoolSize => _pool.Count;
```

6. **Consider object lifetime** - Don't pool short-lived objects

## Thread Safety Considerations

### Thread-Safe Pool
```csharp
public class ThreadSafePool<T>
{
    private readonly ConcurrentBag<T> _objects; // Thread-safe
    
    public T Get()
    {
        if (_objects.TryTake(out var item))
            return item;
        return _factory();
    }
}
```

### Lock-Based Pool
```csharp
public class LockBasedPool<T>
{
    private readonly Stack<T> _objects = new();
    private readonly object _lock = new();
    
    public T Get()
    {
        lock (_lock)
        {
            return _objects.Count > 0 
                ? _objects.Pop() 
                : _factory();
        }
    }
}
```

## Common Mistakes

### Mistake 1: Not Resetting State
```csharp
// Wrong - state persists
public void Return(StringBuilder sb)
{
    _pool.Add(sb); // sb still has old content!
}

// Right - reset state
public void Return(StringBuilder sb)
{
    sb.Clear(); // Reset state
    _pool.Add(sb);
}
```

### Mistake 2: Unbounded Pool Growth
```csharp
// Wrong - pool grows infinitely
public void Return(T item)
{
    _pool.Add(item); // No size limit!
}

// Right - enforce max size
public void Return(T item)
{
    if (_pool.Count < _maxSize)
        _pool.Add(item);
    else
        (item as IDisposable)?.Dispose();
}
```

### Mistake 3: Pooling Short-Lived Objects
```csharp
// Wrong - overhead exceeds benefit
var pool = new ObjectPool<int>(() => new int());

// Right - pool expensive objects only
var pool = new ObjectPool<DbConnection>(() => new DbConnection());
```

## Related Patterns
- **Singleton** - Pool can be singleton
- **Factory Method** - Creates pooled objects
- **Flyweight** - Shares objects to reduce memory
- **Prototype** - Can clone instead of creating new objects

## Performance Metrics

Measure pool effectiveness:
```csharp
public class PoolMetrics
{
    public int TotalRequests { get; set; }
    public int PoolHits { get; set; }
    public int PoolMisses { get; set; }
    public double HitRate => (double)PoolHits / TotalRequests;
}
```

## Testing Considerations

```csharp
[Fact]
public void Pool_ReusesObjects()
{
    var createdCount = 0;
    var pool = new ObjectPool<MyObject>(
        () => { createdCount++; return new MyObject(); }
    );
    
    var obj1 = pool.Get();
    pool.Return(obj1);
    
    var obj2 = pool.Get();
    
    Assert.Equal(1, createdCount); // Only created once
    Assert.Same(obj1, obj2); // Same instance
}
```

## Modern .NET Features

### IDisposable Pattern (C# 8+)
```csharp
await using var pooled = await pool.RentAsync();
// Auto-returned when disposed
```

### Span<T> with ArrayPool
```csharp
var buffer = ArrayPool<byte>.Shared.Rent(1024);
try
{
    Span<byte> span = buffer.AsSpan(0, actualSize);
    // Use span
}
finally
{
    ArrayPool<byte>.Shared.Return(buffer);
}
```

## When Not to Use .NET Built-in Pools

Custom pool needed when:
- Object initialization is complex
- Custom reset logic required
- Specific pool sizing strategy needed
- Pool monitoring/metrics required
