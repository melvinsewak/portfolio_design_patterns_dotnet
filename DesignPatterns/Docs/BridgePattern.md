# Bridge Pattern

## Intent
The Bridge pattern decouples an abstraction from its implementation so that the two can vary independently. It separates the interface from the implementation, allowing both to be developed and extended independently.

## Problem
When you have a class hierarchy that combines abstraction with implementation, changes in the implementation require changes in the abstraction, leading to a rigid and tightly coupled design. For example, if you have shapes that can be drawn using different rendering engines, creating separate subclasses for each combination (VectorCircle, RasterCircle, VectorRectangle, RasterRectangle) leads to class explosion.

### Issues Without Bridge Pattern:
- **Class Explosion**: Combining abstraction and implementation creates exponential growth in subclasses
- **Tight Coupling**: Changes in implementation affect abstraction classes
- **Poor Flexibility**: Difficult to add new abstractions or implementations
- **Code Duplication**: Similar code repeated across different implementation variants
- **Violation of SRP**: Classes handle both abstraction logic and implementation details

## Solution
The Bridge pattern suggests splitting the classes into two separate hierarchies:
1. **Abstraction**: High-level control layer (doesn't do actual work)
2. **Implementation**: Platform-specific code (does the actual work)

The abstraction contains a reference to an implementation object and delegates the actual work to it.

## Structure

```
                  Abstraction
                      |
                      | contains
                      ↓
               Implementation ←─────┐
                      ↑             │
                      |             │
         ┌────────────┴─────────┐  │
         |                      |  │
   ConcreteImpl1          ConcreteImpl2
         
         
Client → RefinedAbstraction → Implementation
            |                       ↑
            | uses                  |
            ↓                       |
         [delegates to]    ConcreteImplementation
```

### Detailed ASCII UML Diagram:

```
┌─────────────────────────┐
│     <<abstract>>        │
│      Abstraction        │
├─────────────────────────┤
│ - impl: Implementation  │
├─────────────────────────┤
│ + Operation()           │
└───────────┬─────────────┘
            │
            │ inherits
            ↓
┌─────────────────────────┐         ┌──────────────────────┐
│  RefinedAbstraction     │────────>│  <<interface>>       │
├─────────────────────────┤ uses    │  Implementation      │
│ + Operation()           │         ├──────────────────────┤
│ + SpecialOperation()    │         │ + OperationImpl()    │
└─────────────────────────┘         └──────────┬───────────┘
                                               │
                                    ┌──────────┴──────────┐
                                    │                     │
                         ┌──────────────────┐  ┌─────────────────┐
                         │ ConcreteImplA    │  │ ConcreteImplB   │
                         ├──────────────────┤  ├─────────────────┤
                         │ + OperationImpl()│  │ + OperationImpl()│
                         └──────────────────┘  └─────────────────┘
```

## When to Use

### Use Bridge Pattern When:
1. **Avoid Permanent Binding**: You want to avoid permanent binding between abstraction and implementation
2. **Independent Extension**: Both abstractions and implementations need to be extended independently
3. **Runtime Switching**: You need to switch implementations at runtime
4. **Share Implementation**: You want to share an implementation among multiple objects
5. **Platform Independence**: Building cross-platform applications with platform-specific implementations
6. **Multiple Dimensions**: Class hierarchy grows in multiple independent dimensions

### Don't Use Bridge Pattern When:
1. **Simple One-to-One**: You have only one implementation for one abstraction
2. **No Variation**: Neither abstraction nor implementation is likely to change
3. **Performance Critical**: The extra indirection impacts performance significantly
4. **Over-Engineering**: The added complexity isn't justified by flexibility needs

## Real-World Examples

### 1. **Graphics Rendering Systems**
   - **Abstraction**: Shapes (Circle, Rectangle, Triangle)
   - **Implementation**: Rendering APIs (OpenGL, DirectX, Vulkan, SVG)
   - Each shape can be rendered using any graphics API

### 2. **Database Connectivity**
   - **Abstraction**: Database operations (Query, Insert, Update)
   - **Implementation**: Database drivers (MySQL, PostgreSQL, MongoDB)
   - Same operations work across different database systems

### 3. **UI Frameworks**
   - **Abstraction**: UI Components (Button, TextField, CheckBox)
   - **Implementation**: Platform renderers (Windows, macOS, Linux, Web)
   - Same component renders differently on each platform

### 4. **Messaging Systems**
   - **Abstraction**: Message types (Alert, Notification, Email)
   - **Implementation**: Delivery channels (SMTP, SMS, Push, Slack)
   - Any message type can use any delivery mechanism

### 5. **Remote Controls**
   - **Abstraction**: Remote control types (Basic, Advanced, Universal)
   - **Implementation**: Devices (TV, Radio, DVD Player, Smart Home)
   - Any remote can control any compatible device

## Advantages

1. **Decoupling**: Separates interface from implementation
2. **Extensibility**: Add new abstractions and implementations independently
3. **Single Responsibility**: Abstraction focuses on high-level logic, implementation on details
4. **Runtime Binding**: Switch implementations at runtime
5. **Reduced Subclassing**: Eliminates combinatorial class explosion
6. **Platform Independence**: Easier to build cross-platform systems
7. **Better Organization**: Clear separation of concerns
8. **Reusability**: Implementations can be shared across abstractions

## Disadvantages

1. **Complexity**: Increases code complexity with additional layers
2. **Indirection**: Extra level of indirection may impact performance
3. **More Classes**: Creates more classes and interfaces
4. **Initial Overhead**: More effort to set up initially
5. **Learning Curve**: Harder for developers unfamiliar with the pattern
6. **Debugging**: Stack traces become deeper with delegation

## Implementation Best Practices

### 1. **Use Interfaces for Implementation**
```csharp
// Good: Use interface for implementation hierarchy
public interface IDrawingAPI
{
    void DrawCircle(double x, double y, double radius);
}

// Avoid: Using abstract class when interface suffices
public abstract class DrawingAPI
{
    public abstract void DrawCircle(double x, double y, double radius);
}
```

### 2. **Inject Implementation via Constructor**
```csharp
// Good: Constructor injection for flexibility
public abstract class Shape
{
    protected IDrawingAPI drawingAPI;
    
    protected Shape(IDrawingAPI drawingAPI)
    {
        this.drawingAPI = drawingAPI;
    }
}

// Avoid: Creating implementation inside
public abstract class Shape
{
    protected IDrawingAPI drawingAPI = new VectorGraphicsAPI(); // Tight coupling
}
```

### 3. **Keep Abstraction Focused**
```csharp
// Good: Abstraction handles high-level logic
public class Circle : Shape
{
    public override void Draw()
    {
        // High-level: What to draw
        drawingAPI.DrawCircle(x, y, radius);
    }
}

// Avoid: Mixing implementation details
public class Circle : Shape
{
    public override void Draw()
    {
        // Low-level: How to draw (belongs in implementation)
        for (int i = 0; i < pixels; i++) { /* ... */ }
    }
}
```

### 4. **Make Implementations Stateless When Possible**
```csharp
// Good: Stateless implementation can be shared
public class VectorGraphicsAPI : IDrawingAPI
{
    public void DrawCircle(double x, double y, double radius)
    {
        // No state, can be reused
    }
}

// Consider: Stateful implementation may need per-instance creation
public class StatefulAPI : IDrawingAPI
{
    private List<Shape> drawnShapes = new();
    // State makes sharing more complex
}
```

## C# Specific Features

### 1. **Use Properties for Implementation Reference**
```csharp
public abstract class Shape
{
    protected IDrawingAPI DrawingAPI { get; init; }
    
    protected Shape(IDrawingAPI drawingAPI)
    {
        DrawingAPI = drawingAPI;
    }
}
```

### 2. **Leverage Record Types for Immutable Abstractions**
```csharp
public record Circle(double X, double Y, double Radius, IDrawingAPI API) : Shape(API)
{
    public override void Draw() => API.DrawCircle(X, Y, Radius);
}
```

### 3. **Use Default Interface Methods (C# 8+)**
```csharp
public interface IDrawingAPI
{
    void DrawCircle(double x, double y, double radius);
    
    // Default implementation
    void DrawFilledCircle(double x, double y, double radius)
    {
        DrawCircle(x, y, radius);
        Console.WriteLine("Filling circle...");
    }
}
```

### 4. **Pattern Matching for Implementation Selection**
```csharp
IDrawingAPI CreateAPI(string type) => type switch
{
    "vector" => new VectorGraphicsAPI(),
    "raster" => new RasterGraphicsAPI(),
    "3d" => new Graphics3DAPI(),
    _ => throw new ArgumentException("Unknown API type")
};
```

### 5. **Async/Await Support**
```csharp
public interface IMessageSender
{
    Task SendMessageAsync(string recipient, string message);
}

public abstract class Message
{
    protected IMessageSender sender;
    public abstract Task SendAsync();
}
```

## Related Patterns

### **Bridge vs Adapter**
- **Bridge**: Designed upfront to let abstraction and implementation vary independently
- **Adapter**: Applied to existing systems to make incompatible interfaces work together
- **Bridge**: Both sides expect to vary
- **Adapter**: Wraps existing interface to match expected interface

### **Bridge vs Strategy**
- **Bridge**: Structural pattern about separating abstraction from implementation
- **Strategy**: Behavioral pattern about interchangeable algorithms
- **Bridge**: Both abstraction and implementation are hierarchies
- **Strategy**: Usually one context with multiple algorithm variants

### **Bridge + Abstract Factory**
- Use Abstract Factory to create and configure Bridge structures
- Factory can decide which implementation to pair with abstraction

### **Bridge + Composite**
- Bridge can work with Composite for complex UI hierarchies
- Each composite component can have different implementations

### **Bridge + Decorator**
- Can combine both patterns
- Decorator adds responsibilities; Bridge separates concerns

## Comparison with Similar Patterns

| Aspect | Bridge | Adapter | Strategy |
|--------|--------|---------|----------|
| Intent | Separate abstraction from implementation | Make incompatible interfaces compatible | Define family of algorithms |
| Design Time | Designed upfront | Usually retrofitted | Designed upfront |
| Structure | Two hierarchies connected | Wraps existing class | One context, multiple algorithms |
| Flexibility | Both sides can vary | Adapter is fixed | Algorithm varies |
| Complexity | More complex | Simpler | Simpler |

## Common Pitfalls

### 1. **Confusing with Adapter**
```csharp
// Bridge: Designed for flexibility
public abstract class Shape
{
    protected IDrawingAPI api; // Expected to vary
}

// Adapter: Makes existing code work
public class LegacySystemAdapter : IModernInterface
{
    private LegacySystem legacy; // Wraps existing system
}
```

### 2. **Creating Too Many Abstractions**
```csharp
// Avoid: Over-engineering with unnecessary abstractions
public abstract class ShapeAbstraction { }
public abstract class ShapeAbstractionBase : ShapeAbstraction { }
public abstract class ShapeAbstractionCore : ShapeAbstractionBase { }

// Better: Keep it simple
public abstract class Shape { }
```

### 3. **Tight Coupling in Implementation**
```csharp
// Avoid: Implementation depending on abstraction
public class VectorAPI : IDrawingAPI
{
    public void Draw(Circle circle) // Knows about Circle
    {
        DrawCircle(circle.X, circle.Y, circle.Radius);
    }
}

// Better: Implementation is independent
public class VectorAPI : IDrawingAPI
{
    public void DrawCircle(double x, double y, double radius)
    {
        // Doesn't know about Shape classes
    }
}
```

## Testing Considerations

### 1. **Test Abstraction and Implementation Separately**
```csharp
[Test]
public void Circle_Should_Delegate_To_DrawingAPI()
{
    var mockAPI = new Mock<IDrawingAPI>();
    var circle = new Circle(5, 10, 3, mockAPI.Object);
    
    circle.Draw();
    
    mockAPI.Verify(api => api.DrawCircle(5, 10, 3), Times.Once);
}
```

### 2. **Test Different Implementation Combinations**
```csharp
[Test]
public void Shape_Should_Work_With_Any_Implementation()
{
    var implementations = new IDrawingAPI[]
    {
        new VectorGraphicsAPI(),
        new RasterGraphicsAPI(),
        new Graphics3DAPI()
    };
    
    foreach (var impl in implementations)
    {
        var circle = new Circle(0, 0, 5, impl);
        Assert.DoesNotThrow(() => circle.Draw());
    }
}
```

## Summary

The Bridge pattern is a powerful structural pattern that separates abstraction from implementation, allowing both to vary independently. It's particularly useful when:
- Building cross-platform applications
- Implementing multiple variations of similar functionality
- Avoiding class explosion from combining features
- Need runtime switching of implementations

By keeping abstraction and implementation in separate hierarchies connected through composition, Bridge provides flexibility and maintainability while following SOLID principles, especially the Single Responsibility and Open/Closed principles.
