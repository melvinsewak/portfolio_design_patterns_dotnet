namespace DesignPatterns.BehavioralPatterns;

/// <summary>
/// Visitor Pattern - Represent an operation to be performed on elements of an object structure
/// Lets you define a new operation without changing the classes of the elements on which it operates
/// </summary>

#region Example 1: Shopping Cart with Tax and Discount Calculations

public interface IVisitor
{
    void Visit(Book book);
    void Visit(Electronics electronics);
    void Visit(Clothing clothing);
}

public interface IVisitable
{
    void Accept(IVisitor visitor);
}

// Concrete elements
public class Book : IVisitable
{
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ISBN { get; set; } = string.Empty;

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}

public class Electronics : IVisitable
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int WarrantyMonths { get; set; }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}

public class Clothing : IVisitable
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Size { get; set; } = string.Empty;

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}

// Concrete visitors
public class TaxCalculator : IVisitor
{
    public decimal TotalTax { get; private set; }

    public void Visit(Book book)
    {
        // Books have no tax
        Console.WriteLine($"  [{book.Title}] Tax: $0.00 (Books are tax-exempt)");
    }

    public void Visit(Electronics electronics)
    {
        var tax = electronics.Price * 0.15m; // 15% tax
        TotalTax += tax;
        Console.WriteLine($"  [{electronics.Name}] Tax: ${tax:F2} (15% electronics tax)");
    }

    public void Visit(Clothing clothing)
    {
        var tax = clothing.Price * 0.08m; // 8% tax
        TotalTax += tax;
        Console.WriteLine($"  [{clothing.Name}] Tax: ${tax:F2} (8% clothing tax)");
    }

    public void Reset() => TotalTax = 0;
}

public class DiscountCalculator : IVisitor
{
    public decimal TotalDiscount { get; private set; }

    public void Visit(Book book)
    {
        var discount = book.Price * 0.10m; // 10% book discount
        TotalDiscount += discount;
        Console.WriteLine($"  [{book.Title}] Discount: ${discount:F2} (10% book promotion)");
    }

    public void Visit(Electronics electronics)
    {
        var discount = electronics.WarrantyMonths > 12 ? electronics.Price * 0.05m : 0;
        TotalDiscount += discount;
        if (discount > 0)
            Console.WriteLine($"  [{electronics.Name}] Discount: ${discount:F2} (Extended warranty bonus)");
        else
            Console.WriteLine($"  [{electronics.Name}] No discount");
    }

    public void Visit(Clothing clothing)
    {
        var discount = clothing.Size == "XL" ? clothing.Price * 0.20m : 0;
        TotalDiscount += discount;
        if (discount > 0)
            Console.WriteLine($"  [{clothing.Name}] Discount: ${discount:F2} (XL clearance sale)");
        else
            Console.WriteLine($"  [{clothing.Name}] No discount");
    }

    public void Reset() => TotalDiscount = 0;
}

public class InventoryExporter : IVisitor
{
    private readonly List<string> _exportData = new();

    public void Visit(Book book)
    {
        _exportData.Add($"BOOK|{book.Title}|{book.ISBN}|${book.Price:F2}");
        Console.WriteLine($"  Exported book: {book.Title}");
    }

    public void Visit(Electronics electronics)
    {
        _exportData.Add($"ELECTRONICS|{electronics.Name}|{electronics.WarrantyMonths}mo|${electronics.Price:F2}");
        Console.WriteLine($"  Exported electronics: {electronics.Name}");
    }

    public void Visit(Clothing clothing)
    {
        _exportData.Add($"CLOTHING|{clothing.Name}|Size:{clothing.Size}|${clothing.Price:F2}");
        Console.WriteLine($"  Exported clothing: {clothing.Name}");
    }

    public List<string> GetExportData() => _exportData;
}

#endregion

#region Example 2: Company Structure Salary and Bonus

public interface IEmployee
{
    void Accept(IEmployeeVisitor visitor);
}

public interface IEmployeeVisitor
{
    void VisitManager(EmployeeManager manager);
    void VisitDeveloper(Developer developer);
    void VisitDesigner(Designer designer);
}

public class EmployeeManager : IEmployee
{
    public string Name { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public int TeamSize { get; set; }

    public void Accept(IEmployeeVisitor visitor)
    {
        visitor.VisitManager(this);
    }
}

public class Developer : IEmployee
{
    public string Name { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public int ProjectsCompleted { get; set; }

    public void Accept(IEmployeeVisitor visitor)
    {
        visitor.VisitDeveloper(this);
    }
}

public class Designer : IEmployee
{
    public string Name { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public int DesignsCreated { get; set; }

    public void Accept(IEmployeeVisitor visitor)
    {
        visitor.VisitDesigner(this);
    }
}

public class BonusCalculator : IEmployeeVisitor
{
    public decimal TotalBonuses { get; private set; }

    public void VisitManager(EmployeeManager manager)
    {
        var bonus = manager.BaseSalary * 0.20m + (manager.TeamSize * 500);
        TotalBonuses += bonus;
        Console.WriteLine($"  [Manager] {manager.Name}: ${bonus:F2} bonus (20% + ${500 * manager.TeamSize} team bonus)");
    }

    public void VisitDeveloper(Developer developer)
    {
        var bonus = developer.BaseSalary * 0.15m + (developer.ProjectsCompleted * 1000);
        TotalBonuses += bonus;
        Console.WriteLine($"  [Developer] {developer.Name}: ${bonus:F2} bonus (15% + ${1000 * developer.ProjectsCompleted} project bonus)");
    }

    public void VisitDesigner(Designer designer)
    {
        var bonus = designer.BaseSalary * 0.12m + (designer.DesignsCreated * 300);
        TotalBonuses += bonus;
        Console.WriteLine($"  [Designer] {designer.Name}: ${bonus:F2} bonus (12% + ${300 * designer.DesignsCreated} design bonus)");
    }
}

public class PerformanceReviewer : IEmployeeVisitor
{
    public void VisitManager(EmployeeManager manager)
    {
        var rating = manager.TeamSize > 10 ? "Excellent" : manager.TeamSize > 5 ? "Good" : "Average";
        Console.WriteLine($"  [Manager] {manager.Name}: {rating} - Managing {manager.TeamSize} people");
    }

    public void VisitDeveloper(Developer developer)
    {
        var rating = developer.ProjectsCompleted > 10 ? "Excellent" : developer.ProjectsCompleted > 5 ? "Good" : "Average";
        Console.WriteLine($"  [Developer] {developer.Name}: {rating} - Completed {developer.ProjectsCompleted} projects");
    }

    public void VisitDesigner(Designer designer)
    {
        var rating = designer.DesignsCreated > 20 ? "Excellent" : designer.DesignsCreated > 10 ? "Good" : "Average";
        Console.WriteLine($"  [Designer] {designer.Name}: {rating} - Created {designer.DesignsCreated} designs");
    }
}

#endregion

#region Example 3: Document Elements Rendering

public interface IDocumentElement
{
    void Accept(IDocumentVisitor visitor);
}

public interface IDocumentVisitor
{
    void VisitParagraph(Paragraph paragraph);
    void VisitImage(Image image);
    void VisitTable(Table table);
    void VisitHeading(Heading heading);
}

public class Paragraph : IDocumentElement
{
    public string Text { get; set; } = string.Empty;

    public void Accept(IDocumentVisitor visitor)
    {
        visitor.VisitParagraph(this);
    }
}

public class Image : IDocumentElement
{
    public string Url { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }

    public void Accept(IDocumentVisitor visitor)
    {
        visitor.VisitImage(this);
    }
}

public class Table : IDocumentElement
{
    public int Rows { get; set; }
    public int Columns { get; set; }

    public void Accept(IDocumentVisitor visitor)
    {
        visitor.VisitTable(this);
    }
}

public class Heading : IDocumentElement
{
    public string Text { get; set; } = string.Empty;
    public int Level { get; set; } // 1-6

    public void Accept(IDocumentVisitor visitor)
    {
        visitor.VisitHeading(this);
    }
}

public class HtmlRenderer : IDocumentVisitor
{
    private readonly List<string> _html = new();

    public void VisitParagraph(Paragraph paragraph)
    {
        _html.Add($"<p>{paragraph.Text}</p>");
        Console.WriteLine($"  Rendered paragraph: <p>...</p>");
    }

    public void VisitImage(Image image)
    {
        _html.Add($"<img src=\"{image.Url}\" width=\"{image.Width}\" height=\"{image.Height}\" />");
        Console.WriteLine($"  Rendered image: <img src=\"{image.Url}\" />");
    }

    public void VisitTable(Table table)
    {
        _html.Add($"<table rows=\"{table.Rows}\" cols=\"{table.Columns}\">...</table>");
        Console.WriteLine($"  Rendered table: {table.Rows}x{table.Columns}");
    }

    public void VisitHeading(Heading heading)
    {
        _html.Add($"<h{heading.Level}>{heading.Text}</h{heading.Level}>");
        Console.WriteLine($"  Rendered heading: <h{heading.Level}>{heading.Text}</h{heading.Level}>");
    }

    public string GetHtml() => string.Join("\n", _html);
}

public class MarkdownRenderer : IDocumentVisitor
{
    private readonly List<string> _markdown = new();

    public void VisitParagraph(Paragraph paragraph)
    {
        _markdown.Add($"{paragraph.Text}\n");
        Console.WriteLine($"  Rendered paragraph to Markdown");
    }

    public void VisitImage(Image image)
    {
        _markdown.Add($"![Image]({image.Url})");
        Console.WriteLine($"  Rendered image: ![Image]({image.Url})");
    }

    public void VisitTable(Table table)
    {
        _markdown.Add($"| Table {table.Rows}x{table.Columns} |");
        Console.WriteLine($"  Rendered table to Markdown: {table.Rows}x{table.Columns}");
    }

    public void VisitHeading(Heading heading)
    {
        var hashes = new string('#', heading.Level);
        _markdown.Add($"{hashes} {heading.Text}");
        Console.WriteLine($"  Rendered heading: {hashes} {heading.Text}");
    }

    public string GetMarkdown() => string.Join("\n", _markdown);
}

public class WordCounter : IDocumentVisitor
{
    public int TotalWords { get; private set; }

    public void VisitParagraph(Paragraph paragraph)
    {
        var words = paragraph.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        TotalWords += words;
        Console.WriteLine($"  Paragraph: {words} words");
    }

    public void VisitImage(Image image)
    {
        Console.WriteLine($"  Image: 0 words");
    }

    public void VisitTable(Table table)
    {
        var words = table.Rows * table.Columns * 3; // Estimate
        TotalWords += words;
        Console.WriteLine($"  Table: ~{words} words (estimated)");
    }

    public void VisitHeading(Heading heading)
    {
        var words = heading.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        TotalWords += words;
        Console.WriteLine($"  Heading: {words} words");
    }
}

#endregion

public static class VisitorDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Visitor Pattern Demo ===\n");

        // Example 1: Shopping Cart
        Console.WriteLine("--- Example 1: Shopping Cart ---");
        var items = new List<IVisitable>
        {
            new Book { Title = "Design Patterns", Price = 45.00m, ISBN = "978-0201633610" },
            new Electronics { Name = "Laptop", Price = 1200.00m, WarrantyMonths = 24 },
            new Clothing { Name = "T-Shirt", Price = 25.00m, Size = "XL" },
            new Book { Title = "Clean Code", Price = 35.00m, ISBN = "978-0132350884" },
            new Electronics { Name = "Mouse", Price = 30.00m, WarrantyMonths = 6 }
        };

        var taxCalc = new TaxCalculator();
        Console.WriteLine("\nCalculating taxes:");
        foreach (var item in items)
            item.Accept(taxCalc);
        Console.WriteLine($"\n💰 Total Tax: ${taxCalc.TotalTax:F2}");

        var discountCalc = new DiscountCalculator();
        Console.WriteLine("\nCalculating discounts:");
        foreach (var item in items)
            item.Accept(discountCalc);
        Console.WriteLine($"\n💵 Total Discount: ${discountCalc.TotalDiscount:F2}");

        var exporter = new InventoryExporter();
        Console.WriteLine("\nExporting inventory:");
        foreach (var item in items)
            item.Accept(exporter);
        Console.WriteLine("\nExported data:");
        foreach (var data in exporter.GetExportData())
            Console.WriteLine($"  {data}");

        // Example 2: Company Structure
        Console.WriteLine("\n\n--- Example 2: Employee Bonuses and Reviews ---");
        var employees = new List<IEmployee>
        {
            new EmployeeManager { Name = "Alice", BaseSalary = 120000, TeamSize = 8 },
            new Developer { Name = "Bob", BaseSalary = 90000, ProjectsCompleted = 12 },
            new Designer { Name = "Charlie", BaseSalary = 80000, DesignsCreated = 25 },
            new Developer { Name = "Diana", BaseSalary = 95000, ProjectsCompleted = 15 }
        };

        var bonusCalc = new BonusCalculator();
        Console.WriteLine("\nCalculating year-end bonuses:");
        foreach (var employee in employees)
            employee.Accept(bonusCalc);
        Console.WriteLine($"\n💰 Total Bonuses: ${bonusCalc.TotalBonuses:F2}");

        var reviewer = new PerformanceReviewer();
        Console.WriteLine("\nPerformance Reviews:");
        foreach (var employee in employees)
            employee.Accept(reviewer);

        // Example 3: Document Rendering
        Console.WriteLine("\n\n--- Example 3: Document Rendering ---");
        var document = new List<IDocumentElement>
        {
            new Heading { Text = "Introduction", Level = 1 },
            new Paragraph { Text = "This is a sample document demonstrating the Visitor pattern." },
            new Image { Url = "diagram.png", Width = 800, Height = 600 },
            new Heading { Text = "Data Analysis", Level = 2 },
            new Table { Rows = 5, Columns = 3 },
            new Paragraph { Text = "The table above shows the key metrics for our analysis." }
        };

        var htmlRenderer = new HtmlRenderer();
        Console.WriteLine("\nRendering to HTML:");
        foreach (var element in document)
            element.Accept(htmlRenderer);

        var markdownRenderer = new MarkdownRenderer();
        Console.WriteLine("\nRendering to Markdown:");
        foreach (var element in document)
            element.Accept(markdownRenderer);

        var wordCounter = new WordCounter();
        Console.WriteLine("\nCounting words:");
        foreach (var element in document)
            element.Accept(wordCounter);
        Console.WriteLine($"\n📊 Total word count: {wordCounter.TotalWords}");
    }
}
