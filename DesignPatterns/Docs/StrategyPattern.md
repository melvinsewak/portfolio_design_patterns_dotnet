# Strategy Pattern

## Intent
The Strategy pattern defines a family of algorithms, encapsulates each one, and makes them interchangeable. Strategy lets the algorithm vary independently from clients that use it.

## Problem
When you need to:
- Use different variants of an algorithm
- Switch between algorithms at runtime
- Avoid exposing algorithm implementation details
- Eliminate conditional statements for algorithm selection

**Without the pattern:**
- Algorithm selection hardcoded with conditionals
- Difficult to add new algorithms
- Algorithm details exposed to client
- Cannot change algorithm at runtime

## Solution
Define a family of algorithms, encapsulate each in a separate class with a common interface, and make them interchangeable through that interface.

## UML Diagram
```
┌────────────────┐         ┌──────────────────┐
│    Context     │         │  <<interface>>   │
├────────────────┤         │    Strategy      │
│- strategy      │────────→├──────────────────┤
│+ SetStrategy() │         │+ Execute()       │
│+ DoWork()      │         └────────△─────────┘
└────────────────┘                  │
                           ┌────────┴────────┬─────────────┐
                           │                 │             │
                  ┌────────┴─────────┐ ┌────┴─────┐ ┌────┴──────┐
                  │ ConcreteStrategyA│ │Strategy B│ │Strategy C │
                  ├──────────────────┤ ├──────────┤ ├───────────┤
                  │+ Execute()       │ │+Execute()│ │+Execute() │
                  └──────────────────┘ └──────────┘ └───────────┘
```

## When to Use

### Use When:
- **Multiple Algorithms** - Need different variants of an algorithm
- **Runtime Selection** - Must select algorithm at runtime
- **Avoid Conditionals** - Replace complex conditional logic
- **Encapsulation** - Hide algorithm implementation details
- **Similar Classes** - Classes differ only in behavior
- **Algorithm Families** - Related algorithms with common interface

### Avoid When:
- **Few Algorithms** - Only 1-2 simple algorithms
- **Stable Algorithms** - Algorithms rarely change
- **Simple Logic** - Simple if-else is clearer
- **Client Awareness** - Client must know all strategies anyway

## Real-World Examples

### 1. **Payment Processing**
   - Strategies: CreditCard, PayPal, Cryptocurrency, BankTransfer
   - Select payment method at checkout
   - Same interface, different processing

### 2. **Sorting Algorithms**
   - Strategies: QuickSort, MergeSort, BubbleSort, HeapSort
   - Choose based on data size and characteristics
   - Different performance tradeoffs

### 3. **Compression**
   - Strategies: ZIP, GZIP, LZMA, RAR
   - Select based on speed vs size requirements
   - Interchangeable compression algorithms

### 4. **Route Planning**
   - Strategies: Fastest, Shortest, Avoid Highways, Scenic
   - User selects preferred route type
   - Different optimization criteria

### 5. **Data Validation**
   - Strategies: Email, Phone, CreditCard, SSN validation
   - Different rules for different field types
   - Pluggable validators

### 6. **Image Filters**
   - Strategies: Blur, Sharpen, Grayscale, Sepia
   - Apply different filters to images
   - Composable effects

## Advantages

1. **Runtime Flexibility** - Switch algorithms at runtime
2. **Open/Closed Principle** - Add strategies without modifying context
3. **Eliminates Conditionals** - Replaces algorithmic conditionals
4. **Encapsulation** - Isolates algorithm implementation
5. **Testability** - Test each strategy independently
6. **Reusability** - Strategies reusable in different contexts

## Disadvantages

1. **More Classes** - Increases number of classes
2. **Client Awareness** - Clients must know available strategies
3. **Communication Overhead** - Strategy may need context data
4. **Complexity** - Can be overkill for simple cases

## Best Practices

### 1. **Define Strategy Interface**
```csharp
public interface IStrategy<TInput, TOutput>
{
    TOutput Execute(TInput input);
}
```

### 2. **Use Dependency Injection**
```csharp
public class Context
{
    private readonly IStrategy _strategy;
    
    public Context(IStrategy strategy)
    {
        _strategy = strategy;
    }
    
    public void DoWork() => _strategy.Execute();
}
```

### 3. **Allow Runtime Strategy Changes**
```csharp
public class FlexibleContext
{
    private IStrategy _strategy;
    
    public void SetStrategy(IStrategy strategy)
    {
        _strategy = strategy;
    }
    
    public void Execute() => _strategy?.Execute();
}
```

### 4. **Use Strategy Factory**
```csharp
public class StrategyFactory
{
    public static IStrategy CreateStrategy(StrategyType type)
    {
        return type switch
        {
            StrategyType.A => new StrategyA(),
            StrategyType.B => new StrategyB(),
            _ => throw new ArgumentException()
        };
    }
}
```

### 5. **Combine with Template Method**
```csharp
public abstract class BaseStrategy : IStrategy
{
    public void Execute()
    {
        Initialize();
        DoWork();
        Cleanup();
    }
    
    protected virtual void Initialize() { }
    protected abstract void DoWork();
    protected virtual void Cleanup() { }
}
```

### 6. **Use Func<> for Simple Strategies**
```csharp
public class Context<T>
{
    private Func<T, T> _strategy;
    
    public Context(Func<T, T> strategy)
    {
        _strategy = strategy;
    }
    
    public T Process(T input) => _strategy(input);
}
```

### 7. **Implement Null Strategy**
```csharp
public class NullStrategy : IStrategy
{
    public void Execute()
    {
        // Do nothing - safe default behavior
    }
}
```

## Related Patterns

- **State**: Similar structure, different intent (state transitions vs algorithm selection)
- **Template Method**: Strategy uses composition, Template uses inheritance
- **Decorator**: Both alter object behavior, but Strategy replaces entire algorithm
- **Factory**: Often used to create strategy objects

## C# Specific Considerations

### Using Delegates/Lambdas
```csharp
public class Sorter<T>
{
    public void Sort(List<T> list, Comparison<T> comparisonStrategy)
    {
        list.Sort(comparisonStrategy);
    }
}

// Usage
sorter.Sort(list, (a, b) => a.CompareTo(b));
```

### Using LINQ for Strategy Selection
```csharp
var strategies = new Dictionary<string, IStrategy>
{
    ["fast"] = new FastStrategy(),
    ["slow"] = new SlowStrategy()
};

var strategy = strategies[userChoice];
```

### Async Strategies
```csharp
public interface IAsyncStrategy<TInput, TOutput>
{
    Task<TOutput> ExecuteAsync(TInput input);
}

public class AsyncContext<TIn, TOut>
{
    private readonly IAsyncStrategy<TIn, TOut> _strategy;
    
    public Task<TOut> ProcessAsync(TIn input) =>
        _strategy.ExecuteAsync(input);
}
```

### Using Pattern Matching (C# 8+)
```csharp
public TOutput Execute<TInput, TOutput>(TInput input) =>
    _strategy switch
    {
        FastStrategy fast => fast.Process(input),
        SlowStrategy slow => slow.Process(input),
        _ => throw new InvalidOperationException()
    };
```

## Implementation Variations

### 1. **Functional Strategy**
```csharp
public class Calculator
{
    public int Calculate(int a, int b, Func<int, int, int> operation)
    {
        return operation(a, b);
    }
}

// Usage
var result = calculator.Calculate(5, 3, (x, y) => x + y);
```

### 2. **Strategy with State**
```csharp
public interface IStatefulStrategy
{
    void Initialize();
    void Execute();
    void Cleanup();
}
```

### 3. **Composite Strategy**
```csharp
public class CompositeStrategy : IStrategy
{
    private readonly List<IStrategy> _strategies = new();
    
    public void Add(IStrategy strategy) => _strategies.Add(strategy);
    
    public void Execute()
    {
        foreach (var strategy in _strategies)
            strategy.Execute();
    }
}
```

### 4. **Configurable Strategy**
```csharp
public class ConfigurableStrategy : IStrategy
{
    private readonly StrategyOptions _options;
    
    public ConfigurableStrategy(StrategyOptions options)
    {
        _options = options;
    }
    
    public void Execute()
    {
        // Use _options to customize behavior
    }
}
```

## Summary

The Strategy pattern provides a flexible way to select and change algorithms at runtime. It's essential for systems that need to support multiple variations of an algorithm, such as payment processing, data compression, or sorting. Use it to eliminate complex conditional logic and make your code more maintainable and extensible.
