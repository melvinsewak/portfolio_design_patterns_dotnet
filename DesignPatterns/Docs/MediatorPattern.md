# Mediator Pattern

## Intent
The Mediator pattern defines an object that encapsulates how a set of objects interact. It promotes loose coupling by keeping objects from referring to each other explicitly, and lets you vary their interaction independently.

## Problem
When you need to:
- Reduce complex communication between multiple objects
- Simplify object dependencies and interactions
- Centralize control logic
- Reuse objects in different contexts without tight coupling

**Without the pattern:**
- Objects reference each other directly (many-to-many relationships)
- Complex dependencies make system hard to understand
- Difficult to modify one object without affecting others
- Hard to reuse objects independently

## Solution
Create a mediator object that encapsulates all object interactions. Objects communicate through the mediator instead of directly, reducing dependencies.

## UML Diagram
```
┌──────────┐         ┌─────────────────────┐         ┌──────────┐
│Colleague1│────────→│  <<interface>>      │←────────│Colleague2│
└──────────┘ notifies│     Mediator        │ notifies└──────────┘
                     ├─────────────────────┤
                     │+ Notify(sender,     │
                     │         event)      │
                     └──────────△──────────┘
                                │
                                │implements
                                │
                     ┌──────────┴──────────┐
                     │ ConcreteMediator    │
                     ├─────────────────────┤
                     │- colleague1         │
                     │- colleague2         │
                     │+ Notify()           │
                     └─────────────────────┘
```

## When to Use

### Use When:
- **Complex Communications** - Many-to-many object relationships
- **Tight Coupling** - Objects reference each other directly
- **Centralized Control** - Need to centralize complex communications
- **Reusability** - Want to reuse objects without their communications
- **Protocol Variation** - Communication protocols vary
- **Event Distribution** - Central event dispatcher needed

### Avoid When:
- **Simple Interactions** - Only a few simple object interactions
- **Performance Critical** - Centralized control creates bottleneck
- **No Reuse Needed** - Objects won't be reused in different contexts

## Real-World Examples

### 1. **Air Traffic Control**
   - Aircraft communicate via ATC tower, not directly
   - Tower coordinates takeoffs, landings, routing
   - Centralized control prevents collisions

### 2. **Chat Room**
   - Users send messages through chat room
   - Room handles message distribution
   - Users don't need to know about each other

### 3. **GUI Dialog**
   - Widgets communicate via dialog controller
   - Button clicks affect other widgets
   - Central coordination of UI state

### 4. **Smart Home Hub**
   - Devices communicate through central hub
   - Motion sensor triggers lights via hub
   - Hub coordinates complex automations

### 5. **MVC Controller**
   - Controller mediates between Model and View
   - Decouples UI from business logic
   - Coordinates application flow

## Advantages

1. **Reduced Coupling** - Objects don't reference each other directly
2. **Centralized Control** - Easier to understand and modify interactions
3. **Simplified Communication** - One-to-many becomes one-to-one
4. **Reusability** - Components more reusable
5. **Easier Testing** - Can test components independently
6. **Flexibility** - Easy to change interaction logic

## Disadvantages

1. **Complexity** - Mediator can become complex "god object"
2. **Single Point of Failure** - Mediator failure affects all objects
3. **Performance** - Can become performance bottleneck
4. **Over-Engineering** - May be overkill for simple interactions

## Best Practices

### 1. **Define Clear Mediator Interface**
```csharp
public interface IMediator
{
    void Notify(object sender, string eventType);
}
```

### 2. **Keep Mediator Focused**
```csharp
public class ChatMediator : IMediator
{
    private readonly List<User> _users = new();
    
    public void RegisterUser(User user) => _users.Add(user);
    
    public void Notify(object sender, string message)
    {
        if (sender is User senderUser)
        {
            foreach (var user in _users.Where(u => u != senderUser))
                user.Receive(message, senderUser);
        }
    }
}
```

### 3. **Avoid God Object**
```csharp
// Split complex mediators into smaller, focused ones
public class UIMediator { /* UI coordination */ }
public class DataMediator { /* Data coordination */ }
```

### 4. **Use Events for Loose Coupling**
```csharp
public class Mediator
{
    public event EventHandler<NotificationArgs>? Notification;
    
    protected virtual void OnNotify(NotificationArgs e)
    {
        Notification?.Invoke(this, e);
    }
}
```

### 5. **Implement Colleague Base Class**
```csharp
public abstract class Colleague
{
    protected IMediator Mediator { get; }
    
    protected Colleague(IMediator mediator)
    {
        Mediator = mediator;
    }
    
    protected void Notify(string eventType)
    {
        Mediator.Notify(this, eventType);
    }
}
```

### 6. **Use Dependency Injection**
```csharp
public class Component
{
    private readonly IMediator _mediator;
    
    public Component(IMediator mediator)
    {
        _mediator = mediator;
    }
}
```

## Related Patterns

- **Observer**: Mediator uses Observer pattern for notifications
- **Facade**: Mediator is similar but coordinates behavior, not just provides interface
- **Command**: Mediator can use Commands for operations

## C# Specific Considerations

### Using MediatR Library
```csharp
public class SendEmailCommand : IRequest<bool>
{
    public string To { get; set; }
    public string Subject { get; set; }
}

public class SendEmailHandler : IRequestHandler<SendEmailCommand, bool>
{
    public Task<bool> Handle(SendEmailCommand request, CancellationToken ct)
    {
        // Send email
        return Task.FromResult(true);
    }
}

// Usage
await mediator.Send(new SendEmailCommand { To = "user@example.com" });
```

### Using Events
```csharp
public class EventMediator
{
    public event EventHandler<DataChangedEventArgs>? DataChanged;
    
    public void RaiseDataChanged(object source, DataChangedEventArgs e)
    {
        DataChanged?.Invoke(source, e);
    }
}
```

## Summary

The Mediator pattern centralizes complex communications and control logic, reducing coupling between components. It's particularly useful in scenarios with many-to-many object relationships, like GUI frameworks, chat systems, and orchestration layers. Be careful not to create an overly complex mediator that becomes hard to maintain.
