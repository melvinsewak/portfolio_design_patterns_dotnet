# Abstract Factory Pattern

## Intent
Provide an interface for creating families of related or dependent objects without specifying their concrete classes.

## Also Known As
Kit

## Problem
You need to create multiple related objects that must be used together, and you want to:
- Ensure objects from the same family are used together
- Support multiple families of products
- Isolate concrete classes from clients
- Easily switch between different families

For example, a UI library that needs to create buttons, checkboxes, and text boxes that all match the same platform style (Windows, Mac, or Linux).

## Solution
The Abstract Factory pattern:
1. Declares interfaces for creating each product type
2. Declares a factory interface with creation methods for all product types
3. Implements concrete factories for each product family
4. Clients work with factories and products through their abstract interfaces

## Structure
```
┌─────────────────┐
│ AbstractFactory │
├─────────────────┤                Creates
│+CreateProductA()│───────┐         ┌──────────────┐
│+CreateProductB()│───┐   └────────>│ ProductA     │
└─────────────────┘   │             └──────────────┘
        △             │                    △
        │             │                    │
┌─────────────────┐   │   Creates    ┌──────────────┐
│ConcreteFactory1 │   └─────────────>│ ProductB     │
├─────────────────┤                  └──────────────┘
│+CreateProductA()│                         △
│+CreateProductB()│                         │
└─────────────────┘                  ┌──────────────┐
                                     │ConcreteProduct│
                                     └──────────────┘
```

## Participants
1. **AbstractFactory (IUIFactory)** - Declares product creation methods
2. **ConcreteFactory (WindowsUIFactory)** - Implements creation methods
3. **AbstractProduct (IButton, ICheckbox)** - Declares product interfaces
4. **ConcreteProduct (WindowsButton, MacButton)** - Implements products
5. **Client (Application)** - Uses only abstract interfaces

## Implementation in C#

### Basic Implementation
```csharp
// Abstract Products
public interface IButton { void Render(); }
public interface ICheckbox { void Render(); }

// Concrete Products - Windows
public class WindowsButton : IButton
{
    public void Render() => Console.WriteLine("Windows Button");
}

public class WindowsCheckbox : ICheckbox
{
    public void Render() => Console.WriteLine("Windows Checkbox");
}

// Concrete Products - Mac
public class MacButton : IButton
{
    public void Render() => Console.WriteLine("Mac Button");
}

public class MacCheckbox : ICheckbox
{
    public void Render() => Console.WriteLine("Mac Checkbox");
}

// Abstract Factory
public interface IUIFactory
{
    IButton CreateButton();
    ICheckbox CreateCheckbox();
}

// Concrete Factories
public class WindowsUIFactory : IUIFactory
{
    public IButton CreateButton() => new WindowsButton();
    public ICheckbox CreateCheckbox() => new WindowsCheckbox();
}

public class MacUIFactory : IUIFactory
{
    public IButton CreateButton() => new MacButton();
    public ICheckbox CreateCheckbox() => new MacCheckbox();
}

// Client
public class Application
{
    private readonly IButton _button;
    private readonly ICheckbox _checkbox;
    
    public Application(IUIFactory factory)
    {
        _button = factory.CreateButton();
        _checkbox = factory.CreateCheckbox();
    }
    
    public void Render()
    {
        _button.Render();
        _checkbox.Render();
    }
}
```

## When to Use
✅ **Use when:**
- System must be independent of how its products are created
- System should be configured with one of multiple families of products
- Family of related products must be used together (constraint)
- You want to provide a library of products revealing only interfaces

❌ **Avoid when:**
- Product families don't vary or rarely change
- Adding new product types requires changing all factories
- The abstraction overhead isn't justified

## Real-World Examples
1. **Cross-platform UI Frameworks** - Windows, Mac, Linux widgets
2. **Database Providers** - SQL Server, PostgreSQL, MySQL components
3. **Document Formats** - PDF, Word, HTML generators
4. **Theme Systems** - Light mode, dark mode, high contrast
5. **Cloud Providers** - AWS, Azure, GCP service clients

## Advantages
✅ Isolates concrete classes from client
✅ Easy to exchange product families
✅ Promotes consistency among products
✅ Supports Open/Closed Principle
✅ Products guaranteed to be compatible

## Disadvantages
❌ Difficult to support new product kinds
❌ Increases code complexity with many interfaces
❌ Can lead to parallel class hierarchies
❌ Factory interface grows with each new product type

## Abstract Factory vs Factory Method

| Abstract Factory | Factory Method |
|-----------------|----------------|
| Creates families of objects | Creates one type of object |
| Uses object composition | Uses inheritance |
| Factory is an object | Factory is a method |
| Emphasizes family of products | Emphasizes single product |

## Variations

### 1. Factory with Parameters
```csharp
public interface IUIFactory
{
    IButton CreateButton(ButtonStyle style);
}
```

### 2. Singleton Factory
```csharp
public class WindowsUIFactory : IUIFactory
{
    private static readonly Lazy<WindowsUIFactory> _instance = 
        new Lazy<WindowsUIFactory>(() => new WindowsUIFactory());
    
    public static WindowsUIFactory Instance => _instance.Value;
    
    private WindowsUIFactory() { }
}
```

### 3. Prototypal Factory
Use prototypes instead of creating new objects:
```csharp
public class PrototypeFactory : IUIFactory
{
    private readonly IButton _buttonPrototype;
    
    public PrototypeFactory(IButton buttonPrototype)
    {
        _buttonPrototype = buttonPrototype;
    }
    
    public IButton CreateButton() => _buttonPrototype.Clone();
}
```

## Best Practices
1. **Keep product families cohesive** - Related products only
2. **Use dependency injection** - Inject factory into clients
3. **Consider configuration** - Select factory based on config
4. **Document product families** - Make relationships clear
5. **Limit factory interface size** - Don't overcomplicate

## C# Specific Features
```csharp
// Using generic abstract factory
public interface IFactory<TProduct>
{
    TProduct Create();
}

// Using covariance
public interface IUIFactory<out TButton, out TCheckbox>
    where TButton : IButton
    where TCheckbox : ICheckbox
{
    TButton CreateButton();
    TCheckbox CreateCheckbox();
}

// Using records for configuration
public record UIConfiguration(string Platform, string Theme);

public class ConfigurableUIFactory : IUIFactory
{
    private readonly UIConfiguration _config;
    
    public ConfigurableUIFactory(UIConfiguration config)
    {
        _config = config;
    }
    
    public IButton CreateButton() => _config.Platform switch
    {
        "Windows" => new WindowsButton(),
        "Mac" => new MacButton(),
        _ => throw new NotSupportedException()
    };
}
```

## Related Patterns
- **Factory Method** - Often used to implement factories
- **Singleton** - Factory instances often singletons
- **Prototype** - Can use prototypes instead of creating
- **Builder** - Can build complex product families

## Testing Considerations
Abstract Factory improves testability:
```csharp
// Create mock factory for testing
public class MockUIFactory : IUIFactory
{
    public IButton CreateButton() => new MockButton();
    public ICheckbox CreateCheckbox() => new MockCheckbox();
}

[Fact]
public void Application_UsesFactory()
{
    var mockFactory = new MockUIFactory();
    var app = new Application(mockFactory);
    // Test application behavior
}
```

## Modern Alternatives
In modern C# with dependency injection:
```csharp
// Register factories in DI container
services.AddSingleton<IUIFactory, WindowsUIFactory>();

// Or use factory pattern with DI
services.AddSingleton<Func<string, IUIFactory>>(provider => 
{
    return platform => platform switch
    {
        "Windows" => new WindowsUIFactory(),
        "Mac" => new MacUIFactory(),
        _ => new WindowsUIFactory()
    };
});
```
