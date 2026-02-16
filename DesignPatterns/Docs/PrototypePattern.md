# Prototype Pattern

## Intent
Specify the kinds of objects to create using a prototypical instance, and create new objects by copying this prototype.

## Also Known As
Clone

## Problem
You need to create new objects that are similar to existing ones, and:
- Object creation is expensive (database query, file I/O, network call)
- You want to avoid subclasses for each product type
- Object initialization is complex
- You need to create objects with the same state as an existing object

Creating objects from scratch each time is inefficient when you can copy existing ones.

## Solution
The Prototype pattern:
1. Declares a cloning interface (Clone method)
2. Implements cloning in concrete classes
3. Creates new objects by cloning prototypes
4. Optionally uses a registry to manage prototypes

## Structure
```
┌──────────────┐
│   Client     │
└──────────────┘
       │
       │ creates
       ▼
┌──────────────┐
│  Prototype   │
├──────────────┤
│ + Clone()    │
└──────────────┘
       △
       │
┌──────────────┐
│ConcreteProto │
│  -type1      │
├──────────────┤
│ + Clone()    │───returns───> Copy of ConcretePrototype1
└──────────────┘
```

## Participants
1. **Prototype** - Declares cloning interface
2. **ConcretePrototype** - Implements cloning operation
3. **Client** - Creates objects by cloning prototypes
4. **PrototypeRegistry** (Optional) - Maintains prototypes catalog

## Implementation in C#

### Basic Implementation
```csharp
// Prototype interface
public interface IPrototype<T>
{
    T Clone();
}

// Concrete prototype
public class Document : IPrototype<Document>
{
    public string Title { get; set; }
    public string Content { get; set; }
    public List<string> Tags { get; set; } = new();
    
    public Document Clone()
    {
        return new Document
        {
            Title = this.Title,
            Content = this.Content,
            Tags = new List<string>(this.Tags) // Deep copy
        };
    }
}

// Usage
var original = new Document 
{ 
    Title = "Original",
    Tags = new() { "important" }
};

var copy = original.Clone();
copy.Title = "Copy";
```

### Using MemberwiseClone (Shallow Copy)
```csharp
public class Document
{
    public Document ShallowCopy()
    {
        return (Document)this.MemberwiseClone();
    }
}
```

### Prototype Registry
```csharp
public class DocumentRegistry
{
    private readonly Dictionary<string, Document> _prototypes = new();
    
    public void Register(string key, Document prototype)
    {
        _prototypes[key] = prototype;
    }
    
    public Document? Get(string key)
    {
        return _prototypes.TryGetValue(key, out var proto) 
            ? proto.Clone() 
            : null;
    }
}

// Usage
var registry = new DocumentRegistry();
registry.Register("letter", letterTemplate);
var newLetter = registry.Get("letter");
```

## Deep Copy vs Shallow Copy

### Shallow Copy
Copies object with references to same nested objects:
```csharp
public object ShallowCopy()
{
    return this.MemberwiseClone(); // Built-in .NET method
}
```

### Deep Copy
Copies object and all nested objects:
```csharp
public Document DeepCopy()
{
    return new Document
    {
        Title = this.Title,
        Tags = new List<string>(this.Tags), // New list
        Metadata = this.Metadata.Clone() // Clone nested object
    };
}
```

## When to Use
✅ **Use when:**
- Object creation is expensive
- You need to create many similar objects
- Object initialization is complex
- You want to avoid class explosion from Factory
- You need to create objects with similar state

❌ **Avoid when:**
- Objects are simple to create from scratch
- Deep cloning is complex (circular references, etc.)
- Object initialization doesn't benefit from copying
- Cloning behavior varies significantly per type

## Real-World Examples
1. **Document Templates** - Clone templates for new documents
2. **Game Objects** - Clone enemy prototypes
3. **Configuration Objects** - Clone default configurations
4. **Cell Division** - Biological metaphor
5. **GUI Widgets** - Clone configured UI components

## Advantages
✅ Reduces object creation cost
✅ Hides complexity of creating objects
✅ Reduces subclassing
✅ Can specify new objects by varying values
✅ Can add/remove prototypes at runtime
✅ Supports dynamic configuration

## Disadvantages
❌ Cloning complex objects with circular references is difficult
❌ Deep copy can be expensive
❌ Each class must implement Clone()
❌ May violate Single Responsibility Principle
❌ Can be confusing when to use deep vs shallow copy

## Implementation Approaches in C#

### 1. Manual Cloning
```csharp
public Person Clone()
{
    return new Person
    {
        Name = this.Name,
        Age = this.Age,
        Address = this.Address.Clone() // Manual deep copy
    };
}
```

### 2. Using ICloneable (Not Recommended)
```csharp
public class Person : ICloneable
{
    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
```

**Problems:**
- Returns `object`, not strongly typed
- Unclear if deep or shallow copy
- No generic support

### 3. Copy Constructor
```csharp
public class Person
{
    public Person(Person other)
    {
        this.Name = other.Name;
        this.Age = other.Age;
    }
}
```

### 4. Using Records (Modern C# 9+)
```csharp
public record Person(string Name, int Age);

var person1 = new Person("Alice", 30);
var person2 = person1 with { Age = 31 }; // Clone with modification
```

**Benefits:**
- Built-in value-based equality
- Immutable by default
- `with` expression for cloning
- Compiler-generated Clone method

### 5. Serialization-Based Cloning
```csharp
public static T DeepClone<T>(T obj)
{
    var json = JsonSerializer.Serialize(obj);
    return JsonSerializer.Deserialize<T>(json)!;
}
```

**Warning:** Performance overhead, may not preserve all state

## Best Practices
1. **Be clear about deep vs shallow** - Document behavior
2. **Clone all mutable nested objects** - Avoid shared state issues
3. **Use custom interface** - Avoid ICloneable
4. **Consider immutability** - Use records for simple cases
5. **Handle circular references** - If they exist
6. **Validate after cloning** - Ensure valid state

## C# Specific Features

### Records with Cloning
```csharp
// Immutable record
public record Product(string Name, decimal Price)
{
    public List<string> Tags { get; init; } = new();
}

// Clone with modification
var product1 = new Product("Laptop", 999.99m);
var product2 = product1 with { Price = 899.99m };

// Note: Collections are shallow copied!
product2.Tags.Add("on-sale");
// Both product1 and product2 share same Tags list!
```

### Extension Method for Deep Copy
```csharp
public static class CloneExtensions
{
    public static T DeepClone<T>(this T source) where T : IPrototype<T>
    {
        return source.Clone();
    }
}
```

## Common Mistakes

### Mistake 1: Shallow Copy When Deep Copy Needed
```csharp
// Wrong - shallow copy
public Person Clone()
{
    return (Person)this.MemberwiseClone(); // Shares Address reference!
}

// Right - deep copy
public Person Clone()
{
    return new Person
    {
        Name = this.Name,
        Address = this.Address.Clone() // Clone nested object
    };
}
```

### Mistake 2: Using ICloneable
```csharp
// Not recommended
public class Person : ICloneable
{
    public object Clone() { ... } // Returns object
}

// Better
public class Person : IPrototype<Person>
{
    public Person Clone() { ... } // Returns Person
}
```

## Related Patterns
- **Factory Method** - Can use prototypes instead of creating from scratch
- **Abstract Factory** - Can store prototypes and clone them
- **Composite** - Often combined with Prototype for tree structures
- **Decorator** - Prototypes can be decorated
- **Memento** - Similar but for saving/restoring state

## Testing Considerations
```csharp
[Fact]
public void Clone_CreatesIndependentCopy()
{
    var original = new Document 
    { 
        Title = "Original",
        Tags = new() { "tag1" }
    };
    
    var clone = original.Clone();
    clone.Title = "Clone";
    clone.Tags.Add("tag2");
    
    Assert.Equal("Original", original.Title);
    Assert.Single(original.Tags); // Original unchanged
    Assert.Equal(2, clone.Tags.Count);
}
```

## Modern Alternatives
Modern C# offers better alternatives:

### 1. Records (C# 9+)
```csharp
public record Person(string Name, int Age);
var copy = original with { }; // Perfect clone
```

### 2. Object Initializers
```csharp
var copy = new Person
{
    Name = original.Name,
    Age = original.Age
};
```

### 3. AutoMapper
```csharp
var copy = mapper.Map<Person>(original);
```

## Performance Considerations
- **Shallow copy**: Fast (just copy references)
- **Deep copy**: Slower (must recursively copy)
- **Serialization-based**: Slowest (serialize + deserialize)
- **Consider object pooling** if creating many objects
