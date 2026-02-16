# Factory Method Pattern

## Intent
Define an interface for creating an object, but let subclasses decide which class to instantiate. Factory Method lets a class defer instantiation to subclasses.

## Also Known As
Virtual Constructor

## Problem
You need to create objects but want to:
- Avoid tight coupling between creator and concrete products
- Provide flexibility to introduce new product types
- Delegate the instantiation logic to subclasses

For example, a logging framework that needs to support different log destinations (console, file, database) without hardcoding the creation logic.

## Solution
The Factory Method pattern:
1. Defines an interface for creating objects
2. Lets subclasses decide which class to instantiate
3. Refers to the created object through a common interface

## Structure
```
┌──────────────────┐                 ┌──────────────┐
│     Creator      │                 │   Product    │
├──────────────────┤                 ├──────────────┤
│ + FactoryMethod()│────creates─────>│ + Operation()│
│ + AnOperation()  │                 └──────────────┘
└──────────────────┘                        △
         △                                  │
         │                                  │
┌──────────────────┐                 ┌──────────────┐
│ ConcreteCreator  │                 │ConcreteProduct│
├──────────────────┤                 ├──────────────┤
│ + FactoryMethod()│────creates─────>│ + Operation()│
└──────────────────┘                 └──────────────┘
```

## Participants
1. **Product (ILogger)** - Defines interface of objects the factory creates
2. **ConcreteProduct (ConsoleLogger, FileLogger)** - Implements Product interface
3. **Creator (LoggerFactory)** - Declares factory method
4. **ConcreteCreator (ConsoleLoggerFactory)** - Overrides factory method

## Implementation in C#

### Classic Implementation
```csharp
// Product interface
public interface ILogger
{
    void Log(string message);
}

// Concrete products
public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine(message);
}

public class FileLogger : ILogger
{
    public void Log(string message) => File.AppendAllText("log.txt", message);
}

// Creator
public abstract class LoggerFactory
{
    public abstract ILogger CreateLogger(); // Factory Method
    
    public void LogMessage(string message)
    {
        var logger = CreateLogger();
        logger.Log(message);
    }
}

// Concrete creators
public class ConsoleLoggerFactory : LoggerFactory
{
    public override ILogger CreateLogger() => new ConsoleLogger();
}

public class FileLoggerFactory : LoggerFactory
{
    public override ILogger CreateLogger() => new FileLogger();
}
```

### Parameterized Factory Method
```csharp
public static class LoggerFactory
{
    public static ILogger CreateLogger(string type)
    {
        return type switch
        {
            "console" => new ConsoleLogger(),
            "file" => new FileLogger(),
            _ => throw new ArgumentException("Unknown type")
        };
    }
}
```

## When to Use
✅ **Use when:**
- A class can't anticipate the type of objects it needs to create
- A class wants its subclasses to specify the objects it creates
- Classes delegate responsibility to one of several helper subclasses
- You want to localize the knowledge of which class gets created

❌ **Avoid when:**
- Object creation is simple and unlikely to change
- You don't need subclassing flexibility
- The abstraction adds unnecessary complexity

## Real-World Examples
1. **Document Creation** - Word, Excel, PDF documents
2. **Database Connections** - Different database providers
3. **UI Controls** - Platform-specific UI elements
4. **Parsers** - JSON, XML, CSV parsers
5. **Notification Services** - Email, SMS, Push notifications

## Advantages
✅ Loose coupling between creator and products
✅ Single Responsibility Principle - creation code in one place
✅ Open/Closed Principle - new products without changing existing code
✅ More flexible than direct object creation
✅ Eliminates binding to application-specific classes

## Disadvantages
❌ Code becomes more complex with many new subclasses
❌ Clients might need to subclass Creator just to create a product
❌ Can be overkill for simple object creation

## Variations

### 1. Parameterized Factory Method
Pass parameter to determine which product to create:
```csharp
public ILogger CreateLogger(LogLevel level) { }
```

### 2. Static Factory Method
Use static method instead of instance method:
```csharp
public static ILogger CreateLogger() { }
```

### 3. Lazy Initialization
Create and cache products on first use:
```csharp
private ILogger? _logger;
public ILogger CreateLogger() => _logger ??= new ConsoleLogger();
```

## Best Practices
1. **Return interfaces, not concrete types** - Maximizes flexibility
2. **Use meaningful factory method names** - `CreateLogger()` not `Create()`
3. **Consider Simple Factory first** - Factory Method might be overkill
4. **Document what gets created** - Make factory responsibilities clear
5. **Use dependency injection** - Modern alternative for many cases

## C# Specific Features
```csharp
// Using records for immutable products
public record Product(string Name, decimal Price);

// Using switch expressions
public IProduct CreateProduct(string type) => type switch
{
    "physical" => new PhysicalProduct(),
    "digital" => new DigitalProduct(),
    _ => throw new ArgumentException()
};

// Using generic factory method
public abstract class Factory<T>
{
    public abstract T Create();
}
```

## Related Patterns
- **Abstract Factory** - Often implemented using Factory Methods
- **Prototype** - Alternative to Factory Method for object creation
- **Template Method** - Factory Methods often called within template methods
- **Dependency Injection** - Modern alternative approach

## Testing Considerations
Factory Method improves testability:
```csharp
// Easy to mock or stub
public class TestLoggerFactory : LoggerFactory
{
    public override ILogger CreateLogger() => new MockLogger();
}
```

## Modern Alternatives
In modern C# with dependency injection:
```csharp
// Register in DI container
services.AddTransient<ILogger, ConsoleLogger>();

// Use factory delegate
services.AddTransient<ILogger>(provider => 
    new ConsoleLogger(provider.GetService<IConfiguration>()));
```
