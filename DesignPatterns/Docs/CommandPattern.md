# Command Pattern

## Intent
The Command pattern encapsulates a request as an object, thereby allowing you to parameterize clients with different requests, queue or log requests, and support undoable operations.

## Problem
When you need to:
- Parameterize objects with operations
- Specify, queue, and execute requests at different times
- Support undo/redo functionality
- Log changes for recovery purposes
- Structure a system around high-level operations built on primitive operations

**Without the pattern:**
- Direct coupling between invoker and receiver
- Difficult to implement undo/redo
- Hard to queue or log operations
- Cannot easily add new commands

## Solution
Encapsulate a request and its parameters as a command object. The object can be stored, passed around, and executed at any time. Commands can also implement undo functionality.

## UML Diagram
```
┌─────────────┐              ┌──────────────────┐
│   Client    │──creates────→│  ConcreteCommand │
└─────────────┘              ├──────────────────┤
                             │ - receiver       │
                             │ + Execute()      │
                             │ + Undo()         │
                             └────────┬─────────┘
                                      │
                                      │ calls
                                      ▼
                             ┌─────────────────┐
┌──────────┐   executes     │  <<interface>>  │
│ Invoker  │───────────────→│    Command      │
├──────────┤                ├─────────────────┤
│-commands │                │+ Execute()      │
│+Do()     │                │+ Undo()         │
│+Undo()   │                └─────────────────┘
└──────────┘                         △
                                     │
                            ┌────────┴────────┐
                            │                 │
                     ┌──────┴──────┐  ┌──────┴──────┐
                     │  Command1   │  │  Command2   │
                     └─────────────┘  └─────────────┘

                             ┌─────────────────┐
                             │    Receiver     │
                             ├─────────────────┤
                             │ + Action()      │
                             └─────────────────┘
```

## When to Use

### Use When:
- **Undo/Redo** - Need to implement undo/redo functionality
- **Queueing** - Want to queue, schedule, or execute requests at different times
- **Logging** - Need to log operations for auditing or recovery
- **Macro Commands** - Want to compose operations into macro commands
- **Callback abstraction** - Need to parameterize objects with actions
- **Transaction systems** - Implementing transactional behavior
- **Remote execution** - Executing operations on remote systems

### Avoid When:
- **Simple operations** - The operation is simple and doesn't need encapsulation
- **No undo needed** - Undo/redo is not required
- **Performance critical** - The command object overhead is unacceptable
- **Direct calls sufficient** - Simple method calls work fine

## Real-World Examples

### 1. **Text Editor**
   - InsertCommand, DeleteCommand, ReplaceCommand
   - Each command can be undone/redone
   - Command history stored in a stack

### 2. **Remote Control**
   - LightOnCommand, LightOffCommand, TVChannelCommand
   - Program buttons with different commands
   - Macro commands for multiple operations

### 3. **Database Transactions**
   - InsertCommand, UpdateCommand, DeleteCommand
   - Commit/Rollback functionality
   - Transaction log for recovery

### 4. **Task Scheduler**
   - EmailCommand, BackupCommand, ReportCommand
   - Schedule commands to run at specific times
   - Retry failed commands

### 5. **GUI Actions**
   - MenuItem → Command → Action
   - Toolbar buttons execute commands
   - Keyboard shortcuts mapped to commands

### 6. **Game Input System**
   - MoveCommand, JumpCommand, AttackCommand
   - Record and replay player actions
   - Networked multiplayer command transmission

## Advantages

1. **Decoupling** - Decouples object that invokes operation from object that performs it
2. **Undo/Redo** - Easy to implement undo/redo functionality
3. **Composition** - Can create composite commands (macros)
4. **Extensibility** - Easy to add new commands without changing existing code
5. **Logging** - Can log commands for auditing or crash recovery
6. **Queueing** - Commands can be queued and executed later

## Disadvantages

1. **Complexity** - Increases number of classes in the system
2. **Indirection** - Adds another layer of indirection
3. **Memory** - Storing command history consumes memory
4. **Overhead** - May be overkill for simple operations

## Best Practices

### 1. **Define Clear Command Interface**
```csharp
public interface ICommand
{
    void Execute();
    void Undo();
}
```

### 2. **Store Command State for Undo**
```csharp
public class InsertCommand : ICommand
{
    private readonly Document _document;
    private readonly string _text;
    private readonly int _position;
    
    public void Execute() => _document.Insert(_text, _position);
    public void Undo() => _document.Delete(_position, _text.Length);
}
```

### 3. **Use Command History**
```csharp
public class CommandHistory
{
    private readonly Stack<ICommand> _undoStack = new();
    private readonly Stack<ICommand> _redoStack = new();
    
    public void Execute(ICommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear();
    }
    
    public void Undo()
    {
        if (_undoStack.Count > 0)
        {
            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);
        }
    }
}
```

### 4. **Implement Macro Commands**
```csharp
public class MacroCommand : ICommand
{
    private readonly List<ICommand> _commands = new();
    
    public void Add(ICommand command) => _commands.Add(command);
    
    public void Execute()
    {
        foreach (var command in _commands)
            command.Execute();
    }
    
    public void Undo()
    {
        for (int i = _commands.Count - 1; i >= 0; i--)
            _commands[i].Undo();
    }
}
```

### 5. **Use Async Commands**
```csharp
public interface IAsyncCommand
{
    Task ExecuteAsync();
    Task UndoAsync();
}

public class AsyncCommandExecutor
{
    public async Task ExecuteAsync(IAsyncCommand command)
    {
        await command.ExecuteAsync();
        // Store in history
    }
}
```

### 6. **Implement Command Pattern with Func/Action**
```csharp
public class DelegateCommand : ICommand
{
    private readonly Action _execute;
    private readonly Action _undo;
    
    public DelegateCommand(Action execute, Action undo)
    {
        _execute = execute;
        _undo = undo;
    }
    
    public void Execute() => _execute();
    public void Undo() => _undo();
}
```

### 7. **Limit History Size**
```csharp
public class LimitedHistory
{
    private readonly Queue<ICommand> _history = new();
    private const int MaxHistorySize = 100;
    
    public void AddCommand(ICommand command)
    {
        if (_history.Count >= MaxHistorySize)
            _history.Dequeue();
        _history.Enqueue(command);
    }
}
```

## Related Patterns

- **Memento**: Commands can use Memento to store state for undo
- **Composite**: Macro commands are composite commands
- **Chain of Responsibility**: Commands can be passed along a chain
- **Prototype**: Commands can be cloned

## C# Specific Considerations

### Using ICommand Interface in WPF/MVVM
```csharp
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;
    
    public event EventHandler? CanExecuteChanged;
    
    public bool CanExecute(object? parameter) => 
        _canExecute?.Invoke(parameter) ?? true;
    
    public void Execute(object? parameter) => _execute(parameter);
}
```

### Using Task-based Commands
```csharp
public interface IAsyncCommand<TResult>
{
    Task<TResult> ExecuteAsync(CancellationToken ct);
    Task UndoAsync(CancellationToken ct);
}
```

### Using Records for Immutable Commands (C# 9+)
```csharp
public record SetValueCommand(
    string PropertyName,
    object OldValue,
    object NewValue
) : ICommand;
```

## Implementation Variations

### 1. **Simple Command**
```csharp
public class SimpleCommand : ICommand
{
    private readonly Action _action;
    public void Execute() => _action();
}
```

### 2. **Reversible Command**
```csharp
public abstract class ReversibleCommand : ICommand
{
    public abstract void Execute();
    public abstract void Undo();
    public virtual bool CanUndo() => true;
}
```

### 3. **Parameterized Command**
```csharp
public class Command<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly T _parameter;
    
    public void Execute() => _execute(_parameter);
}
```

## Summary

The Command pattern is essential for implementing undo/redo functionality, queuing operations, and logging actions. It encapsulates requests as objects, providing flexibility in how and when operations are executed. Best used in applications requiring transactional behavior, macro recording, or complex operation histories.
