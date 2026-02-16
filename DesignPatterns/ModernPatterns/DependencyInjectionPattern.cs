using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesignPatterns.ModernPatterns;

#region Interfaces

// Service interfaces
public interface ILogger
{
    void Log(string message);
    Task LogAsync(string message);
}

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

public interface IDIRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
}

public interface INotificationService
{
    Task NotifyAsync(string message);
}

public interface IPaymentGateway
{
    Task<bool> ProcessPaymentAsync(decimal amount, string cardNumber);
}

#endregion

#region Service Implementations

// Logger implementations
public class ConsoleLogger : ILogger
{
    private readonly string _prefix;

    public ConsoleLogger(string prefix = "LOG")
    {
        _prefix = prefix;
    }

    public void Log(string message)
    {
        Console.WriteLine($"[{_prefix}] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
    }

    public async Task LogAsync(string message)
    {
        await Task.Run(() => Log(message));
    }
}

public class FileLogger : ILogger
{
    private readonly string _filePath;

    public FileLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Log(string message)
    {
        Console.WriteLine($"[FILE] Writing to {_filePath}: {message}");
    }

    public async Task LogAsync(string message)
    {
        await Task.Run(() => Log(message));
    }
}

public class EmailService : IEmailService
{
    private readonly ILogger _logger;

    // Constructor Injection - Logger injected via constructor
    public EmailService(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        await _logger.LogAsync($"Sending email to {to}");
        await Task.Delay(100); // Simulate email sending
        Console.WriteLine($"Email sent to {to} - Subject: {subject}");
    }
}

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly ILogger _logger;

    // Multiple dependencies injected
    public NotificationService(IEmailService emailService, ILogger logger)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task NotifyAsync(string message)
    {
        await _logger.LogAsync($"Notification: {message}");
        await _emailService.SendEmailAsync("admin@example.com", "Notification", message);
    }
}

#endregion

#region Example 1: E-Commerce Order Processing with Constructor Injection

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class ProductRepository : IDIRepository<Product>
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop", Price = 999.99m },
        new Product { Id = 2, Name = "Mouse", Price = 29.99m },
        new Product { Id = 3, Name = "Keyboard", Price = 79.99m }
    };

    public async Task<Product?> GetByIdAsync(int id)
    {
        await Task.Delay(50); // Simulate database access
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        await Task.Delay(50);
        return _products;
    }

    public async Task AddAsync(Product entity)
    {
        await Task.Delay(50);
        _products.Add(entity);
    }
}

public class OrderService
{
    private readonly IDIRepository<Product> _productRepository;
    private readonly ILogger _logger;
    private readonly IEmailService _emailService;
    private readonly IPaymentGateway _paymentGateway;

    // Constructor Injection - All dependencies injected through constructor
    public OrderService(
        IDIRepository<Product> productRepository,
        ILogger logger,
        IEmailService emailService,
        IPaymentGateway paymentGateway)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _paymentGateway = paymentGateway ?? throw new ArgumentNullException(nameof(paymentGateway));
    }

    public async Task<bool> ProcessOrderAsync(int productId, string customerEmail)
    {
        await _logger.LogAsync($"Processing order for product {productId}");

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            await _logger.LogAsync($"Product {productId} not found");
            return false;
        }

        // Process payment
        var paymentSuccess = await _paymentGateway.ProcessPaymentAsync(product.Price, "1234-5678-9012-3456");
        if (!paymentSuccess)
        {
            await _logger.LogAsync("Payment failed");
            return false;
        }

        // Send confirmation email
        await _emailService.SendEmailAsync(
            customerEmail,
            "Order Confirmation",
            $"Your order for {product.Name} has been confirmed!");

        await _logger.LogAsync($"Order completed for {product.Name}");
        return true;
    }
}

public class StripePaymentGateway : IPaymentGateway
{
    private readonly ILogger _logger;

    public StripePaymentGateway(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<bool> ProcessPaymentAsync(decimal amount, string cardNumber)
    {
        await _logger.LogAsync($"Processing payment of ${amount} via Stripe");
        await Task.Delay(100);
        return true; // Simulate successful payment
    }
}

#endregion

#region Example 2: User Service with Property Injection

public class DIUserService
{
    private ILogger? _logger;
    private INotificationService? _notificationService;

    // Property Injection - Optional dependencies set via properties
    public ILogger? Logger
    {
        get => _logger;
        set => _logger = value;
    }

    public INotificationService? NotificationService
    {
        get => _notificationService;
        set => _notificationService = value;
    }

    public async Task RegisterUserAsync(string username, string email)
    {
        // Use logger if available (optional dependency)
        _logger?.Log($"Registering user: {username}");

        await Task.Delay(100); // Simulate user registration
        Console.WriteLine($"User {username} registered successfully!");

        // Send notification if service is available
        if (_notificationService != null)
        {
            await _notificationService.NotifyAsync($"New user registered: {username}");
        }

        _logger?.Log($"User registration completed for {username}");
    }
}

#endregion

#region Example 3: Simple DI Container Implementation

public interface IServiceContainer
{
    void Register<TInterface, TImplementation>() where TImplementation : TInterface;
    void RegisterSingleton<TInterface, TImplementation>() where TImplementation : TInterface;
    T Resolve<T>();
}

public class SimpleDIContainer : IServiceContainer
{
    private readonly Dictionary<Type, Type> _transientServices = new();
    private readonly Dictionary<Type, object> _singletonServices = new();
    private readonly Dictionary<Type, Type> _singletonTypes = new();

    // Register transient service (new instance each time)
    public void Register<TInterface, TImplementation>() where TImplementation : TInterface
    {
        _transientServices[typeof(TInterface)] = typeof(TImplementation);
    }

    // Register singleton service (same instance always)
    public void RegisterSingleton<TInterface, TImplementation>() where TImplementation : TInterface
    {
        _singletonTypes[typeof(TInterface)] = typeof(TImplementation);
    }

    public T Resolve<T>()
    {
        return (T)Resolve(typeof(T));
    }

    private object Resolve(Type type)
    {
        // Check if it's a singleton that's already created
        if (_singletonServices.TryGetValue(type, out var singletonInstance))
        {
            return singletonInstance;
        }

        // Check if it's a registered singleton type
        if (_singletonTypes.TryGetValue(type, out var singletonType))
        {
            var instance = CreateInstance(singletonType);
            _singletonServices[type] = instance;
            return instance;
        }

        // Check if it's a transient service
        if (_transientServices.TryGetValue(type, out var implementationType))
        {
            return CreateInstance(implementationType);
        }

        throw new InvalidOperationException($"Service of type {type.Name} is not registered.");
    }

    private object CreateInstance(Type type)
    {
        // Get constructor with most parameters
        var constructor = type.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (constructor == null)
        {
            throw new InvalidOperationException($"No public constructor found for {type.Name}");
        }

        // Resolve constructor parameters
        var parameters = constructor.GetParameters()
            .Select(p => Resolve(p.ParameterType))
            .ToArray();

        return Activator.CreateInstance(type, parameters)!;
    }
}

#endregion

#region Demo

public static class DependencyInjectionDemo
{
    public static async Task Run()
    {
        Console.WriteLine("=== Dependency Injection Pattern Demo ===\n");

        // Example 1: Constructor Injection with Order Processing
        Console.WriteLine("--- Example 1: Constructor Injection ---");
        await ConstructorInjectionExample();

        Console.WriteLine("\n--- Example 2: Property Injection ---");
        await PropertyInjectionExample();

        Console.WriteLine("\n--- Example 3: Simple DI Container ---");
        await DIContainerExample();
    }

    private static async Task ConstructorInjectionExample()
    {
        // Manually create dependencies (in real apps, use DI container like Microsoft.Extensions.DependencyInjection)
        ILogger logger = new ConsoleLogger("ORDER");
        IEmailService emailService = new EmailService(logger);
        IDIRepository<Product> productRepository = new ProductRepository();
        IPaymentGateway paymentGateway = new StripePaymentGateway(logger);

        // Inject all dependencies via constructor
        var orderService = new OrderService(productRepository, logger, emailService, paymentGateway);

        // Process an order
        var success = await orderService.ProcessOrderAsync(1, "customer@example.com");
        Console.WriteLine($"Order processing result: {(success ? "Success" : "Failed")}");
    }

    private static async Task PropertyInjectionExample()
    {
        // Create optional dependencies
        ILogger logger = new ConsoleLogger("USER");
        IEmailService emailService = new EmailService(logger);
        INotificationService notificationService = new NotificationService(emailService, logger);

        var userService = new DIUserService
        {
            // Property injection - set dependencies via properties
            Logger = logger,
            NotificationService = notificationService
        };

        await userService.RegisterUserAsync("john_doe", "john@example.com");

        // Service works even without optional dependencies
        var minimalUserService = new DIUserService();
        await minimalUserService.RegisterUserAsync("jane_doe", "jane@example.com");
    }

    private static async Task DIContainerExample()
    {
        var container = new SimpleDIContainer();

        // Register services
        container.RegisterSingleton<ILogger, ConsoleLogger>();
        container.Register<IEmailService, EmailService>();
        container.Register<IDIRepository<Product>, ProductRepository>();
        container.Register<IPaymentGateway, StripePaymentGateway>();
        container.Register<OrderService, OrderService>();

        // Resolve service - container automatically resolves all dependencies
        var orderService = container.Resolve<OrderService>();
        var success = await orderService.ProcessOrderAsync(2, "customer@example.com");
        Console.WriteLine($"DI Container order result: {(success ? "Success" : "Failed")}");

        // Verify singleton behavior
        var logger1 = container.Resolve<ILogger>();
        var logger2 = container.Resolve<ILogger>();
        Console.WriteLine($"Singleton check: {object.ReferenceEquals(logger1, logger2)}");
    }
}

#endregion
