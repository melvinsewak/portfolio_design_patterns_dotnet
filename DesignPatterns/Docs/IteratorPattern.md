# Iterator Pattern

## Intent
The Iterator pattern provides a way to access the elements of an aggregate object sequentially without exposing its underlying representation.

## Problem
When you need to:
- Access elements of a collection without exposing internal structure
- Support multiple simultaneous traversals of a collection
- Provide a uniform interface for traversing different collection types
- Decouple collection traversal from collection implementation

**Without the pattern:**
- Client code depends on collection's internal structure
- Hard to change collection implementation
- Cannot easily support different traversal algorithms
- Violates encapsulation

## Solution
Define an iterator interface that provides methods to traverse a collection. Concrete iterators implement the traversal algorithm, and collections provide methods to create appropriate iterators.

## UML Diagram
```
┌──────────────────┐       ┌───────────────────┐
│   <<interface>>  │       │  <<interface>>    │
│    Iterator<T>   │       │   Aggregate<T>    │
├──────────────────┤       ├───────────────────┤
│+ HasNext(): bool │       │+ CreateIterator() │
│+ Next(): T       │       │  : Iterator<T>    │
│+ Reset()         │       └─────────┬─────────┘
└────────△─────────┘                 △
         │                           │
         │implements                 │implements
         │                           │
┌────────┴─────────┐       ┌────────┴──────────┐
│ ConcreteIterator │←──────│ ConcreteAggregate │
├──────────────────┤       ├───────────────────┤
│- collection      │       │- items[]          │
│- position        │       │+ Count            │
│+ HasNext()       │       │+ CreateIterator() │
│+ Next()          │       │+ GetItem(index)   │
└──────────────────┘       └───────────────────┘
```

## When to Use

### Use When:
- **Encapsulation** - Want to hide collection's internal structure
- **Multiple traversals** - Need different ways to traverse a collection
- **Uniform interface** - Want consistent access to different collections
- **Simplified client** - Reduce client complexity by delegating iteration
- **Custom traversal** - Need specialized iteration algorithms
- **Concurrent iteration** - Multiple iterators on same collection

### Avoid When:
- **Simple collections** - Built-in foreach/LINQ is sufficient
- **Single traversal** - Only one way to iterate
- **Direct access needed** - Indexed access is more appropriate
- **Performance critical** - Iterator overhead is unacceptable

## Real-World Examples

### 1. **File System Navigation**
   - Directory trees with in-order, pre-order, post-order traversal
   - Breadth-first vs depth-first iteration

### 2. **Social Media Feeds**
   - Chronological iterator (newest first)
   - Popularity iterator (most liked first)
   - Filtered iterator (specific categories)

### 3. **Music Playlist**
   - Sequential playback
   - Shuffle mode
   - Repeat mode

### 4. **Database Result Sets**
   - Forward-only iterator
   - Bidirectional iterator
   - Scrollable cursor

### 5. **Binary Tree Traversal**
   - In-order traversal
   - Pre-order traversal
   - Post-order traversal
   - Level-order (breadth-first)

### 6. **Pagination**
   - Page-by-page iteration through large datasets
   - Lazy loading of results

## Advantages

1. **Encapsulation** - Collection internals remain hidden
2. **Multiple Iterators** - Can have multiple concurrent traversals
3. **Uniform Interface** - Same interface for different collections
4. **Flexibility** - Easy to add new iteration algorithms
5. **Single Responsibility** - Separates traversal from collection
6. **Simplified Collection** - Collection doesn't need iteration logic

## Disadvantages

1. **Overhead** - Extra classes and complexity
2. **Less Efficient** - Can be slower than direct access
3. **Modification Issues** - Concurrent modification problems
4. **Memory** - Multiple iterators consume memory
5. **Overkill** - Simple collections don't need custom iterators

## Best Practices

### 1. **Implement IEnumerable<T> in C#**
```csharp
public class CustomCollection<T> : IEnumerable<T>
{
    private readonly List<T> _items = new();
    
    public IEnumerator<T> GetEnumerator()
    {
        return _items.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
```

### 2. **Use yield return for Simple Iterators**
```csharp
public IEnumerable<T> GetItems()
{
    foreach (var item in _items)
    {
        yield return item;
    }
}
```

### 3. **Provide Multiple Iterators**
```csharp
public class TreeNode<T>
{
    public IEnumerable<T> InOrder() { /* ... */ }
    public IEnumerable<T> PreOrder() { /* ... */ }
    public IEnumerable<T> PostOrder() { /* ... */ }
}
```

### 4. **Handle Concurrent Modification**
```csharp
public class SafeIterator<T> : IEnumerator<T>
{
    private readonly int _version;
    private readonly Collection<T> _collection;
    
    public bool MoveNext()
    {
        if (_version != _collection.Version)
            throw new InvalidOperationException("Collection was modified");
        // ...
    }
}
```

### 5. **Implement Reset Carefully**
```csharp
public void Reset()
{
    _position = -1;
    // Be cautious: Reset is often not implemented in .NET
}
```

### 6. **Use Iterator Pattern with LINQ**
```csharp
public IEnumerable<T> Where(Func<T, bool> predicate)
{
    foreach (var item in this)
    {
        if (predicate(item))
            yield return item;
    }
}
```

### 7. **Lazy Evaluation**
```csharp
public IEnumerable<T> LazyLoad()
{
    for (int i = 0; i < _count; i++)
    {
        yield return LoadItem(i); // Load on demand
    }
}
```

## Related Patterns

- **Composite**: Often used with Composite for tree traversal
- **Factory Method**: Iterator creation can use Factory Method
- **Memento**: Can save iterator state with Memento
- **Visitor**: Visitor can use Iterator to traverse structures

## C# Specific Considerations

### Using IEnumerable<T> and IEnumerator<T>
```csharp
public class MyCollection<T> : IEnumerable<T>
{
    public IEnumerator<T> GetEnumerator()
    {
        // Return enumerator
    }
}

// Usage with foreach
foreach (var item in collection)
{
    Console.WriteLine(item);
}
```

### Iterator Method with yield
```csharp
public IEnumerable<int> GetNumbers()
{
    yield return 1;
    yield return 2;
    yield return 3;
}
```

### LINQ and Deferred Execution
```csharp
var query = collection
    .Where(x => x > 10)
    .Select(x => x * 2);
// Query not executed until enumeration
```

### Async Iterators (C# 8+)
```csharp
public async IAsyncEnumerable<T> GetItemsAsync()
{
    await foreach (var item in source)
    {
        yield return await ProcessAsync(item);
    }
}
```

## Implementation Variations

### 1. **External Iterator**
```csharp
var iterator = collection.CreateIterator();
while (iterator.HasNext())
{
    var item = iterator.Next();
}
```

### 2. **Internal Iterator**
```csharp
collection.ForEach(item => Console.WriteLine(item));
```

### 3. **Bidirectional Iterator**
```csharp
public interface IBidirectionalIterator<T>
{
    bool HasNext();
    bool HasPrevious();
    T Next();
    T Previous();
}
```

### 4. **Filtered Iterator**
```csharp
public class FilteredIterator<T> : IEnumerator<T>
{
    private readonly Predicate<T> _filter;
    // Only return items matching filter
}
```

## Summary

The Iterator pattern provides a standard way to traverse collections without exposing their internal structure. In C#, this is built into the language through IEnumerable<T> and foreach. Custom iterators are useful for complex traversal algorithms, lazy loading, or providing multiple iteration strategies for the same collection.
