namespace DesignPatterns.CreationalPatterns;

/// <summary>
/// Prototype interface
/// </summary>
public interface IPrototype<T>
{
    T Clone();
}

/// <summary>
/// Concrete Prototype - Document
/// </summary>
public class Document : IPrototype<Document>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public List<string> Tags { get; set; } = new();
    public DocumentMetadata Metadata { get; set; } = new();
    
    public Document Clone()
    {
        // Deep copy
        return new Document
        {
            Title = this.Title,
            Content = this.Content,
            CreatedDate = this.CreatedDate,
            Tags = new List<string>(this.Tags), // New list with same elements
            Metadata = this.Metadata.Clone() // Deep copy metadata
        };
    }
    
    public Document ShallowCopy()
    {
        // Shallow copy using MemberwiseClone
        return (Document)this.MemberwiseClone();
    }
    
    public override string ToString()
    {
        return $"Document: {Title}\n" +
               $"Created: {CreatedDate:yyyy-MM-dd}\n" +
               $"Tags: {string.Join(", ", Tags)}\n" +
               $"Metadata: {Metadata}";
    }
}

public class DocumentMetadata : IPrototype<DocumentMetadata>
{
    public string Author { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int Version { get; set; }
    
    public DocumentMetadata Clone()
    {
        return new DocumentMetadata
        {
            Author = this.Author,
            Department = this.Department,
            Version = this.Version
        };
    }
    
    public override string ToString()
    {
        return $"Author={Author}, Dept={Department}, v{Version}";
    }
}

/// <summary>
/// Prototype Registry/Manager
/// </summary>
public class DocumentRegistry
{
    private readonly Dictionary<string, Document> _prototypes = new();
    
    public void RegisterPrototype(string key, Document prototype)
    {
        _prototypes[key] = prototype;
    }
    
    public Document? GetPrototype(string key)
    {
        return _prototypes.TryGetValue(key, out var prototype) 
            ? prototype.Clone() 
            : null;
    }
    
    public void InitializeDefaultPrototypes()
    {
        // Register common document templates
        RegisterPrototype("letter", new Document
        {
            Title = "Letter Template",
            Content = "Dear [Recipient],\n\n[Content]\n\nSincerely,\n[Sender]",
            CreatedDate = DateTime.Now,
            Tags = new List<string> { "letter", "template" },
            Metadata = new DocumentMetadata
            {
                Author = "System",
                Department = "General",
                Version = 1
            }
        });
        
        RegisterPrototype("report", new Document
        {
            Title = "Report Template",
            Content = "# Report Title\n\n## Executive Summary\n\n## Details\n\n## Conclusion",
            CreatedDate = DateTime.Now,
            Tags = new List<string> { "report", "template" },
            Metadata = new DocumentMetadata
            {
                Author = "System",
                Department = "Reports",
                Version = 1
            }
        });
        
        RegisterPrototype("memo", new Document
        {
            Title = "Memo Template",
            Content = "TO: [Recipients]\nFROM: [Sender]\nDATE: [Date]\nRE: [Subject]\n\n[Content]",
            CreatedDate = DateTime.Now,
            Tags = new List<string> { "memo", "template" },
            Metadata = new DocumentMetadata
            {
                Author = "System",
                Department = "Internal",
                Version = 1
            }
        });
    }
}

/// <summary>
/// Alternative: Using IClonable interface (not recommended but shown for completeness)
/// </summary>
public class Shape : ICloneable
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Color { get; set; } = "Black";
    
    public object Clone()
    {
        return this.MemberwiseClone();
    }
    
    public Shape TypedClone()
    {
        return (Shape)this.Clone();
    }
}

public class Circle : Shape
{
    public int Radius { get; set; }
    
    public new Circle Clone()
    {
        var clone = (Circle)this.MemberwiseClone();
        // Perform any additional deep copy operations if needed
        return clone;
    }
}

/// <summary>
/// Modern approach: Record types (C# 9+)
/// Records provide built-in cloning with 'with' expression
/// </summary>
public record PersonRecord(string Name, int Age, string Email)
{
    public List<string> Hobbies { get; init; } = new();
}

public static class PrototypeDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Prototype Pattern Demo ===\n");
        
        // Document cloning
        Console.WriteLine("1. Document Cloning:");
        var originalDoc = new Document
        {
            Title = "Project Proposal",
            Content = "This is the original proposal content",
            CreatedDate = DateTime.Now,
            Tags = new List<string> { "project", "proposal" },
            Metadata = new DocumentMetadata
            {
                Author = "John Doe",
                Department = "Engineering",
                Version = 1
            }
        };
        
        Console.WriteLine("Original:");
        Console.WriteLine(originalDoc);
        
        // Deep copy
        var clonedDoc = originalDoc.Clone();
        clonedDoc.Title = "Project Proposal - REVISED";
        clonedDoc.Tags.Add("revised");
        clonedDoc.Metadata.Version = 2;
        
        Console.WriteLine("\nCloned (Deep Copy):");
        Console.WriteLine(clonedDoc);
        
        Console.WriteLine("\nOriginal (unchanged):");
        Console.WriteLine(originalDoc);
        
        // Prototype Registry
        Console.WriteLine("\n2. Using Prototype Registry:");
        var registry = new DocumentRegistry();
        registry.InitializeDefaultPrototypes();
        
        var letter = registry.GetPrototype("letter");
        if (letter != null)
        {
            letter.Title = "Welcome Letter";
            Console.WriteLine($"Created from 'letter' template: {letter.Title}");
        }
        
        var report = registry.GetPrototype("report");
        if (report != null)
        {
            report.Title = "Q4 2024 Financial Report";
            Console.WriteLine($"Created from 'report' template: {report.Title}");
        }
        
        // Shape cloning
        Console.WriteLine("\n3. Shape Cloning:");
        var circle1 = new Circle { X = 10, Y = 20, Radius = 5, Color = "Red" };
        var circle2 = circle1.Clone();
        circle2.X = 30;
        circle2.Color = "Blue";
        
        Console.WriteLine($"Circle 1: ({circle1.X}, {circle1.Y}), Radius={circle1.Radius}, Color={circle1.Color}");
        Console.WriteLine($"Circle 2: ({circle2.X}, {circle2.Y}), Radius={circle2.Radius}, Color={circle2.Color}");
        
        // Record cloning (modern C#)
        Console.WriteLine("\n4. Record Cloning (Modern C#):");
        var person1 = new PersonRecord("Alice", 30, "alice@example.com")
        {
            Hobbies = new() { "Reading", "Gaming" }
        };
        
        // Using 'with' expression for cloning with modifications
        var person2 = person1 with { Name = "Bob", Age = 25 };
        
        Console.WriteLine($"Person 1: {person1.Name}, Age {person1.Age}");
        Console.WriteLine($"Person 2: {person2.Name}, Age {person2.Age}");
        
        // Note: 'with' creates shallow copy of reference types
        Console.WriteLine($"Hobbies shared: {ReferenceEquals(person1.Hobbies, person2.Hobbies)}");
    }
}
