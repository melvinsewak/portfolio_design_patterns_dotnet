# Template Method Pattern

## Intent
The Template Method pattern defines the skeleton of an algorithm in a base class, letting subclasses override specific steps without changing the algorithm's structure.

## Problem
When you need to:
- Define an algorithm's structure while allowing step customization
- Avoid code duplication in similar algorithms
- Control which parts of an algorithm can be overridden
- Enforce a specific algorithm sequence

**Without the pattern:**
- Code duplication across similar algorithms
- No guarantee of consistent algorithm structure
- Difficult to maintain multiple variations
- Hard to ensure proper step sequencing

## Solution
Define an abstract class with a template method that calls primitive operations. Subclasses override the primitive operations to provide specific behavior.

## UML Diagram
```
┌───────────────────────────┐
│   AbstractClass           │
├───────────────────────────┤
│+ TemplateMethod()         │ ◄──── defines algorithm skeleton
│# PrimitiveOperation1()    │       calls primitive operations
│# PrimitiveOperation2()    │
│# Hook()                   │ ◄──── optional hooks
└─────────────△─────────────┘
              │
              │extends
              │
┌─────────────┴─────────────┐
│   ConcreteClass           │
├───────────────────────────┤
│# PrimitiveOperation1()    │ ◄──── implements specific steps
│# PrimitiveOperation2()    │
└───────────────────────────┘

Template Method calls in sequence:
1. PrimitiveOperation1()
2. PrimitiveOperation2()
3. Hook()
```

## When to Use

### Use When:
- **Common Algorithm Structure** - Multiple classes share algorithm structure
- **Controlled Customization** - Want to control which steps can vary
- **Code Reuse** - Avoid duplication in similar algorithms
- **Framework Development** - Defining extension points
- **Invariant Parts** - Some algorithm parts must not change
- **Hollywood Principle** - "Don't call us, we'll call you"

### Avoid When:
- **Completely Different Algorithms** - No common structure
- **High Variability** - Most steps vary across subclasses
- **Composition Better** - Strategy pattern more appropriate
- **Few Variations** - Only 1-2 variations exist

## Real-World Examples

### 1. **Data Mining**
   - Template: OpenFile → ExtractData → ParseData → AnalyzeData → CloseFile
   - Variations: PDF, CSV, XML miners
   - Common workflow, different implementations

### 2. **Game AI**
   - Template: CollectInput → Update → Render
   - Variations: Player, Enemy, NPC
   - Same game loop, different behaviors

### 3. **Unit Testing**
   - Template: SetUp → RunTest → TearDown
   - Variations: Different test cases
   - xUnit, NUnit frameworks

### 4. **Recipe Preparation**
   - Template: GatherIngredients → Prepare → Cook → Serve
   - Variations: Pasta, Steak, Soup
   - Same process, different dishes

### 5. **HTTP Request Handling**
   - Template: Authenticate → Validate → Process → Log
   - Variations: Different endpoints
   - ASP.NET pipeline

### 6. **Build Process**
   - Template: Clean → Compile → Test → Package → Deploy
   - Variations: Different project types
   - Build systems (MSBuild, Maven)

## Advantages

1. **Code Reuse** - Common code in base class
2. **Consistent Structure** - Enforces algorithm structure
3. **Control Points** - Clear customization points
4. **Inversion of Control** - Framework calls user code
5. **Easier Maintenance** - Changes in one place
6. **Open/Closed Principle** - Extend without modifying base

## Disadvantages

1. **Inheritance Constraint** - Requires subclassing
2. **Liskov Violation Risk** - Subclass might violate expectations
3. **Limited Flexibility** - Can't change algorithm structure
4. **Template Complexity** - Complex templates hard to understand
5. **Tight Coupling** - Subclasses coupled to base class

## Best Practices

### 1. **Use Abstract Methods for Required Steps**
```csharp
public abstract class DataProcessor
{
    // Template method
    public void Process()
    {
        LoadData();
        TransformData();
        SaveData();
    }
    
    protected abstract void LoadData();
    protected abstract void TransformData();
    protected abstract void SaveData();
}
```

### 2. **Use Virtual Methods for Optional Steps (Hooks)**
```csharp
public abstract class Algorithm
{
    public void Execute()
    {
        Initialize();
        DoWork();
        if (RequiresCleanup())
            Cleanup();
    }
    
    protected virtual void Initialize() { }
    protected abstract void DoWork();
    protected virtual bool RequiresCleanup() => true;
    protected virtual void Cleanup() { }
}
```

### 3. **Make Template Method Final/Sealed**
```csharp
public abstract class BaseTemplate
{
    // sealed prevents override
    public sealed void TemplateMethod()
    {
        Step1();
        Step2();
        Step3();
    }
    
    protected abstract void Step1();
    protected abstract void Step2();
    protected abstract void Step3();
}
```

### 4. **Document Template Method Flow**
```csharp
/// <summary>
/// Template method defining the workflow:
/// 1. Initialize
/// 2. Execute core logic
/// 3. Cleanup
/// </summary>
public void Process()
{
    Initialize();
    Execute();
    Cleanup();
}
```

### 5. **Use Hollywood Principle**
```csharp
// Framework calls user code, not vice versa
public abstract class Framework
{
    public void Run()
    {
        // Framework controls the flow
        OnStart();
        ProcessLoop();
        OnEnd();
    }
    
    protected abstract void OnStart();
    protected abstract void ProcessLoop();
    protected abstract void OnEnd();
}
```

### 6. **Provide Default Implementations**
```csharp
public abstract class DefaultTemplate
{
    public void Process()
    {
        PreProcess();
        Execute();
        PostProcess();
    }
    
    protected virtual void PreProcess()
    {
        // Default implementation
        Console.WriteLine("Default pre-processing");
    }
    
    protected abstract void Execute();
    
    protected virtual void PostProcess()
    {
        // Default implementation
    }
}
```

### 7. **Use Template Method for Testing**
```csharp
public abstract class TestCase
{
    public void Run()
    {
        SetUp();
        try
        {
            RunTest();
        }
        finally
        {
            TearDown();
        }
    }
    
    protected virtual void SetUp() { }
    protected abstract void RunTest();
    protected virtual void TearDown() { }
}
```

## Related Patterns

- **Strategy**: Template Method uses inheritance, Strategy uses composition
- **Factory Method**: Often called by Template Method
- **Hook Method**: Template Method uses hooks for optional behavior

## C# Specific Considerations

### Using async/await
```csharp
public abstract class AsyncTemplate
{
    public async Task ProcessAsync()
    {
        await InitializeAsync();
        await ExecuteAsync();
        await CleanupAsync();
    }
    
    protected virtual Task InitializeAsync() => Task.CompletedTask;
    protected abstract Task ExecuteAsync();
    protected virtual Task CleanupAsync() => Task.CompletedTask;
}
```

### Using LINQ and Functional Style
```csharp
public abstract class FunctionalTemplate
{
    protected abstract IEnumerable<Func<Task>> GetSteps();
    
    public async Task Execute()
    {
        foreach (var step in GetSteps())
            await step();
    }
}
```

### Using Extension Methods
```csharp
public static class TemplateExtensions
{
    public static void ProcessWithLogging<T>(this T processor)
        where T : IProcessor
    {
        Log("Starting");
        processor.Process();
        Log("Complete");
    }
}
```

## Implementation Variations

### 1. **Simple Template**
```csharp
public abstract class Simple
{
    public void Execute()
    {
        Step1();
        Step2();
    }
    
    protected abstract void Step1();
    protected abstract void Step2();
}
```

### 2. **Template with Hooks**
```csharp
public abstract class WithHooks
{
    public void Execute()
    {
        Initialize();
        if (ShouldProcess())
            Process();
        Finalize();
    }
    
    protected virtual void Initialize() { }
    protected virtual bool ShouldProcess() => true;
    protected abstract void Process();
    protected virtual void Finalize() { }
}
```

### 3. **Configurable Template**
```csharp
public abstract class Configurable
{
    protected Options Config { get; }
    
    public void Execute()
    {
        if (Config.PreProcess)
            PreProcess();
        
        CoreProcess();
        
        if (Config.PostProcess)
            PostProcess();
    }
}
```

## Summary

The Template Method pattern is ideal for defining algorithm skeletons while allowing specific steps to be customized. It promotes code reuse and ensures consistent algorithm structure across variations. Use it in frameworks, testing infrastructure, and anywhere you need to define a fixed process with customizable steps.
