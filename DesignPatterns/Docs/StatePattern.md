# State Pattern

## Intent
The State pattern allows an object to alter its behavior when its internal state changes. The object will appear to change its class.

## Problem
When you need to:
- Change object behavior based on internal state
- Avoid large conditional statements for state-dependent behavior
- Make state transitions explicit
- Add new states without modifying existing code

**Without the pattern:**
- Large switch/if-else statements
- State logic scattered across methods
- Difficult to add new states
- Hard to understand state transitions

## Solution
Encapsulate state-specific behavior in separate state classes. The context delegates state-dependent behavior to the current state object.

## UML Diagram
```
┌─────────────────┐          ┌──────────────────┐
│    Context      │          │  <<interface>>   │
├─────────────────┤          │      State       │
│- state: State   │─────────→├──────────────────┤
│+ Request()      │          │+ Handle(Context) │
└─────────────────┘          └────────△─────────┘
                                      │
                             ┌────────┴────────┐
                             │                 │
                   ┌─────────┴────────┐ ┌─────┴──────────┐
                   │  ConcreteStateA  │ │ ConcreteStateB │
                   ├──────────────────┤ ├────────────────┤
                   │+ Handle(Context) │ │+ Handle(...)   │
                   └──────────────────┘ └────────────────┘
```

## When to Use

### Use When:
- **State-Dependent Behavior** - Behavior varies with state
- **Complex State Machine** - Many states and transitions
- **Avoid Conditionals** - Replace large state conditionals
- **State Transitions** - Need explicit state transition logic
- **Open/Closed Principle** - Add states without modifying context
- **Encapsulation** - Keep state-specific logic together

### Avoid When:
- **Few States** - Only 2-3 simple states
- **No State Logic** - State doesn't affect behavior significantly
- **Simple Transitions** - Simple if-else is clearer
- **Performance Critical** - State object overhead is problematic

## Real-World Examples

### 1. **TCP Connection**
   - States: Closed, Listen, Established, TimeWait
   - Behavior changes per state (send, receive, close)
   - State transitions on events

### 2. **Document Workflow**
   - States: Draft, Moderation, Published
   - Different operations available per state
   - Approval workflow

### 3. **Vending Machine**
   - States: NoCoin, HasCoin, Dispensing, SoldOut
   - Insert coin, select product, dispense
   - State-dependent actions

### 4. **Order Processing**
   - States: New, Paid, Shipped, Delivered, Cancelled
   - Different operations at each stage
   - Order lifecycle

### 5. **Media Player**
   - States: Stopped, Playing, Paused
   - Play/pause/stop behavior varies
   - State machine for controls

### 6. **Game Character**
   - States: Idle, Walking, Running, Jumping, Attacking
   - Different animations and capabilities
   - Context-sensitive controls

## Advantages

1. **Single Responsibility** - Each state encapsulates its behavior
2. **Open/Closed Principle** - Add states without changing context
3. **Eliminates Conditionals** - Replaces large state conditionals
4. **Explicit States** - Makes state machine explicit
5. **Easier Testing** - Test each state independently
6. **Clearer Code** - State-specific logic grouped together

## Disadvantages

1. **More Classes** - Each state requires a class
2. **Complexity** - Can be overkill for simple state machines
3. **State Explosion** - Many states create many classes
4. **Context Dependency** - States often need context reference

## Best Practices

### 1. **Define State Interface**
```csharp
public interface IState
{
    void Handle(Context context);
}

public abstract class State
{
    public abstract void Request1(Context context);
    public abstract void Request2(Context context);
}
```

### 2. **Implement State Transitions in States**
```csharp
public class ConcreteStateA : State
{
    public override void Request1(Context context)
    {
        // Perform action
        context.SetState(new ConcreteStateB());
    }
}
```

### 3. **Use State Factory**
```csharp
public class StateFactory
{
    private static readonly Dictionary<StateType, IState> _states = new()
    {
        [StateType.A] = new StateA(),
        [StateType.B] = new StateB()
    };
    
    public static IState GetState(StateType type) => _states[type];
}
```

### 4. **Implement State Context**
```csharp
public class Context
{
    private IState _state;
    
    public Context(IState initialState)
    {
        _state = initialState;
    }
    
    public void SetState(IState state)
    {
        Console.WriteLine($"Transitioning to {state.GetType().Name}");
        _state = state;
    }
    
    public void Request() => _state.Handle(this);
}
```

### 5. **Use Singleton for Stateless States**
```csharp
public class IdleState : IState
{
    private static IdleState? _instance;
    public static IdleState Instance =>
        _instance ??= new IdleState();
    
    private IdleState() { }
}
```

### 6. **Add State History**
```csharp
public class StatefulContext
{
    private readonly Stack<IState> _stateHistory = new();
    private IState _currentState;
    
    public void SetState(IState newState)
    {
        _stateHistory.Push(_currentState);
        _currentState = newState;
    }
    
    public void RestorePreviousState()
    {
        if (_stateHistory.Count > 0)
            _currentState = _stateHistory.Pop();
    }
}
```

### 7. **Validate State Transitions**
```csharp
public abstract class ValidatedState
{
    protected abstract HashSet<Type> AllowedTransitions { get; }
    
    protected void TransitionTo(Context context, IState newState)
    {
        if (!AllowedTransitions.Contains(newState.GetType()))
            throw new InvalidOperationException(
                $"Cannot transition from {GetType().Name} to {newState.GetType().Name}");
        
        context.SetState(newState);
    }
}
```

## Related Patterns

- **Strategy**: Similar structure, different intent (behavior selection vs state-dependent behavior)
- **Flyweight**: Can share state objects to save memory
- **Singleton**: Stateless states can be singletons

## C# Specific Considerations

### Using Enum for Simple State Machines
```csharp
public enum OrderState { New, Paid, Shipped, Delivered }

public class Order
{
    private OrderState _state = OrderState.New;
    
    public void Process()
    {
        switch (_state)
        {
            case OrderState.New:
                // Process payment
                _state = OrderState.Paid;
                break;
            // ...
        }
    }
}
```

### Using State Pattern with async/await
```csharp
public interface IAsyncState
{
    Task HandleAsync(Context context);
}

public class AsyncState : IAsyncState
{
    public async Task HandleAsync(Context context)
    {
        await Task.Delay(100); // Simulate work
        context.TransitionTo(new NextState());
    }
}
```

### Using Pattern Matching (C# 7+)
```csharp
public class Context
{
    public void Handle() => _state switch
    {
        StateA a => a.HandleA(this),
        StateB b => b.HandleB(this),
        StateC c => c.HandleC(this),
        _ => throw new InvalidOperationException()
    };
}
```

## Implementation Variations

### 1. **State-Driven Transitions**
```csharp
// States control their own transitions
public class PlayingState : IState
{
    public void Pause(Context ctx) => ctx.SetState(new PausedState());
}
```

### 2. **Context-Driven Transitions**
```csharp
// Context controls transitions
public class Context
{
    public void Pause()
    {
        if (_state is PlayingState)
            SetState(new PausedState());
    }
}
```

### 3. **Table-Driven State Machine**
```csharp
public class StateMachine
{
    private readonly Dictionary<(State, Event), State> _transitions = new()
    {
        [(State.A, Event.E1)] = State.B,
        [(State.B, Event.E2)] = State.C
    };
}
```

## Summary

The State pattern provides a clean way to implement state machines by encapsulating state-specific behavior in separate classes. It's particularly useful for complex workflows, protocols, and systems with many states and transitions. Use it to eliminate large conditional statements and make state transitions explicit and manageable.
