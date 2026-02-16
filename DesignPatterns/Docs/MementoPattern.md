# Memento Pattern

## Intent
The Memento pattern captures and externalizes an object's internal state without violating encapsulation, so that the object can be restored to this state later.

## Problem
When you need to:
- Save and restore object state
- Implement undo/redo functionality
- Create checkpoints or snapshots
- Maintain state history without exposing internals

**Without the pattern:**
- Must expose internal state (violates encapsulation)
- Difficult to save/restore complex objects
- No clean way to implement undo
- State management scattered across codebase

## Solution
Use three roles: Originator (creates memento), Memento (stores state), and Caretaker (manages mementos). The memento stores a snapshot of the originator's state.

## UML Diagram
```
┌──────────────┐   creates   ┌────────────────┐
│  Originator  │────────────→│    Memento     │
├──────────────┤             ├────────────────┤
│- state       │             │- state         │
│+ CreateMemento()           │+ GetState()    │
│+ Restore(m)  │             │                │
└──────────────┘             └────────────────┘
       △                              │
       │                              │
       │                              │stores
       │                              ▼
┌──────────────┐             ┌────────────────┐
│  Caretaker   │────────────→│   Memento      │
├──────────────┤   manages   └────────────────┘
│- mementos[]  │
│+ Backup()    │
│+ Undo()      │
└──────────────┘
```

## When to Use

### Use When:
- **Undo/Redo** - Need to implement undo/redo operations
- **Checkpoints** - Want to create snapshots of object state
- **Transactions** - Need rollback capability
- **State History** - Must maintain state history
- **Encapsulation** - Must preserve encapsulation while saving state
- **Recovery** - Need to restore state after failures

### Avoid When:
- **Simple State** - State is simple enough to copy directly
- **Memory Limited** - State storage consumes too much memory
- **Frequent Changes** - State changes too frequently
- **No Undo Needed** - Don't need to restore previous states

## Real-World Examples

### 1. **Text Editor**
   - Save document state before each edit
   - Undo/redo by restoring mementos
   - Multiple undo levels

### 2. **Game Save System**
   - Save game state (level, score, inventory)
   - Load saved games
   - Autosave checkpoints

### 3. **Form State**
   - Save form data before submission
   - Restore on validation failure
   - Session state management

### 4. **Database Transactions**
   - BEGIN TRANSACTION
   - COMMIT or ROLLBACK
   - Savepoints within transactions

### 5. **Configuration Management**
   - Save configuration snapshots
   - Rollback to previous settings
   - Configuration history

### 6. **Version Control**
   - Git commits are mementos
   - Checkout previous versions
   - Branch and merge states

## Advantages

1. **Encapsulation** - Preserves encapsulation boundaries
2. **Simplification** - Simplifies originator by delegating state storage
3. **Undo/Redo** - Easy implementation of undo/redo
4. **State History** - Can maintain complete state history
5. **Snapshots** - Create checkpoints for recovery

## Disadvantages

1. **Memory Overhead** - Storing mementos consumes memory
2. **Performance** - Creating mementos can be expensive
3. **Caretaker Complexity** - Caretaker must manage memento lifecycle
4. **State Size** - Large states difficult to store
5. **Serialization** - May need complex serialization logic

## Best Practices

### 1. **Make Memento Immutable**
```csharp
public class Memento
{
    public string Content { get; }
    public DateTime Timestamp { get; }
    
    public Memento(string content)
    {
        Content = content;
        Timestamp = DateTime.Now;
    }
}
```

### 2. **Use Nested Memento Class**
```csharp
public class Editor
{
    private string _content;
    
    public class Memento
    {
        internal string Content { get; }
        internal Memento(string content) => Content = content;
    }
    
    public Memento Save() => new(_content);
    public void Restore(Memento m) => _content = m.Content;
}
```

### 3. **Implement History Manager**
```csharp
public class History<T> where T : class
{
    private readonly Stack<T> _states = new();
    private const int MaxStates = 100;
    
    public void Save(T state)
    {
        if (_states.Count >= MaxStates)
            _states.Clear(); // or remove oldest
        _states.Push(state);
    }
    
    public T? Undo() => _states.Count > 0 ? _states.Pop() : null;
}
```

### 4. **Use Command Pattern with Memento**
```csharp
public interface ICommand
{
    void Execute();
    void Undo();
}

public class EditCommand : ICommand
{
    private readonly Editor _editor;
    private Editor.Memento? _backup;
    
    public void Execute()
    {
        _backup = _editor.Save();
        _editor.Edit();
    }
    
    public void Undo()
    {
        if (_backup != null)
            _editor.Restore(_backup);
    }
}
```

### 5. **Implement Incremental Mementos**
```csharp
public class IncrementalMemento
{
    public List<Change> Changes { get; } = new();
    
    public void AddChange(Change change) => Changes.Add(change);
}
```

### 6. **Use Serialization for Complex State**
```csharp
public class SerializableMemento
{
    public byte[] State { get; }
    
    public SerializableMemento(object state)
    {
        State = Serialize(state);
    }
    
    private byte[] Serialize(object obj)
    {
        using var ms = new MemoryStream();
        var formatter = new BinaryFormatter();
        formatter.Serialize(ms, obj);
        return ms.ToArray();
    }
}
```

## Related Patterns

- **Command**: Often used together for undo/redo
- **Iterator**: Can use memento to save iteration state
- **Prototype**: Similar state copying, different purpose

## C# Specific Considerations

### Using ICloneable
```csharp
public class Originator : ICloneable
{
    public object Clone()
    {
        return MemberwiseClone(); // Shallow copy
    }
    
    public Originator DeepClone()
    {
        // Deep copy implementation
    }
}
```

### Using Records (C# 9+)
```csharp
public record EditorState(string Content, int CursorPosition);

public class Editor
{
    private EditorState _state;
    
    public EditorState Save() => _state with { }; // Copy
    public void Restore(EditorState state) => _state = state;
}
```

### Using System.Text.Json
```csharp
public class JsonMemento<T>
{
    public string Json { get; }
    
    public JsonMemento(T state)
    {
        Json = JsonSerializer.Serialize(state);
    }
    
    public T Restore() => JsonSerializer.Deserialize<T>(Json)!;
}
```

## Summary

The Memento pattern provides a clean way to implement undo/redo and state management while preserving encapsulation. It's essential for editors, games, and any application requiring state restoration. Balance memory usage with functionality by limiting history size and using incremental or compressed mementos for large states.
