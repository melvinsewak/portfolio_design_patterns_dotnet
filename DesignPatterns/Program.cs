using DesignPatterns.CreationalPatterns;
using DesignPatterns.StructuralPatterns;

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
BridgeDemo.Run();
CompositeDemo.Run();
DecoratorDemo.Run();

Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                    Demo Complete                             ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
