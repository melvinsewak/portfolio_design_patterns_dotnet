# Composite Pattern

## Intent
The Composite pattern allows you to compose objects into tree structures to represent part-whole hierarchies. It lets clients treat individual objects and compositions of objects uniformly, meaning you can work with both simple and complex elements through a common interface.

## Problem
When dealing with tree-like structures where objects can contain other objects (like file systems, organization charts, or UI component hierarchies), you often need to differentiate between leaf nodes and composite nodes. This leads to complex client code with type checking and special handling for different object types.

### Issues Without Composite Pattern:
- **Complex Client Code**: Clients must distinguish between leaf and composite objects
- **Type Checking**: Frequent use of `instanceof` or type checks
- **Code Duplication**: Similar operations repeated for leaves and composites
- **Rigid Structure**: Difficult to add new component types
- **Poor Recursion**: Hard to implement recursive operations on tree structures
- **Violation of OCP**: Adding new component types requires changing client code

## Solution
The Composite pattern suggests using a common interface for both simple (leaf) and complex (composite) objects. The composite objects store references to child components and delegate operations to them, while leaf objects implement operations directly.

## Structure

```
           Component (Interface/Abstract)
                 ↑
                 |
        ┌────────┴────────┐
        |                 |
      Leaf           Composite
                          |
                    [children: List<Component>]
```

### Detailed ASCII UML Diagram:

```
┌──────────────────────────────┐
│      <<abstract>>            │
│        Component             │
├──────────────────────────────┤
│ + Operation()                │
│ + Add(Component)             │
│ + Remove(Component)          │
│ + GetChild(int): Component   │
└───────────┬──────────────────┘
            │
            │ inherits
    ┌───────┴────────┐
    │                │
┌───┴────────┐   ┌──┴────────────────────────┐
│   Leaf     │   │      Composite            │
├────────────┤   ├───────────────────────────┤
│ Operation()│   │ - children: List<Component>│
└────────────┘   │ + Operation()             │
                 │ + Add(Component)          │
                 │ + Remove(Component)       │
                 │ + GetChild(int)           │
                 └───────────────────────────┘
                         │
                         │ aggregates
                         ↓
                   [Component]*
```

### Tree Structure Example:

```
            Root (Composite)
                 │
        ┌────────┼────────┐
        │        │        │
     Leaf A  Composite B  Leaf C
                 │
            ┌────┴────┐
            │         │
         Leaf D    Leaf E
```

## When to Use

### Use Composite Pattern When:
1. **Tree Structures**: You need to represent part-whole hierarchies of objects
2. **Uniform Treatment**: Clients should treat leaf and composite objects uniformly
3. **Recursive Composition**: Objects can be composed recursively
4. **Hierarchical Data**: Working with hierarchical data like file systems, org charts, or menus
5. **Aggregate Operations**: Need to perform operations on groups of objects
6. **Flexible Structure**: Structure can grow dynamically with varying depth

### Don't Use Composite Pattern When:
1. **Flat Structure**: You're working with flat, non-hierarchical data
2. **Different Operations**: Leaf and composite objects need fundamentally different operations
3. **Type Safety**: You need strong compile-time type checking between component types
4. **Simple Lists**: A simple collection would suffice
5. **Performance Critical**: The overhead of recursive traversal is unacceptable

## Real-World Examples

### 1. **File System**
   - **Composite**: Directory (can contain files and directories)
   - **Leaf**: File (cannot contain other items)
   - Operations: Display, GetSize, Search

### 2. **Organization Chart**
   - **Composite**: Manager (has subordinates)
   - **Leaf**: Individual Contributor (no subordinates)
   - Operations: GetSalary, CountEmployees, Display

### 3. **GUI Component Tree**
   - **Composite**: Container (Panel, Window, Frame)
   - **Leaf**: Control (Button, Label, TextBox)
   - Operations: Render, HandleEvent, Resize

### 4. **Menu System**
   - **Composite**: Menu (contains menu items and submenus)
   - **Leaf**: MenuItem (executable action)
   - Operations: Display, Execute, Navigate

### 5. **Graphic Editor**
   - **Composite**: Group (contains multiple shapes)
   - **Leaf**: Shape (Circle, Rectangle, Line)
   - Operations: Draw, Move, Resize

### 6. **Mathematical Expressions**
   - **Composite**: Complex Expression (contains sub-expressions)
   - **Leaf**: Number, Variable
   - Operations: Evaluate, Simplify, Differentiate

## Advantages

1. **Simplifies Client Code**: Clients treat all components uniformly
2. **Recursive Composition**: Easy to create complex tree structures
3. **Open/Closed Principle**: Easy to add new component types
4. **Flexibility**: Tree structure can be modified at runtime
5. **Natural Recursion**: Recursive algorithms map naturally to the structure
6. **Single Responsibility**: Each component handles its own behavior
7. **Polymorphism**: Leverage polymorphism for uniform operations

## Disadvantages

1. **Overly General**: Component interface may become too general
2. **Type Safety**: Hard to restrict component types at compile time
3. **Complexity**: Can be overkill for simple hierarchies
4. **Performance**: Recursive operations may be slower
5. **Memory**: Composite objects consume more memory
6. **Design Restrictions**: Some operations don't make sense for all components

## Implementation Best Practices

### 1. **Abstract Component Base Class**
```csharp
// Good: Use abstract base with common operations
public abstract class Component
{
    public abstract void Operation();
    
    public virtual void Add(Component component)
    {
        throw new NotSupportedException();
    }
}

// Avoid: Interface without default implementations
public interface IComponent
{
    void Operation();
    void Add(IComponent component); // Leaf must implement unnecessarily
}
```

### 2. **Safe Child Management**
```csharp
// Good: Validate operations
public override void Add(Component component)
{
    if (component == null)
        throw new ArgumentNullException(nameof(component));
    if (component == this)
        throw new InvalidOperationException("Cannot add component to itself");
    
    children.Add(component);
}

// Avoid: No validation
public override void Add(Component component)
{
    children.Add(component); // May cause circular references
}
```

### 3. **Provide Parent References When Needed**
```csharp
// Good: Track parent for navigation
public abstract class Component
{
    protected Component? parent;
    
    public virtual void Add(Component component)
    {
        component.parent = this;
        // Add to children
    }
}
```

### 4. **Cache Computed Values**
```csharp
// Good: Cache expensive computations
public class Directory : Component
{
    private long? cachedSize;
    
    public override long GetSize()
    {
        if (cachedSize.HasValue)
            return cachedSize.Value;
        
        cachedSize = children.Sum(c => c.GetSize());
        return cachedSize.Value;
    }
    
    public override void Add(Component component)
    {
        cachedSize = null; // Invalidate cache
        children.Add(component);
    }
}
```

## C# Specific Features

### 1. **Use Collections Initializers**
```csharp
var root = new Directory("root")
{
    new File("file1.txt", 100),
    new File("file2.txt", 200),
    new Directory("subdir")
    {
        new File("file3.txt", 300)
    }
};
```

### 2. **LINQ for Tree Operations**
```csharp
public class Composite : Component
{
    private List<Component> children = new();
    
    public IEnumerable<Component> GetAllLeaves()
    {
        return children.SelectMany(c => 
            c is Composite composite 
                ? composite.GetAllLeaves() 
                : new[] { c });
    }
    
    public Component? FindByName(string name)
    {
        return children.FirstOrDefault(c => c.Name == name) 
            ?? children.OfType<Composite>()
                      .Select(c => c.FindByName(name))
                      .FirstOrDefault(c => c != null);
    }
}
```

### 3. **Yield Return for Lazy Traversal**
```csharp
public IEnumerable<Component> Traverse()
{
    yield return this;
    
    if (this is Composite composite)
    {
        foreach (var child in composite.GetChildren())
        {
            foreach (var descendant in child.Traverse())
            {
                yield return descendant;
            }
        }
    }
}
```

### 4. **Pattern Matching**
```csharp
public void ProcessComponent(Component component)
{
    switch (component)
    {
        case File file:
            ProcessFile(file);
            break;
        case Directory { Children.Count: > 0 } dir:
            ProcessNonEmptyDirectory(dir);
            break;
        case Directory dir:
            ProcessEmptyDirectory(dir);
            break;
    }
}
```

### 5. **Records for Immutable Composites**
```csharp
public abstract record Component(string Name);

public record File(string Name, long Size) : Component(Name);

public record Directory(string Name, ImmutableList<Component> Children) 
    : Component(Name)
{
    public long GetTotalSize() => Children.Sum(c => 
        c switch
        {
            File f => f.Size,
            Directory d => d.GetTotalSize(),
            _ => 0
        });
}
```

## Related Patterns

### **Composite + Iterator**
- Use Iterator to traverse composite structures
- Simplifies navigation through tree hierarchies

### **Composite + Visitor**
- Visitor pattern helps add new operations to composite structures
- Separates algorithms from object structure

### **Composite + Decorator**
- Both use recursive composition
- Decorator adds responsibilities; Composite represents hierarchies

### **Composite + Chain of Responsibility**
- Can combine for request handling up/down the tree
- Requests propagate through parent-child relationships

### **Composite + Flyweight**
- Share leaf objects across multiple composites to save memory
- Useful when many similar leaves exist

### **Composite + Builder**
- Use Builder to construct complex composite structures
- Provides fluent interface for tree construction

## Comparison with Similar Patterns

| Aspect | Composite | Decorator | Adapter |
|--------|-----------|-----------|---------|
| Intent | Part-whole hierarchies | Add responsibilities | Interface compatibility |
| Structure | Tree structure | Linear wrapping chain | Single wrapper |
| Components | Multiple children | One wrapped object | One adaptee |
| Purpose | Group objects | Enhance objects | Convert interface |
| Recursion | Yes, naturally recursive | Can be recursive | Not recursive |

## Common Pitfalls

### 1. **Circular References**
```csharp
// Avoid: Allowing circular references
var dir1 = new Directory("dir1");
var dir2 = new Directory("dir2");
dir1.Add(dir2);
dir2.Add(dir1); // Circular!

// Better: Check for circular references
public override void Add(Component component)
{
    if (IsAncestor(component))
        throw new InvalidOperationException("Circular reference detected");
    children.Add(component);
}

private bool IsAncestor(Component component)
{
    var current = this.parent;
    while (current != null)
    {
        if (current == component) return true;
        current = current.parent;
    }
    return false;
}
```

### 2. **Breaking Type Safety**
```csharp
// Avoid: Allowing incompatible components
public class NumberComposite : Component
{
    public void Add(Component component)
    {
        children.Add(component); // What if it's not a number?
    }
}

// Better: Use generics or validation
public class NumberComposite : Component
{
    public void Add(Number number)
    {
        if (number == null) throw new ArgumentNullException();
        children.Add(number);
    }
}
```

### 3. **Exposing Internal Structure**
```csharp
// Avoid: Exposing mutable collection
public class Composite
{
    public List<Component> Children { get; } = new();
}

// Better: Provide controlled access
public class Composite
{
    private readonly List<Component> children = new();
    
    public IReadOnlyList<Component> Children => children.AsReadOnly();
    
    public void Add(Component component) { /* controlled add */ }
}
```

### 4. **Inefficient Recursion**
```csharp
// Avoid: Recalculating on every call
public override long GetSize()
{
    return children.Sum(c => c.GetSize()); // Recalculates entire tree!
}

// Better: Cache when appropriate
private long? cachedSize;

public override long GetSize()
{
    return cachedSize ??= children.Sum(c => c.GetSize());
}

public override void Add(Component component)
{
    InvalidateCache();
    children.Add(component);
}
```

## Testing Considerations

### 1. **Test Leaf and Composite Separately**
```csharp
[Test]
public void File_Should_Return_Its_Size()
{
    var file = new File("test.txt", 1024);
    Assert.AreEqual(1024, file.GetSize());
}

[Test]
public void Directory_Should_Sum_Children_Sizes()
{
    var dir = new Directory("test");
    dir.Add(new File("file1.txt", 100));
    dir.Add(new File("file2.txt", 200));
    
    Assert.AreEqual(300, dir.GetSize());
}
```

### 2. **Test Recursive Operations**
```csharp
[Test]
public void Directory_Should_Calculate_Nested_Sizes()
{
    var root = new Directory("root");
    var subdir = new Directory("subdir");
    
    subdir.Add(new File("file1.txt", 100));
    root.Add(subdir);
    root.Add(new File("file2.txt", 200));
    
    Assert.AreEqual(300, root.GetSize());
}
```

### 3. **Test Edge Cases**
```csharp
[Test]
public void Empty_Directory_Should_Have_Zero_Size()
{
    var dir = new Directory("empty");
    Assert.AreEqual(0, dir.GetSize());
}

[Test]
public void Should_Prevent_Circular_References()
{
    var dir1 = new Directory("dir1");
    var dir2 = new Directory("dir2");
    
    dir1.Add(dir2);
    
    Assert.Throws<InvalidOperationException>(() => dir2.Add(dir1));
}
```

## Performance Considerations

### 1. **Use Lazy Loading for Large Trees**
```csharp
public class LazyDirectory : Component
{
    private List<Component>? children;
    
    private List<Component> Children => 
        children ??= LoadChildrenFromDisk();
}
```

### 2. **Implement Iterative Traversal for Deep Trees**
```csharp
public IEnumerable<Component> IterativeTraversal()
{
    var stack = new Stack<Component>();
    stack.Push(this);
    
    while (stack.Count > 0)
    {
        var current = stack.Pop();
        yield return current;
        
        if (current is Composite composite)
        {
            foreach (var child in composite.GetChildren().Reverse())
                stack.Push(child);
        }
    }
}
```

## Summary

The Composite pattern is essential for representing part-whole hierarchies where clients need to treat individual objects and compositions uniformly. It's particularly valuable when:
- Building tree structures (file systems, UI components, organization charts)
- Implementing recursive algorithms
- Need uniform treatment of simple and complex objects
- Structure can grow dynamically

By using a common interface for both leaf and composite objects, the pattern simplifies client code and enables powerful recursive operations while maintaining flexibility. It's a cornerstone pattern for any system dealing with hierarchical data structures.
