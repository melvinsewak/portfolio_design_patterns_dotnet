namespace DesignPatterns.StructuralPatterns;

// Composite Pattern: Compose objects into tree structures to represent part-whole hierarchies

#region Example 1: File System

// Component interface
public abstract class FileSystemComponent
{
    protected string name;

    protected FileSystemComponent(string name)
    {
        this.name = name;
    }

    public abstract void Display(int depth = 0);
    public abstract long GetSize();
    
    public virtual void Add(FileSystemComponent component)
    {
        throw new NotSupportedException("Cannot add to a leaf component");
    }

    public virtual void Remove(FileSystemComponent component)
    {
        throw new NotSupportedException("Cannot remove from a leaf component");
    }

    public virtual FileSystemComponent? GetChild(int index)
    {
        throw new NotSupportedException("Leaf components have no children");
    }
}

// Leaf: File
public class File : FileSystemComponent
{
    private readonly long size;
    private readonly string extension;

    public File(string name, long size, string extension) : base(name)
    {
        this.size = size;
        this.extension = extension;
    }

    public override void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}📄 {name}.{extension} ({FormatSize(size)})");
    }

    public override long GetSize() => size;

    private static string FormatSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double len = bytes;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}

// Composite: Directory
public class Directory : FileSystemComponent
{
    private readonly List<FileSystemComponent> children = new();

    public Directory(string name) : base(name) { }

    public override void Add(FileSystemComponent component)
    {
        children.Add(component);
    }

    public override void Remove(FileSystemComponent component)
    {
        children.Remove(component);
    }

    public override FileSystemComponent? GetChild(int index)
    {
        return index >= 0 && index < children.Count ? children[index] : null;
    }

    public override void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}📁 {name}/ ({FormatSize(GetSize())})");
        foreach (var child in children)
        {
            child.Display(depth + 1);
        }
    }

    public override long GetSize()
    {
        return children.Sum(child => child.GetSize());
    }

    private static string FormatSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double len = bytes;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}

#endregion

#region Example 2: Organization Hierarchy

// Component interface for employees
public abstract class Employee
{
    protected string name;
    protected string position;
    protected decimal salary;

    protected Employee(string name, string position, decimal salary)
    {
        this.name = name;
        this.position = position;
        this.salary = salary;
    }

    public abstract void DisplayHierarchy(int depth = 0);
    public abstract decimal GetTotalSalary();
    public abstract int GetEmployeeCount();

    public virtual void Add(Employee employee)
    {
        throw new NotSupportedException("Cannot add subordinates to this position");
    }

    public virtual void Remove(Employee employee)
    {
        throw new NotSupportedException("Cannot remove subordinates from this position");
    }

    public virtual List<Employee> GetSubordinates()
    {
        return new List<Employee>();
    }
}

// Leaf: Individual Contributor
public class IndividualContributor : Employee
{
    private readonly string department;

    public IndividualContributor(string name, string position, decimal salary, string department)
        : base(name, position, salary)
    {
        this.department = department;
    }

    public override void DisplayHierarchy(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}👤 {name} - {position} ({department}) - ${salary:N0}");
    }

    public override decimal GetTotalSalary() => salary;

    public override int GetEmployeeCount() => 1;
}

// Composite: Manager
public class Manager : Employee
{
    private readonly List<Employee> subordinates = new();
    private readonly string department;

    public Manager(string name, string position, decimal salary, string department)
        : base(name, position, salary)
    {
        this.department = department;
    }

    public override void Add(Employee employee)
    {
        subordinates.Add(employee);
    }

    public override void Remove(Employee employee)
    {
        subordinates.Remove(employee);
    }

    public override List<Employee> GetSubordinates() => new(subordinates);

    public override void DisplayHierarchy(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}👔 {name} - {position} ({department}) - ${salary:N0}");
        Console.WriteLine($"{new string(' ', depth * 2)}   Team size: {GetEmployeeCount() - 1}, Budget: ${GetTotalSalary():N0}");
        
        foreach (var subordinate in subordinates)
        {
            subordinate.DisplayHierarchy(depth + 1);
        }
    }

    public override decimal GetTotalSalary()
    {
        return salary + subordinates.Sum(emp => emp.GetTotalSalary());
    }

    public override int GetEmployeeCount()
    {
        return 1 + subordinates.Sum(emp => emp.GetEmployeeCount());
    }
}

#endregion

#region Example 3: Menu System

// Component interface for menu items
public abstract class MenuComponent
{
    protected string name;

    protected MenuComponent(string name)
    {
        this.name = name;
    }

    public abstract void Display(int depth = 0);
    public abstract void Execute();

    public virtual void Add(MenuComponent component)
    {
        throw new NotSupportedException("Cannot add items to this component");
    }

    public virtual void Remove(MenuComponent component)
    {
        throw new NotSupportedException("Cannot remove items from this component");
    }

    public virtual MenuComponent? GetChild(int index)
    {
        throw new NotSupportedException("This component has no children");
    }

    public virtual int GetChildCount() => 0;
}

// Leaf: Menu Item
public class MenuItem : MenuComponent
{
    private readonly Action action;
    private readonly string shortcut;

    public MenuItem(string name, string shortcut, Action action) : base(name)
    {
        this.shortcut = shortcut;
        this.action = action;
    }

    public override void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}▶ {name} ({shortcut})");
    }

    public override void Execute()
    {
        Console.WriteLine($"Executing: {name}");
        action?.Invoke();
    }
}

// Composite: Menu
public class Menu : MenuComponent
{
    private readonly List<MenuComponent> items = new();

    public Menu(string name) : base(name) { }

    public override void Add(MenuComponent component)
    {
        items.Add(component);
    }

    public override void Remove(MenuComponent component)
    {
        items.Remove(component);
    }

    public override MenuComponent? GetChild(int index)
    {
        return index >= 0 && index < items.Count ? items[index] : null;
    }

    public override int GetChildCount() => items.Count;

    public override void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}📋 {name}");
        foreach (var item in items)
        {
            item.Display(depth + 1);
        }
    }

    public override void Execute()
    {
        Console.WriteLine($"Opening menu: {name}");
        foreach (var item in items)
        {
            item.Display(1);
        }
    }
}

// Composite: Submenu with separator support
public class MenuWithSeparators : MenuComponent
{
    private readonly List<MenuComponent> items = new();

    public MenuWithSeparators(string name) : base(name) { }

    public override void Add(MenuComponent component)
    {
        items.Add(component);
    }

    public void AddSeparator()
    {
        items.Add(new MenuSeparator());
    }

    public override void Remove(MenuComponent component)
    {
        items.Remove(component);
    }

    public override MenuComponent? GetChild(int index)
    {
        return index >= 0 && index < items.Count ? items[index] : null;
    }

    public override void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}📋 {name}");
        foreach (var item in items)
        {
            item.Display(depth + 1);
        }
    }

    public override void Execute()
    {
        Console.WriteLine($"Opening submenu: {name}");
    }
}

// Special leaf: Separator
public class MenuSeparator : MenuComponent
{
    public MenuSeparator() : base("---") { }

    public override void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}───────────────");
    }

    public override void Execute()
    {
        // Separators don't execute
    }
}

#endregion

// Demo class
public static class CompositeDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Composite Pattern Demo ===\n");

        // Example 1: File System
        Console.WriteLine("--- Example 1: File System ---");
        var root = new Directory("root");
        
        var documents = new Directory("Documents");
        documents.Add(new File("resume", 45000, "pdf"));
        documents.Add(new File("cover-letter", 23000, "docx"));
        
        var projects = new Directory("Projects");
        var projectA = new Directory("ProjectA");
        projectA.Add(new File("main", 15000, "cs"));
        projectA.Add(new File("config", 2000, "json"));
        projects.Add(projectA);
        
        var images = new Directory("Images");
        images.Add(new File("photo1", 2048000, "jpg"));
        images.Add(new File("photo2", 3145728, "png"));
        
        root.Add(documents);
        root.Add(projects);
        root.Add(images);
        root.Add(new File("readme", 5000, "txt"));
        
        root.Display();
        Console.WriteLine($"\nTotal size: {root.GetSize()} bytes");

        // Example 2: Organization Hierarchy
        Console.WriteLine("\n--- Example 2: Organization Hierarchy ---");
        var ceo = new Manager("Alice Johnson", "CEO", 250000, "Executive");
        
        var cto = new Manager("Bob Smith", "CTO", 180000, "Technology");
        cto.Add(new Manager("Carol White", "Engineering Manager", 140000, "Engineering"));
        cto.Add(new IndividualContributor("David Brown", "Senior Developer", 120000, "Engineering"));
        cto.Add(new IndividualContributor("Eve Davis", "DevOps Engineer", 110000, "Engineering"));
        
        var cfo = new Manager("Frank Miller", "CFO", 180000, "Finance");
        cfo.Add(new IndividualContributor("Grace Lee", "Accountant", 80000, "Finance"));
        cfo.Add(new IndividualContributor("Henry Wilson", "Financial Analyst", 85000, "Finance"));
        
        ceo.Add(cto);
        ceo.Add(cfo);
        ceo.Add(new IndividualContributor("Ivy Taylor", "Executive Assistant", 70000, "Executive"));
        
        ceo.DisplayHierarchy();
        Console.WriteLine($"\nTotal employees: {ceo.GetEmployeeCount()}");
        Console.WriteLine($"Total salary budget: ${ceo.GetTotalSalary():N0}");

        // Example 3: Menu System
        Console.WriteLine("\n--- Example 3: Menu System ---");
        var mainMenu = new Menu("Main Menu");
        
        var fileMenu = new MenuWithSeparators("File");
        fileMenu.Add(new MenuItem("New", "Ctrl+N", () => Console.WriteLine("Creating new file...")));
        fileMenu.Add(new MenuItem("Open", "Ctrl+O", () => Console.WriteLine("Opening file...")));
        fileMenu.AddSeparator();
        fileMenu.Add(new MenuItem("Save", "Ctrl+S", () => Console.WriteLine("Saving file...")));
        fileMenu.Add(new MenuItem("Exit", "Alt+F4", () => Console.WriteLine("Exiting application...")));
        
        var editMenu = new Menu("Edit");
        editMenu.Add(new MenuItem("Cut", "Ctrl+X", () => Console.WriteLine("Cutting...")));
        editMenu.Add(new MenuItem("Copy", "Ctrl+C", () => Console.WriteLine("Copying...")));
        editMenu.Add(new MenuItem("Paste", "Ctrl+V", () => Console.WriteLine("Pasting...")));
        
        var helpMenu = new Menu("Help");
        helpMenu.Add(new MenuItem("Documentation", "F1", () => Console.WriteLine("Opening docs...")));
        helpMenu.Add(new MenuItem("About", "", () => Console.WriteLine("About this app...")));
        
        mainMenu.Add(fileMenu);
        mainMenu.Add(editMenu);
        mainMenu.Add(helpMenu);
        
        mainMenu.Display();
        
        Console.WriteLine("\nExecuting some menu items:");
        var saveItem = fileMenu.GetChild(3);
        saveItem?.Execute();
        
        var copyItem = editMenu.GetChild(1);
        copyItem?.Execute();

        Console.WriteLine("\n=== Composite Pattern Demo Complete ===");
    }
}
