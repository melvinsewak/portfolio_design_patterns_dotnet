namespace DesignPatterns.BehavioralPatterns;

/// <summary>
/// Memento Pattern - Capture and restore an object's internal state without violating encapsulation
/// Useful for implementing undo/redo functionality and checkpoints
/// </summary>

#region Example 1: Text Editor with Undo/Redo

// Memento - stores editor state
public class EditorMemento
{
    public string Content { get; }
    public int CursorPosition { get; }
    public DateTime Timestamp { get; }

    public EditorMemento(string content, int cursorPosition)
    {
        Content = content;
        CursorPosition = cursorPosition;
        Timestamp = DateTime.Now;
    }
}

// Originator - creates and restores from mementos
public class TextEditor
{
    private string _content = string.Empty;
    private int _cursorPosition = 0;

    public void Type(string text)
    {
        _content = _content.Insert(_cursorPosition, text);
        _cursorPosition += text.Length;
        Console.WriteLine($"Typed: '{text}' → Content: '{_content}'");
    }

    public void Delete(int length)
    {
        if (_cursorPosition >= length)
        {
            _content = _content.Remove(_cursorPosition - length, length);
            _cursorPosition -= length;
            Console.WriteLine($"Deleted {length} chars → Content: '{_content}'");
        }
    }

    public void MoveCursor(int position)
    {
        _cursorPosition = Math.Clamp(position, 0, _content.Length);
        Console.WriteLine($"Cursor moved to position {_cursorPosition}");
    }

    public EditorMemento Save()
    {
        Console.WriteLine($"💾 Saving state: '{_content}' (cursor at {_cursorPosition})");
        return new EditorMemento(_content, _cursorPosition);
    }

    public void Restore(EditorMemento memento)
    {
        _content = memento.Content;
        _cursorPosition = memento.CursorPosition;
        Console.WriteLine($"↶ Restored state: '{_content}' (cursor at {_cursorPosition})");
    }

    public string GetContent() => _content;
}

// Caretaker - manages mementos
public class EditorHistory
{
    private readonly Stack<EditorMemento> _undoStack = new();
    private readonly Stack<EditorMemento> _redoStack = new();
    private EditorMemento? _currentState;

    public void SaveState(EditorMemento memento)
    {
        if (_currentState != null)
            _undoStack.Push(_currentState);
        
        _currentState = memento;
        _redoStack.Clear();
    }

    public EditorMemento? Undo()
    {
        if (_undoStack.Count == 0)
        {
            Console.WriteLine("Nothing to undo");
            return null;
        }

        if (_currentState != null)
            _redoStack.Push(_currentState);

        _currentState = _undoStack.Pop();
        return _currentState;
    }

    public EditorMemento? Redo()
    {
        if (_redoStack.Count == 0)
        {
            Console.WriteLine("Nothing to redo");
            return null;
        }

        if (_currentState != null)
            _undoStack.Push(_currentState);

        _currentState = _redoStack.Pop();
        return _currentState;
    }

    public void ShowHistory()
    {
        Console.WriteLine($"\nHistory: {_undoStack.Count} undo states, {_redoStack.Count} redo states");
    }
}

#endregion

#region Example 2: Game Save System

public class GameMemento
{
    public int Level { get; }
    public int Score { get; }
    public int Health { get; }
    public Dictionary<string, int> Inventory { get; }
    public DateTime SaveTime { get; }
    public string SaveName { get; }

    public GameMemento(int level, int score, int health, Dictionary<string, int> inventory, string saveName)
    {
        Level = level;
        Score = score;
        Health = health;
        Inventory = new Dictionary<string, int>(inventory);
        SaveTime = DateTime.Now;
        SaveName = saveName;
    }
}

public class GameState
{
    public int Level { get; private set; } = 1;
    public int Score { get; private set; } = 0;
    public int Health { get; private set; } = 100;
    public Dictionary<string, int> Inventory { get; } = new();

    public void Play(string action)
    {
        Console.WriteLine($"\n→ {action}");
        
        // Simulate different game actions
        if (action.Contains("complete level"))
        {
            Level++;
            Score += 1000;
            Console.WriteLine($"  Level up! Now at level {Level}");
        }
        else if (action.Contains("collect"))
        {
            var item = action.Split("collect ")[1];
            Inventory[item] = Inventory.GetValueOrDefault(item, 0) + 1;
            Score += 100;
            Console.WriteLine($"  Collected {item}. Score: {Score}");
        }
        else if (action.Contains("damage"))
        {
            Health -= 25;
            Console.WriteLine($"  Took damage! Health: {Health}");
        }
    }

    public GameMemento CreateSave(string saveName)
    {
        Console.WriteLine($"\n💾 Creating save: {saveName}");
        return new GameMemento(Level, Score, Health, Inventory, saveName);
    }

    public void LoadSave(GameMemento memento)
    {
        Level = memento.Level;
        Score = memento.Score;
        Health = memento.Health;
        Inventory.Clear();
        foreach (var item in memento.Inventory)
            Inventory[item.Key] = item.Value;

        Console.WriteLine($"\n↶ Loaded save: {memento.SaveName} (from {memento.SaveTime:HH:mm:ss})");
        ShowStatus();
    }

    public void ShowStatus()
    {
        Console.WriteLine($"  Level: {Level} | Score: {Score} | Health: {Health}");
        if (Inventory.Any())
        {
            Console.WriteLine($"  Inventory: {string.Join(", ", Inventory.Select(i => $"{i.Key}({i.Value})"))}");
        }
    }
}

public class SaveManager
{
    private readonly Dictionary<string, GameMemento> _saves = new();

    public void Save(GameMemento memento)
    {
        _saves[memento.SaveName] = memento;
        Console.WriteLine($"  Save '{memento.SaveName}' stored in slot");
    }

    public GameMemento? Load(string saveName)
    {
        return _saves.GetValueOrDefault(saveName);
    }

    public void ListSaves()
    {
        Console.WriteLine("\n📁 Available saves:");
        foreach (var save in _saves.Values.OrderBy(s => s.SaveTime))
        {
            Console.WriteLine($"  - {save.SaveName}: Level {save.Level}, Score {save.Score} ({save.SaveTime:g})");
        }
    }
}

#endregion

#region Example 3: Configuration Manager with Rollback

public class ConfigMemento
{
    public Dictionary<string, string> Settings { get; }
    public DateTime SnapshotTime { get; }
    public string Description { get; }

    public ConfigMemento(Dictionary<string, string> settings, string description)
    {
        Settings = new Dictionary<string, string>(settings);
        SnapshotTime = DateTime.Now;
        Description = description;
    }
}

public class ConfigurationManager
{
    private readonly Dictionary<string, string> _settings = new();

    public void SetSetting(string key, string value)
    {
        _settings[key] = value;
        Console.WriteLine($"  Set {key} = {value}");
    }

    public string? GetSetting(string key)
    {
        return _settings.GetValueOrDefault(key);
    }

    public void ApplyConfiguration(Dictionary<string, string> config, string description)
    {
        Console.WriteLine($"\n→ Applying: {description}");
        foreach (var setting in config)
        {
            SetSetting(setting.Key, setting.Value);
        }
    }

    public ConfigMemento CreateSnapshot(string description)
    {
        Console.WriteLine($"\n📸 Creating snapshot: {description}");
        return new ConfigMemento(_settings, description);
    }

    public void RestoreSnapshot(ConfigMemento memento)
    {
        _settings.Clear();
        foreach (var setting in memento.Settings)
        {
            _settings[setting.Key] = setting.Value;
        }
        Console.WriteLine($"\n↶ Restored snapshot: {memento.Description} (from {memento.SnapshotTime:HH:mm:ss})");
    }

    public void ShowSettings()
    {
        Console.WriteLine("  Current settings:");
        foreach (var setting in _settings)
        {
            Console.WriteLine($"    {setting.Key}: {setting.Value}");
        }
    }
}

public class SnapshotManager
{
    private readonly Stack<ConfigMemento> _snapshots = new();
    private const int MaxSnapshots = 10;

    public void TakeSnapshot(ConfigMemento snapshot)
    {
        _snapshots.Push(snapshot);
        
        // Limit number of snapshots
        if (_snapshots.Count > MaxSnapshots)
        {
            var snapshotsList = _snapshots.ToList();
            snapshotsList.RemoveAt(snapshotsList.Count - 1);
            _snapshots.Clear();
            foreach (var s in snapshotsList.Reverse<ConfigMemento>())
            {
                _snapshots.Push(s);
            }
            Console.WriteLine($"  Removed oldest snapshot");
        }
        
        Console.WriteLine($"  Snapshot saved ({_snapshots.Count}/{MaxSnapshots} slots used)");
    }

    public ConfigMemento? RollbackToPrevious()
    {
        if (_snapshots.Count == 0)
        {
            Console.WriteLine("No snapshots available for rollback");
            return null;
        }

        return _snapshots.Pop();
    }

    public ConfigMemento? RollbackTo(string description)
    {
        var snapshot = _snapshots.FirstOrDefault(s => s.Description == description);
        if (snapshot != null)
        {
            // Remove all snapshots after the target
            var snapshotsToKeep = new Stack<ConfigMemento>();
            foreach (var s in _snapshots)
            {
                if (s.Description == description)
                    break;
                snapshotsToKeep.Push(s);
            }
            
            // Rebuild stack
            _snapshots.Clear();
            foreach (var s in snapshotsToKeep.Reverse())
            {
                _snapshots.Push(s);
            }
        }
        
        return snapshot;
    }

    public void ListSnapshots()
    {
        Console.WriteLine($"\n📋 Snapshots ({_snapshots.Count}):");
        foreach (var snapshot in _snapshots.Reverse())
        {
            Console.WriteLine($"  - {snapshot.Description} ({snapshot.SnapshotTime:HH:mm:ss})");
        }
    }
}

#endregion

public static class MementoDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Memento Pattern Demo ===\n");

        // Example 1: Text Editor
        Console.WriteLine("--- Example 1: Text Editor with Undo/Redo ---");
        var editor = new TextEditor();
        var history = new EditorHistory();

        editor.Type("Hello");
        history.SaveState(editor.Save());

        editor.Type(" World");
        history.SaveState(editor.Save());

        editor.Type("!");
        history.SaveState(editor.Save());

        Console.WriteLine("\n--- Undoing changes ---");
        var state = history.Undo();
        if (state != null) editor.Restore(state);

        state = history.Undo();
        if (state != null) editor.Restore(state);

        Console.WriteLine("\n--- Redoing changes ---");
        state = history.Redo();
        if (state != null) editor.Restore(state);

        history.ShowHistory();

        // Example 2: Game Save System
        Console.WriteLine("\n\n--- Example 2: Game Save System ---");
        var game = new GameState();
        var saveManager = new SaveManager();

        game.ShowStatus();
        game.Play("collect sword");
        game.Play("collect potion");
        
        var checkpoint1 = game.CreateSave("Checkpoint 1");
        saveManager.Save(checkpoint1);

        game.Play("complete level");
        game.Play("collect shield");
        
        var checkpoint2 = game.CreateSave("Checkpoint 2");
        saveManager.Save(checkpoint2);

        game.Play("take damage from boss");
        game.Play("take damage from boss");
        game.ShowStatus();

        saveManager.ListSaves();

        Console.WriteLine("\n--- Loading earlier save ---");
        var savedGame = saveManager.Load("Checkpoint 1");
        if (savedGame != null)
            game.LoadSave(savedGame);

        // Example 3: Configuration Manager
        Console.WriteLine("\n\n--- Example 3: Configuration Snapshots ---");
        var config = new ConfigurationManager();
        var snapshotMgr = new SnapshotManager();

        config.ApplyConfiguration(new Dictionary<string, string>
        {
            ["host"] = "localhost",
            ["port"] = "8080",
            ["debug"] = "false"
        }, "Initial configuration");
        config.ShowSettings();

        var snapshot1 = config.CreateSnapshot("Initial setup");
        snapshotMgr.TakeSnapshot(snapshot1);

        config.ApplyConfiguration(new Dictionary<string, string>
        {
            ["port"] = "9090",
            ["debug"] = "true",
            ["timeout"] = "30"
        }, "Development configuration");
        config.ShowSettings();

        var snapshot2 = config.CreateSnapshot("Dev setup");
        snapshotMgr.TakeSnapshot(snapshot2);

        snapshotMgr.ListSnapshots();

        Console.WriteLine("\n--- Rolling back to initial setup ---");
        var rollbackSnapshot = snapshotMgr.RollbackToPrevious();
        if (rollbackSnapshot != null)
        {
            config.RestoreSnapshot(rollbackSnapshot);
            config.ShowSettings();
        }
    }
}
