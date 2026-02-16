# Builder Pattern

## Intent
Separate the construction of a complex object from its representation so that the same construction process can create different representations.

## Problem
You need to create complex objects with:
- Many optional parameters
- Multiple configuration steps
- Different representations using the same construction process
- Validation during construction

Creating such objects with constructors becomes unwieldy:
```csharp
// Telescoping constructor anti-pattern
public Computer(string cpu, string gpu, int ram, int storage, 
                string storageType, string motherboard, string psu, 
                string cooling, params string[] extras) // Too many parameters!
```

## Solution
The Builder pattern:
1. Separates object construction from representation
2. Provides a step-by-step approach to building objects
3. Allows different representations using the same building process
4. Enables method chaining for fluent interface

## Structure
```
┌─────────────┐
│   Director  │
├─────────────┤
│ + Construct()│─────uses────┐
└─────────────┘              │
                             ▼
                      ┌─────────────┐
                      │   Builder   │
                      ├─────────────┤
                      │ + BuildPart()│
                      └─────────────┘
                             △
                             │
                      ┌─────────────┐        ┌─────────────┐
                      │ConcreteBuilder│──────>│   Product   │
                      ├─────────────┤ builds └─────────────┘
                      │ + BuildPart()│
                      │ + GetResult()│
                      └─────────────┘
```

## Participants
1. **Builder** - Abstract interface for creating product parts
2. **ConcreteBuilder** - Implements Builder interface, constructs parts
3. **Director** - Constructs object using Builder interface
4. **Product** - Complex object being constructed

## Implementation in C#

### Classic Implementation with Director
```csharp
// Product
public class Computer
{
    public string CPU { get; set; }
    public int RAM { get; set; }
    public int Storage { get; set; }
}

// Builder interface
public interface IComputerBuilder
{
    IComputerBuilder SetCPU(string cpu);
    IComputerBuilder SetRAM(int ram);
    IComputerBuilder SetStorage(int storage);
    Computer Build();
}

// Concrete builder
public class ComputerBuilder : IComputerBuilder
{
    private readonly Computer _computer = new();
    
    public IComputerBuilder SetCPU(string cpu)
    {
        _computer.CPU = cpu;
        return this; // Enable method chaining
    }
    
    public IComputerBuilder SetRAM(int ram)
    {
        _computer.RAM = ram;
        return this;
    }
    
    public IComputerBuilder SetStorage(int storage)
    {
        _computer.Storage = storage;
        return this;
    }
    
    public Computer Build() => _computer;
}

// Director (optional)
public class ComputerDirector
{
    public Computer BuildGamingPC(IComputerBuilder builder)
    {
        return builder
            .SetCPU("AMD Ryzen 9")
            .SetRAM(64)
            .SetStorage(2000)
            .Build();
    }
}
```

### Fluent Builder (Modern C#)
```csharp
var computer = new ComputerBuilder()
    .SetCPU("Intel i9")
    .SetRAM(32)
    .SetStorage(1000)
    .Build();
```

## When to Use
✅ **Use when:**
- Object construction is complex with many steps
- Object needs different representations
- You want to isolate complex construction code
- Construction process must allow different representations
- You need to build immutable objects step by step

❌ **Avoid when:**
- Object construction is simple
- Object has few parameters
- Object doesn't need validation during construction
- Simple factory or constructor is sufficient

## Real-World Examples
1. **StringBuilder** - Building strings character by character
2. **HTTP Request Builders** - Configure requests fluently
3. **Query Builders** - SQL or LINQ query construction
4. **Document Builders** - PDF, HTML, XML generation
5. **Test Data Builders** - Create test objects

## Advantages
✅ Allows construction step by step
✅ Can vary product's internal representation
✅ Isolates complex construction code
✅ Enables reuse of construction code
✅ Single Responsibility Principle
✅ Provides control over construction process
✅ Supports immutability

## Disadvantages
❌ Increases overall code complexity
❌ Requires creating new builder for each product type
❌ Can be overkill for simple objects
❌ Clients must be aware of builder existence

## Variations

### 1. Fluent Interface
Return `this` from setter methods:
```csharp
public IBuilder SetProperty(string value)
{
    _property = value;
    return this; // Enable chaining
}
```

### 2. Builder with Validation
```csharp
public Product Build()
{
    if (string.IsNullOrEmpty(_requiredField))
        throw new InvalidOperationException("Required field missing");
    return _product;
}
```

### 3. Nested Builder Class
```csharp
public class Computer
{
    private Computer() { }
    
    public class Builder
    {
        private readonly Computer _computer = new();
        
        public Builder SetCPU(string cpu)
        {
            _computer.CPU = cpu;
            return this;
        }
        
        public Computer Build() => _computer;
    }
}

// Usage:
var computer = new Computer.Builder()
    .SetCPU("Intel i9")
    .Build();
```

### 4. Builder with Factory Method
```csharp
public static class Computer
{
    public static IComputerBuilder CreateBuilder() => new ComputerBuilder();
}
```

## Best Practices
1. **Return builder from setters** - Enable method chaining
2. **Validate in Build()** - Ensure object is valid before creation
3. **Make product immutable** - Set properties only during building
4. **Use meaningful names** - `SetCPU()` not `CPU()`
5. **Consider director** - For common configurations
6. **Document required fields** - Make it clear what's needed

## C# Specific Features

### Using Records
```csharp
public record Computer(string CPU, int RAM, int Storage);

public class ComputerBuilder
{
    private string? _cpu;
    private int _ram;
    private int _storage;
    
    public ComputerBuilder WithCPU(string cpu)
    {
        _cpu = cpu;
        return this;
    }
    
    public Computer Build() => new(_cpu!, _ram, _storage);
}
```

### Using Init-Only Properties
```csharp
public class Computer
{
    public string CPU { get; init; }
    public int RAM { get; init; }
}
```

### Using Expression-Bodied Members
```csharp
public IBuilder SetCPU(string cpu) => Set(() => _cpu = cpu);
```

## Director vs No Director

**With Director:**
- Encapsulates construction logic
- Provides predefined configurations
- Useful for complex, common builds

**Without Director:**
- More flexibility for clients
- Simpler architecture
- Direct builder usage

## Builder vs Factory

| Builder | Factory |
|---------|---------|
| Constructs objects step by step | Creates objects in one step |
| Returns product at the end | Returns product immediately |
| Different representations possible | Same representation |
| Focuses on HOW to build | Focuses on WHAT to build |

## Related Patterns
- **Abstract Factory** - Can use Builder to create products
- **Composite** - Builder can construct composite trees
- **Prototype** - Builder can use prototypes as templates
- **Fluent Interface** - Often used with Builder

## Testing Considerations
Builder pattern improves testability:
```csharp
[Fact]
public void Builder_CreatesValidComputer()
{
    var computer = new ComputerBuilder()
        .SetCPU("Test CPU")
        .SetRAM(16)
        .SetStorage(512)
        .Build();
    
    Assert.Equal("Test CPU", computer.CPU);
    Assert.Equal(16, computer.RAM);
}
```

## Modern Alternatives
Modern C# offers alternatives:
```csharp
// Object initializer syntax
var computer = new Computer
{
    CPU = "Intel i9",
    RAM = 32,
    Storage = 1000
};

// Target-typed new expression (C# 9+)
Computer computer = new()
{
    CPU = "Intel i9",
    RAM = 32
};

// With expression for records (C# 9+)
var computer2 = computer with { RAM = 64 };
```

However, Builder is still valuable for:
- Complex construction logic
- Validation requirements
- Immutable objects
- Multi-step construction
