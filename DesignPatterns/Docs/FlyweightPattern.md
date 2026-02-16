# Flyweight Pattern

## Intent
The Flyweight pattern minimizes memory usage by sharing as much data as possible with similar objects. It allows programs to support vast quantities of fine-grained objects efficiently by sharing common state between multiple objects instead of keeping all data in each object.

## Problem
Applications that need to create a large number of similar objects can quickly consume excessive memory. For example:

### Issues Without Flyweight Pattern:
- **Memory Overhead**: Each object stores redundant duplicate data
- **Poor Performance**: Creating millions of objects is slow and memory-intensive
- **Scalability Issues**: Application can't handle large datasets
- **Cache Misses**: Too many objects cause cache thrashing
- **GC Pressure**: Garbage collector struggles with millions of objects
- **Wasted Resources**: Most objects share majority of their state

### Real-World Analogy:
Think of a text editor displaying a document with 100,000 characters. Without Flyweight, each character would store its own font, size, color, and style data. That's 100,000 copies of essentially the same formatting information! With Flyweight, you share a few formatting objects (Arial 12pt Black, Arial 12pt Bold Red, etc.) across all characters, dramatically reducing memory usage.

## Solution
The Flyweight pattern suggests splitting object state into two categories:

1. **Intrinsic State**: Shared, immutable data stored in flyweight
   - Can be shared across multiple contexts
   - Example: Font name, size, color in text editor

2. **Extrinsic State**: Context-dependent, unique data stored in client
   - Varies for each object instance
   - Passed to flyweight methods when needed
   - Example: Character position, actual character value

The pattern creates a **Flyweight Factory** that manages a pool of shared flyweight objects, reusing them instead of creating new ones.

## Structure

```
Client ──────────► FlyweightFactory ──────────► Flyweight
  |                      |                    (intrinsic state)
  | stores               | manages pool              ↑
  ↓                      |                           |
Extrinsic State         creates/reuses      ConcreteFlyweight
(position, etc.)         └──────────────────────────┘
```

### Detailed ASCII UML Diagram:

```
┌─────────────────────────────────────────────────────────────┐
│                         Client                              │
├─────────────────────────────────────────────────────────────┤
│ - extrinsicState: data                                      │
│ - flyweights: List<Flyweight>                               │
├─────────────────────────────────────────────────────────────┤
│ + Operation()                                               │
└────────────┬────────────────────────────────────────────────┘
             │ requests
             ↓
┌─────────────────────────────────────────────────────────────┐
│                   FlyweightFactory                          │
├─────────────────────────────────────────────────────────────┤
│ - flyweights: Dictionary<key, Flyweight>                    │
├─────────────────────────────────────────────────────────────┤
│ + GetFlyweight(key): Flyweight                              │
│   {                                                         │
│     if (!flyweights.ContainsKey(key))                       │
│       flyweights[key] = new ConcreteFlyweight(intrinsic)    │
│     return flyweights[key]                                  │
│   }                                                         │
└────────────┬────────────────────────────────────────────────┘
             │ creates/manages
             ↓
┌─────────────────────────────────────────────────────────────┐
│              <<interface>> Flyweight                        │
├─────────────────────────────────────────────────────────────┤
│ + Operation(extrinsicState)                                 │
└────────────┬────────────────────────────────────────────────┘
             △
             │ implements
             │
┌────────────┴────────────────────────────────────────────────┐
│                  ConcreteFlyweight                          │
├─────────────────────────────────────────────────────────────┤
│ - intrinsicState: shared data                               │
├─────────────────────────────────────────────────────────────┤
│ + Operation(extrinsicState)                                 │
│   {                                                         │
│     use intrinsicState + extrinsicState                     │
│   }                                                         │
└─────────────────────────────────────────────────────────────┘
```

## When to Use

### Use Flyweight Pattern When:
- ✅ Application uses a large number of similar objects
- ✅ Objects share significant common state
- ✅ Storage costs are high due to object quantity
- ✅ Most object state can be made extrinsic
- ✅ Object identity is not important (can share instances)
- ✅ Application doesn't depend on object identity

### Avoid Flyweight Pattern When:
- ❌ Few objects are needed
- ❌ Objects don't share much common state
- ❌ Extrinsic state is difficult to compute or pass around
- ❌ Object identity matters (can't share instances)
- ❌ The complexity overhead isn't worth memory savings

## Real-World Examples

### 1. **Text Editor** (Software)
```
Flyweight: CharacterStyle (font, size, color, bold, italic)
Extrinsic: Character value, position in document
Benefit: 100,000 characters share 10-20 style objects
```

### 2. **Game Development** (Entertainment)
```
Flyweight: ParticleType (texture, color, size, physics properties)
Extrinsic: Position, velocity, lifetime
Benefit: 10,000 particles share 5-10 particle types
```

### 3. **Forest Simulation** (Visualization)
```
Flyweight: TreeType (species, texture, color, model)
Extrinsic: Position, age, height
Benefit: 1,000,000 trees share 10-20 tree types
```

### 4. **String Interning** (.NET Framework)
```
Flyweight: Interned strings
Extrinsic: String usage context
Benefit: Duplicate strings reference same memory location
```

### 5. **Font Rendering** (UI/Graphics)
```
Flyweight: Font glyphs (character shapes)
Extrinsic: Character position, text content
Benefit: One glyph per character type, reused thousands of times
```

## Implementation Example

```csharp
// Flyweight - Shared immutable state
public class CharacterStyle
{
    public string Font { get; }
    public int Size { get; }
    public string Color { get; }
    
    public CharacterStyle(string font, int size, string color)
    {
        Font = font;
        Size = size;
        Color = color;
    }
    
    public void Display(char c, int position)
    {
        Console.WriteLine($"Char '{c}' at {position}: {Font} {Size}pt {Color}");
    }
}

// Flyweight Factory
public class StyleFactory
{
    private Dictionary<string, CharacterStyle> _styles = new();
    
    public CharacterStyle GetStyle(string font, int size, string color)
    {
        string key = $"{font}_{size}_{color}";
        
        if (!_styles.ContainsKey(key))
            _styles[key] = new CharacterStyle(font, size, color);
            
        return _styles[key];
    }
}

// Client - Stores extrinsic state
public class Character
{
    private char _value;              // Extrinsic
    private int _position;            // Extrinsic
    private CharacterStyle _style;    // Flyweight (shared)
    
    public Character(char value, int position, CharacterStyle style)
    {
        _value = value;
        _position = position;
        _style = style;
    }
    
    public void Display() => _style.Display(_value, _position);
}

// Usage
var factory = new StyleFactory();
var style1 = factory.GetStyle("Arial", 12, "Black");
var style2 = factory.GetStyle("Arial", 12, "Black"); // Returns same instance!

var char1 = new Character('H', 0, style1);
var char2 = new Character('i', 1, style2); // Shares style with char1
```

## Advantages

1. **Memory Efficiency**: Drastically reduces memory consumption
2. **Performance**: Fewer objects mean better cache locality
3. **Scalability**: Support millions of objects efficiently
4. **GC Friendly**: Fewer objects reduce garbage collection pressure
5. **Reusability**: Shared objects are highly reusable
6. **Separation of Concerns**: Clearly separates intrinsic vs extrinsic state

## Disadvantages

1. **Complexity**: Adds complexity to codebase
2. **Runtime Cost**: Need to compute/pass extrinsic state
3. **Immutability Requirement**: Shared state must be immutable
4. **Debugging**: Harder to debug shared object references
5. **Thread Safety**: Shared objects require careful synchronization
6. **Trade-off**: Trades CPU time (lookups) for memory savings

## Best Practices

### Design Guidelines:
1. **Immutable Flyweights**: Make intrinsic state immutable
2. **Thread Safety**: Ensure flyweight factory is thread-safe
3. **Proper Key Design**: Use composite keys for factory lookups
4. **Clear Separation**: Clearly distinguish intrinsic vs extrinsic state
5. **Factory Pattern**: Always use factory to manage flyweights
6. **Don't Overuse**: Only use when memory savings justify complexity

### Implementation Tips:
```csharp
// ✅ GOOD: Immutable flyweight
public class ParticleType
{
    public string Texture { get; }      // Immutable
    public int Size { get; }             // Immutable
    
    public ParticleType(string texture, int size)
    {
        Texture = texture;
        Size = size;
    }
}

// ❌ BAD: Mutable flyweight (dangerous when shared!)
public class ParticleType
{
    public string Texture { get; set; } // Mutable - BAD!
    public int Size { get; set; }        // Mutable - BAD!
}

// ✅ GOOD: Thread-safe factory
public class FlyweightFactory
{
    private readonly ConcurrentDictionary<string, Flyweight> _cache = new();
    
    public Flyweight GetFlyweight(string key)
    {
        return _cache.GetOrAdd(key, k => new ConcreteFlyweight(k));
    }
}

// ❌ BAD: Not thread-safe
public class FlyweightFactory
{
    private Dictionary<string, Flyweight> _cache = new();
    
    public Flyweight GetFlyweight(string key)
    {
        if (!_cache.ContainsKey(key))
            _cache[key] = new ConcreteFlyweight(key); // Race condition!
        return _cache[key];
    }
}
```

## C# Specific Features

### 1. **ConcurrentDictionary for Thread Safety**
```csharp
public class FlyweightFactory
{
    private readonly ConcurrentDictionary<string, Flyweight> _flyweights = new();
    
    public Flyweight GetFlyweight(string key)
    {
        return _flyweights.GetOrAdd(key, k => new ConcreteFlyweight(k));
    }
}
```

### 2. **String Interning**
```csharp
// .NET automatically interns compile-time string literals
string s1 = "Hello";
string s2 = "Hello";
Console.WriteLine(Object.ReferenceEquals(s1, s2)); // True - same instance!

// Manual interning
string s3 = new string(new[] { 'H', 'e', 'l', 'l', 'o' });
string s4 = String.Intern(s3);
Console.WriteLine(Object.ReferenceEquals(s2, s4)); // True - interned!
```

### 3. **Record Types (C# 9+)**
```csharp
// Perfect for immutable flyweights
public record ParticleType(string Texture, string Color, int Size)
{
    public void Render(int x, int y) 
    { 
        /* rendering logic */ 
    }
}
```

### 4. **Lazy Initialization**
```csharp
public class FlyweightFactory
{
    private readonly Lazy<Dictionary<string, Flyweight>> _flyweights = 
        new(() => new Dictionary<string, Flyweight>());
    
    public Flyweight GetFlyweight(string key)
    {
        var dict = _flyweights.Value;
        if (!dict.ContainsKey(key))
            dict[key] = new ConcreteFlyweight(key);
        return dict[key];
    }
}
```

### 5. **Value Tuples for Composite Keys**
```csharp
public class StyleFactory
{
    private Dictionary<(string font, int size, string color), CharacterStyle> _styles = new();
    
    public CharacterStyle GetStyle(string font, int size, string color)
    {
        var key = (font, size, color);
        
        if (!_styles.ContainsKey(key))
            _styles[key] = new CharacterStyle(font, size, color);
            
        return _styles[key];
    }
}
```

### 6. **Memory-Mapped Files for Large Datasets**
```csharp
// For extremely large flyweight pools
public class LargeFlyweightFactory
{
    private MemoryMappedFile _mmf;
    
    public void Initialize(long capacity)
    {
        _mmf = MemoryMappedFile.CreateNew("FlyweightCache", capacity);
    }
    
    // Access flyweights from memory-mapped storage
}
```

## Related Patterns

### **Flyweight vs Singleton**
- **Flyweight**: Many shared instances (one per unique state)
- **Singleton**: One shared instance globally
- Flyweight factory often uses Singleton pattern

### **Flyweight vs Prototype**
- **Flyweight**: Sharing objects to save memory
- **Prototype**: Cloning objects for customization
- Can combine: Clone flyweight to create customized version

### **Flyweight vs State**
- **Flyweight**: Share state across objects
- **State**: Encapsulate varying behavior in state objects
- State objects can be implemented as flyweights

### Complementary Patterns:
- **Factory**: Flyweight Factory manages flyweight creation
- **Composite**: Flyweight can be used with Composite for shared leaf nodes
- **Strategy**: Strategy objects can be flyweights if stateless
- **Object Pool**: Similar concept, but pools are for mutable objects

## Common Use Cases

1. **Text Processing**: Character formatting, fonts, glyphs
2. **Game Development**: Particles, bullets, enemies, terrain tiles
3. **GIS Systems**: Map tiles, geographic features, symbols
4. **3D Graphics**: Meshes, textures, materials, shaders
5. **Data Structures**: Shared nodes in tries, ropes, or graphs
6. **Caching**: Immutable cached objects (colors, icons, etc.)
7. **Configuration**: Shared configuration objects
8. **Enumeration Values**: Shared enumeration instances

## Performance Considerations

### Memory Comparison:
```
Without Flyweight (100,000 characters, 50 unique styles):
  100,000 style objects × 32 bytes = 3.2 MB

With Flyweight:
  50 style objects × 32 bytes = 1.6 KB
  100,000 references × 8 bytes = 800 KB
  Total = ~802 KB (75% memory reduction!)
```

### Benchmark Example:
```csharp
[Benchmark]
public void WithoutFlyweight()
{
    var chars = new List<CharacterWithoutFlyweight>();
    for (int i = 0; i < 100000; i++)
        chars.Add(new CharacterWithoutFlyweight('A', i, "Arial", 12, "Black"));
}

[Benchmark]
public void WithFlyweight()
{
    var factory = new StyleFactory();
    var style = factory.GetStyle("Arial", 12, "Black");
    var chars = new List<Character>();
    for (int i = 0; i < 100000; i++)
        chars.Add(new Character('A', i, style));
}

// Results:
// WithoutFlyweight: 45ms, 3.2 MB allocated
// WithFlyweight:     8ms, 0.8 MB allocated (5.6x faster, 4x less memory!)
```

## Testing Considerations

```csharp
[TestFixture]
public class FlyweightFactoryTests
{
    [Test]
    public void GetFlyweight_SameKey_ReturnsSameInstance()
    {
        // Arrange
        var factory = new StyleFactory();
        
        // Act
        var style1 = factory.GetStyle("Arial", 12, "Black");
        var style2 = factory.GetStyle("Arial", 12, "Black");
        
        // Assert
        Assert.AreSame(style1, style2); // Same reference
    }
    
    [Test]
    public void GetFlyweight_DifferentKey_ReturnsDifferentInstance()
    {
        // Arrange
        var factory = new StyleFactory();
        
        // Act
        var style1 = factory.GetStyle("Arial", 12, "Black");
        var style2 = factory.GetStyle("Arial", 14, "Black");
        
        // Assert
        Assert.AreNotSame(style1, style2); // Different reference
    }
    
    [Test]
    public void Factory_ThreadSafe_NoRaceConditions()
    {
        // Arrange
        var factory = new StyleFactory();
        var tasks = new List<Task>();
        
        // Act - Create 100 concurrent requests
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(() => factory.GetStyle("Arial", 12, "Black")));
        }
        Task.WaitAll(tasks.ToArray());
        
        // Assert - Should only create one instance
        Assert.AreEqual(1, factory.GetTotalStyles());
    }
}
```

## Conclusion

The Flyweight pattern is a powerful optimization technique for applications that need to create large numbers of similar objects. By carefully separating intrinsic (shared) and extrinsic (unique) state, it achieves dramatic memory and performance improvements. The pattern is particularly valuable in graphics, gaming, and data-intensive applications.

**Key Takeaway**: Use Flyweight when you have many objects with shared state. The memory savings and performance gains often justify the additional complexity, especially for applications dealing with millions of objects.
