# Visitor Pattern

## Intent
The Visitor pattern represents an operation to be performed on elements of an object structure. It lets you define a new operation without changing the classes of the elements on which it operates.

## Problem
When you need to:
- Perform operations on elements of complex object structures
- Add new operations without modifying element classes
- Keep related operations together
- Separate algorithm from object structure

**Without the pattern:**
- Must modify element classes to add operations
- Operations scattered across element classes
- Violates Open/Closed Principle
- Difficult to add new operations

## Solution
Define a visitor interface with a visit method for each element type. Elements accept visitors and call the appropriate visit method, allowing visitors to perform operations on them.

## UML Diagram
```
┌──────────────────┐           ┌──────────────────┐
│  <<interface>>   │           │  <<interface>>   │
│     Element      │           │     Visitor      │
├──────────────────┤           ├──────────────────┤
│+ Accept(Visitor) │           │+ Visit(ElementA) │
└────────△─────────┘           │+ Visit(ElementB) │
         │                     └────────△─────────┘
         │                              │
┌────────┴────────┬──────────┐         │
│                 │          │         │implements
│  ConcreteElementA ConcreteElementB   │
├──────────────────┼────────────────┐  │
│+ Accept(v)      │+ Accept(v)     │  │
│  v.Visit(this)  │  v.Visit(this) │  │
└─────────────────┴────────────────┘  │
                                      │
                           ┌──────────┴─────────┐
                           │                    │
                  ┌────────┴─────────┐ ┌───────┴────────┐
                  │ ConcreteVisitor1 │ │ConcreteVisitor2│
                  ├──────────────────┤ ├────────────────┤
                  │+ Visit(ElementA) │ │+ Visit(...)    │
                  │+ Visit(ElementB) │ │                │
                  └──────────────────┘ └────────────────┘
```

## When to Use

### Use When:
- **Many Operations** - Need many unrelated operations on objects
- **Stable Structure** - Object structure rarely changes
- **New Operations** - Frequently add new operations
- **Related Operations** - Group related operations together
- **Type-Based Dispatch** - Operations depend on concrete types
- **Separation of Concerns** - Separate algorithm from data structure

### Avoid When:
- **Unstable Structure** - Object structure changes frequently
- **Few Operations** - Only a few simple operations
- **Privacy Concerns** - Operations need private element data
- **Simple Traversal** - Simple iteration is sufficient

## Real-World Examples

### 1. **Compiler/AST Processing**
   - Visitors: CodeGenerator, TypeChecker, Optimizer
   - Elements: Expressions, Statements, Declarations
   - Different passes over same syntax tree

### 2. **Tax Calculation**
   - Visitors: TaxCalculator, DiscountCalculator, ReportGenerator
   - Elements: Book, Electronics, Clothing
   - Different calculations on product catalog

### 3. **Document Export**
   - Visitors: HTMLRenderer, PDFRenderer, MarkdownRenderer
   - Elements: Paragraph, Image, Table, Heading
   - Multiple export formats

### 4. **File System Operations**
   - Visitors: SizeCalculator, Searcher, Deleter
   - Elements: File, Directory
   - Various operations on file tree

### 5. **Shape Drawing**
   - Visitors: Drawer, BoundsCalculator, Serializer
   - Elements: Circle, Rectangle, Triangle
   - Multiple operations on shapes

### 6. **Insurance Claims**
   - Visitors: PremiumCalculator, RiskAssessor, ClaimValidator
   - Elements: Car, Home, Health policies
   - Different analyses on policies

## Advantages

1. **Open/Closed Principle** - Add operations without modifying elements
2. **Single Responsibility** - Operations grouped by visitor
3. **Accumulate State** - Visitor can accumulate state across elements
4. **Work with Different Objects** - Visit heterogeneous collections
5. **Type Safety** - Compile-time type checking

## Disadvantages

1. **Adding Elements Hard** - New element type requires all visitors to change
2. **Breaks Encapsulation** - May need to expose element internals
3. **Circular Dependencies** - Elements and visitors depend on each other
4. **Complexity** - Additional classes and indirection
5. **Double Dispatch** - Can be confusing to understand

## Best Practices

### 1. **Define Clear Visitor Interface**
```csharp
public interface IVisitor
{
    void Visit(ConcreteElementA element);
    void Visit(ConcreteElementB element);
    void Visit(ConcreteElementC element);
}
```

### 2. **Use Double Dispatch**
```csharp
public abstract class Element
{
    public abstract void Accept(IVisitor visitor);
}

public class ConcreteElement : Element
{
    public override void Accept(IVisitor visitor)
    {
        visitor.Visit(this); // Double dispatch
    }
}
```

### 3. **Return Results from Visitors**
```csharp
public interface IVisitor<TResult>
{
    TResult Visit(ElementA element);
    TResult Visit(ElementB element);
}

public class Calculator : IVisitor<decimal>
{
    public decimal Visit(Product product) => product.Price * 0.1m;
}
```

### 4. **Accumulate State in Visitor**
```csharp
public class TotalCalculator : IVisitor
{
    public decimal Total { get; private set; }
    
    public void Visit(Product product)
    {
        Total += product.Price;
    }
}
```

### 5. **Use Visitor for Complex Traversals**
```csharp
public class CompositeVisitor : IVisitor
{
    public void Visit(Composite composite)
    {
        foreach (var child in composite.Children)
            child.Accept(this);
    }
}
```

### 6. **Provide Base Visitor Implementation**
```csharp
public abstract class BaseVisitor : IVisitor
{
    public virtual void Visit(ElementA a) { }
    public virtual void Visit(ElementB b) { }
    // Default empty implementations
}
```

### 7. **Use Generics for Type-Safe Visitors**
```csharp
public interface IVisitor<in TElement, out TResult>
{
    TResult Visit(TElement element);
}
```

## Related Patterns

- **Composite**: Visitor often used with Composite structures
- **Iterator**: Visitor can use Iterator to traverse structure
- **Interpreter**: Similar structure for tree walking

## C# Specific Considerations

### Using Pattern Matching (C# 7+)
```csharp
public decimal Calculate(Element element) => element switch
{
    Book book => book.Price * 0.9m,
    Electronics elec => elec.Price * 0.85m,
    Clothing cloth => cloth.Price * 0.8m,
    _ => 0m
};
```

### Using dynamic (Avoid in Production)
```csharp
public class DynamicVisitor
{
    public void Visit(dynamic element)
    {
        Process(element); // Runtime dispatch
    }
    
    private void Process(Book book) { }
    private void Process(Electronics electronics) { }
}
```

### Async Visitors
```csharp
public interface IAsyncVisitor
{
    Task VisitAsync(ElementA element);
    Task VisitAsync(ElementB element);
}

public abstract class Element
{
    public abstract Task AcceptAsync(IAsyncVisitor visitor);
}
```

### Using Extension Methods
```csharp
public static class VisitorExtensions
{
    public static void AcceptAll(
        this IEnumerable<Element> elements,
        IVisitor visitor)
    {
        foreach (var element in elements)
            element.Accept(visitor);
    }
}
```

## Implementation Variations

### 1. **Reflective Visitor (Anti-pattern)**
```csharp
// Avoid - uses reflection instead of double dispatch
public class ReflectiveVisitor
{
    public void Visit(object obj)
    {
        var method = GetType().GetMethod("Process", new[] { obj.GetType() });
        method?.Invoke(this, new[] { obj });
    }
}
```

### 2. **Hierarchical Visitor**
```csharp
public interface IVisitor
{
    void VisitPre(Element element);  // Before children
    void VisitPost(Element element); // After children
}
```

### 3. **Parameterized Visitor**
```csharp
public interface IVisitor<TParam, TResult>
{
    TResult Visit(Element element, TParam parameter);
}
```

## Summary

The Visitor pattern is powerful for performing operations on complex object structures, especially when you need to add many operations without modifying the elements. It's commonly used in compilers, document processors, and reporting systems. The main tradeoff is that adding new element types requires updating all visitors, so use it when your object structure is stable but operations vary.
