namespace DesignPatterns.StructuralPatterns;

// Example 1: Home Theater System
// Complex subsystem components

/// <summary>
/// DVD Player subsystem component
/// </summary>
public class DvdPlayer
{
    public void On()
    {
        Console.WriteLine("DVD Player: Powering on");
    }
    
    public void Off()
    {
        Console.WriteLine("DVD Player: Powering off");
    }
    
    public void Play(string movie)
    {
        Console.WriteLine($"DVD Player: Playing '{movie}'");
    }
    
    public void Stop()
    {
        Console.WriteLine("DVD Player: Stopped");
    }
    
    public void Eject()
    {
        Console.WriteLine("DVD Player: Ejecting disc");
    }
}

/// <summary>
/// Amplifier subsystem component
/// </summary>
public class Amplifier
{
    public void On()
    {
        Console.WriteLine("Amplifier: Turning on");
    }
    
    public void Off()
    {
        Console.WriteLine("Amplifier: Turning off");
    }
    
    public void SetDvd(DvdPlayer dvd)
    {
        Console.WriteLine("Amplifier: Setting DVD player as input");
    }
    
    public void SetSurroundSound()
    {
        Console.WriteLine("Amplifier: Setting surround sound mode (5.1)");
    }
    
    public void SetVolume(int level)
    {
        Console.WriteLine($"Amplifier: Setting volume to {level}");
    }
}

/// <summary>
/// Projector subsystem component
/// </summary>
public class Projector
{
    public void On()
    {
        Console.WriteLine("Projector: Powering on");
    }
    
    public void Off()
    {
        Console.WriteLine("Projector: Powering off");
    }
    
    public void WideScreenMode()
    {
        Console.WriteLine("Projector: Setting wide screen mode (16:9)");
    }
}

/// <summary>
/// Screen subsystem component
/// </summary>
public class Screen
{
    public void Down()
    {
        Console.WriteLine("Theater Screen: Lowering");
    }
    
    public void Up()
    {
        Console.WriteLine("Theater Screen: Raising");
    }
}

/// <summary>
/// Lights subsystem component
/// </summary>
public class TheaterLights
{
    public void Dim(int level)
    {
        Console.WriteLine($"Theater Lights: Dimming to {level}%");
    }
    
    public void On()
    {
        Console.WriteLine("Theater Lights: Turning on to full brightness");
    }
}

/// <summary>
/// Popcorn Maker subsystem component
/// </summary>
public class PopcornMaker
{
    public void On()
    {
        Console.WriteLine("Popcorn Maker: Turning on");
    }
    
    public void Off()
    {
        Console.WriteLine("Popcorn Maker: Turning off");
    }
    
    public void Pop()
    {
        Console.WriteLine("Popcorn Maker: Popping popcorn!");
    }
}

/// <summary>
/// Facade - Provides simplified interface to the complex home theater system
/// </summary>
public class HomeTheaterFacade
{
    private readonly Amplifier _amplifier;
    private readonly DvdPlayer _dvdPlayer;
    private readonly Projector _projector;
    private readonly Screen _screen;
    private readonly TheaterLights _lights;
    private readonly PopcornMaker _popcornMaker;
    
    public HomeTheaterFacade(
        Amplifier amplifier,
        DvdPlayer dvdPlayer,
        Projector projector,
        Screen screen,
        TheaterLights lights,
        PopcornMaker popcornMaker)
    {
        _amplifier = amplifier;
        _dvdPlayer = dvdPlayer;
        _projector = projector;
        _screen = screen;
        _lights = lights;
        _popcornMaker = popcornMaker;
    }
    
    public void WatchMovie(string movie)
    {
        Console.WriteLine("\n=== Get ready to watch a movie ===");
        _popcornMaker.On();
        _popcornMaker.Pop();
        _lights.Dim(10);
        _screen.Down();
        _projector.On();
        _projector.WideScreenMode();
        _amplifier.On();
        _amplifier.SetDvd(_dvdPlayer);
        _amplifier.SetSurroundSound();
        _amplifier.SetVolume(5);
        _dvdPlayer.On();
        _dvdPlayer.Play(movie);
        Console.WriteLine("=== Movie is now playing ===\n");
    }
    
    public void EndMovie()
    {
        Console.WriteLine("\n=== Shutting down movie theater ===");
        _popcornMaker.Off();
        _lights.On();
        _screen.Up();
        _projector.Off();
        _amplifier.Off();
        _dvdPlayer.Stop();
        _dvdPlayer.Eject();
        _dvdPlayer.Off();
        Console.WriteLine("=== Movie theater is off ===\n");
    }
}

// Example 2: Computer System Startup
/// <summary>
/// CPU subsystem
/// </summary>
public class CPU
{
    public void Freeze() => Console.WriteLine("CPU: Freezing processor");
    public void Jump(long position) => Console.WriteLine($"CPU: Jumping to position {position}");
    public void Execute() => Console.WriteLine("CPU: Executing instructions");
}

/// <summary>
/// Memory subsystem
/// </summary>
public class Memory
{
    public void Load(long position, byte[] data)
    {
        Console.WriteLine($"Memory: Loading {data.Length} bytes at position {position}");
    }
}

/// <summary>
/// Hard Drive subsystem
/// </summary>
public class HardDrive
{
    public byte[] Read(long lba, int size)
    {
        Console.WriteLine($"HardDrive: Reading {size} bytes from sector {lba}");
        return new byte[size];
    }
}

/// <summary>
/// Computer Facade - Simplifies the computer boot process
/// </summary>
public class ComputerFacade
{
    private readonly CPU _cpu;
    private readonly Memory _memory;
    private readonly HardDrive _hardDrive;
    
    private const long BOOT_ADDRESS = 0x00;
    private const long BOOT_SECTOR = 0x00;
    private const int SECTOR_SIZE = 512;
    
    public ComputerFacade()
    {
        _cpu = new CPU();
        _memory = new Memory();
        _hardDrive = new HardDrive();
    }
    
    public void Start()
    {
        Console.WriteLine("\n=== Starting Computer ===");
        _cpu.Freeze();
        _memory.Load(BOOT_ADDRESS, _hardDrive.Read(BOOT_SECTOR, SECTOR_SIZE));
        _cpu.Jump(BOOT_ADDRESS);
        _cpu.Execute();
        Console.WriteLine("=== Computer started successfully ===\n");
    }
}

// Example 3: Banking System Facade
/// <summary>
/// Account verification subsystem
/// </summary>
public class AccountVerification
{
    public bool VerifyAccount(string accountNumber)
    {
        Console.WriteLine($"AccountVerification: Verifying account {accountNumber}");
        return accountNumber.Length == 10;
    }
}

/// <summary>
/// Security subsystem
/// </summary>
public class SecurityCheck
{
    public bool CheckPin(string accountNumber, int pin)
    {
        Console.WriteLine($"SecurityCheck: Checking PIN for account {accountNumber}");
        return pin == 1234; // Simplified for demo
    }
}

/// <summary>
/// Funds checking subsystem
/// </summary>
public class FundsCheck
{
    private readonly Dictionary<string, decimal> _accountBalances = new()
    {
        { "1234567890", 1000.00m },
        { "0987654321", 500.00m }
    };
    
    public decimal GetBalance(string accountNumber)
    {
        Console.WriteLine($"FundsCheck: Retrieving balance for account {accountNumber}");
        return _accountBalances.GetValueOrDefault(accountNumber, 0);
    }
    
    public bool HasSufficientFunds(string accountNumber, decimal amount)
    {
        var balance = GetBalance(accountNumber);
        Console.WriteLine($"FundsCheck: Checking if ${amount} <= ${balance}");
        return balance >= amount;
    }
    
    public void DeductFunds(string accountNumber, decimal amount)
    {
        if (_accountBalances.ContainsKey(accountNumber))
        {
            _accountBalances[accountNumber] -= amount;
            Console.WriteLine($"FundsCheck: Deducted ${amount} from account {accountNumber}");
        }
    }
}

/// <summary>
/// Banking Facade - Simplifies banking operations
/// </summary>
public class BankingFacade
{
    private readonly AccountVerification _accountVerification;
    private readonly SecurityCheck _securityCheck;
    private readonly FundsCheck _fundsCheck;
    
    public BankingFacade()
    {
        _accountVerification = new AccountVerification();
        _securityCheck = new SecurityCheck();
        _fundsCheck = new FundsCheck();
    }
    
    public void WithdrawCash(string accountNumber, int pin, decimal amount)
    {
        Console.WriteLine($"\n=== Processing withdrawal of ${amount} ===");
        
        if (!_accountVerification.VerifyAccount(accountNumber))
        {
            Console.WriteLine("ERROR: Invalid account number");
            return;
        }
        
        if (!_securityCheck.CheckPin(accountNumber, pin))
        {
            Console.WriteLine("ERROR: Invalid PIN");
            return;
        }
        
        if (!_fundsCheck.HasSufficientFunds(accountNumber, amount))
        {
            Console.WriteLine("ERROR: Insufficient funds");
            return;
        }
        
        _fundsCheck.DeductFunds(accountNumber, amount);
        Console.WriteLine($"SUCCESS: Withdrawal of ${amount} completed");
        Console.WriteLine($"New balance: ${_fundsCheck.GetBalance(accountNumber)}\n");
    }
}

/// <summary>
/// Demonstration class for Facade Pattern
/// </summary>
public static class FacadeDemo
{
    public static void Run()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║           FACADE PATTERN DEMONSTRATION                     ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");
        
        // Example 1: Home Theater System
        Console.WriteLine("--- Example 1: Home Theater System ---");
        var amplifier = new Amplifier();
        var dvdPlayer = new DvdPlayer();
        var projector = new Projector();
        var screen = new Screen();
        var lights = new TheaterLights();
        var popcornMaker = new PopcornMaker();
        
        var homeTheater = new HomeTheaterFacade(
            amplifier, dvdPlayer, projector, screen, lights, popcornMaker);
        
        homeTheater.WatchMovie("Inception");
        Thread.Sleep(1000);
        homeTheater.EndMovie();
        
        // Example 2: Computer Startup
        Console.WriteLine("\n--- Example 2: Computer Startup ---");
        var computer = new ComputerFacade();
        computer.Start();
        
        // Example 3: Banking System
        Console.WriteLine("\n--- Example 3: Banking System ---");
        var bank = new BankingFacade();
        
        // Successful withdrawal
        bank.WithdrawCash("1234567890", 1234, 200.00m);
        
        // Failed withdrawal - insufficient funds
        bank.WithdrawCash("1234567890", 1234, 2000.00m);
        
        // Failed withdrawal - invalid PIN
        bank.WithdrawCash("1234567890", 9999, 100.00m);
        
        Console.WriteLine("\n" + new string('═', 62));
        Console.WriteLine("Key Benefits Demonstrated:");
        Console.WriteLine("• Simplified complex subsystem interactions");
        Console.WriteLine("• Reduced client dependencies on subsystem classes");
        Console.WriteLine("• Improved code readability and maintainability");
        Console.WriteLine("• Subsystems remain accessible for advanced users");
        Console.WriteLine(new string('═', 62));
    }
}
