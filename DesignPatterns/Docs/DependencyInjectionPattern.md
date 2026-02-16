# Dependency Injection Pattern

## Intent
Dependency Injection (DI) is a design pattern that implements Inversion of Control (IoC) for resolving dependencies. It allows objects to receive their dependencies from external sources rather than creating them internally, promoting loose coupling and easier testing.

## Problem
In traditional object-oriented programming, classes often directly instantiate their dependencies, leading to:
- **Tight coupling** between classes
- **Difficult testing** - hard to mock or replace dependencies
- **Reduced flexibility** - changing implementations requires modifying multiple classes
- **Poor maintainability** - dependencies are scattered throughout the codebase
- **Violation of SOLID principles** - particularly Dependency Inversion Principle

Example of tightly coupled code:
```csharp
public class OrderService
{
    private EmailService _emailService = new EmailService();  // Hard-coded dependency
    private Logger _logger = new Logger();                     // Can't be replaced
    
    public void ProcessOrder(Order order)
    {
        _logger.Log("Processing order");
        _emailService.SendConfirmation(order);
    }
}
```

## Solution
Dependency Injection solves this by:
1. **Defining abstractions** (interfaces) for dependencies
2. **Injecting dependencies** through constructor, properties, or methods
3. **Using IoC containers** to manage dependency lifecycles
4. **Inverting control** - let the framework provide dependencies

The pattern separates the creation of dependencies from their usage, allowing for:
- Easy swapping of implementations
- Better testability with mock objects
- Centralized dependency configuration
- Reduced coupling between components

## Structure

```
┌─────────────────────────────────────────────────────────────┐
│                    DI Container / Injector                   │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Service Registry                                     │  │
│  │  • ILogger → ConsoleLogger                           │  │
│  │  • IEmailService → EmailService                      │  │
│  │  • IRepository → Repository                          │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                         │ Resolves & Injects
                         ▼
┌──────────────────────────────────────────────────────────────┐
│                    Client Class                              │
│                                                              │
│  public class OrderService                                   │
│  {                                                           │
│      private readonly ILogger _logger;                       │
│      private readonly IEmailService _emailService;           │
│                                                              │
│      // Constructor Injection                                │
│      public OrderService(ILogger logger,                     │
│                         IEmailService emailService)          │
│      {                                                       │
│          _logger = logger;                                   │
│          _emailService = emailService;                       │
│      }                                                       │
│  }                                                           │
└──────────────────────────────────────────────────────────────┘
                         │ Uses
                         ▼
┌──────────────────────────────────────────────────────────────┐
│                Service Interfaces                            │
│                                                              │
│  ┌────────────────┐  ┌────────────────┐  ┌──────────────┐  │
│  │   ILogger      │  │ IEmailService  │  │ IRepository  │  │
│  └────────────────┘  └────────────────┘  └──────────────┘  │
│          ▲                   ▲                   ▲          │
│          │                   │                   │          │
│  ┌───────┴──────┐   ┌────────┴────────┐  ┌──────┴──────┐  │
│  │ConsoleLogger │   │  EmailService   │  │ Repository  │  │
│  │  FileLogger  │   │ SmtpEmailSvc    │  │ CachedRepo  │  │
│  └──────────────┘   └─────────────────┘  └─────────────┘  │
└──────────────────────────────────────────────────────────────┘
```

## Types of Dependency Injection

### 1. Constructor Injection
**Most common and recommended**
```csharp
public class OrderService
{
    private readonly ILogger _logger;
    
    public OrderService(ILogger logger)  // Injected via constructor
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
```

### 2. Property Injection
**For optional dependencies**
```csharp
public class UserService
{
    public ILogger? Logger { get; set; }  // Optional dependency
    
    public void RegisterUser(string username)
    {
        Logger?.Log($"Registering {username}");
    }
}
```

### 3. Method Injection
**For dependencies needed only for specific operations**
```csharp
public class ReportService
{
    public void GenerateReport(IDataProvider dataProvider)  // Injected per method call
    {
        var data = dataProvider.GetData();
        // Generate report
    }
}
```

## When to Use

### Use Dependency Injection When:
- ✅ Building applications with **multiple interchangeable implementations**
- ✅ You need **testable code** with mock objects
- ✅ Working with **framework-managed lifecycles** (ASP.NET Core, etc.)
- ✅ You want to follow **SOLID principles**
- ✅ Building **enterprise applications** with complex dependencies
- ✅ You need **configuration-based** component selection
- ✅ Working in **team environments** with modular code

### Avoid When:
- ❌ Building very **simple applications** with few dependencies
- ❌ Performance is **critical** and object creation overhead matters
- ❌ Dependencies are **never expected to change**
- ❌ You're working with **static utility classes**
- ❌ The added complexity **doesn't provide value**

## Real-World Use Cases

### 1. **ASP.NET Core Applications**
```csharp
// Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddSingleton<ILogger, FileLogger>();
    services.AddTransient<IEmailService, SendGridEmailService>();
}

// Controller
public class UserController : Controller
{
    private readonly IUserRepository _repository;
    
    public UserController(IUserRepository repository)
    {
        _repository = repository;  // Automatically injected by framework
    }
}
```

### 2. **Testing with Mock Objects**
```csharp
// Production
public class PaymentService
{
    private readonly IPaymentGateway _gateway;
    
    public PaymentService(IPaymentGateway gateway)
    {
        _gateway = gateway;
    }
}

// Test
[Test]
public void TestPayment()
{
    var mockGateway = new Mock<IPaymentGateway>();
    mockGateway.Setup(g => g.Process(It.IsAny<decimal>())).Returns(true);
    
    var service = new PaymentService(mockGateway.Object);
    // Test with mock
}
```

### 3. **Multi-Tenant Applications**
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddScoped<IDataProvider>(sp => 
    {
        var tenantId = GetCurrentTenantId();
        return tenantId == "premium" 
            ? new PremiumDataProvider() 
            : new StandardDataProvider();
    });
}
```

### 4. **Plugin Architecture**
```csharp
public class PluginManager
{
    private readonly IEnumerable<IPlugin> _plugins;
    
    public PluginManager(IEnumerable<IPlugin> plugins)  // All registered plugins injected
    {
        _plugins = plugins;
    }
}
```

## Advantages

1. **Loose Coupling**
   - Classes depend on abstractions, not concrete implementations
   - Easy to swap implementations without changing client code

2. **Improved Testability**
   - Easy to inject mock/stub dependencies for unit testing
   - Isolated testing of individual components

3. **Better Code Organization**
   - Clear separation of concerns
   - Dependencies are explicit and visible

4. **Flexibility**
   - Runtime configuration of dependencies
   - Easy to add new implementations

5. **Maintainability**
   - Changes to implementations don't affect clients
   - Centralized dependency configuration

6. **Lifecycle Management**
   - DI containers manage object lifecycles (Singleton, Transient, Scoped)
   - Automatic disposal of resources

7. **Follows SOLID Principles**
   - Dependency Inversion Principle
   - Open/Closed Principle
   - Single Responsibility Principle

## Disadvantages

1. **Increased Complexity**
   - More interfaces and abstractions
   - Learning curve for DI containers

2. **Runtime Errors**
   - Missing registrations discovered at runtime
   - Circular dependency issues

3. **Performance Overhead**
   - Reflection-based dependency resolution
   - Object creation overhead (usually negligible)

4. **Debugging Challenges**
   - Stack traces can be deeper
   - Harder to trace object creation

5. **Over-abstraction**
   - Can lead to unnecessary interfaces for simple cases
   - May overcomplicate simple scenarios

## Best Practices

1. **Prefer Constructor Injection**
   - Makes dependencies explicit and required
   - Ensures objects are always in valid state
   ```csharp
   public OrderService(ILogger logger, IRepository repo)  // ✅ Good
   ```

2. **Use Interfaces for Abstraction**
   ```csharp
   services.AddScoped<IUserService, UserService>();  // ✅ Good
   services.AddScoped<UserService>();                 // ❌ Less flexible
   ```

3. **Register at the Composition Root**
   - Register all dependencies in one central location
   - Typically in `Startup.cs` or `Program.cs` for ASP.NET Core

4. **Choose Appropriate Lifetimes**
   - **Transient**: New instance each time (stateless services)
   - **Scoped**: One instance per request (database contexts)
   - **Singleton**: One instance for application lifetime (configuration, caching)

5. **Avoid Service Locator Anti-Pattern**
   ```csharp
   // ❌ Bad - Service Locator
   var logger = ServiceLocator.Get<ILogger>();
   
   // ✅ Good - Constructor Injection
   public MyClass(ILogger logger) { }
   ```

6. **Guard Against Null**
   ```csharp
   public OrderService(ILogger logger)
   {
       _logger = logger ?? throw new ArgumentNullException(nameof(logger));
   }
   ```

7. **Don't Inject Too Many Dependencies**
   - If a class needs > 5 dependencies, consider refactoring
   - May indicate violation of Single Responsibility Principle

8. **Use Property Injection Sparingly**
   - Only for truly optional dependencies
   - Constructor injection is preferred

## Integration with .NET Ecosystem

### Microsoft.Extensions.DependencyInjection
```csharp
var builder = WebApplication.CreateBuilder(args);

// Built-in DI container
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddSingleton<ICache, MemoryCache>();

// Generic registration
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Factory-based registration
builder.Services.AddScoped<IDataService>(sp => 
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new DataService(config["ConnectionString"]);
});
```

### ASP.NET Core Integration
```csharp
// Automatic controller injection
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }
}
```

### Entity Framework Core
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Automatically available for injection
public class UserRepository
{
    private readonly ApplicationDbContext _context;
    
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
}
```

### Options Pattern
```csharp
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

public class EmailService
{
    private readonly EmailSettings _settings;
    
    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }
}
```

### Third-Party Containers
- **Autofac**: Feature-rich, assembly scanning, modules
- **Ninject**: Fluent API, convention-based binding
- **Castle Windsor**: Interceptors, decorators, advanced features
- **Simple Injector**: Performance-focused, diagnostics

## Related Patterns

- **Service Locator**: Anti-pattern alternative to DI
- **Factory Pattern**: Can be used with DI to create objects
- **Strategy Pattern**: DI enables easy strategy swapping
- **Decorator Pattern**: DI containers can apply decorators
- **Repository Pattern**: Often used with DI for data access

## Summary

Dependency Injection is a fundamental pattern in modern .NET development, especially with ASP.NET Core's built-in DI container. It promotes loose coupling, testability, and maintainability by inverting control of dependency creation. While it adds some complexity, the benefits in terms of code quality, testability, and flexibility far outweigh the costs for most applications. The pattern is essential for building enterprise-grade, maintainable applications that follow SOLID principles.
