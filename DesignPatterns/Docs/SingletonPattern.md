# Singleton Pattern

## Intent
Ensure a class has only one instance and provide a global point of access to it.

## Problem
Sometimes we need exactly one instance of a class, such as:
- Configuration managers
- Logger instances
- Database connection pools
- Cache managers
- Thread pools

Creating multiple instances would waste resources or cause conflicts.

## Solution
The Singleton pattern ensures a class has only one instance by:
1. Making the constructor private
2. Providing a static method/property that returns the same instance
3. Storing the instance as a static variable

## Structure
```
┌─────────────────────┐
│     Singleton       │
├─────────────────────┤
│ - instance: static  │
│ - Singleton()       │
├─────────────────────┤
│ + Instance: static  │
│ + DoWork()          │
└─────────────────────┘
```

## Implementation in C#

### Lazy<T> Implementation (Recommended)
```csharp
public sealed class Singleton
{
    private static readonly Lazy<Singleton> _instance = 
        new Lazy<Singleton>(() => new Singleton());
    
    private Singleton() { }
    
    public static Singleton Instance => _instance.Value;
}
```

**Benefits:**
- Thread-safe
- Lazy initialization
- Simple and clean
- No explicit locking needed

### Double-Checked Locking
```csharp
public sealed class SingletonDoubleChecked
{
    private static SingletonDoubleChecked? _instance;
    private static readonly object _lock = new object();
    
    private SingletonDoubleChecked() { }
    
    public static SingletonDoubleChecked Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SingletonDoubleChecked();
                    }
                }
            }
            return _instance;
        }
    }
}
```

## When to Use
✅ **Use when:**
- You need exactly one instance of a class
- The instance must be accessible from a well-known access point
- The sole instance should be extensible by subclassing

❌ **Avoid when:**
- You need multiple instances with similar behavior (use Factory instead)
- Testing is important (Singletons can make unit testing difficult)
- You're working in a distributed system (each process gets its own instance)

## Real-World Examples
1. **Configuration Manager** - Application settings
2. **Logger** - Centralized logging
3. **Cache Manager** - In-memory cache
4. **Database Connection Pool** - Manage connections
5. **Device Drivers** - Hardware access

## Advantages
✅ Controlled access to the sole instance
✅ Reduced namespace pollution
✅ Permits refinement of operations and representation
✅ Lazy initialization possible
✅ Better than global variables

## Disadvantages
❌ Difficult to unit test
❌ Violates Single Responsibility Principle
❌ Can hide dependencies
❌ Requires special treatment in multi-threaded environments
❌ Makes parallel testing difficult

## Best Practices
1. **Use `sealed`** - Prevent inheritance
2. **Make constructor private** - Prevent external instantiation
3. **Use Lazy<T>** - Simple and thread-safe
4. **Consider alternatives** - Dependency Injection is often better
5. **Document thread-safety** - Make guarantees clear

## Modern Alternatives
In modern C# applications, consider using:
- **Dependency Injection** - Register as singleton in DI container
- **Static classes** - For stateless utilities
- **Options pattern** - For configuration

## Related Patterns
- **Abstract Factory** - Can use Singleton for factory instances
- **Builder** - Can be implemented as Singleton
- **Prototype** - Can use Singleton for prototype registry
- **Facade** - Often implemented as Singleton

## Testing Considerations
Singletons make testing difficult because:
- State persists between tests
- Can't easily mock dependencies
- Tests become order-dependent

**Solutions:**
- Use Dependency Injection instead
- Provide a reset mechanism (only for testing)
- Use interfaces and inject the singleton
