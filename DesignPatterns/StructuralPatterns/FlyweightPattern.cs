namespace DesignPatterns.StructuralPatterns;

// Example 1: Text Editor Character Formatting
/// <summary>
/// Flyweight - Shared character formatting (intrinsic state)
/// </summary>
public class CharacterStyle
{
    public string FontFamily { get; }
    public int FontSize { get; }
    public string Color { get; }
    public bool Bold { get; }
    public bool Italic { get; }
    
    public CharacterStyle(string fontFamily, int fontSize, string color, bool bold, bool italic)
    {
        FontFamily = fontFamily;
        FontSize = fontSize;
        Color = color;
        Bold = bold;
        Italic = italic;
        
        Console.WriteLine($"Creating CharacterStyle: {fontFamily}, {fontSize}pt, {color}, " +
                         $"Bold={bold}, Italic={italic}");
    }
    
    public void Display(char character, int position)
    {
        Console.Write($"[{character}");
        Console.Write($" @{position}");
        Console.Write($" {FontFamily} {FontSize}pt {Color}");
        if (Bold) Console.Write(" Bold");
        if (Italic) Console.Write(" Italic");
        Console.Write("] ");
    }
}

/// <summary>
/// Flyweight Factory - Manages and shares CharacterStyle instances
/// </summary>
public class CharacterStyleFactory
{
    private readonly Dictionary<string, CharacterStyle> _styles = new();
    
    public CharacterStyle GetStyle(string fontFamily, int fontSize, string color, 
                                   bool bold, bool italic)
    {
        string key = $"{fontFamily}_{fontSize}_{color}_{bold}_{italic}";
        
        if (!_styles.ContainsKey(key))
        {
            _styles[key] = new CharacterStyle(fontFamily, fontSize, color, bold, italic);
        }
        else
        {
            Console.WriteLine($"Reusing existing CharacterStyle: {key}");
        }
        
        return _styles[key];
    }
    
    public int GetTotalStyles() => _styles.Count;
}

/// <summary>
/// Context - Contains extrinsic state (character and position)
/// </summary>
public class Character
{
    private readonly char _symbol;
    private readonly int _position;
    private readonly CharacterStyle _style;
    
    public Character(char symbol, int position, CharacterStyle style)
    {
        _symbol = symbol;
        _position = position;
        _style = style;
    }
    
    public void Display()
    {
        _style.Display(_symbol, _position);
    }
}

/// <summary>
/// Client - Text Editor
/// </summary>
public class TextEditor
{
    private readonly List<Character> _characters = new();
    private readonly CharacterStyleFactory _styleFactory = new();
    
    public void AddText(string text, string fontFamily, int fontSize, string color, 
                       bool bold = false, bool italic = false)
    {
        var style = _styleFactory.GetStyle(fontFamily, fontSize, color, bold, italic);
        
        foreach (char c in text)
        {
            _characters.Add(new Character(c, _characters.Count, style));
        }
    }
    
    public void Display()
    {
        Console.WriteLine("\n--- Document Content ---");
        foreach (var character in _characters)
        {
            character.Display();
        }
        Console.WriteLine($"\n\nTotal characters: {_characters.Count}");
        Console.WriteLine($"Total unique styles: {_styleFactory.GetTotalStyles()}");
        Console.WriteLine($"Memory saved: {_characters.Count - _styleFactory.GetTotalStyles()} style objects\n");
    }
}

// Example 2: Game Particle System
/// <summary>
/// Particle Type Flyweight - Shared particle properties
/// </summary>
public class ParticleType
{
    public string Texture { get; }
    public string Color { get; }
    public int Size { get; }
    public double Mass { get; }
    
    public ParticleType(string texture, string color, int size, double mass)
    {
        Texture = texture;
        Color = color;
        Size = size;
        Mass = mass;
        
        Console.WriteLine($"Loading ParticleType: {texture} ({color}, {size}px, {mass}kg)");
    }
    
    public void Render(int x, int y, int velocityX, int velocityY)
    {
        Console.WriteLine($"Rendering {Color} {Texture} at ({x},{y}) " +
                         $"velocity=({velocityX},{velocityY}) size={Size}px");
    }
}

/// <summary>
/// Particle Type Factory - Manages shared particle types
/// </summary>
public class ParticleTypeFactory
{
    private readonly Dictionary<string, ParticleType> _particleTypes = new();
    
    public ParticleType GetParticleType(string texture, string color, int size, double mass)
    {
        string key = $"{texture}_{color}_{size}_{mass}";
        
        if (!_particleTypes.ContainsKey(key))
        {
            _particleTypes[key] = new ParticleType(texture, color, size, mass);
        }
        
        return _particleTypes[key];
    }
    
    public int GetTypeCount() => _particleTypes.Count;
}

/// <summary>
/// Particle - Contains extrinsic state (position, velocity)
/// </summary>
public class Particle
{
    private readonly int _x;
    private readonly int _y;
    private readonly int _velocityX;
    private readonly int _velocityY;
    private readonly ParticleType _type;
    
    public Particle(int x, int y, int velocityX, int velocityY, ParticleType type)
    {
        _x = x;
        _y = y;
        _velocityX = velocityX;
        _velocityY = velocityY;
        _type = type;
    }
    
    public void Render()
    {
        _type.Render(_x, _y, _velocityX, _velocityY);
    }
}

/// <summary>
/// Particle System - Manages thousands of particles efficiently
/// </summary>
public class ParticleSystem
{
    private readonly List<Particle> _particles = new();
    private readonly ParticleTypeFactory _typeFactory = new();
    
    public void CreateExplosion(int centerX, int centerY, int particleCount)
    {
        Console.WriteLine($"\nCreating explosion at ({centerX},{centerY}) with {particleCount} particles");
        
        var random = new Random();
        var types = new[]
        {
            _typeFactory.GetParticleType("spark", "Orange", 2, 0.01),
            _typeFactory.GetParticleType("spark", "Yellow", 3, 0.02),
            _typeFactory.GetParticleType("smoke", "Gray", 5, 0.05)
        };
        
        for (int i = 0; i < particleCount; i++)
        {
            var type = types[random.Next(types.Length)];
            var particle = new Particle(
                centerX + random.Next(-10, 10),
                centerY + random.Next(-10, 10),
                random.Next(-5, 5),
                random.Next(-5, 5),
                type
            );
            _particles.Add(particle);
        }
    }
    
    public void RenderAll()
    {
        Console.WriteLine("\n--- Rendering Particles ---");
        int displayCount = Math.Min(5, _particles.Count);
        for (int i = 0; i < displayCount; i++)
        {
            _particles[i].Render();
        }
        
        if (_particles.Count > displayCount)
        {
            Console.WriteLine($"... and {_particles.Count - displayCount} more particles");
        }
        
        Console.WriteLine($"\nTotal particles: {_particles.Count}");
        Console.WriteLine($"Unique particle types: {_typeFactory.GetTypeCount()}");
        Console.WriteLine($"Memory efficiency: {_particles.Count / _typeFactory.GetTypeCount()}:1 ratio\n");
    }
}

// Example 3: Forest Simulation
/// <summary>
/// Tree Type Flyweight - Shared tree properties
/// </summary>
public class TreeType
{
    public string Name { get; }
    public string Color { get; }
    public string Texture { get; }
    
    public TreeType(string name, string color, string texture)
    {
        Name = name;
        Color = color;
        Texture = texture;
        
        // Simulate loading large texture data
        Console.WriteLine($"Loading tree type: {name} (Color: {color}, Texture: {texture})");
    }
    
    public void Display(int x, int y, int height)
    {
        Console.WriteLine($"{Name} tree at ({x},{y}) - Height: {height}m - {Color} leaves");
    }
}

/// <summary>
/// Tree Type Factory
/// </summary>
public class TreeTypeFactory
{
    private readonly Dictionary<string, TreeType> _treeTypes = new();
    
    public TreeType GetTreeType(string name, string color, string texture)
    {
        string key = $"{name}_{color}_{texture}";
        
        if (!_treeTypes.ContainsKey(key))
        {
            _treeTypes[key] = new TreeType(name, color, texture);
        }
        
        return _treeTypes[key];
    }
    
    public int GetTypeCount() => _treeTypes.Count;
}

/// <summary>
/// Tree - Contains extrinsic state (position, height)
/// </summary>
public class Tree
{
    private readonly int _x;
    private readonly int _y;
    private readonly int _height;
    private readonly TreeType _type;
    
    public Tree(int x, int y, int height, TreeType type)
    {
        _x = x;
        _y = y;
        _height = height;
        _type = type;
    }
    
    public void Display()
    {
        _type.Display(_x, _y, _height);
    }
}

/// <summary>
/// Forest - Manages thousands of trees efficiently
/// </summary>
public class Forest
{
    private readonly List<Tree> _trees = new();
    private readonly TreeTypeFactory _treeTypeFactory = new();
    
    public void PlantTree(int x, int y, string name, string color, string texture, int height)
    {
        var type = _treeTypeFactory.GetTreeType(name, color, texture);
        var tree = new Tree(x, y, height, type);
        _trees.Add(tree);
    }
    
    public void Display()
    {
        Console.WriteLine("\n--- Forest Simulation ---");
        int displayCount = Math.Min(10, _trees.Count);
        
        for (int i = 0; i < displayCount; i++)
        {
            _trees[i].Display();
        }
        
        if (_trees.Count > displayCount)
        {
            Console.WriteLine($"... and {_trees.Count - displayCount} more trees");
        }
        
        Console.WriteLine($"\nTotal trees: {_trees.Count}");
        Console.WriteLine($"Unique tree types: {_treeTypeFactory.GetTypeCount()}");
        
        // Calculate memory saved
        int objectsWithoutFlyweight = _trees.Count;
        int objectsWithFlyweight = _trees.Count + _treeTypeFactory.GetTypeCount();
        Console.WriteLine($"Objects without Flyweight: {objectsWithoutFlyweight} tree type objects");
        Console.WriteLine($"Objects with Flyweight: {objectsWithFlyweight} total objects");
        Console.WriteLine($"Memory saved: ~{((1 - (double)objectsWithFlyweight / objectsWithoutFlyweight) * 100):F1}%\n");
    }
}

/// <summary>
/// Demonstration class for Flyweight Pattern
/// </summary>
public static class FlyweightDemo
{
    public static void Run()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║          FLYWEIGHT PATTERN DEMONSTRATION                   ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");
        
        // Example 1: Text Editor
        Console.WriteLine("--- Example 1: Text Editor Character Formatting ---");
        var editor = new TextEditor();
        
        editor.AddText("Hello ", "Arial", 12, "Black", bold: true);
        editor.AddText("World", "Arial", 12, "Red", italic: true);
        editor.AddText("!", "Arial", 12, "Black", bold: true);
        editor.AddText(" Welcome ", "Times", 14, "Blue");
        editor.AddText("to ", "Arial", 12, "Black", bold: true);
        editor.AddText("Flyweight Pattern", "Courier", 10, "Green");
        
        editor.Display();
        
        // Example 2: Game Particle System
        Console.WriteLine("\n--- Example 2: Game Particle System ---");
        var particleSystem = new ParticleSystem();
        
        particleSystem.CreateExplosion(100, 100, 1000);
        particleSystem.CreateExplosion(200, 150, 500);
        
        particleSystem.RenderAll();
        
        // Example 3: Forest Simulation
        Console.WriteLine("\n--- Example 3: Forest Simulation ---");
        var forest = new Forest();
        var random = new Random(42); // Fixed seed for consistent output
        
        // Plant 1000 trees of various types
        Console.WriteLine("Planting forest with 1000 trees...\n");
        
        for (int i = 0; i < 1000; i++)
        {
            int treeTypeChoice = random.Next(4);
            int x = random.Next(1000);
            int y = random.Next(1000);
            int height = random.Next(5, 30);
            
            switch (treeTypeChoice)
            {
                case 0:
                    forest.PlantTree(x, y, "Oak", "Green", "oak_texture.png", height);
                    break;
                case 1:
                    forest.PlantTree(x, y, "Pine", "Dark Green", "pine_texture.png", height);
                    break;
                case 2:
                    forest.PlantTree(x, y, "Birch", "Light Green", "birch_texture.png", height);
                    break;
                case 3:
                    forest.PlantTree(x, y, "Maple", "Red", "maple_texture.png", height);
                    break;
            }
        }
        
        forest.Display();
        
        Console.WriteLine("\n" + new string('═', 62));
        Console.WriteLine("Key Benefits Demonstrated:");
        Console.WriteLine("• Massive memory savings by sharing immutable state");
        Console.WriteLine("• Support for large numbers of fine-grained objects");
        Console.WriteLine("• Separation of intrinsic (shared) and extrinsic state");
        Console.WriteLine("• Improved performance with reduced object allocation");
        Console.WriteLine(new string('═', 62));
    }
}
