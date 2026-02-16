namespace DesignPatterns.CreationalPatterns;

/// <summary>
/// Product interface that factory will create
/// </summary>
public interface ILogger
{
    void Log(string message);
    LogLevel Level { get; }
}

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error
}

/// <summary>
/// Concrete Products
/// </summary>
public class ConsoleLogger : ILogger
{
    public LogLevel Level { get; }
    
    public ConsoleLogger(LogLevel level = LogLevel.Info)
    {
        Level = level;
    }
    
    public void Log(string message)
    {
        Console.ForegroundColor = Level switch
        {
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Info => ConsoleColor.White,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            _ => ConsoleColor.White
        };
        Console.WriteLine($"[CONSOLE {Level}] {DateTime.Now:HH:mm:ss} - {message}");
        Console.ResetColor();
    }
}

public class FileLogger : ILogger
{
    private readonly string _filePath;
    public LogLevel Level { get; }
    
    public FileLogger(LogLevel level = LogLevel.Info, string filePath = "app.log")
    {
        Level = level;
        _filePath = filePath;
    }
    
    public void Log(string message)
    {
        var logEntry = $"[FILE {Level}] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        Console.WriteLine($"Writing to file: {logEntry}");
        // In real implementation: File.AppendAllText(_filePath, logEntry + Environment.NewLine);
    }
}

public class DatabaseLogger : ILogger
{
    private readonly string _connectionString;
    public LogLevel Level { get; }
    
    public DatabaseLogger(LogLevel level = LogLevel.Warning, string connectionString = "Server=localhost")
    {
        Level = level;
        _connectionString = connectionString;
    }
    
    public void Log(string message)
    {
        Console.WriteLine($"[DATABASE {Level}] Logging to DB: {message}");
        // In real implementation: Insert into database
    }
}

/// <summary>
/// Creator abstract class with Factory Method
/// </summary>
public abstract class LoggerFactory
{
    // Factory Method
    public abstract ILogger CreateLogger();
    
    // Template method that uses the factory method
    public void LogMessage(string message)
    {
        var logger = CreateLogger();
        logger.Log(message);
    }
}

/// <summary>
/// Concrete Creators
/// </summary>
public class ConsoleLoggerFactory : LoggerFactory
{
    private readonly LogLevel _level;
    
    public ConsoleLoggerFactory(LogLevel level = LogLevel.Info)
    {
        _level = level;
    }
    
    public override ILogger CreateLogger()
    {
        return new ConsoleLogger(_level);
    }
}

public class FileLoggerFactory : LoggerFactory
{
    private readonly LogLevel _level;
    private readonly string _filePath;
    
    public FileLoggerFactory(LogLevel level = LogLevel.Info, string filePath = "app.log")
    {
        _level = level;
        _filePath = filePath;
    }
    
    public override ILogger CreateLogger()
    {
        return new FileLogger(_level, _filePath);
    }
}

public class DatabaseLoggerFactory : LoggerFactory
{
    private readonly LogLevel _level;
    
    public DatabaseLoggerFactory(LogLevel level = LogLevel.Warning)
    {
        _level = level;
    }
    
    public override ILogger CreateLogger()
    {
        return new DatabaseLogger(_level);
    }
}

/// <summary>
/// Alternative: Parameterized Factory Method
/// </summary>
public static class LoggerFactorySimple
{
    public static ILogger CreateLogger(string type, LogLevel level = LogLevel.Info)
    {
        return type.ToLower() switch
        {
            "console" => new ConsoleLogger(level),
            "file" => new FileLogger(level),
            "database" => new DatabaseLogger(level),
            _ => throw new ArgumentException($"Unknown logger type: {type}")
        };
    }
}

public static class FactoryMethodDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Factory Method Pattern Demo ===\n");
        
        // Using factory classes
        Console.WriteLine("1. Using Factory Classes:");
        LoggerFactory consoleFactory = new ConsoleLoggerFactory(LogLevel.Info);
        LoggerFactory fileFactory = new FileLoggerFactory(LogLevel.Warning);
        LoggerFactory dbFactory = new DatabaseLoggerFactory(LogLevel.Error);
        
        consoleFactory.LogMessage("Application started");
        fileFactory.LogMessage("Warning: High memory usage");
        dbFactory.LogMessage("Critical error occurred");
        
        // Using parameterized factory
        Console.WriteLine("\n2. Using Parameterized Factory:");
        var logger1 = LoggerFactorySimple.CreateLogger("console", LogLevel.Debug);
        var logger2 = LoggerFactorySimple.CreateLogger("file", LogLevel.Info);
        var logger3 = LoggerFactorySimple.CreateLogger("database", LogLevel.Error);
        
        logger1.Log("Debug message");
        logger2.Log("Info message");
        logger3.Log("Error message");
        
        // Runtime logger selection
        Console.WriteLine("\n3. Runtime Selection:");
        string environment = "production"; // Could come from config
        LoggerFactory factory = environment switch
        {
            "development" => new ConsoleLoggerFactory(LogLevel.Debug),
            "production" => new DatabaseLoggerFactory(LogLevel.Error),
            _ => new FileLoggerFactory(LogLevel.Info)
        };
        
        factory.LogMessage($"Running in {environment} mode");
    }
}
