namespace DesignPatterns.CreationalPatterns;

/// <summary>
/// Thread-safe Singleton implementation using Lazy<T>
/// Ensures only one instance of a class exists throughout the application lifecycle
/// </summary>
public sealed class Singleton
{
    private static readonly Lazy<Singleton> _instance = new Lazy<Singleton>(() => new Singleton());
    
    private Singleton()
    {
        InstanceId = Guid.NewGuid();
        CreationTime = DateTime.UtcNow;
        Console.WriteLine($"Singleton instance created at {CreationTime}");
    }
    
    public static Singleton Instance => _instance.Value;
    
    public Guid InstanceId { get; }
    public DateTime CreationTime { get; }
    
    public void DoWork()
    {
        Console.WriteLine($"Singleton working with ID: {InstanceId}");
    }
}

/// <summary>
/// Alternative: Double-checked locking Singleton
/// Provides more control over initialization
/// </summary>
public sealed class SingletonDoubleChecked
{
    private static SingletonDoubleChecked? _instance;
    private static readonly object _lock = new object();
    
    private SingletonDoubleChecked()
    {
        InstanceId = Guid.NewGuid();
    }
    
    public static SingletonDoubleChecked Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SingletonDoubleChecked();
                    }
                }
            }
            return _instance;
        }
    }
    
    public Guid InstanceId { get; }
}

/// <summary>
/// Example: Configuration Manager using Singleton
/// </summary>
public sealed class ConfigurationManager
{
    private static readonly Lazy<ConfigurationManager> _instance = 
        new Lazy<ConfigurationManager>(() => new ConfigurationManager());
    
    private readonly Dictionary<string, string> _settings;
    
    private ConfigurationManager()
    {
        _settings = new Dictionary<string, string>
        {
            { "AppName", "Design Patterns Demo" },
            { "Version", "1.0.0" },
            { "Environment", "Development" }
        };
    }
    
    public static ConfigurationManager Instance => _instance.Value;
    
    public string GetSetting(string key)
    {
        return _settings.TryGetValue(key, out var value) ? value : string.Empty;
    }
    
    public void SetSetting(string key, string value)
    {
        _settings[key] = value;
    }
}

public static class SingletonDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Singleton Pattern Demo ===\n");
        
        // Basic Singleton
        var instance1 = Singleton.Instance;
        var instance2 = Singleton.Instance;
        
        Console.WriteLine($"Instance 1 ID: {instance1.InstanceId}");
        Console.WriteLine($"Instance 2 ID: {instance2.InstanceId}");
        Console.WriteLine($"Are they same? {ReferenceEquals(instance1, instance2)}");
        
        instance1.DoWork();
        
        // Configuration Manager Example
        Console.WriteLine("\nConfiguration Manager:");
        var config = ConfigurationManager.Instance;
        Console.WriteLine($"App Name: {config.GetSetting("AppName")}");
        Console.WriteLine($"Version: {config.GetSetting("Version")}");
        
        config.SetSetting("Environment", "Production");
        var config2 = ConfigurationManager.Instance;
        Console.WriteLine($"Environment (from second reference): {config2.GetSetting("Environment")}");
    }
}
