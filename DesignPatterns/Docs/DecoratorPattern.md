# Decorator Pattern

## Intent
The Decorator pattern allows you to attach additional responsibilities to an object dynamically. Decorators provide a flexible alternative to subclassing for extending functionality. They wrap objects to add new behaviors while keeping the same interface.

## Problem
When you need to add responsibilities to objects at runtime, traditional inheritance becomes inflexible and leads to class explosion. Creating a subclass for every combination of features results in a rigid design where behaviors are bound at compile time.

### Issues Without Decorator Pattern:
- **Class Explosion**: Creating subclasses for every feature combination (e.g., Coffee, CoffeeWithMilk, CoffeeWithSugar, CoffeeWithMilkAndSugar)
- **Static Behavior**: Features bound at compile time, can't change at runtime
- **Inflexibility**: Can't add/remove features dynamically
- **Code Duplication**: Similar features duplicated across subclasses
- **Complexity**: Difficult to combine multiple features
- **Violation of OCP**: Adding new features requires modifying existing code

## Solution
The Decorator pattern wraps objects in decorator objects that add new behaviors. Decorators have the same interface as the objects they wrap, so they can be stacked recursively. Each decorator adds one specific behavior and delegates the rest to the wrapped object.

## Structure

```
         Component (Interface)
               ↑
               |
    ┌──────────┴──────────┐
    |                     |
ConcreteComponent    Decorator (Abstract)
                          |
                    [wraps: Component]
                          ↑
                          |
              ┌───────────┴───────────┐
              |                       |
      ConcreteDecoratorA      ConcreteDecoratorB
```

### Detailed ASCII UML Diagram:

```
┌──────────────────────────┐
│    <<interface>>         │
│      Component           │
├──────────────────────────┤
│ + Operation()            │
└────────┬─────────────────┘
         │
         │ implements
    ┌────┴────────────────────────────┐
    │                                 │
┌───┴────────────┐        ┌──────────┴──────────┐
│ ConcreteComp   │        │    Decorator        │
├────────────────┤        │   (Abstract)        │
│ + Operation()  │        ├─────────────────────┤
└────────────────┘        │ - component: Component │
                          │ + Operation()       │
                          └──────────┬──────────┘
                                     │
                          ┌──────────┴──────────┐
                          │                     │
                ┌─────────┴────────┐   ┌───────┴─────────┐
                │ ConcreteDecorA   │   │ ConcreteDecorB  │
                ├──────────────────┤   ├─────────────────┤
                │ + Operation()    │   │ + Operation()   │
                │ + AddedBehavior()│   │ + ExtraBehavior()│
                └──────────────────┘   └─────────────────┘
```

### Wrapping Example:

```
Original Object:          Component
                             |
After 1st Decorator:      Decorator1 → Component
                             |
After 2nd Decorator:      Decorator2 → Decorator1 → Component
                             |
After 3rd Decorator:      Decorator3 → Decorator2 → Decorator1 → Component
```

## When to Use

### Use Decorator Pattern When:
1. **Dynamic Extension**: Need to add responsibilities to objects dynamically and transparently
2. **Reversible Changes**: Want to add features that can be withdrawn or changed at runtime
3. **Multiple Combinations**: Have many possible combinations of features
4. **Extension by Wrapping**: Prefer composition over inheritance for extending behavior
5. **Avoid Subclassing**: Subclassing is impractical due to class explosion
6. **Single Responsibility**: Want each decorator to add one specific feature

### Don't Use Decorator Pattern When:
1. **Simple Extension**: A simple subclass would suffice
2. **Order Matters**: The order of decorators significantly affects behavior in non-obvious ways
3. **Identity Checking**: Code relies heavily on object identity checks
4. **Performance Critical**: The overhead of multiple wrapper objects is problematic
5. **Few Variations**: Only a handful of combinations exist

## Real-World Examples

### 1. **Coffee Shop**
   - **Base**: Coffee types (Espresso, DarkRoast, Decaf)
   - **Decorators**: Milk, Mocha, Whip, Soy, Caramel
   - Add condiments dynamically, each with its cost

### 2. **I/O Streams (Java/C#)**
   - **Base**: FileStream, MemoryStream
   - **Decorators**: BufferedStream, CryptoStream, GZipStream
   - Stack multiple stream decorators for compression, encryption, buffering

### 3. **Text Formatting**
   - **Base**: PlainText
   - **Decorators**: Bold, Italic, Underline, Color, Size
   - Combine multiple formatting styles

### 4. **UI Components**
   - **Base**: Window, Panel
   - **Decorators**: ScrollDecorator, BorderDecorator, ShadowDecorator
   - Add visual features to UI components

### 5. **Data Validation**
   - **Base**: Input field
   - **Decorators**: EmailValidator, LengthValidator, RegexValidator
   - Stack multiple validation rules

### 6. **Notification System**
   - **Base**: Notification
   - **Decorators**: SMS, Email, Slack, Push
   - Send notifications through multiple channels

## Advantages

1. **Flexibility**: Add/remove responsibilities at runtime
2. **Single Responsibility**: Each decorator focuses on one feature
3. **Open/Closed Principle**: Extend behavior without modifying existing code
4. **Composition**: Combine multiple decorators for complex behavior
5. **Alternative to Inheritance**: More flexible than static subclassing
6. **Transparency**: Decorators are transparent to clients
7. **Reversibility**: Can unwrap decorators to remove features

## Disadvantages

1. **Complexity**: Many small objects and classes
2. **Order Dependency**: Order of decorators may matter
3. **Identity Issues**: Decorated object has different identity than original
4. **Instantiation**: More complex object instantiation
5. **Debugging**: Harder to debug with multiple layers of wrapping
6. **Type Checking**: Hard to check for specific decorator types
7. **Configuration**: Complex decorator stacks can be hard to configure

## Implementation Best Practices

### 1. **Use Abstract Decorator Base Class**
```csharp
// Good: Abstract decorator handles delegation
public abstract class BeverageDecorator : Beverage
{
    protected Beverage beverage;
    
    protected BeverageDecorator(Beverage beverage)
    {
        this.beverage = beverage;
    }
}

// Avoid: Each decorator repeats delegation logic
public class Milk : Beverage
{
    private Beverage beverage; // Repeated in every decorator
    // ... repetitive delegation code
}
```

### 2. **Maintain Interface Compatibility**
```csharp
// Good: Decorator implements same interface
public abstract class StreamDecorator : Stream
{
    protected Stream stream;
    
    public override int Read(byte[] buffer, int offset, int count)
        => stream.Read(buffer, offset, count);
}

// Avoid: Breaking interface contract
public class BrokenDecorator // Doesn't implement expected interface
{
    private Stream stream;
    public byte[] ReadAll() { } // Different method signature
}
```

### 3. **Forward Unknown Methods**
```csharp
// Good: Forward all interface methods
public class BufferedDecorator : IDataStream
{
    private IDataStream stream;
    
    public byte[] Read() => stream.Read();
    public void Write(byte[] data) => stream.Write(data);
    public void Flush() => stream.Flush(); // Don't forget supporting methods
}
```

### 4. **Keep Decorators Simple**
```csharp
// Good: One responsibility per decorator
public class CompressionDecorator : StreamDecorator
{
    public override void Write(byte[] data)
    {
        var compressed = Compress(data);
        base.Write(compressed);
    }
}

// Avoid: Multiple responsibilities in one decorator
public class ComplexDecorator : StreamDecorator
{
    public override void Write(byte[] data)
    {
        var compressed = Compress(data);
        var encrypted = Encrypt(compressed);
        var validated = Validate(encrypted);
        base.Write(validated);
    }
}
```

## C# Specific Features

### 1. **Use Properties for Decorated Object**
```csharp
public abstract class BeverageDecorator : Beverage
{
    protected Beverage Beverage { get; init; }
    
    protected BeverageDecorator(Beverage beverage)
    {
        Beverage = beverage;
    }
}
```

### 2. **Extension Methods for Fluent API**
```csharp
public static class BeverageExtensions
{
    public static Beverage WithMilk(this Beverage beverage)
        => new Milk(beverage);
    
    public static Beverage WithMocha(this Beverage beverage)
        => new Mocha(beverage);
    
    public static Beverage WithWhip(this Beverage beverage)
        => new Whip(beverage);
}

// Usage
var coffee = new Espresso()
    .WithMilk()
    .WithMocha()
    .WithWhip();
```

### 3. **Record Types for Immutable Decorators**
```csharp
public abstract record Beverage
{
    public abstract string Description { get; }
    public abstract decimal Cost { get; }
}

public record Espresso : Beverage
{
    public override string Description => "Espresso";
    public override decimal Cost => 1.99m;
}

public record Milk(Beverage Inner) : Beverage
{
    public override string Description => $"{Inner.Description}, Milk";
    public override decimal Cost => Inner.Cost + 0.50m;
}
```

### 4. **Pattern Matching for Unwrapping**
```csharp
public static Beverage RemoveMilk(Beverage beverage)
{
    return beverage switch
    {
        Milk milk => milk.Beverage,
        BeverageDecorator decorator => 
            new DecoratorWrapper(RemoveMilk(decorator.Beverage)),
        _ => beverage
    };
}
```

### 5. **Using Directive for Auto-Disposal**
```csharp
public class DisposableDecorator : StreamDecorator, IDisposable
{
    public DisposableDecorator(Stream stream) : base(stream) { }
    
    public void Dispose()
    {
        stream?.Dispose();
        GC.SuppressFinalize(this);
    }
}

// Usage
using var stream = new DisposableDecorator(
    new BufferedStream(new FileStream("file.txt", FileMode.Open))
);
```

### 6. **Async/Await Support**
```csharp
public abstract class AsyncStreamDecorator : IAsyncStream
{
    protected IAsyncStream stream;
    
    public virtual async Task<byte[]> ReadAsync()
        => await stream.ReadAsync();
    
    public virtual async Task WriteAsync(byte[] data)
        => await stream.WriteAsync(data);
}
```

## Related Patterns

### **Decorator vs Adapter**
- **Decorator**: Adds responsibilities to an object
- **Adapter**: Changes interface of an object
- **Decorator**: Can be stacked recursively
- **Adapter**: Usually single wrapper

### **Decorator vs Composite**
- **Decorator**: Adds responsibilities, one wrapped object
- **Composite**: Composes objects into trees, multiple children
- **Decorator**: Linear chain of wrapping
- **Composite**: Tree structure

### **Decorator vs Proxy**
- **Decorator**: Adds behavior
- **Proxy**: Controls access
- **Decorator**: Client knows about decoration
- **Proxy**: Client may not know about proxying

### **Decorator + Strategy**
- Decorator wraps objects; Strategy swaps algorithms
- Can combine: decorate with different strategies

### **Decorator + Factory**
- Use Factory to create complex decorator chains
- Factory Method to create appropriate decorators

## Comparison with Similar Patterns

| Aspect | Decorator | Proxy | Adapter | Strategy |
|--------|-----------|-------|---------|----------|
| Intent | Add behavior | Control access | Match interfaces | Swap algorithms |
| Interface | Same as wrapped | Same as wrapped | Different | Different |
| Wrapping | Multiple layers | Single layer | Single layer | No wrapping |
| Transparency | Visible to client | Hidden from client | Hidden | Visible |
| Purpose | Enhance object | Protect/lazy load | Convert interface | Choose algorithm |

## Common Pitfalls

### 1. **Order-Dependent Decorators**
```csharp
// Problem: Order matters but isn't obvious
var stream1 = new Encryption(new Compression(baseStream)); // Compress then encrypt
var stream2 = new Compression(new Encryption(baseStream)); // Encrypt then compress
// These produce different results!

// Better: Document order requirements or enforce it
public class SecureCompressedStream
{
    public static IStream Create(IStream baseStream)
    {
        // Always compress first, then encrypt (documented order)
        return new Encryption(new Compression(baseStream));
    }
}
```

### 2. **Identity Checking Issues**
```csharp
// Avoid: Checking concrete types
if (beverage is Espresso espresso)
{
    // Fails if beverage is decorated
}

// Better: Use capabilities/interfaces
if (beverage is ICaffeinated)
{
    // Works regardless of decoration
}
```

### 3. **Leaking Decorators**
```csharp
// Avoid: Exposing decorator in interface
public class BadDecorator : Component
{
    public Component GetWrappedComponent() // Breaks encapsulation
    {
        return wrappedComponent;
    }
}

// Better: Keep wrapping private
public class GoodDecorator : Component
{
    private Component wrappedComponent; // Encapsulated
}
```

### 4. **Not Forwarding All Methods**
```csharp
// Avoid: Forgetting to forward methods
public class IncompleteDecorator : Stream
{
    private Stream stream;
    
    public override void Write(byte[] buffer, int offset, int count)
        => stream.Write(buffer, offset, count);
    
    // Forgot to override Read, Seek, etc.!
}

// Better: Override all interface methods
public class CompleteDecorator : Stream
{
    private Stream stream;
    
    public override void Write(...) => stream.Write(...);
    public override int Read(...) => stream.Read(...);
    public override long Seek(...) => stream.Seek(...);
    // ... all methods forwarded
}
```

## Testing Considerations

### 1. **Test Decorators in Isolation**
```csharp
[Test]
public void Milk_Should_Add_Cost_And_Description()
{
    var baseBeverage = new Mock<Beverage>();
    baseBeverage.Setup(b => b.GetCost()).Returns(2.00m);
    baseBeverage.Setup(b => b.GetDescription()).Returns("Coffee");
    
    var milk = new Milk(baseBeverage.Object);
    
    Assert.AreEqual(2.50m, milk.GetCost());
    Assert.AreEqual("Coffee, Milk", milk.GetDescription());
}
```

### 2. **Test Decorator Combinations**
```csharp
[Test]
public void Should_Stack_Multiple_Decorators()
{
    Beverage beverage = new Espresso();
    beverage = new Milk(beverage);
    beverage = new Mocha(beverage);
    beverage = new Whip(beverage);
    
    Assert.AreEqual("Espresso, Milk, Mocha, Whipped Cream", 
        beverage.GetDescription());
}
```

### 3. **Test Order Independence (if applicable)**
```csharp
[Test]
public void Decorators_Should_Be_Order_Independent()
{
    var option1 = new Milk(new Mocha(new Espresso()));
    var option2 = new Mocha(new Milk(new Espresso()));
    
    Assert.AreEqual(option1.GetCost(), option2.GetCost());
}
```

## Performance Considerations

### 1. **Cache Decorator Results**
```csharp
public class CachingDecorator : StreamDecorator
{
    private byte[]? cachedData;
    
    public override byte[] Read()
    {
        return cachedData ??= base.Read();
    }
}
```

### 2. **Lazy Decoration**
```csharp
public class LazyDecorator : StreamDecorator
{
    private Stream? decoratedStream;
    
    private Stream DecoratedStream => 
        decoratedStream ??= CreateDecoratedStream();
}
```

### 3. **Object Pooling for Frequently Used Decorators**
```csharp
public static class DecoratorPool
{
    private static ObjectPool<CompressionDecorator> pool = 
        new ObjectPool<CompressionDecorator>(() => new CompressionDecorator());
    
    public static CompressionDecorator Rent() => pool.Rent();
    public static void Return(CompressionDecorator decorator) => pool.Return(decorator);
}
```

## Summary

The Decorator pattern is a powerful structural pattern that adds responsibilities to objects dynamically. It's particularly useful when:
- Need to add features at runtime
- Have many possible combinations of features
- Want to avoid class explosion from subclassing
- Need reversible enhancements

By wrapping objects in decorator objects that share the same interface, the pattern provides a flexible alternative to inheritance while maintaining the Open/Closed Principle. It's widely used in stream processing, UI frameworks, and anywhere dynamic behavior composition is needed.

The key is to keep decorators focused on single responsibilities and maintain interface compatibility so they can be stacked transparently.
