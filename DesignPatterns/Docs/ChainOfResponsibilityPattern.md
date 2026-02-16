# Chain of Responsibility Pattern

## Intent
The Chain of Responsibility pattern creates a chain of receiver objects for a request. This pattern decouples sender and receiver by giving multiple objects a chance to handle the request. The request is passed along the chain until an object handles it.

## Problem
When you need to:
- Send a request to one of several objects without specifying the receiver explicitly
- Allow multiple objects to handle a request, and the handler isn't known a priori
- Issue a request to one of several objects without specifying the receiver
- Dynamically determine which object should handle a request based on runtime conditions

**Without the pattern:**
- Tight coupling between sender and receiver
- Hard-coded request handling logic
- Difficult to add or remove handlers
- Complex conditional logic to route requests

## Solution
Create a chain of handler objects. Each handler has a reference to the next handler in the chain. When a handler receives a request, it decides whether to process it or pass it to the next handler.

## UML Diagram
```
┌─────────────────┐
│     Client      │
└────────┬────────┘
         │
         │ sends request
         ▼
┌─────────────────────────┐
│   <<interface>>         │
│      Handler            │
├─────────────────────────┤
│ + SetNext(Handler)      │
│ + HandleRequest()       │
└───────────┬─────────────┘
            △
            │
     ┌──────┴──────┬──────────────┐
     │             │              │
┌────┴──────┐ ┌───┴────────┐ ┌───┴────────┐
│Concrete   │ │Concrete    │ │Concrete    │
│Handler1   │ │Handler2    │ │Handler3    │
├───────────┤ ├────────────┤ ├────────────┤
│+Handle()  │ │+Handle()   │ │+Handle()   │
└───────────┘ └────────────┘ └────────────┘
```

## When to Use

### Use When:
- **Multiple handlers** - More than one object may handle a request
- **Dynamic handling** - The handler isn't known until runtime
- **Decoupling needed** - You want to decouple senders from receivers
- **Flexible chains** - The set of handlers should be specified dynamically
- **Pipeline processing** - Requests need to pass through multiple processing stages
- **Middleware** - Implementing middleware or filter chains
- **Approval workflows** - Multi-level approval systems

### Avoid When:
- **Single handler** - Only one object will handle each request
- **Performance critical** - The chain traversal overhead is unacceptable
- **Guaranteed handling** - Every request must be handled (chain might not guarantee this)
- **Simple routing** - A simple if-else or switch statement suffices

## Real-World Examples

### 1. **Web Server Request Pipeline**
   - Authentication filter → Authorization filter → Logging filter → Request handler
   - Each middleware decides whether to process or pass to next

### 2. **Customer Support System**
   - Level 1 Support → Level 2 Support → Senior Support → Engineering Team
   - Tickets escalated based on complexity

### 3. **Expense Approval**
   - Team Lead ($0-$1000) → Manager ($1000-$5000) → Director ($5000-$20000) → Board ($20000+)
   - Each level has approval authority limits

### 4. **Event Handling in GUI**
   - Button → Panel → Form → Application
   - Events bubble up the UI hierarchy

### 5. **Logging Framework**
   - Console Logger → File Logger → Database Logger → Email Logger
   - Different log levels handled by different loggers

### 6. **Credit Card Processing**
   - Fraud Detection → Balance Check → Authorization → Transaction Processing

## Advantages

1. **Reduced Coupling** - Sender doesn't need to know which object handles the request
2. **Flexibility** - Can add or remove handlers dynamically
3. **Responsibility Distribution** - Each handler has a single responsibility
4. **Runtime Configuration** - Chain can be configured at runtime
5. **Open/Closed Principle** - Add new handlers without modifying existing code
6. **Single Responsibility** - Each handler focuses on one type of request

## Disadvantages

1. **No Guarantee** - Request might reach the end of chain without being handled
2. **Performance** - Can be slow if chain is long or requests frequently go to end
3. **Debugging Difficulty** - Hard to observe the runtime characteristics
4. **Complex Configuration** - Setting up the chain correctly can be tricky
5. **Request Ordering** - Order of handlers matters and must be carefully managed

## Best Practices

### 1. **Define Clear Interfaces**
```csharp
public abstract class Handler
{
    protected Handler? NextHandler;
    
    public Handler SetNext(Handler handler)
    {
        NextHandler = handler;
        return handler; // Fluent interface
    }
    
    public abstract void HandleRequest(Request request);
}
```

### 2. **Use Fluent Interface for Chain Building**
```csharp
var chain = new Handler1()
    .SetNext(new Handler2())
    .SetNext(new Handler3());
```

### 3. **Implement Default Behavior**
```csharp
public abstract class BaseHandler : Handler
{
    public override void HandleRequest(Request request)
    {
        if (CanHandle(request))
        {
            Process(request);
        }
        else
        {
            NextHandler?.HandleRequest(request);
        }
    }
    
    protected abstract bool CanHandle(Request request);
    protected abstract void Process(Request request);
}
```

### 4. **Provide End-of-Chain Handler**
```csharp
public class DefaultHandler : Handler
{
    public override void HandleRequest(Request request)
    {
        // Handle unprocessed requests or log them
        Logger.Warning($"No handler found for {request}");
    }
}
```

### 5. **Use for Validation Pipelines**
```csharp
var validator = new AuthenticationValidator()
    .SetNext(new AuthorizationValidator())
    .SetNext(new InputValidator());

validator.Validate(request);
```

### 6. **Consider Async Operations**
```csharp
public abstract class AsyncHandler
{
    protected AsyncHandler? NextHandler;
    
    public async Task<bool> HandleAsync(Request request)
    {
        if (await CanHandleAsync(request))
        {
            await ProcessAsync(request);
            return true;
        }
        
        return NextHandler != null 
            ? await NextHandler.HandleAsync(request) 
            : false;
    }
}
```

### 7. **Log Chain Traversal**
```csharp
public override void HandleRequest(Request request)
{
    Logger.Debug($"{GetType().Name} processing request");
    
    if (CanHandle(request))
    {
        Logger.Info($"{GetType().Name} handled request");
        Process(request);
    }
    else
    {
        Logger.Debug($"{GetType().Name} passing to next handler");
        NextHandler?.HandleRequest(request);
    }
}
```

## Related Patterns

- **Composite**: Chain of Responsibility often used with Composite pattern
- **Command**: Handler can use Command objects to represent requests
- **Decorator**: Similar structure but different intent (adding behavior vs handling requests)

## C# Specific Considerations

### Using LINQ for Chain Building
```csharp
var handlers = new[] { handler1, handler2, handler3 }
    .Aggregate((current, next) => { current.SetNext(next); return next; });
```

### Middleware Pattern in ASP.NET Core
```csharp
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    
    public async Task InvokeAsync(HttpContext context)
    {
        // Process request
        await _next(context); // Pass to next middleware
    }
}
```

### Using Delegates
```csharp
public delegate bool RequestHandler(Request request);

public class HandlerChain
{
    private readonly List<RequestHandler> _handlers = new();
    
    public void AddHandler(RequestHandler handler) => _handlers.Add(handler);
    
    public void Process(Request request)
    {
        foreach (var handler in _handlers)
        {
            if (handler(request)) break;
        }
    }
}
```

## Summary

The Chain of Responsibility pattern provides a flexible way to handle requests by passing them through a chain of handlers. It promotes loose coupling and allows handlers to be added, removed, or reordered dynamically. Best used in scenarios like middleware pipelines, approval workflows, and event handling systems where multiple objects might handle a request.
