using DesignPatterns.CreationalPatterns;
using DesignPatterns.StructuralPatterns;
using DesignPatterns.ModernPatterns;

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║     Design Patterns Portfolio - .NET 10.0 Examples          ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

// Creational Patterns
Console.WriteLine("\n█ CREATIONAL PATTERNS █\n");
SingletonDemo.Run();
FactoryMethodDemo.Run();
AbstractFactoryDemo.Run();
BuilderDemo.Run();
PrototypeDemo.Run();
ObjectPoolDemo.Run();

// Structural Patterns
Console.WriteLine("\n\n█ STRUCTURAL PATTERNS █\n");
AdapterDemo.Run();
BridgeDemo.Run();
CompositeDemo.Run();
DecoratorDemo.Run();
FacadeDemo.Run();
FlyweightDemo.Run();
ProxyDemo.Run();

// Modern Patterns
Console.WriteLine("\n\n█ MODERN PATTERNS █\n");
await DependencyInjectionDemo.Run();
await RepositoryPatternDemo.Run();
await UnitOfWorkDemo.Run();
await CQRSPatternDemo.Run();
await SpecificationPatternDemo.Run();

Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                    Demo Complete                             ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
