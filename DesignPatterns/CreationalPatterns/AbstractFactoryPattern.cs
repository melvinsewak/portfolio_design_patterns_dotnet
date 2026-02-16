namespace DesignPatterns.CreationalPatterns;

/// <summary>
/// Abstract Products - UI Components
/// </summary>
public interface IButton
{
    void Render();
    void Click();
}

public interface ICheckbox
{
    void Render();
    void Toggle();
}

public interface ITextBox
{
    void Render();
    void SetText(string text);
}

/// <summary>
/// Concrete Products - Windows UI
/// </summary>
public class WindowsButton : IButton
{
    public void Render() => Console.WriteLine("Rendering Windows style button");
    public void Click() => Console.WriteLine("Windows button clicked");
}

public class WindowsCheckbox : ICheckbox
{
    public void Render() => Console.WriteLine("Rendering Windows style checkbox");
    public void Toggle() => Console.WriteLine("Windows checkbox toggled");
}

public class WindowsTextBox : ITextBox
{
    public void Render() => Console.WriteLine("Rendering Windows style textbox");
    public void SetText(string text) => Console.WriteLine($"Windows textbox text: {text}");
}

/// <summary>
/// Concrete Products - Mac UI
/// </summary>
public class MacButton : IButton
{
    public void Render() => Console.WriteLine("Rendering Mac style button");
    public void Click() => Console.WriteLine("Mac button clicked");
}

public class MacCheckbox : ICheckbox
{
    public void Render() => Console.WriteLine("Rendering Mac style checkbox");
    public void Toggle() => Console.WriteLine("Mac checkbox toggled");
}

public class MacTextBox : ITextBox
{
    public void Render() => Console.WriteLine("Rendering Mac style textbox");
    public void SetText(string text) => Console.WriteLine($"Mac textbox text: {text}");
}

/// <summary>
/// Concrete Products - Linux UI
/// </summary>
public class LinuxButton : IButton
{
    public void Render() => Console.WriteLine("Rendering Linux style button");
    public void Click() => Console.WriteLine("Linux button clicked");
}

public class LinuxCheckbox : ICheckbox
{
    public void Render() => Console.WriteLine("Rendering Linux style checkbox");
    public void Toggle() => Console.WriteLine("Linux checkbox toggled");
}

public class LinuxTextBox : ITextBox
{
    public void Render() => Console.WriteLine("Rendering Linux style textbox");
    public void SetText(string text) => Console.WriteLine($"Linux textbox text: {text}");
}

/// <summary>
/// Abstract Factory Interface
/// </summary>
public interface IUIFactory
{
    IButton CreateButton();
    ICheckbox CreateCheckbox();
    ITextBox CreateTextBox();
}

/// <summary>
/// Concrete Factories
/// </summary>
public class WindowsUIFactory : IUIFactory
{
    public IButton CreateButton() => new WindowsButton();
    public ICheckbox CreateCheckbox() => new WindowsCheckbox();
    public ITextBox CreateTextBox() => new WindowsTextBox();
}

public class MacUIFactory : IUIFactory
{
    public IButton CreateButton() => new MacButton();
    public ICheckbox CreateCheckbox() => new MacCheckbox();
    public ITextBox CreateTextBox() => new MacTextBox();
}

public class LinuxUIFactory : IUIFactory
{
    public IButton CreateButton() => new LinuxButton();
    public ICheckbox CreateCheckbox() => new LinuxCheckbox();
    public ITextBox CreateTextBox() => new LinuxTextBox();
}

/// <summary>
/// Client code that uses the abstract factory
/// </summary>
public class Application
{
    private readonly IButton _button;
    private readonly ICheckbox _checkbox;
    private readonly ITextBox _textBox;
    
    public Application(IUIFactory factory)
    {
        _button = factory.CreateButton();
        _checkbox = factory.CreateCheckbox();
        _textBox = factory.CreateTextBox();
    }
    
    public void RenderUI()
    {
        Console.WriteLine("\n--- Rendering Application UI ---");
        _button.Render();
        _checkbox.Render();
        _textBox.Render();
    }
    
    public void InteractWithUI()
    {
        Console.WriteLine("\n--- User Interactions ---");
        _button.Click();
        _checkbox.Toggle();
        _textBox.SetText("Hello, Abstract Factory!");
    }
}

/// <summary>
/// Alternative example: Database Access Factory
/// </summary>
public interface IDatabaseConnection
{
    void Open();
    void Close();
}

public interface IDatabaseCommand
{
    void Execute(string sql);
}

public interface IDatabaseFactory
{
    IDatabaseConnection CreateConnection();
    IDatabaseCommand CreateCommand();
}

public class SqlServerConnection : IDatabaseConnection
{
    public void Open() => Console.WriteLine("Opening SQL Server connection");
    public void Close() => Console.WriteLine("Closing SQL Server connection");
}

public class SqlServerCommand : IDatabaseCommand
{
    public void Execute(string sql) => Console.WriteLine($"Executing SQL Server: {sql}");
}

public class PostgreSqlConnection : IDatabaseConnection
{
    public void Open() => Console.WriteLine("Opening PostgreSQL connection");
    public void Close() => Console.WriteLine("Closing PostgreSQL connection");
}

public class PostgreSqlCommand : IDatabaseCommand
{
    public void Execute(string sql) => Console.WriteLine($"Executing PostgreSQL: {sql}");
}

public class SqlServerFactory : IDatabaseFactory
{
    public IDatabaseConnection CreateConnection() => new SqlServerConnection();
    public IDatabaseCommand CreateCommand() => new SqlServerCommand();
}

public class PostgreSqlFactory : IDatabaseFactory
{
    public IDatabaseConnection CreateConnection() => new PostgreSqlConnection();
    public IDatabaseCommand CreateCommand() => new PostgreSqlCommand();
}

public static class AbstractFactoryDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Abstract Factory Pattern Demo ===\n");
        
        // Determine platform at runtime
        var platform = GetPlatform();
        Console.WriteLine($"Detected platform: {platform}");
        
        IUIFactory factory = platform switch
        {
            "Windows" => new WindowsUIFactory(),
            "Mac" => new MacUIFactory(),
            "Linux" => new LinuxUIFactory(),
            _ => new WindowsUIFactory()
        };
        
        var app = new Application(factory);
        app.RenderUI();
        app.InteractWithUI();
        
        // Database example
        Console.WriteLine("\n=== Database Factory Example ===\n");
        
        string dbType = "PostgreSQL"; // Could come from configuration
        IDatabaseFactory dbFactory = dbType switch
        {
            "SQLServer" => new SqlServerFactory(),
            "PostgreSQL" => new PostgreSqlFactory(),
            _ => new SqlServerFactory()
        };
        
        var connection = dbFactory.CreateConnection();
        var command = dbFactory.CreateCommand();
        
        connection.Open();
        command.Execute("SELECT * FROM Users");
        connection.Close();
    }
    
    private static string GetPlatform()
    {
        if (OperatingSystem.IsWindows()) return "Windows";
        if (OperatingSystem.IsMacOS()) return "Mac";
        if (OperatingSystem.IsLinux()) return "Linux";
        return "Unknown";
    }
}
