namespace DesignPatterns.StructuralPatterns;

// Decorator Pattern: Attach additional responsibilities to an object dynamically

#region Example 1: Coffee Shop

// Component interface
public abstract class Beverage
{
    public abstract string GetDescription();
    public abstract decimal GetCost();
}

// Concrete Component
public class Espresso : Beverage
{
    public override string GetDescription() => "Espresso";
    public override decimal GetCost() => 1.99m;
}

public class DarkRoast : Beverage
{
    public override string GetDescription() => "Dark Roast Coffee";
    public override decimal GetCost() => 2.49m;
}

public class Decaf : Beverage
{
    public override string GetDescription() => "Decaf Coffee";
    public override decimal GetCost() => 2.29m;
}

// Base Decorator
public abstract class BeverageDecorator : Beverage
{
    protected Beverage beverage;

    protected BeverageDecorator(Beverage beverage)
    {
        this.beverage = beverage;
    }
}

// Concrete Decorators
public class Milk : BeverageDecorator
{
    public Milk(Beverage beverage) : base(beverage) { }

    public override string GetDescription() => $"{beverage.GetDescription()}, Milk";
    public override decimal GetCost() => beverage.GetCost() + 0.50m;
}

public class Mocha : BeverageDecorator
{
    public Mocha(Beverage beverage) : base(beverage) { }

    public override string GetDescription() => $"{beverage.GetDescription()}, Mocha";
    public override decimal GetCost() => beverage.GetCost() + 0.70m;
}

public class Whip : BeverageDecorator
{
    public Whip(Beverage beverage) : base(beverage) { }

    public override string GetDescription() => $"{beverage.GetDescription()}, Whipped Cream";
    public override decimal GetCost() => beverage.GetCost() + 0.60m;
}

public class Soy : BeverageDecorator
{
    public Soy(Beverage beverage) : base(beverage) { }

    public override string GetDescription() => $"{beverage.GetDescription()}, Soy";
    public override decimal GetCost() => beverage.GetCost() + 0.55m;
}

public class CaramelSyrup : BeverageDecorator
{
    public CaramelSyrup(Beverage beverage) : base(beverage) { }

    public override string GetDescription() => $"{beverage.GetDescription()}, Caramel Syrup";
    public override decimal GetCost() => beverage.GetCost() + 0.65m;
}

#endregion

#region Example 2: Text Formatting

// Component interface for text
public interface IText
{
    string GetContent();
    void Display();
}

// Concrete Component
public class PlainText : IText
{
    private readonly string content;

    public PlainText(string content)
    {
        this.content = content;
    }

    public string GetContent() => content;

    public void Display()
    {
        Console.WriteLine(content);
    }
}

// Base Decorator
public abstract class TextDecorator : IText
{
    protected IText text;

    protected TextDecorator(IText text)
    {
        this.text = text;
    }

    public abstract string GetContent();
    public abstract void Display();
}

// Concrete Decorators
public class BoldText : TextDecorator
{
    public BoldText(IText text) : base(text) { }

    public override string GetContent() => $"**{text.GetContent()}**";

    public override void Display()
    {
        Console.Write("\x1b[1m"); // ANSI bold
        text.Display();
        Console.Write("\x1b[0m"); // ANSI reset
    }
}

public class ItalicText : TextDecorator
{
    public ItalicText(IText text) : base(text) { }

    public override string GetContent() => $"_{text.GetContent()}_";

    public override void Display()
    {
        Console.Write("\x1b[3m"); // ANSI italic
        text.Display();
        Console.Write("\x1b[0m"); // ANSI reset
    }
}

public class UnderlinedText : TextDecorator
{
    public UnderlinedText(IText text) : base(text) { }

    public override string GetContent() => $"__{text.GetContent()}__";

    public override void Display()
    {
        Console.Write("\x1b[4m"); // ANSI underline
        text.Display();
        Console.Write("\x1b[0m"); // ANSI reset
    }
}

public class ColoredText : TextDecorator
{
    private readonly string colorCode;

    public ColoredText(IText text, string colorCode) : base(text)
    {
        this.colorCode = colorCode;
    }

    public override string GetContent() => $"[{colorCode}]{text.GetContent()}[/]";

    public override void Display()
    {
        Console.ForegroundColor = colorCode switch
        {
            "red" => ConsoleColor.Red,
            "green" => ConsoleColor.Green,
            "blue" => ConsoleColor.Blue,
            "yellow" => ConsoleColor.Yellow,
            "cyan" => ConsoleColor.Cyan,
            _ => ConsoleColor.White
        };
        text.Display();
        Console.ResetColor();
    }
}

public class QuotedText : TextDecorator
{
    public QuotedText(IText text) : base(text) { }

    public override string GetContent() => $"\"{text.GetContent()}\"";

    public override void Display()
    {
        Console.Write("\"");
        text.Display();
        Console.Write("\"");
    }
}

#endregion

#region Example 3: Data Stream Processing

// Component interface for data streams
public interface IDataStream
{
    byte[] Read();
    void Write(byte[] data);
    string GetDescription();
}

// Concrete Component
public class FileDataStream : IDataStream
{
    private readonly string filename;
    private byte[] data = Array.Empty<byte>();

    public FileDataStream(string filename)
    {
        this.filename = filename;
    }

    public byte[] Read()
    {
        Console.WriteLine($"Reading from file: {filename}");
        return data;
    }

    public void Write(byte[] data)
    {
        Console.WriteLine($"Writing to file: {filename}");
        this.data = data;
    }

    public string GetDescription() => $"File: {filename}";
}

// Base Decorator
public abstract class DataStreamDecorator : IDataStream
{
    protected IDataStream stream;

    protected DataStreamDecorator(IDataStream stream)
    {
        this.stream = stream;
    }

    public abstract byte[] Read();
    public abstract void Write(byte[] data);
    public abstract string GetDescription();
}

// Concrete Decorators
public class CompressionDecorator : DataStreamDecorator
{
    public CompressionDecorator(IDataStream stream) : base(stream) { }

    public override byte[] Read()
    {
        var data = stream.Read();
        Console.WriteLine("Decompressing data...");
        return Decompress(data);
    }

    public override void Write(byte[] data)
    {
        Console.WriteLine("Compressing data...");
        var compressed = Compress(data);
        stream.Write(compressed);
    }

    public override string GetDescription() => $"{stream.GetDescription()} + Compression";

    private byte[] Compress(byte[] data)
    {
        // Simulate compression (in reality, use GZipStream or similar)
        Console.WriteLine($"  Original size: {data.Length} bytes");
        var compressed = data.Take(data.Length / 2).ToArray(); // Mock compression
        Console.WriteLine($"  Compressed size: {compressed.Length} bytes");
        return compressed;
    }

    private byte[] Decompress(byte[] data)
    {
        // Simulate decompression
        Console.WriteLine($"  Compressed size: {data.Length} bytes");
        var decompressed = data.Concat(data).ToArray(); // Mock decompression
        Console.WriteLine($"  Decompressed size: {decompressed.Length} bytes");
        return decompressed;
    }
}

public class EncryptionDecorator : DataStreamDecorator
{
    private readonly string key;

    public EncryptionDecorator(IDataStream stream, string key) : base(stream)
    {
        this.key = key;
    }

    public override byte[] Read()
    {
        var data = stream.Read();
        Console.WriteLine($"Decrypting data with key: {key}");
        return Decrypt(data);
    }

    public override void Write(byte[] data)
    {
        Console.WriteLine($"Encrypting data with key: {key}");
        var encrypted = Encrypt(data);
        stream.Write(encrypted);
    }

    public override string GetDescription() => $"{stream.GetDescription()} + Encryption({key})";

    private byte[] Encrypt(byte[] data)
    {
        // Simulate encryption (in reality, use AES or similar)
        Console.WriteLine($"  Encrypting {data.Length} bytes...");
        return data.Select(b => (byte)(b ^ key[0])).ToArray(); // Simple XOR for demo
    }

    private byte[] Decrypt(byte[] data)
    {
        // Simulate decryption
        Console.WriteLine($"  Decrypting {data.Length} bytes...");
        return data.Select(b => (byte)(b ^ key[0])).ToArray(); // Simple XOR for demo
    }
}

public class BufferingDecorator : DataStreamDecorator
{
    private readonly int bufferSize;

    public BufferingDecorator(IDataStream stream, int bufferSize) : base(stream)
    {
        this.bufferSize = bufferSize;
    }

    public override byte[] Read()
    {
        Console.WriteLine($"Reading with buffer size: {bufferSize} bytes");
        return stream.Read();
    }

    public override void Write(byte[] data)
    {
        Console.WriteLine($"Writing with buffer size: {bufferSize} bytes");
        stream.Write(data);
    }

    public override string GetDescription() => $"{stream.GetDescription()} + Buffering({bufferSize})";
}

public class LoggingDecorator : DataStreamDecorator
{
    public LoggingDecorator(IDataStream stream) : base(stream) { }

    public override byte[] Read()
    {
        Console.WriteLine($"[LOG] Reading from: {stream.GetDescription()}");
        var data = stream.Read();
        Console.WriteLine($"[LOG] Read {data.Length} bytes");
        return data;
    }

    public override void Write(byte[] data)
    {
        Console.WriteLine($"[LOG] Writing to: {stream.GetDescription()}");
        Console.WriteLine($"[LOG] Writing {data.Length} bytes");
        stream.Write(data);
        Console.WriteLine($"[LOG] Write complete");
    }

    public override string GetDescription() => $"{stream.GetDescription()} + Logging";
}

#endregion

// Demo class
public static class DecoratorDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Decorator Pattern Demo ===\n");

        // Example 1: Coffee Shop
        Console.WriteLine("--- Example 1: Coffee Shop ---");
        
        // Simple espresso
        Beverage beverage1 = new Espresso();
        Console.WriteLine($"{beverage1.GetDescription()} - ${beverage1.GetCost():F2}");

        // Dark roast with mocha and whip
        Beverage beverage2 = new DarkRoast();
        beverage2 = new Mocha(beverage2);
        beverage2 = new Whip(beverage2);
        Console.WriteLine($"{beverage2.GetDescription()} - ${beverage2.GetCost():F2}");

        // Decaf with multiple add-ons
        Beverage beverage3 = new Decaf();
        beverage3 = new Soy(beverage3);
        beverage3 = new Mocha(beverage3);
        beverage3 = new Whip(beverage3);
        beverage3 = new CaramelSyrup(beverage3);
        Console.WriteLine($"{beverage3.GetDescription()} - ${beverage3.GetCost():F2}");

        // Example 2: Text Formatting
        Console.WriteLine("\n--- Example 2: Text Formatting ---");
        
        IText text1 = new PlainText("Hello, World!");
        Console.Write("Plain: ");
        Console.WriteLine(text1.GetContent());

        IText text2 = new BoldText(new PlainText("Important Message"));
        Console.Write("Bold (markdown): ");
        Console.WriteLine(text2.GetContent());

        IText text3 = new ItalicText(new BoldText(new PlainText("Emphasized")));
        Console.Write("Bold + Italic (markdown): ");
        Console.WriteLine(text3.GetContent());

        IText text4 = new QuotedText(new UnderlinedText(new PlainText("Famous Quote")));
        Console.Write("Underlined + Quoted (markdown): ");
        Console.WriteLine(text4.GetContent());

        IText text5 = new ColoredText(new BoldText(new PlainText("Colored Bold Text")), "green");
        Console.Write("Colored display: ");
        text5.Display();
        Console.WriteLine();

        // Example 3: Data Stream Processing
        Console.WriteLine("\n--- Example 3: Data Stream Processing ---");
        
        // Simple file stream
        IDataStream stream1 = new FileDataStream("document.txt");
        Console.WriteLine($"\nStream 1: {stream1.GetDescription()}");
        stream1.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

        // File with compression
        IDataStream stream2 = new CompressionDecorator(new FileDataStream("compressed.dat"));
        Console.WriteLine($"\nStream 2: {stream2.GetDescription()}");
        stream2.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

        // File with encryption and compression
        IDataStream stream3 = new EncryptionDecorator(
            new CompressionDecorator(new FileDataStream("secure.dat")),
            "secret123"
        );
        Console.WriteLine($"\nStream 3: {stream3.GetDescription()}");
        stream3.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

        // File with all features: logging, buffering, encryption, compression
        IDataStream stream4 = new LoggingDecorator(
            new BufferingDecorator(
                new EncryptionDecorator(
                    new CompressionDecorator(new FileDataStream("full-featured.dat")),
                    "key456"
                ),
                1024
            )
        );
        Console.WriteLine($"\nStream 4: {stream4.GetDescription()}");
        stream4.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
        Console.WriteLine();
        stream4.Read();

        Console.WriteLine("\n=== Decorator Pattern Demo Complete ===");
    }
}
