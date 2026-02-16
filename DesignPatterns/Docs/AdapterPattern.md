# Adapter Pattern

## Intent
The Adapter pattern converts the interface of a class into another interface that clients expect. It allows classes with incompatible interfaces to work together by wrapping an object with an adapter that translates calls to the expected interface.

## Problem
When integrating existing classes, libraries, or third-party components, you often encounter interface incompatibility issues:

### Issues Without Adapter Pattern:
- **Interface Mismatch**: Existing class has different interface than what client expects
- **Cannot Modify Source**: Third-party libraries or legacy code can't be changed
- **Multiple Implementations**: Different implementations with different interfaces
- **Tight Coupling**: Client code tightly coupled to specific implementation
- **Code Duplication**: Repeating conversion logic throughout codebase
- **Integration Challenges**: Difficult to integrate incompatible systems

### Real-World Analogy:
Think of a power adapter when traveling internationally. Your US laptop charger (Adaptee) has a two-prong plug, but European outlets (Client) expect a different plug type. The power adapter (Adapter) converts the interface without changing the charger or outlet, allowing them to work together.

## Solution
The Adapter pattern creates an adapter class that:
1. **Implements the target interface** that the client expects
2. **Wraps the adaptee** (the incompatible class)
3. **Translates calls** from the target interface to the adaptee's interface
4. **Allows reuse** of existing functionality without modification

There are two types of adapters:
- **Object Adapter**: Uses composition (recommended in C#)
- **Class Adapter**: Uses multiple inheritance (not possible in C#)

## Structure

```
Client ──────► <<interface>> Target
                     △
                     │ implements
                     │
                  Adapter ──────► Adaptee
                     │            (incompatible
                     │             interface)
                     │
                 translates
                  calls to
                  Adaptee
```

### Detailed ASCII UML Diagram (Object Adapter):

```
┌─────────────────────────────────────────────────────────────┐
│                        Client                               │
├─────────────────────────────────────────────────────────────┤
│ - target: ITarget                                           │
├─────────────────────────────────────────────────────────────┤
│ + DoSomething()                                             │
│   {                                                         │
│     target.Request()  // Expects ITarget interface          │
│   }                                                         │
└────────────┬────────────────────────────────────────────────┘
             │ uses
             ↓
┌─────────────────────────────────────────────────────────────┐
│               <<interface>> ITarget                         │
├─────────────────────────────────────────────────────────────┤
│ + Request()                                                 │
└────────────┬────────────────────────────────────────────────┘
             △
             │ implements
             │
┌────────────┴────────────────────────────────────────────────┐
│                      Adapter                                │
├─────────────────────────────────────────────────────────────┤
│ - adaptee: Adaptee                                          │
├─────────────────────────────────────────────────────────────┤
│ + Request()                                                 │
│   {                                                         │
│     // Translate to adaptee's interface                     │
│     adaptee.SpecificRequest()                               │
│   }                                                         │
└────────────┬────────────────────────────────────────────────┘
             │ contains/uses
             ↓
┌─────────────────────────────────────────────────────────────┐
│                      Adaptee                                │
├─────────────────────────────────────────────────────────────┤
│ + SpecificRequest()                                         │
│   {                                                         │
│     // Existing implementation                              │
│     // Different interface than ITarget                     │
│   }                                                         │
└─────────────────────────────────────────────────────────────┘
```

## When to Use

### Use Adapter Pattern When:
- ✅ You want to use an existing class with an incompatible interface
- ✅ You need to integrate third-party libraries or legacy code
- ✅ You want to create reusable classes that work with unrelated classes
- ✅ Multiple existing classes need to be used but have different interfaces
- ✅ You can't modify the source code of the adaptee
- ✅ You need to decouple client from specific implementations

### Avoid Adapter Pattern When:
- ❌ You can modify the original class to match expected interface
- ❌ The interface differences are minimal or trivial
- ❌ It's simpler to change client code than create adapter
- ❌ The pattern adds unnecessary complexity

## Real-World Examples

### 1. **Media Player** (Entertainment)
```
Target: IMediaPlayer (expects Play(filename))
Adaptee: AdvancedAudioPlayer (has PlayMp4(), PlayVlc(), PlayMkv())
Adapter: MediaAdapter (converts Play() to specific format methods)
```

### 2. **Payment Gateway Integration** (Finance)
```
Target: IPaymentProcessor (expects ProcessPayment(amount))
Adaptees: PayPal, Stripe, Square (each with different APIs)
Adapters: PayPalAdapter, StripeAdapter, SquareAdapter
```

### 3. **Database Drivers** (Data Access)
```
Target: IDbConnection (standard .NET interface)
Adaptees: MySQL, PostgreSQL, Oracle native drivers
Adapters: MySqlConnection, NpgsqlConnection, OracleConnection
```

### 4. **Logging Frameworks** (Infrastructure)
```
Target: ILogger (application logging interface)
Adaptees: Log4Net, NLog, Serilog (different logging libraries)
Adapters: Log4NetAdapter, NLogAdapter, SerilogAdapter
```

### 5. **.NET Framework Examples**
```
- StreamReader/StreamWriter: Adapt streams to text reading/writing
- XmlReader/XmlWriter: Adapt different XML sources to common interface
- DbDataAdapter: Adapts database results to DataSet
- Collection adapters: Array to IList, IEnumerable adapters
```

## Implementation Example

```csharp
// Target Interface - What client expects
public interface IMediaPlayer
{
    void Play(string filename);
}

// Adaptee - Existing incompatible interface
public class AdvancedAudioPlayer
{
    public void PlayMp4(string filename)
    {
        Console.WriteLine($"Playing MP4: {filename}");
    }
    
    public void PlayVlc(string filename)
    {
        Console.WriteLine($"Playing VLC: {filename}");
    }
    
    public void PlayMkv(string filename)
    {
        Console.WriteLine($"Playing MKV: {filename}");
    }
}

// Adapter - Converts AdvancedAudioPlayer to IMediaPlayer
public class MediaAdapter : IMediaPlayer
{
    private readonly AdvancedAudioPlayer _advancedPlayer;
    
    public MediaAdapter()
    {
        _advancedPlayer = new AdvancedAudioPlayer();
    }
    
    public void Play(string filename)
    {
        string extension = Path.GetExtension(filename).ToLower();
        
        switch (extension)
        {
            case ".mp4":
                _advancedPlayer.PlayMp4(filename);
                break;
            case ".vlc":
                _advancedPlayer.PlayVlc(filename);
                break;
            case ".mkv":
                _advancedPlayer.PlayMkv(filename);
                break;
            default:
                throw new NotSupportedException($"Format {extension} not supported");
        }
    }
}

// Client - Works with IMediaPlayer interface
public class AudioPlayer : IMediaPlayer
{
    private readonly MediaAdapter _adapter;
    
    public AudioPlayer()
    {
        _adapter = new MediaAdapter();
    }
    
    public void Play(string filename)
    {
        string extension = Path.GetExtension(filename).ToLower();
        
        if (extension == ".mp3")
        {
            Console.WriteLine($"Playing MP3: {filename}");
        }
        else
        {
            // Delegate to adapter for other formats
            _adapter.Play(filename);
        }
    }
}

// Usage
IMediaPlayer player = new AudioPlayer();
player.Play("song.mp3");     // Native support
player.Play("movie.mp4");    // Via adapter
player.Play("video.mkv");    // Via adapter
```

## Advantages

1. **Single Responsibility**: Separates interface conversion from business logic
2. **Open/Closed Principle**: Add new adapters without changing existing code
3. **Reusability**: Reuse existing classes without modification
4. **Flexibility**: Can work with multiple incompatible classes
5. **Decoupling**: Client decoupled from specific implementations
6. **Legacy Integration**: Easily integrate legacy code
7. **Third-Party Libraries**: Wrap external libraries with custom interface
8. **Testability**: Easy to mock adapted interface

## Disadvantages

1. **Complexity**: Adds extra classes and indirection
2. **Performance**: Extra layer may impact performance slightly
3. **Maintenance**: More classes to maintain
4. **Over-Engineering**: May be overkill for simple conversions
5. **Learning Curve**: Developers must understand adapter pattern
6. **Debugging**: Extra layer can complicate debugging

## Best Practices

### Design Guidelines:
1. **Prefer Object Adapter**: Use composition over inheritance
2. **Single Responsibility**: Each adapter should adapt one interface
3. **Minimize Logic**: Keep adapters thin, focus on translation
4. **Interface Segregation**: Don't force adapters to implement unused methods
5. **Dependency Injection**: Inject adaptees for better testability
6. **Clear Naming**: Name adapters clearly (e.g., PayPalAdapter)

### Implementation Tips:
```csharp
// ✅ GOOD: Object adapter with composition
public class PayPalAdapter : IPaymentProcessor
{
    private readonly PayPalApi _payPalApi;
    
    public PayPalAdapter(PayPalApi payPalApi)
    {
        _payPalApi = payPalApi;
    }
    
    public void ProcessPayment(decimal amount)
    {
        // Simple translation
        _payPalApi.MakePayment(amount.ToString("C"));
    }
}

// ❌ BAD: Adapter with business logic (should be elsewhere)
public class PayPalAdapter : IPaymentProcessor
{
    public void ProcessPayment(decimal amount)
    {
        // BAD: Business logic in adapter!
        if (amount < 0)
            throw new ArgumentException("Amount must be positive");
        
        if (amount > 10000)
            SendFraudAlert();
        
        // Translation should be the only responsibility
    }
}

// ✅ GOOD: Two-way adapter
public class LegacySystemAdapter : IModernInterface, ILegacyInterface
{
    private readonly LegacySystem _legacy;
    private readonly ModernSystem _modern;
    
    // Adapt modern to legacy
    public void LegacyMethod()
    {
        _modern.ModernMethod();
    }
    
    // Adapt legacy to modern
    public void ModernMethod()
    {
        _legacy.LegacyMethod();
    }
}
```

## C# Specific Features

### 1. **Extension Methods as Adapters**
```csharp
// Adapt existing types without creating adapter classes
public static class StringExtensions
{
    public static int ToInteger(this string str)
    {
        return int.Parse(str); // Adapts string to int
    }
}

// Usage
string text = "123";
int number = text.ToInteger(); // Looks like native method
```

### 2. **Implicit/Explicit Operators**
```csharp
public class TemperatureCelsius
{
    public double Degrees { get; set; }
    
    // Adapter as operator
    public static implicit operator TemperatureFahrenheit(TemperatureCelsius c)
    {
        return new TemperatureFahrenheit { Degrees = c.Degrees * 9 / 5 + 32 };
    }
}

// Usage
TemperatureCelsius celsius = new() { Degrees = 25 };
TemperatureFahrenheit fahrenheit = celsius; // Automatic adaptation!
```

### 3. **Generic Adapters**
```csharp
public interface IRepository<T>
{
    T GetById(int id);
    void Save(T entity);
}

// Generic adapter for any entity type
public class LegacyRepositoryAdapter<T> : IRepository<T>
{
    private readonly LegacyDataAccess _legacy;
    
    public T GetById(int id)
    {
        var data = _legacy.FetchRecord(typeof(T).Name, id);
        return (T)Convert.ChangeType(data, typeof(T));
    }
    
    public void Save(T entity)
    {
        _legacy.SaveRecord(typeof(T).Name, entity);
    }
}
```

### 4. **Async Adapters**
```csharp
public interface IAsyncDataService
{
    Task<string> GetDataAsync(int id);
}

public class SyncToAsyncAdapter : IAsyncDataService
{
    private readonly ISyncDataService _syncService;
    
    public SyncToAsyncAdapter(ISyncDataService syncService)
    {
        _syncService = syncService;
    }
    
    public Task<string> GetDataAsync(int id)
    {
        // Adapt synchronous to asynchronous
        return Task.Run(() => _syncService.GetData(id));
    }
}
```

### 5. **LINQ Adapters**
```csharp
public class DataTableAdapter
{
    private readonly DataTable _dataTable;
    
    public IEnumerable<T> AsEnumerable<T>() where T : class, new()
    {
        // Adapt DataTable to IEnumerable<T>
        return _dataTable.Rows.Cast<DataRow>()
            .Select(row => MapToObject<T>(row));
    }
    
    private T MapToObject<T>(DataRow row) where T : class, new()
    {
        var obj = new T();
        // Map properties...
        return obj;
    }
}
```

### 6. **Dependency Injection**
```csharp
// Register adapters in DI container
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Register adaptee
        services.AddSingleton<PayPalApi>();
        
        // Register adapter
        services.AddSingleton<IPaymentProcessor, PayPalAdapter>();
        
        // Different adapters for different implementations
        services.AddKeyedSingleton<IPaymentProcessor, PayPalAdapter>("paypal");
        services.AddKeyedSingleton<IPaymentProcessor, StripeAdapter>("stripe");
    }
}
```

## Related Patterns

### **Adapter vs Bridge**
- **Adapter**: Makes existing interfaces work together (designed after systems exist)
- **Bridge**: Decouples abstraction from implementation (designed upfront)
- Adapter retrofits; Bridge prevents coupling

### **Adapter vs Decorator**
- **Adapter**: Changes interface
- **Decorator**: Keeps same interface, adds functionality
- Adapter converts; Decorator enhances

### **Adapter vs Facade**
- **Adapter**: Makes one interface look like another
- **Facade**: Provides simplified interface to complex subsystem
- Adapter converts interface; Facade simplifies interface

### **Adapter vs Proxy**
- **Adapter**: Different interface
- **Proxy**: Same interface
- Adapter changes interface; Proxy controls access

### Complementary Patterns:
- **Factory**: Create appropriate adapters
- **Bridge**: Adapter can be used to implement bridge
- **Composite**: Adapt individual objects to work in composite
- **Strategy**: Different adapters as different strategies

## Common Use Cases

1. **Legacy System Integration**: Wrap old code with modern interface
2. **Third-Party Library Integration**: Adapt external APIs
3. **Database Abstraction**: Uniform interface for different databases
4. **Payment Gateways**: Adapt different payment providers
5. **Logging Frameworks**: Uniform logging interface
6. **Message Queue Systems**: Adapt RabbitMQ, Kafka, Azure Service Bus
7. **File Formats**: Adapt XML, JSON, CSV readers to common interface
8. **API Versioning**: Adapt old API versions to new interface

## Multiple Adaptees Example

```csharp
// One adapter for multiple payment gateways
public class UnifiedPaymentAdapter : IPaymentProcessor
{
    private readonly string _provider;
    private readonly PayPalApi? _payPal;
    private readonly StripeApi? _stripe;
    
    public UnifiedPaymentAdapter(string provider)
    {
        _provider = provider;
        
        switch (provider.ToLower())
        {
            case "paypal":
                _payPal = new PayPalApi();
                break;
            case "stripe":
                _stripe = new StripeApi();
                break;
            default:
                throw new ArgumentException("Unknown provider");
        }
    }
    
    public void ProcessPayment(decimal amount)
    {
        switch (_provider.ToLower())
        {
            case "paypal":
                _payPal?.MakePayment(amount.ToString("C"));
                break;
            case "stripe":
                _stripe?.ChargeCard((int)(amount * 100)); // Stripe uses cents
                break;
        }
    }
}
```

## Testing Considerations

```csharp
[TestFixture]
public class MediaAdapterTests
{
    [Test]
    public void Play_Mp4File_CallsCorrectMethod()
    {
        // Arrange
        var mockAdvancedPlayer = new Mock<AdvancedAudioPlayer>();
        var adapter = new MediaAdapter(mockAdvancedPlayer.Object);
        
        // Act
        adapter.Play("movie.mp4");
        
        // Assert
        mockAdvancedPlayer.Verify(p => p.PlayMp4("movie.mp4"), Times.Once);
        mockAdvancedPlayer.Verify(p => p.PlayVlc(It.IsAny<string>()), Times.Never);
    }
    
    [Test]
    public void Play_UnsupportedFormat_ThrowsException()
    {
        // Arrange
        var adapter = new MediaAdapter();
        
        // Act & Assert
        Assert.Throws<NotSupportedException>(() => adapter.Play("audio.wav"));
    }
}
```

## Performance Considerations

The Adapter pattern introduces minimal overhead:
- One extra method call (negligible)
- One additional object allocation (minimal memory)
- Translation logic (depends on complexity)

```csharp
// Efficient adapter - minimal overhead
public class FastAdapter : ITarget
{
    private readonly Adaptee _adaptee;
    
    public void Request()
    {
        _adaptee.SpecificRequest(); // Direct delegation
    }
}

// Less efficient - complex translation
public class SlowAdapter : ITarget
{
    public void Request()
    {
        // Heavy translation logic
        var data = ParseData();
        var transformed = TransformData(data);
        var validated = ValidateData(transformed);
        _adaptee.ComplexRequest(validated);
    }
}
```

## Conclusion

The Adapter pattern is essential for integrating incompatible interfaces, particularly when working with legacy code, third-party libraries, or multiple implementations. By providing a clean translation layer, it enables code reuse and maintains loose coupling between components. The pattern is widely used in .NET framework itself and is a fundamental tool for building flexible, maintainable systems.

**Key Takeaway**: Use Adapter to make existing classes work with incompatible interfaces without modifying their source code. It's the bridge between what you have and what you need.
