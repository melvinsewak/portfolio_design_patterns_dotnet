# Facade Pattern

## Intent
The Facade pattern provides a simplified, unified interface to a complex subsystem of classes, libraries, or frameworks. It defines a higher-level interface that makes the subsystem easier to use by wrapping a complicated system with a simpler interface.

## Problem
Modern software systems are built from numerous interconnected components, libraries, and subsystems. Directly working with these complex subsystems requires:

### Issues Without Facade Pattern:
- **High Complexity**: Clients must understand and interact with many different classes
- **Tight Coupling**: Client code becomes tightly coupled to subsystem implementation details
- **Difficult to Use**: Simple operations require complex sequences of calls
- **Error-Prone**: Easy to make mistakes when coordinating multiple subsystem components
- **Poor Maintainability**: Changes in subsystem structure affect all client code
- **Learning Curve**: Developers must learn entire subsystem API for simple tasks

### Real-World Analogy:
Think of a home theater system. To watch a movie, you need to:
1. Turn on the projector
2. Lower the screen
3. Turn on the amplifier
4. Set the amplifier to DVD input
5. Turn on the DVD player
6. Start the movie
7. Dim the lights
8. Make popcorn

A facade would be like a "Watch Movie" button that does all these steps automatically.

## Solution
The Facade pattern creates a single, simplified interface that:
1. **Wraps complex subsystems** behind a clean API
2. **Provides high-level operations** combining multiple low-level operations
3. **Reduces dependencies** between clients and subsystem classes
4. **Still allows direct access** to subsystem components when needed

The facade doesn't hide the subsystem; it provides a convenient interface to the most commonly used features.

## Structure

```
                     Client
                       |
                       | uses
                       ↓
                    Facade ────────────────┐
                       |                   |
                       | coordinates       | delegates
                       ↓                   ↓
         ┌─────────────┼─────────────┬────────────┐
         ↓             ↓             ↓            ↓
   SubsystemA    SubsystemB    SubsystemC   SubsystemD
   ┌────────┐    ┌────────┐    ┌────────┐   ┌────────┐
   │MethodA │    │MethodB │    │MethodC │   │MethodD │
   │MethodX │    │MethodY │    │MethodZ │   │MethodW │
   └────────┘    └────────┘    └────────┘   └────────┘
```

### Detailed ASCII UML Diagram:

```
┌─────────────────────────────────────────────────────────────┐
│                          Client                             │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ calls simple methods
                         ↓
┌─────────────────────────────────────────────────────────────┐
│                         Facade                              │
├─────────────────────────────────────────────────────────────┤
│ - subsystemA: SubsystemA                                    │
│ - subsystemB: SubsystemB                                    │
│ - subsystemC: SubsystemC                                    │
├─────────────────────────────────────────────────────────────┤
│ + SimplifiedOperation1()                                    │
│ + SimplifiedOperation2()                                    │
└──────────┬──────────────┬──────────────┬────────────────────┘
           │              │              │
           │ delegates    │ delegates    │ delegates
           ↓              ↓              ↓
  ┌────────────┐  ┌────────────┐  ┌────────────┐
  │SubsystemA  │  │SubsystemB  │  │SubsystemC  │
  ├────────────┤  ├────────────┤  ├────────────┤
  │+ OperationA│  │+ OperationB│  │+ OperationC│
  │+ OperationA2  │+ OperationB2  │+ OperationC2
  └────────────┘  └────────────┘  └────────────┘
```

## When to Use

### Use Facade Pattern When:
- ✅ You want to provide a simple interface to a complex subsystem
- ✅ There are many interdependent classes in a subsystem
- ✅ You want to layer your subsystems (facade as entry point to each layer)
- ✅ You want to reduce coupling between clients and implementation classes
- ✅ You need to wrap a poorly designed API with a better interface
- ✅ You want to define entry points for different levels of subsystem abstraction

### Avoid Facade Pattern When:
- ❌ The subsystem is already simple and easy to use
- ❌ You need fine-grained control over subsystem operations
- ❌ Creating the facade would add unnecessary complexity
- ❌ The subsystem changes frequently (facade becomes maintenance burden)

## Real-World Examples

### 1. **Home Theater System** (Entertainment)
```
Facade: HomeTheater.WatchMovie()
Subsystems: DVD Player, Amplifier, Projector, Screen, Lights, Popcorn Maker
Simplification: One method call instead of 10+ individual operations
```

### 2. **Computer System** (Hardware)
```
Facade: Computer.Start()
Subsystems: CPU, Memory, Hard Drive, BIOS
Simplification: Boot sequence abstraction
```

### 3. **Banking System** (Finance)
```
Facade: Bank.WithdrawCash()
Subsystems: Account Verification, Security Check, Funds Check, Transaction Log
Simplification: Complex transaction validation made simple
```

### 4. **E-Commerce Checkout** (Retail)
```
Facade: Checkout.PlaceOrder()
Subsystems: Inventory, Payment Gateway, Shipping, Email, Analytics
Simplification: Multi-step order processing in one call
```

### 5. **Compiler** (Software Development)
```
Facade: Compiler.Compile()
Subsystems: Lexer, Parser, Semantic Analyzer, Code Generator, Optimizer
Simplification: Hide complex compilation pipeline
```

## Implementation Example

```csharp
// Complex subsystems
public class DvdPlayer
{
    public void On() { }
    public void Play(string movie) { }
    public void Stop() { }
    public void Off() { }
}

public class Amplifier
{
    public void On() { }
    public void SetVolume(int level) { }
    public void SetSurroundSound() { }
    public void Off() { }
}

public class Projector
{
    public void On() { }
    public void WideScreenMode() { }
    public void Off() { }
}

// Facade providing simple interface
public class HomeTheaterFacade
{
    private readonly DvdPlayer _dvd;
    private readonly Amplifier _amp;
    private readonly Projector _projector;
    
    public HomeTheaterFacade(DvdPlayer dvd, Amplifier amp, Projector projector)
    {
        _dvd = dvd;
        _amp = amp;
        _projector = projector;
    }
    
    // Simple method hiding complexity
    public void WatchMovie(string movie)
    {
        _projector.On();
        _projector.WideScreenMode();
        _amp.On();
        _amp.SetSurroundSound();
        _amp.SetVolume(5);
        _dvd.On();
        _dvd.Play(movie);
    }
    
    public void EndMovie()
    {
        _dvd.Stop();
        _dvd.Off();
        _amp.Off();
        _projector.Off();
    }
}

// Client code is simple
var theater = new HomeTheaterFacade(dvd, amp, projector);
theater.WatchMovie("Inception");  // One line instead of many!
```

## Advantages

1. **Simplicity**: Reduces complexity of using a subsystem
2. **Loose Coupling**: Shields clients from subsystem components
3. **Easier to Use**: Provides convenient methods for common tasks
4. **Layering**: Helps structure system into layers
5. **Flexibility**: Clients can still access subsystem directly if needed
6. **Maintainability**: Changes in subsystem don't affect client code
7. **Testing**: Easier to mock or test facade than entire subsystem
8. **Documentation**: Facade serves as documentation for subsystem usage

## Disadvantages

1. **God Object Risk**: Facade can become too large and complex
2. **Additional Layer**: Adds another level of indirection
3. **Limited Flexibility**: May not expose all subsystem capabilities
4. **Maintenance**: Facade must be updated when subsystem changes
5. **Over-Simplification**: Might hide important details users need
6. **Dependency**: Creates dependency on facade implementation

## Best Practices

### Design Guidelines:
1. **Keep It Simple**: Facade should simplify, not add complexity
2. **Single Responsibility**: Each facade should serve one clear purpose
3. **Composition Over Inheritance**: Use composition to aggregate subsystems
4. **Don't Hide Everything**: Allow advanced users to bypass facade
5. **Consistent API**: Provide consistent, intuitive method names
6. **Minimal Coupling**: Facade shouldn't be tightly coupled to all subsystems

### Implementation Tips:
```csharp
// ✅ GOOD: Clear, simple facade
public class OrderFacade
{
    public void PlaceOrder(Order order)
    {
        ValidateOrder(order);
        ProcessPayment(order);
        UpdateInventory(order);
        SendConfirmation(order);
    }
}

// ❌ BAD: Leaky abstraction
public class OrderFacade
{
    public PaymentGateway GetPaymentGateway() { } // Exposes internals
    public InventorySystem GetInventory() { }      // Too much exposure
}

// ✅ GOOD: Subsystems still accessible
public class OrderFacade
{
    public PaymentService PaymentService { get; } // Optional direct access
    
    public void PlaceOrder(Order order) { }       // Simple facade method
}
```

## C# Specific Features

### 1. **Dependency Injection**
```csharp
public class HomeTheaterFacade
{
    private readonly IDvdPlayer _dvd;
    private readonly IAmplifier _amp;
    
    // Constructor injection for testability
    public HomeTheaterFacade(IDvdPlayer dvd, IAmplifier amp)
    {
        _dvd = dvd;
        _amp = amp;
    }
}
```

### 2. **Extension Methods**
```csharp
public static class ComputerFacadeExtensions
{
    public static void QuickStart(this Computer computer)
    {
        computer.CPU.Initialize();
        computer.Memory.Load();
        computer.HardDrive.Boot();
    }
}
```

### 3. **Async/Await**
```csharp
public class AsyncOrderFacade
{
    public async Task PlaceOrderAsync(Order order)
    {
        await ValidateOrderAsync(order);
        await ProcessPaymentAsync(order);
        await UpdateInventoryAsync(order);
        await SendConfirmationAsync(order);
    }
}
```

### 4. **Fluent Interface**
```csharp
public class FluentTheaterFacade
{
    public FluentTheaterFacade SetVolume(int level) { /* ... */ return this; }
    public FluentTheaterFacade DimLights(int level) { /* ... */ return this; }
    public void Play(string movie) { /* ... */ }
}

// Usage
theater.SetVolume(5).DimLights(10).Play("Inception");
```

### 5. **LINQ for Complex Operations**
```csharp
public class ReportFacade
{
    public Report GenerateReport(DateTime start, DateTime end)
    {
        var data = _dataSources
            .SelectMany(source => source.GetData(start, end))
            .Where(d => d.IsValid)
            .OrderBy(d => d.Date)
            .ToList();
            
        return new Report(data);
    }
}
```

## Related Patterns

### **Facade vs Adapter**
- **Facade**: Simplifies complex subsystem (many classes)
- **Adapter**: Makes incompatible interfaces work together (one/few classes)
- Facade provides new interface; Adapter matches existing interface

### **Facade vs Mediator**
- **Facade**: One-way communication (client → subsystem)
- **Mediator**: Two-way communication (objects communicate through mediator)
- Facade simplifies interface; Mediator reduces coupling between colleagues

### **Facade vs Proxy**
- **Facade**: Simplifies complex subsystem
- **Proxy**: Controls access to single object
- Facade wraps multiple objects; Proxy wraps one object

### Complementary Patterns:
- **Abstract Factory**: Create subsystem objects for facade
- **Singleton**: Facade often implemented as singleton
- **Template Method**: Facade methods can use template method pattern
- **Strategy**: Different facades for different subsystem strategies

## Common Use Cases

1. **Legacy System Integration**: Wrap old APIs with modern interface
2. **Third-Party Libraries**: Simplify complex library APIs
3. **Microservices**: Provide unified API gateway to multiple services
4. **Database Access**: Abstract complex ORM or SQL operations
5. **File System Operations**: Simplify file I/O operations
6. **Network Communication**: Hide socket programming complexity
7. **UI Frameworks**: Wrap complex widget hierarchies
8. **Game Engines**: Simplify rendering, physics, audio subsystems

## Testing Considerations

```csharp
// Facade makes testing easier
public class OrderFacadeTests
{
    [Test]
    public async Task PlaceOrder_Should_ProcessAllSteps()
    {
        // Arrange
        var mockPayment = new Mock<IPaymentService>();
        var mockInventory = new Mock<IInventoryService>();
        var facade = new OrderFacade(mockPayment.Object, mockInventory.Object);
        
        // Act
        await facade.PlaceOrderAsync(testOrder);
        
        // Assert - verify facade coordinated all subsystems
        mockPayment.Verify(p => p.ProcessPayment(It.IsAny<Order>()), Times.Once);
        mockInventory.Verify(i => i.UpdateStock(It.IsAny<Order>()), Times.Once);
    }
}
```

## Conclusion

The Facade pattern is essential for managing complexity in software systems. By providing a simplified interface to complex subsystems, it makes code easier to use, maintain, and test. The pattern is particularly valuable when working with legacy code, third-party libraries, or complex business logic that needs to be accessible through a clean API.

**Key Takeaway**: Use Facade to make complex systems simple to use, not to hide complexity. The subsystem should still be accessible for advanced scenarios.
