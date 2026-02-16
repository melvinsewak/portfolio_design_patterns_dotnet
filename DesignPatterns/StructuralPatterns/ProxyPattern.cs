namespace DesignPatterns.StructuralPatterns;

// Example 1: Virtual Proxy - Lazy Loading of Large Images
/// <summary>
/// Subject Interface - Common interface for RealSubject and Proxy
/// </summary>
public interface IImage
{
    void Display();
    string GetInfo();
}

/// <summary>
/// RealSubject - Actual heavy object that loads image from disk
/// </summary>
public class RealImage : IImage
{
    private readonly string _filename;
    private byte[] _imageData = Array.Empty<byte>();
    
    public RealImage(string filename)
    {
        _filename = filename;
        LoadFromDisk();
    }
    
    private void LoadFromDisk()
    {
        Console.WriteLine($"Loading image from disk: {_filename}");
        Thread.Sleep(1000); // Simulate expensive loading operation
        _imageData = new byte[1024 * 1024]; // Simulate 1MB image
        Console.WriteLine($"Image {_filename} loaded successfully ({_imageData.Length} bytes)");
    }
    
    public void Display()
    {
        Console.WriteLine($"Displaying image: {_filename}");
    }
    
    public string GetInfo()
    {
        return $"RealImage: {_filename} ({_imageData.Length} bytes)";
    }
}

/// <summary>
/// Virtual Proxy - Delays loading until actually needed
/// </summary>
public class ImageProxy : IImage
{
    private readonly string _filename;
    private RealImage? _realImage;
    
    public ImageProxy(string filename)
    {
        _filename = filename;
        Console.WriteLine($"ImageProxy created for: {_filename} (not loaded yet)");
    }
    
    public void Display()
    {
        // Lazy initialization - load only when needed
        _realImage ??= new RealImage(_filename);
        _realImage.Display();
    }
    
    public string GetInfo()
    {
        return _realImage != null 
            ? _realImage.GetInfo() 
            : $"ImageProxy: {_filename} (not loaded)";
    }
}

// Example 2: Protection Proxy - Access Control
/// <summary>
/// Subject Interface for Document operations
/// </summary>
public interface IDocument
{
    void View();
    void Edit(string content);
    void Delete();
}

/// <summary>
/// RealSubject - Actual document
/// </summary>
public class Document : IDocument
{
    public string Name { get; }
    public string Content { get; private set; }
    
    public Document(string name, string content)
    {
        Name = name;
        Content = content;
    }
    
    public void View()
    {
        Console.WriteLine($"Viewing document '{Name}':");
        Console.WriteLine($"Content: {Content}");
    }
    
    public void Edit(string content)
    {
        Content = content;
        Console.WriteLine($"Document '{Name}' edited successfully");
    }
    
    public void Delete()
    {
        Console.WriteLine($"Document '{Name}' deleted");
    }
}

/// <summary>
/// User roles for access control
/// </summary>
public enum UserRole
{
    Viewer,
    Editor,
    Admin
}

/// <summary>
/// Protection Proxy - Controls access based on user permissions
/// </summary>
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
        // All roles can view
        Console.WriteLine($"[Access Granted: {_userRole}] ");
        _document.View();
    }
    
    public void Edit(string content)
    {
        if (_userRole == UserRole.Editor || _userRole == UserRole.Admin)
        {
            Console.WriteLine($"[Access Granted: {_userRole}] ");
            _document.Edit(content);
        }
        else
        {
            Console.WriteLine($"[Access Denied: {_userRole}] Insufficient permissions to edit");
        }
    }
    
    public void Delete()
    {
        if (_userRole == UserRole.Admin)
        {
            Console.WriteLine($"[Access Granted: {_userRole}] ");
            _document.Delete();
        }
        else
        {
            Console.WriteLine($"[Access Denied: {_userRole}] Only admins can delete documents");
        }
    }
}

// Example 3: Smart Proxy - Additional functionality (Caching, Logging, Reference Counting)
/// <summary>
/// Subject Interface for Database operations
/// </summary>
public interface IDatabase
{
    string Query(string sql);
    void Execute(string sql);
}

/// <summary>
/// RealSubject - Actual database connection
/// </summary>
public class Database : IDatabase
{
    private readonly string _connectionString;
    
    public Database(string connectionString)
    {
        _connectionString = connectionString;
        Connect();
    }
    
    private void Connect()
    {
        Console.WriteLine($"Connecting to database: {_connectionString}");
        Thread.Sleep(500); // Simulate connection delay
        Console.WriteLine("Database connected");
    }
    
    public string Query(string sql)
    {
        Console.WriteLine($"Executing query: {sql}");
        Thread.Sleep(200); // Simulate query execution
        return $"Result for: {sql}";
    }
    
    public void Execute(string sql)
    {
        Console.WriteLine($"Executing command: {sql}");
        Thread.Sleep(300); // Simulate execution
        Console.WriteLine("Command executed successfully");
    }
}

/// <summary>
/// Smart Proxy - Adds caching, logging, and connection management
/// </summary>
public class SmartDatabaseProxy : IDatabase
{
    private Database? _database;
    private readonly string _connectionString;
    private readonly Dictionary<string, string> _cache = new();
    private int _referenceCount = 0;
    
    public SmartDatabaseProxy(string connectionString)
    {
        _connectionString = connectionString;
        Console.WriteLine("SmartDatabaseProxy created (lazy connection)");
    }
    
    private Database GetDatabase()
    {
        if (_database == null)
        {
            _database = new Database(_connectionString);
            _referenceCount++;
            Console.WriteLine($"Reference count: {_referenceCount}");
        }
        return _database;
    }
    
    public string Query(string sql)
    {
        // Check cache first
        if (_cache.ContainsKey(sql))
        {
            Console.WriteLine($"[CACHE HIT] Returning cached result for: {sql}");
            return _cache[sql];
        }
        
        // Log the query
        LogQuery(sql);
        
        // Execute query
        var result = GetDatabase().Query(sql);
        
        // Cache the result
        _cache[sql] = result;
        Console.WriteLine($"[CACHED] Query result stored in cache");
        
        return result;
    }
    
    public void Execute(string sql)
    {
        // Log the execution
        LogExecution(sql);
        
        // Clear cache on data modification
        if (sql.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase) ||
            sql.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase) ||
            sql.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("[CACHE] Clearing cache due to data modification");
            _cache.Clear();
        }
        
        GetDatabase().Execute(sql);
    }
    
    private void LogQuery(string sql)
    {
        Console.WriteLine($"[LOG] Query at {DateTime.Now:HH:mm:ss}: {sql}");
    }
    
    private void LogExecution(string sql)
    {
        Console.WriteLine($"[LOG] Execution at {DateTime.Now:HH:mm:ss}: {sql}");
    }
    
    public void Dispose()
    {
        if (_database != null)
        {
            _referenceCount--;
            Console.WriteLine($"Reference count: {_referenceCount}");
            if (_referenceCount == 0)
            {
                Console.WriteLine("Closing database connection");
                _database = null;
            }
        }
    }
}

// Example 4: Remote Proxy - Represents object in different address space
/// <summary>
/// Remote service interface
/// </summary>
public interface IRemoteService
{
    string GetData(int id);
    bool UpdateData(int id, string data);
}

/// <summary>
/// Remote Service (simulated as local for demo)
/// </summary>
public class RemoteService : IRemoteService
{
    private readonly Dictionary<int, string> _dataStore = new()
    {
        { 1, "Data 1" },
        { 2, "Data 2" },
        { 3, "Data 3" }
    };
    
    public string GetData(int id)
    {
        Console.WriteLine($"[REMOTE] Fetching data for ID: {id}");
        Thread.Sleep(500); // Simulate network latency
        return _dataStore.GetValueOrDefault(id, "Not found");
    }
    
    public bool UpdateData(int id, string data)
    {
        Console.WriteLine($"[REMOTE] Updating data for ID: {id}");
        Thread.Sleep(500); // Simulate network latency
        _dataStore[id] = data;
        return true;
    }
}

/// <summary>
/// Remote Proxy - Handles communication with remote service
/// </summary>
public class RemoteServiceProxy : IRemoteService
{
    private RemoteService? _remoteService;
    private readonly Dictionary<int, string> _localCache = new();
    
    private RemoteService GetRemoteService()
    {
        if (_remoteService == null)
        {
            Console.WriteLine("[PROXY] Establishing connection to remote service...");
            Thread.Sleep(1000); // Simulate connection establishment
            _remoteService = new RemoteService();
            Console.WriteLine("[PROXY] Connection established");
        }
        return _remoteService;
    }
    
    public string GetData(int id)
    {
        // Check local cache
        if (_localCache.ContainsKey(id))
        {
            Console.WriteLine($"[PROXY CACHE] Returning cached data for ID: {id}");
            return _localCache[id];
        }
        
        // Fetch from remote
        Console.WriteLine($"[PROXY] Forwarding request to remote service...");
        var data = GetRemoteService().GetData(id);
        
        // Cache locally
        _localCache[id] = data;
        
        return data;
    }
    
    public bool UpdateData(int id, string data)
    {
        Console.WriteLine($"[PROXY] Forwarding update to remote service...");
        var result = GetRemoteService().UpdateData(id, data);
        
        // Invalidate cache
        if (result && _localCache.ContainsKey(id))
        {
            Console.WriteLine($"[PROXY] Invalidating cache for ID: {id}");
            _localCache.Remove(id);
        }
        
        return result;
    }
}

/// <summary>
/// Demonstration class for Proxy Pattern
/// </summary>
public static class ProxyDemo
{
    public static void Run()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║            PROXY PATTERN DEMONSTRATION                     ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");
        
        // Example 1: Virtual Proxy - Lazy Loading
        Console.WriteLine("--- Example 1: Virtual Proxy (Lazy Loading) ---\n");
        
        IImage image1 = new ImageProxy("photo1.jpg");
        IImage image2 = new ImageProxy("photo2.jpg");
        IImage image3 = new ImageProxy("photo3.jpg");
        
        Console.WriteLine("\nAll proxies created. Images not loaded yet.");
        Console.WriteLine($"Image 1 info: {image1.GetInfo()}");
        Console.WriteLine($"Image 2 info: {image2.GetInfo()}\n");
        
        Console.WriteLine("Now displaying image 1 (this triggers loading):");
        image1.Display();
        
        Console.WriteLine($"\nImage 1 info after display: {image1.GetInfo()}");
        Console.WriteLine("\nDisplaying image 1 again (already loaded):");
        image1.Display();
        
        // Example 2: Protection Proxy - Access Control
        Console.WriteLine("\n\n--- Example 2: Protection Proxy (Access Control) ---\n");
        
        var document = new Document("Confidential Report", "Sensitive information here");
        
        Console.WriteLine("Viewer attempting operations:");
        IDocument viewerProxy = new DocumentProxy(document, UserRole.Viewer);
        viewerProxy.View();
        viewerProxy.Edit("Modified content");
        viewerProxy.Delete();
        
        Console.WriteLine("\nEditor attempting operations:");
        IDocument editorProxy = new DocumentProxy(document, UserRole.Editor);
        editorProxy.View();
        editorProxy.Edit("Modified by editor");
        editorProxy.Delete();
        
        Console.WriteLine("\nAdmin attempting operations:");
        IDocument adminProxy = new DocumentProxy(document, UserRole.Admin);
        adminProxy.View();
        adminProxy.Edit("Modified by admin");
        adminProxy.Delete();
        
        // Example 3: Smart Proxy - Caching and Logging
        Console.WriteLine("\n\n--- Example 3: Smart Proxy (Caching & Logging) ---\n");
        
        var dbProxy = new SmartDatabaseProxy("Server=localhost;Database=TestDB");
        
        Console.WriteLine("First query (cache miss):");
        var result1 = dbProxy.Query("SELECT * FROM Users");
        Console.WriteLine($"Result: {result1}\n");
        
        Console.WriteLine("Same query again (cache hit):");
        var result2 = dbProxy.Query("SELECT * FROM Users");
        Console.WriteLine($"Result: {result2}\n");
        
        Console.WriteLine("Different query (cache miss):");
        var result3 = dbProxy.Query("SELECT * FROM Orders");
        Console.WriteLine($"Result: {result3}\n");
        
        Console.WriteLine("Modifying data (clears cache):");
        dbProxy.Execute("INSERT INTO Users VALUES (1, 'John')");
        
        Console.WriteLine("\nQuerying after modification (cache cleared):");
        var result4 = dbProxy.Query("SELECT * FROM Users");
        Console.WriteLine($"Result: {result4}");
        
        // Example 4: Remote Proxy
        Console.WriteLine("\n\n--- Example 4: Remote Proxy (Network Communication) ---\n");
        
        IRemoteService remoteProxy = new RemoteServiceProxy();
        
        Console.WriteLine("First request (establishes connection + fetches data):");
        var data1 = remoteProxy.GetData(1);
        Console.WriteLine($"Received: {data1}\n");
        
        Console.WriteLine("Second request for same data (cache hit):");
        var data2 = remoteProxy.GetData(1);
        Console.WriteLine($"Received: {data2}\n");
        
        Console.WriteLine("Request for different data:");
        var data3 = remoteProxy.GetData(2);
        Console.WriteLine($"Received: {data3}\n");
        
        Console.WriteLine("Updating remote data:");
        remoteProxy.UpdateData(1, "Updated Data 1");
        
        Console.WriteLine("\nFetching updated data (cache invalidated):");
        var data4 = remoteProxy.GetData(1);
        Console.WriteLine($"Received: {data4}");
        
        Console.WriteLine("\n" + new string('═', 62));
        Console.WriteLine("Key Benefits Demonstrated:");
        Console.WriteLine("• Virtual Proxy: Delayed object creation until needed");
        Console.WriteLine("• Protection Proxy: Access control and permissions");
        Console.WriteLine("• Smart Proxy: Caching, logging, reference counting");
        Console.WriteLine("• Remote Proxy: Network communication abstraction");
        Console.WriteLine(new string('═', 62));
    }
}
