# Design Patterns Portfolio - C# .NET 10.0

A comprehensive, production-ready implementation of **29 essential design patterns** in modern C# .NET 10.0, complete with real-world examples and extensive documentation.

[![.NET Version](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)
[![C# Version](https://img.shields.io/badge/C%23-12.0-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

## 📚 Table of Contents

- [Overview](#overview)
- [Patterns Implemented](#patterns-implemented)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Running Examples](#running-examples)
- [Documentation](#documentation)
- [Features](#features)
- [Contributing](#contributing)

## 🎯 Overview

This repository serves as a comprehensive educational resource and reference implementation for software design patterns in C#. Each pattern includes:

- ✅ **Production-quality code** following modern C# best practices
- ✅ **2-3 real-world examples** per pattern
- ✅ **Comprehensive markdown documentation** (200+ lines each)
- ✅ **UML diagrams** in ASCII format
- ✅ **Runnable demos** with clear output
- ✅ **When to use/avoid** guidelines
- ✅ **Best practices** and common pitfalls
- ✅ **Thread-safety** considerations where applicable

## 🏗️ Patterns Implemented

### Creational Patterns (6)
Design patterns that deal with object creation mechanisms.

| Pattern | Description | Key Use Case |
|---------|-------------|--------------|
| **Singleton** | Ensure only one instance exists | Configuration manager, Logger |
| **Factory Method** | Defer instantiation to subclasses | Logger factory, Document creator |
| **Abstract Factory** | Families of related objects | Cross-platform UI components |
| **Builder** | Construct complex objects step-by-step | Computer builder, HTTP request |
| **Prototype** | Clone existing objects | Document templates, Game objects |
| **Object Pool** | Reuse expensive objects | Database connections, Thread pool |

### Structural Patterns (7)
Design patterns that deal with object composition and relationships.

| Pattern | Description | Key Use Case |
|---------|-------------|--------------|
| **Adapter** | Make incompatible interfaces work together | Legacy system integration |
| **Bridge** | Separate abstraction from implementation | Cross-platform rendering |
| **Composite** | Tree structure for part-whole hierarchies | File system, Organization chart |
| **Decorator** | Add behavior dynamically | Coffee shop ordering, Stream wrappers |
| **Facade** | Simplified interface to complex subsystem | Home theater system |
| **Flyweight** | Share objects to save memory | Text editor characters, Game particles |
| **Proxy** | Placeholder for another object | Virtual proxy, Protection proxy |

### Behavioral Patterns (11)
Design patterns that deal with object collaboration and responsibility.

| Pattern | Description | Key Use Case |
|---------|-------------|--------------|
| **Chain of Responsibility** | Pass request along handler chain | Support tickets, Approval workflow |
| **Command** | Encapsulate requests as objects | Undo/Redo, Transaction management |
| **Iterator** | Sequential access to elements | Custom collections, Tree traversal |
| **Mediator** | Centralized communication hub | Chat room, Air traffic control |
| **Memento** | Save and restore object state | Editor snapshots, Game saves |
| **Observer** | Notify dependents of state changes | Event system, Stock monitoring |
| **State** | Alter behavior when state changes | Document workflow, Vending machine |
| **Strategy** | Interchangeable algorithms | Payment methods, Sorting algorithms |
| **Template Method** | Define algorithm skeleton | Data mining, Recipe execution |
| **Visitor** | Operations on object structure | Shopping cart, Tax calculator |
| **Interpreter** | Grammar and language interpreter | Expression evaluator, SQL builder |

### Modern Patterns (5)
Contemporary patterns widely used in modern .NET applications.

| Pattern | Description | Key Use Case |
|---------|-------------|--------------|
| **Dependency Injection** | Invert control of dependencies | Service configuration, Testing |
| **Repository** | Abstract data access layer | Database operations, Unit testing |
| **Unit of Work** | Manage transactional operations | Multi-table updates, Atomic commits |
| **CQRS** | Separate read and write operations | High-performance systems, Event sourcing |
| **Specification** | Encapsulate business rules | Product filtering, Validation rules |

## 🚀 Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or later
- Any IDE (Visual Studio 2022, VS Code, Rider)

### Clone and Build

```bash
# Clone the repository
git clone https://github.com/melvinsewak/portfolio_design_patterns_dotnet.git
cd portfolio_design_patterns_dotnet

# Build the solution
dotnet build

# Run all pattern demonstrations
dotnet run --project DesignPatterns
```

### Quick Example

```csharp
// Singleton Pattern
var config = ConfigurationManager.Instance;
config.SetSetting("Environment", "Production");

// Builder Pattern
var computer = new ComputerBuilder()
    .SetCPU("AMD Ryzen 9 7950X")
    .SetGPU("NVIDIA RTX 4090")
    .SetRAM(64)
    .Build();

// Strategy Pattern
IPaymentStrategy payment = new CreditCardPayment();
var processor = new PaymentProcessor(payment);
processor.ProcessPayment(99.99m);
```

## 📁 Project Structure

```
portfolio_design_patterns_dotnet/
├── DesignPatterns/
│   ├── CreationalPatterns/         # 6 creational patterns
│   │   ├── SingletonPattern.cs
│   │   ├── FactoryMethodPattern.cs
│   │   ├── AbstractFactoryPattern.cs
│   │   ├── BuilderPattern.cs
│   │   ├── PrototypePattern.cs
│   │   └── ObjectPoolPattern.cs
│   ├── StructuralPatterns/         # 7 structural patterns
│   │   ├── AdapterPattern.cs
│   │   ├── BridgePattern.cs
│   │   ├── CompositePattern.cs
│   │   ├── DecoratorPattern.cs
│   │   ├── FacadePattern.cs
│   │   ├── FlyweightPattern.cs
│   │   └── ProxyPattern.cs
│   ├── BehavioralPatterns/         # 11 behavioral patterns
│   │   ├── ChainOfResponsibilityPattern.cs
│   │   ├── CommandPattern.cs
│   │   ├── IteratorPattern.cs
│   │   ├── MediatorPattern.cs
│   │   ├── MementoPattern.cs
│   │   ├── ObserverPattern.cs
│   │   ├── StatePattern.cs
│   │   ├── StrategyPattern.cs
│   │   ├── TemplateMethodPattern.cs
│   │   ├── VisitorPattern.cs
│   │   └── InterpreterPattern.cs
│   ├── ModernPatterns/             # 5 modern patterns
│   │   ├── DependencyInjectionPattern.cs
│   │   ├── RepositoryPattern.cs
│   │   ├── UnitOfWorkPattern.cs
│   │   ├── CQRSPattern.cs
│   │   └── SpecificationPattern.cs
│   ├── Docs/                       # Comprehensive documentation
│   │   └── [29 markdown files]
│   ├── Program.cs                  # Main demo runner
│   └── DesignPatterns.csproj
├── DesignPatterns.slnx             # Solution file
└── README.md
```

## 🎮 Running Examples

### Run All Patterns
```bash
dotnet run --project DesignPatterns
```

### Run Specific Pattern Categories

To run specific patterns, modify `Program.cs` and comment out sections you don't want to run:

```csharp
// Creational Patterns only
Console.WriteLine("\n█ CREATIONAL PATTERNS █\n");
SingletonDemo.Run();
FactoryMethodDemo.Run();
// ... etc
```

## 📖 Documentation

Each pattern includes detailed documentation in `/DesignPatterns/Docs/`:

- **Intent**: What problem does it solve?
- **Problem**: When is it needed?
- **Solution**: How does it work?
- **Structure**: UML diagrams
- **Implementation**: Code examples
- **When to Use**: Guidelines and scenarios
- **Advantages**: Benefits of the pattern
- **Disadvantages**: Potential drawbacks
- **Best Practices**: Tips and recommendations
- **Real-World Examples**: Practical applications
- **C# Specific Features**: Modern C# usage
- **Related Patterns**: Connections to other patterns

## ✨ Features

### Modern C# 10.0 Features Used

- ✅ **Nullable reference types** for null safety
- ✅ **Record types** for immutable data
- ✅ **Pattern matching** with switch expressions
- ✅ **Async/await** for asynchronous operations
- ✅ **LINQ** for data manipulation
- ✅ **Lambda expressions** and delegates
- ✅ **Extension methods** for fluent APIs
- ✅ **Generic constraints** for type safety
- ✅ **Expression trees** for dynamic queries
- ✅ **File-scoped namespaces** for cleaner code

### Code Quality

- ✅ **SOLID principles** followed throughout
- ✅ **Thread-safe implementations** where appropriate
- ✅ **Comprehensive XML documentation**
- ✅ **Clean code** with meaningful names
- ✅ **DRY principle** applied
- ✅ **Unit test ready** structure
- ✅ **Dependency injection** compatible

## 📊 Statistics

- **Total Patterns**: 29
- **Total Code Lines**: ~14,000+ lines
- **Total Documentation**: ~9,000+ lines
- **Real-World Examples**: 70+
- **Code Size**: ~500KB

## 🎓 Learning Path

### Beginner
Start with these fundamental patterns:
1. Singleton
2. Factory Method
3. Adapter
4. Strategy
5. Observer

### Intermediate
Move to these patterns:
1. Abstract Factory
2. Builder
3. Decorator
4. Command
5. State

### Advanced
Master these complex patterns:
1. Composite
2. Visitor
3. Interpreter
4. CQRS
5. Specification

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Melvin Sewak**

## 🙏 Acknowledgments

- Gang of Four (GoF) Design Patterns book
- Martin Fowler's Enterprise Application Architecture patterns
- Microsoft .NET documentation
- Community feedback and contributions

## 📚 Additional Resources

- [Microsoft C# Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [Design Patterns: Elements of Reusable Object-Oriented Software](https://en.wikipedia.org/wiki/Design_Patterns)
- [Refactoring Guru - Design Patterns](https://refactoring.guru/design-patterns)
- [Source Making - Design Patterns](https://sourcemaking.com/design_patterns)

---

⭐ **Star this repository** if you find it helpful!

🐛 **Report bugs** or **suggest improvements** via [Issues](https://github.com/melvinsewak/portfolio_design_patterns_dotnet/issues)

📧 **Contact**: Open an issue for questions or discussions
