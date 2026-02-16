namespace DesignPatterns.StructuralPatterns;

// Bridge Pattern: Separates abstraction from implementation so both can vary independently

#region Example 1: Shape Drawing with Different APIs

// Implementation interface
public interface IDrawingAPI
{
    void DrawCircle(double x, double y, double radius);
    void DrawRectangle(double x, double y, double width, double height);
}

// Concrete Implementation 1: Vector Graphics
public class VectorGraphicsAPI : IDrawingAPI
{
    public void DrawCircle(double x, double y, double radius)
    {
        Console.WriteLine($"[Vector] Drawing circle at ({x}, {y}) with radius {radius}");
    }

    public void DrawRectangle(double x, double y, double width, double height)
    {
        Console.WriteLine($"[Vector] Drawing rectangle at ({x}, {y}) with width {width} and height {height}");
    }
}

// Concrete Implementation 2: Raster Graphics
public class RasterGraphicsAPI : IDrawingAPI
{
    public void DrawCircle(double x, double y, double radius)
    {
        Console.WriteLine($"[Raster] Rendering circle at ({x}, {y}) with radius {radius} using pixels");
    }

    public void DrawRectangle(double x, double y, double width, double height)
    {
        Console.WriteLine($"[Raster] Rendering rectangle at ({x}, {y}) with width {width} and height {height} using pixels");
    }
}

// Concrete Implementation 3: 3D Graphics
public class Graphics3DAPI : IDrawingAPI
{
    public void DrawCircle(double x, double y, double radius)
    {
        Console.WriteLine($"[3D] Creating sphere at ({x}, {y}, 0) with radius {radius}");
    }

    public void DrawRectangle(double x, double y, double width, double height)
    {
        Console.WriteLine($"[3D] Creating cuboid at ({x}, {y}, 0) with dimensions {width}x{height}x1");
    }
}

// Abstraction
public abstract class Shape
{
    protected IDrawingAPI drawingAPI;

    protected Shape(IDrawingAPI drawingAPI)
    {
        this.drawingAPI = drawingAPI;
    }

    public abstract void Draw();
    public abstract void Resize(double factor);
}

// Refined Abstraction 1
public class Circle : Shape
{
    private double x, y, radius;

    public Circle(double x, double y, double radius, IDrawingAPI drawingAPI) 
        : base(drawingAPI)
    {
        this.x = x;
        this.y = y;
        this.radius = radius;
    }

    public override void Draw()
    {
        drawingAPI.DrawCircle(x, y, radius);
    }

    public override void Resize(double factor)
    {
        radius *= factor;
    }
}

// Refined Abstraction 2
public class Rectangle : Shape
{
    private double x, y, width, height;

    public Rectangle(double x, double y, double width, double height, IDrawingAPI drawingAPI) 
        : base(drawingAPI)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public override void Draw()
    {
        drawingAPI.DrawRectangle(x, y, width, height);
    }

    public override void Resize(double factor)
    {
        width *= factor;
        height *= factor;
    }
}

#endregion

#region Example 2: Messaging Platform with Different Providers

// Implementation interface for messaging
public interface IMessageSender
{
    void SendMessage(string recipient, string subject, string body);
    string GetProviderName();
}

// Concrete Implementation 1: Email Provider
public class EmailSender : IMessageSender
{
    public void SendMessage(string recipient, string subject, string body)
    {
        Console.WriteLine($"📧 Sending email to: {recipient}");
        Console.WriteLine($"   Subject: {subject}");
        Console.WriteLine($"   Body: {body}");
    }

    public string GetProviderName() => "Email";
}

// Concrete Implementation 2: SMS Provider
public class SMSSender : IMessageSender
{
    public void SendMessage(string recipient, string subject, string body)
    {
        Console.WriteLine($"📱 Sending SMS to: {recipient}");
        Console.WriteLine($"   Message: {body}");
    }

    public string GetProviderName() => "SMS";
}

// Concrete Implementation 3: Slack Provider
public class SlackSender : IMessageSender
{
    public void SendMessage(string recipient, string subject, string body)
    {
        Console.WriteLine($"💬 Sending Slack message to: {recipient}");
        Console.WriteLine($"   Title: {subject}");
        Console.WriteLine($"   Content: {body}");
    }

    public string GetProviderName() => "Slack";
}

// Abstraction for messages
public abstract class Message
{
    protected IMessageSender sender;
    protected string recipient;

    protected Message(IMessageSender sender, string recipient)
    {
        this.sender = sender;
        this.recipient = recipient;
    }

    public abstract void Send();
}

// Refined Abstraction 1: Alert Message
public class AlertMessage : Message
{
    private readonly string alertLevel;
    private readonly string alertText;

    public AlertMessage(IMessageSender sender, string recipient, string alertLevel, string alertText)
        : base(sender, recipient)
    {
        this.alertLevel = alertLevel;
        this.alertText = alertText;
    }

    public override void Send()
    {
        string subject = $"[{alertLevel}] System Alert";
        sender.SendMessage(recipient, subject, alertText);
    }
}

// Refined Abstraction 2: Notification Message
public class NotificationMessage : Message
{
    private readonly string title;
    private readonly string content;

    public NotificationMessage(IMessageSender sender, string recipient, string title, string content)
        : base(sender, recipient)
    {
        this.title = title;
        this.content = content;
    }

    public override void Send()
    {
        sender.SendMessage(recipient, title, content);
    }
}

#endregion

#region Example 3: Remote Control with Different Devices

// Implementation interface for devices
public interface IDevice
{
    void TurnOn();
    void TurnOff();
    void SetChannel(int channel);
    void SetVolume(int volume);
    string GetStatus();
}

// Concrete Implementation 1: TV
public class TV : IDevice
{
    private bool isOn = false;
    private int channel = 1;
    private int volume = 50;

    public void TurnOn()
    {
        isOn = true;
        Console.WriteLine("📺 TV is now ON");
    }

    public void TurnOff()
    {
        isOn = false;
        Console.WriteLine("📺 TV is now OFF");
    }

    public void SetChannel(int channel)
    {
        if (isOn)
        {
            this.channel = channel;
            Console.WriteLine($"📺 TV channel set to {channel}");
        }
    }

    public void SetVolume(int volume)
    {
        if (isOn)
        {
            this.volume = volume;
            Console.WriteLine($"📺 TV volume set to {volume}");
        }
    }

    public string GetStatus() => $"TV [ON: {isOn}, Channel: {channel}, Volume: {volume}]";
}

// Concrete Implementation 2: Radio
public class Radio : IDevice
{
    private bool isOn = false;
    private int frequency = 1000; // in kHz
    private int volume = 30;

    public void TurnOn()
    {
        isOn = true;
        Console.WriteLine("📻 Radio is now ON");
    }

    public void TurnOff()
    {
        isOn = false;
        Console.WriteLine("📻 Radio is now OFF");
    }

    public void SetChannel(int frequency)
    {
        if (isOn)
        {
            this.frequency = frequency;
            Console.WriteLine($"📻 Radio frequency set to {frequency} kHz");
        }
    }

    public void SetVolume(int volume)
    {
        if (isOn)
        {
            this.volume = volume;
            Console.WriteLine($"📻 Radio volume set to {volume}");
        }
    }

    public string GetStatus() => $"Radio [ON: {isOn}, Frequency: {frequency} kHz, Volume: {volume}]";
}

// Abstraction for remote control
public abstract class RemoteControl
{
    protected IDevice device;

    protected RemoteControl(IDevice device)
    {
        this.device = device;
    }

    public abstract void PowerToggle();
    public abstract void VolumeUp();
    public abstract void VolumeDown();
}

// Refined Abstraction 1: Basic Remote
public class BasicRemote : RemoteControl
{
    private bool isPoweredOn = false;
    private int currentVolume = 50;

    public BasicRemote(IDevice device) : base(device) { }

    public override void PowerToggle()
    {
        if (isPoweredOn)
        {
            device.TurnOff();
            isPoweredOn = false;
        }
        else
        {
            device.TurnOn();
            isPoweredOn = true;
        }
    }

    public override void VolumeUp()
    {
        currentVolume = Math.Min(100, currentVolume + 10);
        device.SetVolume(currentVolume);
    }

    public override void VolumeDown()
    {
        currentVolume = Math.Max(0, currentVolume - 10);
        device.SetVolume(currentVolume);
    }
}

// Refined Abstraction 2: Advanced Remote
public class AdvancedRemote : RemoteControl
{
    private bool isPoweredOn = false;
    private int currentVolume = 50;

    public AdvancedRemote(IDevice device) : base(device) { }

    public override void PowerToggle()
    {
        if (isPoweredOn)
        {
            device.TurnOff();
            isPoweredOn = false;
        }
        else
        {
            device.TurnOn();
            isPoweredOn = true;
        }
    }

    public override void VolumeUp()
    {
        currentVolume = Math.Min(100, currentVolume + 10);
        device.SetVolume(currentVolume);
    }

    public override void VolumeDown()
    {
        currentVolume = Math.Max(0, currentVolume - 10);
        device.SetVolume(currentVolume);
    }

    public void Mute()
    {
        device.SetVolume(0);
        Console.WriteLine("🔇 Device muted");
    }

    public void ShowStatus()
    {
        Console.WriteLine($"Status: {device.GetStatus()}");
    }
}

#endregion

// Demo class
public static class BridgeDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Bridge Pattern Demo ===\n");

        // Example 1: Shape Drawing
        Console.WriteLine("--- Example 1: Shape Drawing with Different APIs ---");
        var shapes = new List<Shape>
        {
            new Circle(5, 10, 3, new VectorGraphicsAPI()),
            new Circle(5, 10, 3, new RasterGraphicsAPI()),
            new Rectangle(0, 0, 10, 5, new Graphics3DAPI())
        };

        foreach (var shape in shapes)
        {
            shape.Draw();
            shape.Resize(1.5);
            shape.Draw();
            Console.WriteLine();
        }

        // Example 2: Messaging Platform
        Console.WriteLine("\n--- Example 2: Messaging Platform ---");
        var messages = new List<Message>
        {
            new AlertMessage(new EmailSender(), "admin@example.com", "CRITICAL", "Server CPU usage at 95%"),
            new AlertMessage(new SMSSender(), "+1234567890", "WARNING", "Disk space low"),
            new NotificationMessage(new SlackSender(), "#team-channel", "Deployment Complete", "Version 2.0 deployed successfully")
        };

        foreach (var message in messages)
        {
            message.Send();
            Console.WriteLine();
        }

        // Example 3: Remote Control
        Console.WriteLine("\n--- Example 3: Remote Control with Different Devices ---");
        var tv = new TV();
        var radio = new Radio();

        var basicRemote = new BasicRemote(tv);
        var advancedRemote = new AdvancedRemote(radio);

        Console.WriteLine("Using basic remote with TV:");
        basicRemote.PowerToggle();
        basicRemote.VolumeUp();
        basicRemote.VolumeDown();
        basicRemote.PowerToggle();

        Console.WriteLine("\nUsing advanced remote with Radio:");
        advancedRemote.PowerToggle();
        advancedRemote.VolumeUp();
        advancedRemote.ShowStatus();
        advancedRemote.Mute();
        advancedRemote.ShowStatus();
        advancedRemote.PowerToggle();

        Console.WriteLine("\n=== Bridge Pattern Demo Complete ===");
    }
}
