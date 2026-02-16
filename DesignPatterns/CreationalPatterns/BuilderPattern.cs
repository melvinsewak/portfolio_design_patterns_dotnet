namespace DesignPatterns.CreationalPatterns;

/// <summary>
/// Product - Complex object being built
/// </summary>
public class Computer
{
    public string? CPU { get; set; }
    public string? GPU { get; set; }
    public int RAM { get; set; }
    public int Storage { get; set; }
    public string? StorageType { get; set; }
    public string? Motherboard { get; set; }
    public string? PowerSupply { get; set; }
    public string? CoolingSystem { get; set; }
    public List<string> Extras { get; set; } = new();
    
    public override string ToString()
    {
        return $"""
            Computer Specifications:
            - CPU: {CPU}
            - GPU: {GPU ?? "Integrated"}
            - RAM: {RAM}GB
            - Storage: {Storage}GB {StorageType}
            - Motherboard: {Motherboard}
            - Power Supply: {PowerSupply}
            - Cooling: {CoolingSystem}
            - Extras: {string.Join(", ", Extras)}
            """;
    }
}

/// <summary>
/// Builder Interface
/// </summary>
public interface IComputerBuilder
{
    IComputerBuilder SetCPU(string cpu);
    IComputerBuilder SetGPU(string gpu);
    IComputerBuilder SetRAM(int ram);
    IComputerBuilder SetStorage(int storage, string type);
    IComputerBuilder SetMotherboard(string motherboard);
    IComputerBuilder SetPowerSupply(string powerSupply);
    IComputerBuilder SetCooling(string cooling);
    IComputerBuilder AddExtra(string extra);
    Computer Build();
}

/// <summary>
/// Concrete Builder
/// </summary>
public class ComputerBuilder : IComputerBuilder
{
    private readonly Computer _computer = new();
    
    public IComputerBuilder SetCPU(string cpu)
    {
        _computer.CPU = cpu;
        return this;
    }
    
    public IComputerBuilder SetGPU(string gpu)
    {
        _computer.GPU = gpu;
        return this;
    }
    
    public IComputerBuilder SetRAM(int ram)
    {
        _computer.RAM = ram;
        return this;
    }
    
    public IComputerBuilder SetStorage(int storage, string type)
    {
        _computer.Storage = storage;
        _computer.StorageType = type;
        return this;
    }
    
    public IComputerBuilder SetMotherboard(string motherboard)
    {
        _computer.Motherboard = motherboard;
        return this;
    }
    
    public IComputerBuilder SetPowerSupply(string powerSupply)
    {
        _computer.PowerSupply = powerSupply;
        return this;
    }
    
    public IComputerBuilder SetCooling(string cooling)
    {
        _computer.CoolingSystem = cooling;
        return this;
    }
    
    public IComputerBuilder AddExtra(string extra)
    {
        _computer.Extras.Add(extra);
        return this;
    }
    
    public Computer Build()
    {
        // Validation
        if (string.IsNullOrEmpty(_computer.CPU))
            throw new InvalidOperationException("CPU is required");
        if (_computer.RAM <= 0)
            throw new InvalidOperationException("RAM is required");
        if (_computer.Storage <= 0)
            throw new InvalidOperationException("Storage is required");
            
        return _computer;
    }
}

/// <summary>
/// Director - Knows how to build specific configurations
/// </summary>
public class ComputerDirector
{
    public Computer BuildGamingPC(IComputerBuilder builder)
    {
        return builder
            .SetCPU("AMD Ryzen 9 7950X")
            .SetGPU("NVIDIA RTX 4090")
            .SetRAM(64)
            .SetStorage(2000, "NVMe SSD")
            .SetMotherboard("ASUS ROG Crosshair X670E")
            .SetPowerSupply("1000W 80+ Platinum")
            .SetCooling("Liquid Cooling")
            .AddExtra("RGB Lighting")
            .AddExtra("Tempered Glass Case")
            .Build();
    }
    
    public Computer BuildOfficePC(IComputerBuilder builder)
    {
        return builder
            .SetCPU("Intel Core i5-13600")
            .SetRAM(16)
            .SetStorage(512, "SSD")
            .SetMotherboard("ASUS Prime B760")
            .SetPowerSupply("500W 80+ Bronze")
            .SetCooling("Air Cooling")
            .Build();
    }
    
    public Computer BuildWorkstation(IComputerBuilder builder)
    {
        return builder
            .SetCPU("Intel Xeon W-2295")
            .SetGPU("NVIDIA RTX A5000")
            .SetRAM(128)
            .SetStorage(4000, "NVMe SSD")
            .SetMotherboard("ASUS WS C621E SAGE")
            .SetPowerSupply("1200W 80+ Titanium")
            .SetCooling("Advanced Liquid Cooling")
            .AddExtra("ECC Memory")
            .AddExtra("Redundant Power Supply")
            .Build();
    }
}

/// <summary>
/// Alternative: Fluent Builder without Director
/// </summary>
public record HttpRequest
{
    public string? Url { get; init; }
    public string Method { get; init; } = "GET";
    public Dictionary<string, string> Headers { get; init; } = new();
    public string? Body { get; init; }
    public int Timeout { get; init; } = 30;
}

public class HttpRequestBuilder
{
    private string? _url;
    private string _method = "GET";
    private readonly Dictionary<string, string> _headers = new();
    private string? _body;
    private int _timeout = 30;
    
    public HttpRequestBuilder WithUrl(string url)
    {
        _url = url;
        return this;
    }
    
    public HttpRequestBuilder WithMethod(string method)
    {
        _method = method;
        return this;
    }
    
    public HttpRequestBuilder WithHeader(string key, string value)
    {
        _headers[key] = value;
        return this;
    }
    
    public HttpRequestBuilder WithBody(string body)
    {
        _body = body;
        return this;
    }
    
    public HttpRequestBuilder WithTimeout(int seconds)
    {
        _timeout = seconds;
        return this;
    }
    
    public HttpRequest Build()
    {
        if (string.IsNullOrEmpty(_url))
            throw new InvalidOperationException("URL is required");
            
        return new HttpRequest
        {
            Url = _url,
            Method = _method,
            Headers = new Dictionary<string, string>(_headers),
            Body = _body,
            Timeout = _timeout
        };
    }
}

public static class BuilderDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Builder Pattern Demo ===\n");
        
        // Using Director
        Console.WriteLine("1. Using Director for Predefined Configurations:");
        var director = new ComputerDirector();
        var builder = new ComputerBuilder();
        
        var gamingPC = director.BuildGamingPC(builder);
        Console.WriteLine(gamingPC);
        
        var officePC = director.BuildOfficePC(new ComputerBuilder());
        Console.WriteLine(officePC);
        
        // Using Builder directly (without Director)
        Console.WriteLine("\n2. Custom Build without Director:");
        var customPC = new ComputerBuilder()
            .SetCPU("AMD Ryzen 7 7700X")
            .SetGPU("AMD Radeon RX 7900 XTX")
            .SetRAM(32)
            .SetStorage(1000, "NVMe SSD")
            .SetMotherboard("MSI MAG B650 TOMAHAWK")
            .SetPowerSupply("850W 80+ Gold")
            .SetCooling("AIO Liquid Cooling")
            .AddExtra("WiFi 6E")
            .Build();
        
        Console.WriteLine(customPC);
        
        // HTTP Request Builder Example
        Console.WriteLine("\n3. HTTP Request Builder:");
        var request = new HttpRequestBuilder()
            .WithUrl("https://api.example.com/users")
            .WithMethod("POST")
            .WithHeader("Content-Type", "application/json")
            .WithHeader("Authorization", "Bearer token123")
            .WithBody("{\"name\": \"John Doe\"}")
            .WithTimeout(60)
            .Build();
        
        Console.WriteLine($"Request: {request.Method} {request.Url}");
        Console.WriteLine($"Headers: {request.Headers.Count}");
        Console.WriteLine($"Timeout: {request.Timeout}s");
    }
}
