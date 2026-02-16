# Proxy Pattern

## Intent
The Proxy pattern provides a surrogate or placeholder object to control access to another object. The proxy acts as an intermediary, adding a level of indirection to support controlled access, lazy initialization, logging, caching, or access control.

## Problem
Direct access to objects sometimes isn't desirable or possible due to various constraints:

### Issues Without Proxy Pattern:
- **Resource Intensive Objects**: Creating expensive objects when they might not be used
- **No Access Control**: Can't restrict or monitor object access
- **Remote Objects**: Complex network communication handling
- **No Caching**: Expensive operations repeated unnecessarily
- **No Logging**: Difficult to track object usage
- **Thread Safety**: Can't add synchronization to existing objects
- **No Cleanup**: Can't add reference counting or resource cleanup

### Real-World Analogy:
Think of a credit card as a proxy for your bank account. Instead of carrying cash (the real object), you use a card (proxy) that:
- Controls access (requires PIN)
- Logs transactions
- Provides security
- Works remotely (ATMs worldwide)
- Adds convenience (delay actual bank account access)

The credit card provides all the same operations (payments) but adds control and features.

## Solution
The Proxy pattern creates a proxy class with the same interface as the real object. The proxy:
1. **Holds a reference** to the real object
2. **Controls access** to the real object
3. **May add functionality** (caching, logging, lazy loading, etc.)
4. **Delegates work** to the real object when appropriate

Types of proxies:
- **Virtual Proxy**: Lazy initialization of expensive objects
- **Protection Proxy**: Access control and permissions
- **Remote Proxy**: Represents objects in different address spaces
- **Smart Proxy**: Additional functionality (caching, logging, reference counting)

## Structure

```
        Client
           |
           | uses
           ↓
      <<interface>>
        Subject ←──────────────┐
           △                   │
           |                   |
           | implements        | implements
           |                   |
    ┌──────┴──────┐           |
    |             |           |
RealSubject     Proxy ────────┘
                  |
                  | holds reference to
                  ↓
              RealSubject
```

### Detailed ASCII UML Diagram:

```
┌─────────────────────────────────────────────────────────────┐
│                         Client                              │
├─────────────────────────────────────────────────────────────┤
│ - subject: Subject                                          │
├─────────────────────────────────────────────────────────────┤
│ + DoSomething()                                             │
│   {                                                         │
│     subject.Request()  // Works with Subject interface      │
│   }                                                         │
└────────────┬────────────────────────────────────────────────┘
             │ uses
             ↓
┌─────────────────────────────────────────────────────────────┐
│              <<interface>> Subject                          │
├─────────────────────────────────────────────────────────────┤
│ + Request()                                                 │
└────────────┬───────────────────────────┬────────────────────┘
             △                           △
             │ implements                │ implements
             │                           │
┌────────────┴──────────┐   ┌───────────┴────────────────────┐
│    RealSubject        │   │         Proxy                  │
├───────────────────────┤   ├────────────────────────────────┤
│                       │   │ - realSubject: RealSubject     │
├───────────────────────┤   ├────────────────────────────────┤
│ + Request()           │   │ + Request()                    │
│   {                   │   │   {                            │
│     // Do actual work │   │     // Additional logic        │
│   }                   │   │     CheckAccess()              │
└───────────────────────┘   │     if (realSubject == null)   │
                            │       realSubject = new...     │
                            │     realSubject.Request()      │
                            │     Log()                      │
                            │   }                            │
                            └────────────────────────────────┘
```

## When to Use

### Use Proxy Pattern When:
- ✅ Need lazy initialization (Virtual Proxy) - delay object creation
- ✅ Need access control (Protection Proxy) - restrict access based on permissions
- ✅ Need local representative for remote object (Remote Proxy)
- ✅ Need smart reference with additional functionality
- ✅ Want to add logging, caching, or monitoring to existing object
- ✅ Need to control expensive operations
- ✅ Want to add thread-safety to existing object

### Avoid Proxy Pattern When:
- ❌ Direct access to object is simple and sufficient
- ❌ Proxy adds unnecessary complexity without benefits
- ❌ Performance overhead of extra indirection is unacceptable
- ❌ The interface is unstable or changes frequently

## Real-World Examples

### 1. **Virtual Proxy - Image Loading** (UI/Graphics)
```
Proxy: ImageProxy
Real Object: RealImage (loaded from disk)
Benefit: Don't load images until displayed, saving memory
```

### 2. **Protection Proxy - Document Access** (Security)
```
Proxy: DocumentProxy
Real Object: Document
Benefit: Check user permissions before allowing operations
```

### 3. **Smart Proxy - Database Connection** (Data Access)
```
Proxy: SmartDatabaseProxy
Real Object: Database
Benefit: Connection pooling, caching, logging, lazy connection
```

### 4. **Remote Proxy - Web Service** (Networking)
```
Proxy: ServiceProxy
Real Object: Remote web service
Benefit: Handle network communication, serialization, retry logic
```

### 5. **.NET Examples**
```
- HttpClient: Proxy for HTTP communication
- WCF Service Proxies: Remote service access
- Entity Framework: Lazy loading proxies
- IIS: Reverse proxy for web applications
```

## Implementation Examples

### 1. Virtual Proxy (Lazy Loading)
```csharp
public interface IImage
{
    void Display();
}

public class RealImage : IImage
{
    private readonly string _filename;
    
    public RealImage(string filename)
    {
        _filename = filename;
        LoadFromDisk(); // Expensive operation
    }
    
    private void LoadFromDisk()
    {
        Console.WriteLine($"Loading {_filename} from disk...");
        Thread.Sleep(1000); // Simulate loading
    }
    
    public void Display()
    {
        Console.WriteLine($"Displaying {_filename}");
    }
}

public class ImageProxy : IImage
{
    private readonly string _filename;
    private RealImage? _realImage;
    
    public ImageProxy(string filename)
    {
        _filename = filename;
        // Don't load yet - lazy initialization
    }
    
    public void Display()
    {
        // Load only when needed
        _realImage ??= new RealImage(_filename);
        _realImage.Display();
    }
}

// Usage
IImage image = new ImageProxy("photo.jpg"); // Fast - not loaded yet
// ... later ...
image.Display(); // Now it loads
image.Display(); // Already loaded - fast
```

### 2. Protection Proxy (Access Control)
```csharp
public interface IDocument
{
    void View();
    void Edit(string content);
    void Delete();
}

public class Document : IDocument
{
    public void View() { /* ... */ }
    public void Edit(string content) { /* ... */ }
    public void Delete() { /* ... */ }
}

public class DocumentProxy : IDocument
{
    private readonly Document _document;
    private readonly UserRole _userRole;
    
    public DocumentProxy(Document document, UserRole userRole)
    {
        _document = document;
        _userRole = userRole;
    }
    
    public void View()
    {
        _document.View(); // Everyone can view
    }
    
    public void Edit(string content)
    {
        if (_userRole == UserRole.Editor || _userRole == UserRole.Admin)
            _document.Edit(content);
        else
            throw new UnauthorizedAccessException("No edit permission");
    }
    
    public void Delete()
    {
        if (_userRole == UserRole.Admin)
            _document.Delete();
        else
            throw new UnauthorizedAccessException("Only admins can delete");
    }
}
```

### 3. Smart Proxy (Caching + Logging)
```csharp
public interface IDatabase
{
    string Query(string sql);
}

public class SmartDatabaseProxy : IDatabase
{
    private Database? _database;
    private readonly Dictionary<string, string> _cache = new();
    
    public string Query(string sql)
    {
        // Logging
        Log($"Query: {sql}");
        
        // Caching
        if (_cache.ContainsKey(sql))
        {
            Log("Cache hit!");
            return _cache[sql];
        }
        
        // Lazy initialization
        _database ??= new Database();
        
        // Execute and cache
        var result = _database.Query(sql);
        _cache[sql] = result;
        
        return result;
    }
    
    private void Log(string message) 
    { 
        Console.WriteLine($"[LOG] {message}"); 
    }
}
```

## Advantages

1. **Controlled Access**: Can restrict access to real object
2. **Lazy Initialization**: Create expensive objects only when needed
3. **Added Functionality**: Logging, caching, access control without modifying real object
4. **Location Transparency**: Hide whether object is local or remote
5. **Open/Closed Principle**: Add features without changing real object
6. **Resource Management**: Better control over resource usage
7. **Security**: Add authentication and authorization
8. **Performance**: Can add caching to improve performance

## Disadvantages

1. **Complexity**: Adds extra layer of indirection
2. **Performance Overhead**: Extra method calls and checks
3. **Maintenance**: Another class to maintain
4. **Response Delay**: Especially for remote proxies (network latency)
5. **Memory Overhead**: Proxy object takes memory
6. **Debugging**: Harder to debug with extra layer

## Best Practices

### Design Guidelines:
1. **Same Interface**: Proxy must implement same interface as real object
2. **Transparent**: Client shouldn't know if using proxy or real object
3. **Single Responsibility**: Each proxy type has one responsibility
4. **Composition**: Use composition, not inheritance
5. **Lazy Initialization**: For virtual proxies, delay creation until needed
6. **Thread Safety**: Make proxies thread-safe if needed

### Implementation Tips:
```csharp
// ✅ GOOD: Proxy implements same interface
public interface IService
{
    void DoWork();
}

public class ServiceProxy : IService
{
    private IService _realService;
    
    public void DoWork()
    {
        // Pre-processing
        _realService ??= new RealService();
        _realService.DoWork();
        // Post-processing
    }
}

// ❌ BAD: Different interface
public class ServiceProxy  // No interface!
{
    private RealService _realService;
    
    public void DoSomethingDifferent() // Different method!
    {
        _realService.DoWork();
    }
}

// ✅ GOOD: Thread-safe lazy initialization
public class ThreadSafeProxy : IService
{
    private readonly Lazy<IService> _service = 
        new Lazy<IService>(() => new RealService());
    
    public void DoWork()
    {
        _service.Value.DoWork();
    }
}

// ❌ BAD: Not thread-safe
public class NotThreadSafeProxy : IService
{
    private IService _service;
    
    public void DoWork()
    {
        if (_service == null)  // Race condition!
            _service = new RealService();
        _service.DoWork();
    }
}
```

## C# Specific Features

### 1. **Lazy<T> for Virtual Proxy**
```csharp
public class ImageProxy : IImage
{
    private readonly Lazy<RealImage> _image;
    
    public ImageProxy(string filename)
    {
        _image = new Lazy<RealImage>(() => new RealImage(filename));
    }
    
    public void Display() => _image.Value.Display();
}
```

### 2. **DispatchProxy for Dynamic Proxies**
```csharp
public class LoggingProxy<T> : DispatchProxy
{
    private T _target;
    
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        Console.WriteLine($"Calling {targetMethod?.Name}");
        var result = targetMethod?.Invoke(_target, args);
        Console.WriteLine($"Completed {targetMethod?.Name}");
        return result;
    }
    
    public static T Create(T target)
    {
        var proxy = Create<T, LoggingProxy<T>>() as LoggingProxy<T>;
        proxy._target = target;
        return (T)(object)proxy;
    }
}

// Usage
var service = LoggingProxy<IService>.Create(new RealService());
service.DoWork(); // Automatically logged!
```

### 3. **Async/Await for Remote Proxy**
```csharp
public class AsyncRemoteProxy : IService
{
    private readonly HttpClient _httpClient = new();
    
    public async Task<string> GetDataAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/data/{id}");
        return await response.Content.ReadAsStringAsync();
    }
}
```

### 4. **Using Declarations for Resource Management**
```csharp
public class ResourceProxy : IDisposable
{
    private Resource? _resource;
    
    public void UseResource()
    {
        _resource ??= new Resource();
        _resource.Use();
    }
    
    public void Dispose()
    {
        _resource?.Dispose();
    }
}

// Usage
using var proxy = new ResourceProxy();
proxy.UseResource();
// Automatically disposed
```

### 5. **ValueTask for Performance**
```csharp
public class HighPerformanceProxy : IService
{
    private string? _cachedResult;
    
    public ValueTask<string> GetResultAsync()
    {
        // Return cached synchronously if available
        if (_cachedResult != null)
            return new ValueTask<string>(_cachedResult);
        
        // Otherwise fetch asynchronously
        return new ValueTask<string>(FetchResultAsync());
    }
    
    private async Task<string> FetchResultAsync()
    {
        _cachedResult = await RealService.GetResultAsync();
        return _cachedResult;
    }
}
```

### 6. **Record Types for Immutable Proxies**
```csharp
public record ImageMetadata(string Filename, int Width, int Height);

public class ImageMetadataProxy
{
    private ImageMetadata? _metadata;
    
    public ImageMetadata GetMetadata(string filename)
    {
        return _metadata ??= LoadMetadata(filename);
    }
}
```

## Related Patterns

### **Proxy vs Adapter**
- **Proxy**: Same interface, controls access
- **Adapter**: Different interface, converts interface
- Proxy doesn't change interface; Adapter does

### **Proxy vs Decorator**
- **Proxy**: Controls access, may not create real object
- **Decorator**: Adds behavior, always wraps real object
- Proxy manages object lifecycle; Decorator enhances functionality

### **Proxy vs Facade**
- **Proxy**: Single object surrogate (1:1 relationship)
- **Facade**: Simplifies multiple objects (1:many relationship)
- Proxy same interface as target; Facade different interface

### Complementary Patterns:
- **Factory**: Create appropriate proxy type
- **Singleton**: Proxy factory often singleton
- **Strategy**: Different proxy strategies
- **Composite**: Proxy can be composite member

## Common Use Cases

1. **Lazy Loading**: ORM proxies (Entity Framework, NHibernate)
2. **Caching**: Cache frequently accessed data
3. **Logging**: Log method calls and parameters
4. **Access Control**: Permission checking
5. **Remote Communication**: WCF, gRPC, REST clients
6. **Resource Management**: Connection pooling, file handles
7. **Monitoring**: Performance metrics, usage statistics
8. **Validation**: Input validation before forwarding

## Proxy Types Comparison

| Proxy Type | Purpose | Example | When to Use |
|------------|---------|---------|-------------|
| **Virtual** | Lazy initialization | Image loading | Expensive object creation |
| **Protection** | Access control | Document permissions | Security requirements |
| **Remote** | Network communication | Web service client | Distributed systems |
| **Smart** | Additional functionality | Caching, logging | Need extra features |
| **Copy-on-Write** | Optimize copying | Large data structures | Frequent reads, rare writes |
| **Cache** | Store results | Query results | Expensive computations |

## Testing Considerations

```csharp
[TestFixture]
public class ProxyTests
{
    [Test]
    public void VirtualProxy_DelaysCreation_UntilFirstUse()
    {
        // Arrange
        var proxy = new ImageProxy("test.jpg");
        
        // Act & Assert - Real object not created yet
        Assert.IsNull(GetPrivateField(proxy, "_realImage"));
        
        proxy.Display(); // Triggers creation
        
        Assert.IsNotNull(GetPrivateField(proxy, "_realImage"));
    }
    
    [Test]
    public void ProtectionProxy_DeniesAccess_ForUnauthorizedUsers()
    {
        // Arrange
        var document = new Document("test");
        var proxy = new DocumentProxy(document, UserRole.Viewer);
        
        // Act & Assert
        Assert.DoesNotThrow(() => proxy.View());
        Assert.Throws<UnauthorizedAccessException>(() => proxy.Edit("new content"));
    }
    
    [Test]
    public void SmartProxy_CachesResults()
    {
        // Arrange
        var mock = new Mock<IDatabase>();
        mock.Setup(db => db.Query(It.IsAny<string>())).Returns("result");
        var proxy = new SmartDatabaseProxy(mock.Object);
        
        // Act
        proxy.Query("SELECT * FROM Users");
        proxy.Query("SELECT * FROM Users"); // Same query
        
        // Assert - Real service called only once (cached second time)
        mock.Verify(db => db.Query(It.IsAny<string>()), Times.Once);
    }
}
```

## Performance Considerations

```csharp
// Virtual Proxy - Memory Savings
// Without proxy: 1000 images × 1MB = 1GB loaded immediately
// With proxy: Load only visible images, e.g., 10 × 1MB = 10MB

// Caching Proxy - Speed Improvement
[Benchmark]
public void DirectAccess()
{
    for (int i = 0; i < 1000; i++)
        _database.Query("SELECT * FROM Users"); // 1000 DB calls
}

[Benchmark]
public void ProxyWithCache()
{
    for (int i = 0; i < 1000; i++)
        _proxy.Query("SELECT * FROM Users"); // 1 DB call + 999 cache hits
}

// Results:
// DirectAccess:    5000ms
// ProxyWithCache:    50ms (100x faster!)
```

## Conclusion

The Proxy pattern is a versatile structural pattern that provides controlled access to objects. Whether you need lazy initialization, access control, remote communication, or additional functionality like caching and logging, proxies offer a clean solution without modifying the original object. The pattern is particularly valuable in enterprise applications dealing with expensive resources, security, or distributed systems.

**Key Takeaway**: Use Proxy to add control and functionality around object access. Choose the right proxy type (Virtual, Protection, Remote, Smart) based on your specific needs.
