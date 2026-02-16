namespace DesignPatterns.StructuralPatterns;

/// <summary>
/// Target Interface - What the client expects
/// </summary>
public interface IMediaPlayer
{
    void Play(string fileName);
}

/// <summary>
/// Adaptee - Existing incompatible interface (Advanced Audio Player)
/// </summary>
public class AdvancedAudioPlayer
{
    public void PlayMp4(string fileName)
    {
        Console.WriteLine($"Playing MP4 file: {fileName}");
    }
    
    public void PlayVlc(string fileName)
    {
        Console.WriteLine($"Playing VLC file: {fileName}");
    }
    
    public void PlayMkv(string fileName)
    {
        Console.WriteLine($"Playing MKV file: {fileName}");
    }
}

/// <summary>
/// Adapter - Adapts AdvancedAudioPlayer to IMediaPlayer
/// </summary>
public class MediaAdapter : IMediaPlayer
{
    private readonly AdvancedAudioPlayer _advancedPlayer;
    
    public MediaAdapter()
    {
        _advancedPlayer = new AdvancedAudioPlayer();
    }
    
    public void Play(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();
        
        switch (extension)
        {
            case ".mp4":
                _advancedPlayer.PlayMp4(fileName);
                break;
            case ".vlc":
                _advancedPlayer.PlayVlc(fileName);
                break;
            case ".mkv":
                _advancedPlayer.PlayMkv(fileName);
                break;
            default:
                Console.WriteLine($"Unsupported format: {extension}");
                break;
        }
    }
}

/// <summary>
/// Concrete Implementation - Basic MP3 player
/// </summary>
public class AudioPlayer : IMediaPlayer
{
    public void Play(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();
        
        if (extension == ".mp3")
        {
            Console.WriteLine($"Playing MP3 file: {fileName}");
        }
        else
        {
            // Use adapter for advanced formats
            var adapter = new MediaAdapter();
            adapter.Play(fileName);
        }
    }
}

/// <summary>
/// Alternative Example: Legacy System Adapter
/// </summary>
public interface IPaymentProcessor
{
    void ProcessPayment(decimal amount, string currency);
}

// Legacy system with incompatible interface
public class LegacyPaymentSystem
{
    public void MakePayment(double amountInDollars)
    {
        Console.WriteLine($"Legacy system processing ${amountInDollars:F2}");
    }
}

// Adapter for legacy system
public class PaymentAdapter : IPaymentProcessor
{
    private readonly LegacyPaymentSystem _legacySystem;
    
    public PaymentAdapter()
    {
        _legacySystem = new LegacyPaymentSystem();
    }
    
    public void ProcessPayment(decimal amount, string currency)
    {
        // Convert to format expected by legacy system
        double amountInDollars = currency.ToUpper() switch
        {
            "USD" => (double)amount,
            "EUR" => (double)amount * 1.1, // Example conversion rate
            "GBP" => (double)amount * 1.3,
            _ => (double)amount
        };
        
        _legacySystem.MakePayment(amountInDollars);
    }
}

/// <summary>
/// Object Adapter vs Class Adapter
/// Class adapter using multiple inheritance (not directly supported in C#)
/// Can be simulated with interfaces
/// </summary>
public interface ITarget
{
    void Request();
}

public class Adaptee
{
    public void SpecificRequest()
    {
        Console.WriteLine("Specific request from Adaptee");
    }
}

// Object Adapter (composition)
public class ObjectAdapter : ITarget
{
    private readonly Adaptee _adaptee;
    
    public ObjectAdapter(Adaptee adaptee)
    {
        _adaptee = adaptee;
    }
    
    public void Request()
    {
        _adaptee.SpecificRequest();
    }
}

// Two-way Adapter
public interface INewInterface
{
    void NewMethod();
}

public interface IOldInterface
{
    void OldMethod();
}

public class OldImplementation : IOldInterface
{
    public void OldMethod()
    {
        Console.WriteLine("Old method implementation");
    }
}

public class TwoWayAdapter : INewInterface, IOldInterface
{
    private readonly OldImplementation _oldImpl;
    
    public TwoWayAdapter()
    {
        _oldImpl = new OldImplementation();
    }
    
    public void NewMethod()
    {
        // Adapt new to old
        Console.WriteLine("New method calling old method:");
        _oldImpl.OldMethod();
    }
    
    public void OldMethod()
    {
        // Forward to old implementation
        _oldImpl.OldMethod();
    }
}

public static class AdapterDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Adapter Pattern Demo ===\n");
        
        // Media Player Adapter
        Console.WriteLine("1. Media Player Adapter:");
        var player = new AudioPlayer();
        
        player.Play("song.mp3");
        player.Play("movie.mp4");
        player.Play("video.vlc");
        player.Play("clip.mkv");
        player.Play("file.avi");
        
        // Payment System Adapter
        Console.WriteLine("\n2. Legacy Payment System Adapter:");
        IPaymentProcessor processor = new PaymentAdapter();
        
        processor.ProcessPayment(100m, "USD");
        processor.ProcessPayment(100m, "EUR");
        processor.ProcessPayment(100m, "GBP");
        
        // Object Adapter
        Console.WriteLine("\n3. Object Adapter:");
        var adaptee = new Adaptee();
        ITarget target = new ObjectAdapter(adaptee);
        target.Request();
        
        // Two-way Adapter
        Console.WriteLine("\n4. Two-way Adapter:");
        var twoWay = new TwoWayAdapter();
        
        Console.WriteLine("Using as new interface:");
        INewInterface newInterface = twoWay;
        newInterface.NewMethod();
        
        Console.WriteLine("\nUsing as old interface:");
        IOldInterface oldInterface = twoWay;
        oldInterface.OldMethod();
    }
}
