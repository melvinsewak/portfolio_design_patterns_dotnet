namespace DesignPatterns.BehavioralPatterns;

/// <summary>
/// Mediator Pattern - Defines an object that encapsulates how a set of objects interact
/// Promotes loose coupling by keeping objects from referring to each other explicitly
/// </summary>

#region Example 1: Air Traffic Control System

public interface IAirTrafficControl
{
    void RegisterAircraft(Aircraft aircraft);
    void SendMessage(string message, Aircraft sender);
    void RequestLanding(Aircraft aircraft);
    void RequestTakeoff(Aircraft aircraft);
}

public class AirTrafficControlTower : IAirTrafficControl
{
    private readonly List<Aircraft> _aircrafts = new();
    private readonly object _lock = new();
    private bool _runwayOccupied = false;

    public void RegisterAircraft(Aircraft aircraft)
    {
        _aircrafts.Add(aircraft);
        Console.WriteLine($"[ATC] {aircraft.CallSign} registered with Air Traffic Control");
    }

    public void SendMessage(string message, Aircraft sender)
    {
        Console.WriteLine($"[ATC] Broadcasting from {sender.CallSign}: {message}");
        foreach (var aircraft in _aircrafts.Where(a => a != sender))
        {
            aircraft.ReceiveMessage(message, sender.CallSign);
        }
    }

    public void RequestLanding(Aircraft aircraft)
    {
        lock (_lock)
        {
            if (!_runwayOccupied)
            {
                _runwayOccupied = true;
                Console.WriteLine($"[ATC] {aircraft.CallSign} cleared for landing");
                aircraft.ReceiveMessage("Cleared for landing, runway 09", "ATC");
                
                // Simulate landing
                Task.Delay(2000).ContinueWith(_ =>
                {
                    _runwayOccupied = false;
                    Console.WriteLine($"[ATC] Runway clear");
                });
            }
            else
            {
                Console.WriteLine($"[ATC] {aircraft.CallSign} hold position, runway occupied");
                aircraft.ReceiveMessage("Hold position, runway occupied", "ATC");
            }
        }
    }

    public void RequestTakeoff(Aircraft aircraft)
    {
        lock (_lock)
        {
            if (!_runwayOccupied)
            {
                _runwayOccupied = true;
                Console.WriteLine($"[ATC] {aircraft.CallSign} cleared for takeoff");
                aircraft.ReceiveMessage("Cleared for takeoff, runway 09", "ATC");
                
                Task.Delay(2000).ContinueWith(_ =>
                {
                    _runwayOccupied = false;
                    Console.WriteLine($"[ATC] Runway clear");
                });
            }
            else
            {
                Console.WriteLine($"[ATC] {aircraft.CallSign} hold short, runway occupied");
                aircraft.ReceiveMessage("Hold short of runway", "ATC");
            }
        }
    }
}

public abstract class Aircraft
{
    protected IAirTrafficControl? Mediator;
    public string CallSign { get; }

    protected Aircraft(string callSign)
    {
        CallSign = callSign;
    }

    public void SetMediator(IAirTrafficControl mediator)
    {
        Mediator = mediator;
        mediator.RegisterAircraft(this);
    }

    public virtual void ReceiveMessage(string message, string from)
    {
        Console.WriteLine($"  [{CallSign}] Received from {from}: {message}");
    }

    public void Send(string message)
    {
        Console.WriteLine($"[{CallSign}] Sending: {message}");
        Mediator?.SendMessage(message, this);
    }

    public void RequestLanding()
    {
        Console.WriteLine($"[{CallSign}] Requesting landing clearance");
        Mediator?.RequestLanding(this);
    }

    public void RequestTakeoff()
    {
        Console.WriteLine($"[{CallSign}] Requesting takeoff clearance");
        Mediator?.RequestTakeoff(this);
    }
}

public class CommercialAircraft : Aircraft
{
    public CommercialAircraft(string callSign) : base(callSign) { }
}

public class PrivateJet : Aircraft
{
    public PrivateJet(string callSign) : base(callSign) { }
}

#endregion

#region Example 2: Chat Room

public interface IChatRoomMediator
{
    void RegisterUser(User user);
    void SendMessage(string message, User sender, User? recipient = null);
    void BroadcastMessage(string message, User sender);
}

public class ChatRoom : IChatRoomMediator
{
    private readonly List<User> _users = new();
    private readonly string _name;

    public ChatRoom(string name)
    {
        _name = name;
    }

    public void RegisterUser(User user)
    {
        _users.Add(user);
        Console.WriteLine($"[{_name}] {user.Name} joined the chat room");
        BroadcastMessage($"{user.Name} has joined the room", user);
    }

    public void SendMessage(string message, User sender, User? recipient = null)
    {
        if (recipient == null)
        {
            BroadcastMessage(message, sender);
        }
        else
        {
            Console.WriteLine($"[{_name}] Private: {sender.Name} → {recipient.Name}: {message}");
            recipient.ReceiveMessage(message, sender, isPrivate: true);
        }
    }

    public void BroadcastMessage(string message, User sender)
    {
        Console.WriteLine($"[{_name}] {sender.Name}: {message}");
        foreach (var user in _users.Where(u => u != sender))
        {
            user.ReceiveMessage(message, sender, isPrivate: false);
        }
    }
}

public class User
{
    public string Name { get; }
    private IChatRoomMediator? _chatRoom;

    public User(string name)
    {
        Name = name;
    }

    public void JoinChatRoom(IChatRoomMediator chatRoom)
    {
        _chatRoom = chatRoom;
        chatRoom.RegisterUser(this);
    }

    public void SendMessage(string message)
    {
        _chatRoom?.BroadcastMessage(message, this);
    }

    public void SendPrivateMessage(string message, User recipient)
    {
        Console.WriteLine($"[{Name}] Sending private message to {recipient.Name}");
        _chatRoom?.SendMessage(message, this, recipient);
    }

    public void ReceiveMessage(string message, User sender, bool isPrivate)
    {
        var prefix = isPrivate ? "[Private]" : "";
        Console.WriteLine($"  {prefix}[{Name}] Received from {sender.Name}: {message}");
    }
}

#endregion

#region Example 3: Smart Home System

public interface ISmartHomeMediator
{
    void RegisterDevice(SmartDevice device);
    void Notify(SmartDevice sender, string eventType);
}

public class SmartHomeHub : ISmartHomeMediator
{
    private readonly List<SmartDevice> _devices = new();

    public void RegisterDevice(SmartDevice device)
    {
        _devices.Add(device);
        device.SetMediator(this);
        Console.WriteLine($"[Hub] {device.Name} registered");
    }

    public void Notify(SmartDevice sender, string eventType)
    {
        Console.WriteLine($"[Hub] Event from {sender.Name}: {eventType}");

        switch (eventType)
        {
            case "MotionDetected":
                HandleMotionDetected();
                break;
            case "DoorOpened":
                HandleDoorOpened();
                break;
            case "TemperatureLow":
                HandleTemperatureLow();
                break;
            case "NightMode":
                HandleNightMode();
                break;
        }
    }

    private void HandleMotionDetected()
    {
        Console.WriteLine("[Hub] Motion detected - turning on lights");
        var lights = _devices.OfType<SmartLight>();
        foreach (var light in lights)
        {
            light.TurnOn();
        }
    }

    private void HandleDoorOpened()
    {
        Console.WriteLine("[Hub] Door opened - disarming security");
        var security = _devices.OfType<HomeSecuritySystem>().FirstOrDefault();
        security?.Disarm();
    }

    private void HandleTemperatureLow()
    {
        Console.WriteLine("[Hub] Temperature low - adjusting thermostat");
        var thermostat = _devices.OfType<SmartThermostat>().FirstOrDefault();
        thermostat?.IncreaseTemperature();
    }

    private void HandleNightMode()
    {
        Console.WriteLine("[Hub] Activating night mode");
        var lights = _devices.OfType<SmartLight>();
        foreach (var light in lights)
        {
            light.Dim();
        }
        var security = _devices.OfType<HomeSecuritySystem>().FirstOrDefault();
        security?.Arm();
    }
}

public abstract class SmartDevice
{
    public string Name { get; }
    protected ISmartHomeMediator? Mediator;

    protected SmartDevice(string name)
    {
        Name = name;
    }

    public void SetMediator(ISmartHomeMediator mediator)
    {
        Mediator = mediator;
    }

    protected void NotifyMediator(string eventType)
    {
        Mediator?.Notify(this, eventType);
    }
}

public class SmartLight : SmartDevice
{
    private bool _isOn;
    private int _brightness = 100;

    public SmartLight(string name) : base(name) { }

    public void TurnOn()
    {
        _isOn = true;
        _brightness = 100;
        Console.WriteLine($"  [{Name}] Light turned ON at {_brightness}%");
    }

    public void TurnOff()
    {
        _isOn = false;
        Console.WriteLine($"  [{Name}] Light turned OFF");
    }

    public void Dim()
    {
        _brightness = 30;
        _isOn = true;
        Console.WriteLine($"  [{Name}] Light dimmed to {_brightness}%");
    }
}

public class MotionSensor : SmartDevice
{
    public MotionSensor(string name) : base(name) { }

    public void DetectMotion()
    {
        Console.WriteLine($"[{Name}] Motion detected!");
        NotifyMediator("MotionDetected");
    }
}

public class SmartDoor : SmartDevice
{
    private bool _isOpen;

    public SmartDoor(string name) : base(name) { }

    public void Open()
    {
        _isOpen = true;
        Console.WriteLine($"[{Name}] Door opened");
        NotifyMediator("DoorOpened");
    }

    public void Close()
    {
        _isOpen = false;
        Console.WriteLine($"[{Name}] Door closed");
    }
}

public class SmartThermostat : SmartDevice
{
    private int _temperature = 70;

    public SmartThermostat(string name) : base(name) { }

    public void CheckTemperature()
    {
        if (_temperature < 65)
        {
            Console.WriteLine($"[{Name}] Temperature is low: {_temperature}°F");
            NotifyMediator("TemperatureLow");
        }
    }

    public void IncreaseTemperature()
    {
        _temperature += 5;
        Console.WriteLine($"  [{Name}] Temperature increased to {_temperature}°F");
    }
}

public class HomeSecuritySystem : SmartDevice
{
    private bool _isArmed;

    public HomeSecuritySystem(string name) : base(name) { }

    public void Arm()
    {
        _isArmed = true;
        Console.WriteLine($"  [{Name}] Security system ARMED");
    }

    public void Disarm()
    {
        _isArmed = false;
        Console.WriteLine($"  [{Name}] Security system DISARMED");
    }

    public void ActivateNightMode()
    {
        Console.WriteLine($"[{Name}] Activating night mode");
        NotifyMediator("NightMode");
    }
}

#endregion

public static class MediatorDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Mediator Pattern Demo ===\n");

        // Example 1: Air Traffic Control
        Console.WriteLine("--- Example 1: Air Traffic Control ---");
        var atc = new AirTrafficControlTower();
        
        var flight1 = new CommercialAircraft("AA123");
        var flight2 = new PrivateJet("N456JP");
        var flight3 = new CommercialAircraft("UA789");

        flight1.SetMediator(atc);
        flight2.SetMediator(atc);
        flight3.SetMediator(atc);

        Console.WriteLine();
        flight1.Send("Requesting approach vector");
        Console.WriteLine();
        flight1.RequestLanding();
        flight2.RequestLanding(); // Should be told to hold
        
        // Example 2: Chat Room
        Console.WriteLine("\n\n--- Example 2: Chat Room ---");
        var chatRoom = new ChatRoom("General");
        
        var alice = new User("Alice");
        var bob = new User("Bob");
        var charlie = new User("Charlie");

        alice.JoinChatRoom(chatRoom);
        bob.JoinChatRoom(chatRoom);
        
        Console.WriteLine();
        alice.SendMessage("Hey everyone!");
        Console.WriteLine();
        bob.SendMessage("Hi Alice!");
        
        Console.WriteLine();
        charlie.JoinChatRoom(chatRoom);
        Console.WriteLine();
        alice.SendPrivateMessage("Welcome to the team!", charlie);

        // Example 3: Smart Home System
        Console.WriteLine("\n\n--- Example 3: Smart Home System ---");
        var hub = new SmartHomeHub();

        var livingRoomLight = new SmartLight("Living Room Light");
        var bedroomLight = new SmartLight("Bedroom Light");
        var motionSensor = new MotionSensor("Hallway Motion Sensor");
        var frontDoor = new SmartDoor("Front Door");
        var thermostat = new SmartThermostat("Main Thermostat");
        var security = new HomeSecuritySystem("Home Security");

        hub.RegisterDevice(livingRoomLight);
        hub.RegisterDevice(bedroomLight);
        hub.RegisterDevice(motionSensor);
        hub.RegisterDevice(frontDoor);
        hub.RegisterDevice(thermostat);
        hub.RegisterDevice(security);

        Console.WriteLine();
        motionSensor.DetectMotion();
        
        Console.WriteLine();
        frontDoor.Open();
        
        Console.WriteLine();
        security.ActivateNightMode();
    }
}
