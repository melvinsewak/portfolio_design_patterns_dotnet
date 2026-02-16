namespace DesignPatterns.BehavioralPatterns;

/// <summary>
/// Command Pattern - Encapsulate requests as objects
/// Allows parameterizing clients with different requests, queue or log requests, and support undo operations
/// </summary>

#region Example 1: Text Editor with Undo/Redo

public interface ICommand
{
    void Execute();
    void Undo();
}

public class TextDocument
{
    private string _content = string.Empty;

    public void InsertText(string text, int position)
    {
        _content = _content.Insert(position, text);
        Console.WriteLine($"  Content: '{_content}'");
    }

    public void DeleteText(int position, int length)
    {
        _content = _content.Remove(position, length);
        Console.WriteLine($"  Content: '{_content}'");
    }

    public string GetContent() => _content;
}

public class InsertTextCommand : ICommand
{
    private readonly TextDocument _document;
    private readonly string _text;
    private readonly int _position;

    public InsertTextCommand(TextDocument document, string text, int position)
    {
        _document = document;
        _text = text;
        _position = position;
    }

    public void Execute()
    {
        Console.WriteLine($"Executing: Insert '{_text}' at position {_position}");
        _document.InsertText(_text, _position);
    }

    public void Undo()
    {
        Console.WriteLine($"Undoing: Remove '{_text}' from position {_position}");
        _document.DeleteText(_position, _text.Length);
    }
}

public class DeleteTextCommand : ICommand
{
    private readonly TextDocument _document;
    private readonly int _position;
    private readonly int _length;
    private string _deletedText = string.Empty;

    public DeleteTextCommand(TextDocument document, int position, int length)
    {
        _document = document;
        _position = position;
        _length = length;
    }

    public void Execute()
    {
        _deletedText = _document.GetContent().Substring(_position, _length);
        Console.WriteLine($"Executing: Delete {_length} chars at position {_position}");
        _document.DeleteText(_position, _length);
    }

    public void Undo()
    {
        Console.WriteLine($"Undoing: Restore '{_deletedText}' at position {_position}");
        _document.InsertText(_deletedText, _position);
    }
}

public class CommandHistory
{
    private readonly Stack<ICommand> _undoStack = new();
    private readonly Stack<ICommand> _redoStack = new();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear();
    }

    public void Undo()
    {
        if (_undoStack.Count == 0)
        {
            Console.WriteLine("Nothing to undo");
            return;
        }

        var command = _undoStack.Pop();
        command.Undo();
        _redoStack.Push(command);
    }

    public void Redo()
    {
        if (_redoStack.Count == 0)
        {
            Console.WriteLine("Nothing to redo");
            return;
        }

        var command = _redoStack.Pop();
        command.Execute();
        _undoStack.Push(command);
    }
}

#endregion

#region Example 2: Smart Home Automation

public interface IHomeCommand
{
    void Execute();
    string GetDescription();
}

public class Light
{
    public string Location { get; }
    public bool IsOn { get; private set; }
    public int Brightness { get; private set; } = 100;

    public Light(string location)
    {
        Location = location;
    }

    public void TurnOn()
    {
        IsOn = true;
        Console.WriteLine($"  {Location} light is ON");
    }

    public void TurnOff()
    {
        IsOn = false;
        Console.WriteLine($"  {Location} light is OFF");
    }

    public void SetBrightness(int level)
    {
        Brightness = level;
        Console.WriteLine($"  {Location} light brightness set to {level}%");
    }
}

public class Thermostat
{
    public int Temperature { get; private set; } = 70;

    public void SetTemperature(int temp)
    {
        Temperature = temp;
        Console.WriteLine($"  Thermostat set to {temp}°F");
    }
}

public class SecuritySystem
{
    public bool IsArmed { get; private set; }

    public void Arm()
    {
        IsArmed = true;
        Console.WriteLine("  Security system ARMED");
    }

    public void Disarm()
    {
        IsArmed = false;
        Console.WriteLine("  Security system DISARMED");
    }
}

public class LightOnCommand : IHomeCommand
{
    private readonly Light _light;

    public LightOnCommand(Light light) => _light = light;

    public void Execute() => _light.TurnOn();
    public string GetDescription() => $"Turn on {_light.Location} light";
}

public class LightOffCommand : IHomeCommand
{
    private readonly Light _light;

    public LightOffCommand(Light light) => _light = light;

    public void Execute() => _light.TurnOff();
    public string GetDescription() => $"Turn off {_light.Location} light";
}

public class SetThermostatCommand : IHomeCommand
{
    private readonly Thermostat _thermostat;
    private readonly int _temperature;

    public SetThermostatCommand(Thermostat thermostat, int temperature)
    {
        _thermostat = thermostat;
        _temperature = temperature;
    }

    public void Execute() => _thermostat.SetTemperature(_temperature);
    public string GetDescription() => $"Set temperature to {_temperature}°F";
}

public class ArmSecurityCommand : IHomeCommand
{
    private readonly SecuritySystem _security;

    public ArmSecurityCommand(SecuritySystem security) => _security = security;

    public void Execute() => _security.Arm();
    public string GetDescription() => "Arm security system";
}

// Macro command - executes multiple commands
public class MacroCommand : IHomeCommand
{
    private readonly List<IHomeCommand> _commands = new();
    private readonly string _name;

    public MacroCommand(string name)
    {
        _name = name;
    }

    public void AddCommand(IHomeCommand command)
    {
        _commands.Add(command);
    }

    public void Execute()
    {
        Console.WriteLine($"\nExecuting macro: {_name}");
        foreach (var command in _commands)
        {
            command.Execute();
        }
    }

    public string GetDescription() => _name;
}

public class RemoteControl
{
    private readonly Dictionary<string, IHomeCommand> _commands = new();

    public void SetCommand(string button, IHomeCommand command)
    {
        _commands[button] = command;
    }

    public void PressButton(string button)
    {
        if (_commands.TryGetValue(button, out var command))
        {
            Console.WriteLine($"[Remote] Pressed '{button}' - {command.GetDescription()}");
            command.Execute();
        }
        else
        {
            Console.WriteLine($"[Remote] No command assigned to '{button}'");
        }
    }
}

#endregion

#region Example 3: Database Transaction Commands

public interface IDatabaseCommand
{
    void Execute();
    void Rollback();
    string GetQuery();
}

public class Database
{
    private readonly List<string> _records = new();

    public void Insert(string record)
    {
        _records.Add(record);
        Console.WriteLine($"  Inserted: {record}");
    }

    public void Delete(string record)
    {
        _records.Remove(record);
        Console.WriteLine($"  Deleted: {record}");
    }

    public void Update(string oldRecord, string newRecord)
    {
        var index = _records.IndexOf(oldRecord);
        if (index >= 0)
        {
            _records[index] = newRecord;
            Console.WriteLine($"  Updated: {oldRecord} → {newRecord}");
        }
    }

    public void ShowRecords()
    {
        Console.WriteLine($"  Database contains {_records.Count} records:");
        foreach (var record in _records)
            Console.WriteLine($"    - {record}");
    }
}

public class InsertCommand : IDatabaseCommand
{
    private readonly Database _database;
    private readonly string _record;

    public InsertCommand(Database database, string record)
    {
        _database = database;
        _record = record;
    }

    public void Execute() => _database.Insert(_record);
    public void Rollback() => _database.Delete(_record);
    public string GetQuery() => $"INSERT {_record}";
}

public class DeleteCommand : IDatabaseCommand
{
    private readonly Database _database;
    private readonly string _record;

    public DeleteCommand(Database database, string record)
    {
        _database = database;
        _record = record;
    }

    public void Execute() => _database.Delete(_record);
    public void Rollback() => _database.Insert(_record);
    public string GetQuery() => $"DELETE {_record}";
}

public class Transaction
{
    private readonly List<IDatabaseCommand> _commands = new();
    private bool _isCommitted;

    public void AddCommand(IDatabaseCommand command)
    {
        _commands.Add(command);
    }

    public void Execute()
    {
        Console.WriteLine("\n→ Executing transaction:");
        foreach (var command in _commands)
        {
            Console.WriteLine($"  {command.GetQuery()}");
            command.Execute();
        }
    }

    public void Commit()
    {
        Console.WriteLine("→ Transaction COMMITTED");
        _isCommitted = true;
    }

    public void Rollback()
    {
        if (_isCommitted)
        {
            Console.WriteLine("→ Cannot rollback committed transaction");
            return;
        }

        Console.WriteLine("→ Rolling back transaction:");
        for (int i = _commands.Count - 1; i >= 0; i--)
        {
            Console.WriteLine($"  Rollback: {_commands[i].GetQuery()}");
            _commands[i].Rollback();
        }
    }
}

#endregion

public static class CommandDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Command Pattern Demo ===\n");

        // Example 1: Text Editor with Undo/Redo
        Console.WriteLine("--- Example 1: Text Editor ---");
        var document = new TextDocument();
        var history = new CommandHistory();

        history.ExecuteCommand(new InsertTextCommand(document, "Hello", 0));
        history.ExecuteCommand(new InsertTextCommand(document, " World", 5));
        history.ExecuteCommand(new InsertTextCommand(document, "!", 11));

        Console.WriteLine("\nUndo last command:");
        history.Undo();

        Console.WriteLine("\nRedo:");
        history.Redo();

        Console.WriteLine("\nUndo twice:");
        history.Undo();
        history.Undo();

        // Example 2: Smart Home Automation
        Console.WriteLine("\n\n--- Example 2: Smart Home Automation ---");
        var livingRoomLight = new Light("Living Room");
        var bedroomLight = new Light("Bedroom");
        var thermostat = new Thermostat();
        var security = new SecuritySystem();
        var remote = new RemoteControl();

        // Set up individual commands
        remote.SetCommand("A", new LightOnCommand(livingRoomLight));
        remote.SetCommand("B", new LightOffCommand(livingRoomLight));
        remote.SetCommand("C", new SetThermostatCommand(thermostat, 72));

        // Create "Good Night" macro
        var goodNightMacro = new MacroCommand("Good Night");
        goodNightMacro.AddCommand(new LightOffCommand(livingRoomLight));
        goodNightMacro.AddCommand(new LightOffCommand(bedroomLight));
        goodNightMacro.AddCommand(new SetThermostatCommand(thermostat, 68));
        goodNightMacro.AddCommand(new ArmSecurityCommand(security));

        remote.SetCommand("GOODNIGHT", goodNightMacro);

        // Test commands
        remote.PressButton("A");
        remote.PressButton("C");
        remote.PressButton("GOODNIGHT");

        // Example 3: Database Transaction Commands
        Console.WriteLine("\n\n--- Example 3: Database Transactions ---");
        var db = new Database();
        var transaction = new Transaction();

        transaction.AddCommand(new InsertCommand(db, "User: John"));
        transaction.AddCommand(new InsertCommand(db, "User: Sarah"));
        transaction.AddCommand(new InsertCommand(db, "User: Mike"));

        transaction.Execute();
        db.ShowRecords();

        Console.WriteLine("\nRolling back transaction:");
        transaction.Rollback();
        db.ShowRecords();

        // Successful transaction
        Console.WriteLine("\n\nNew transaction:");
        var transaction2 = new Transaction();
        transaction2.AddCommand(new InsertCommand(db, "User: Alice"));
        transaction2.AddCommand(new InsertCommand(db, "User: Bob"));
        transaction2.Execute();
        transaction2.Commit();
        db.ShowRecords();
    }
}
