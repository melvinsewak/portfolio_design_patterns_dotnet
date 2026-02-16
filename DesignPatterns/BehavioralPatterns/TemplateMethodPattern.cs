namespace DesignPatterns.BehavioralPatterns;

/// <summary>
/// Template Method Pattern - Define the skeleton of an algorithm in a base class
/// Let subclasses override specific steps without changing the algorithm's structure
/// </summary>

#region Example 1: Data Mining Application

public abstract class DataMiner
{
    // Template method - defines the algorithm structure
    public void Mine(string path)
    {
        Console.WriteLine($"\n=== Mining data from {path} ===");
        
        var file = OpenFile(path);
        var rawData = ExtractData(file);
        var parsedData = ParseData(rawData);
        var analysis = AnalyzeData(parsedData);
        SendReport(analysis);
        CloseFile(file);

        Console.WriteLine("=== Mining complete ===\n");
    }

    // Steps to be implemented by subclasses
    protected abstract object OpenFile(string path);
    protected abstract byte[] ExtractData(object file);
    protected abstract object ParseData(byte[] rawData);
    
    // Common implementation with optional override
    protected virtual string AnalyzeData(object data)
    {
        Console.WriteLine("  Performing standard analysis...");
        return $"Analysis of {data}";
    }

    protected virtual void SendReport(string analysis)
    {
        Console.WriteLine($"  Sending report: {analysis}");
    }

    protected virtual void CloseFile(object file)
    {
        Console.WriteLine("  Closing file");
    }
}

public class PdfDataMiner : DataMiner
{
    protected override object OpenFile(string path)
    {
        Console.WriteLine("  Opening PDF file...");
        return new { Type = "PDF", Path = path };
    }

    protected override byte[] ExtractData(object file)
    {
        Console.WriteLine("  Extracting text from PDF...");
        Console.WriteLine("  Processing embedded fonts and images...");
        return new byte[1000]; // Simulated data
    }

    protected override object ParseData(byte[] rawData)
    {
        Console.WriteLine("  Parsing PDF structure...");
        Console.WriteLine("  Converting to readable format...");
        return new { Format = "PDF", Size = rawData.Length, Content = "Parsed PDF data" };
    }
}

public class CsvDataMiner : DataMiner
{
    protected override object OpenFile(string path)
    {
        Console.WriteLine("  Opening CSV file...");
        return new { Type = "CSV", Path = path };
    }

    protected override byte[] ExtractData(object file)
    {
        Console.WriteLine("  Reading CSV rows...");
        return new byte[500]; // Simulated data
    }

    protected override object ParseData(byte[] rawData)
    {
        Console.WriteLine("  Parsing CSV columns...");
        Console.WriteLine("  Creating data table...");
        return new { Format = "CSV", Rows = 100, Columns = 5 };
    }

    protected override string AnalyzeData(object data)
    {
        Console.WriteLine("  Performing statistical analysis on CSV data...");
        Console.WriteLine("  Calculating averages and trends...");
        return $"Statistical analysis complete: {data}";
    }
}

public class ExcelDataMiner : DataMiner
{
    protected override object OpenFile(string path)
    {
        Console.WriteLine("  Opening Excel file...");
        return new { Type = "Excel", Path = path };
    }

    protected override byte[] ExtractData(object file)
    {
        Console.WriteLine("  Reading Excel worksheets...");
        Console.WriteLine("  Processing formulas and charts...");
        return new byte[2000]; // Simulated data
    }

    protected override object ParseData(byte[] rawData)
    {
        Console.WriteLine("  Parsing Excel workbook...");
        Console.WriteLine("  Extracting cell values...");
        return new { Format = "Excel", Sheets = 3, TotalCells = 5000 };
    }
}

#endregion

#region Example 2: Food Recipe Preparation

public abstract class RecipeTemplate
{
    // Template method
    public void PrepareRecipe()
    {
        Console.WriteLine($"\n=== Preparing {GetRecipeName()} ===");
        
        GatherIngredients();
        PrepareIngredients();
        Cook();
        
        if (RequiresPlating())
        {
            Plate();
        }
        
        Serve();
        
        Console.WriteLine($"=== {GetRecipeName()} is ready! ===\n");
    }

    protected abstract string GetRecipeName();
    protected abstract void GatherIngredients();
    protected abstract void PrepareIngredients();
    protected abstract void Cook();

    // Hook method - subclasses can override to change behavior
    protected virtual bool RequiresPlating()
    {
        return true;
    }

    protected virtual void Plate()
    {
        Console.WriteLine("  Arranging on plate...");
    }

    protected void Serve()
    {
        Console.WriteLine("  Serving hot and fresh!");
    }
}

public class PastaRecipe : RecipeTemplate
{
    protected override string GetRecipeName() => "Pasta Carbonara";

    protected override void GatherIngredients()
    {
        Console.WriteLine("  Gathering: Pasta, eggs, bacon, parmesan, black pepper");
    }

    protected override void PrepareIngredients()
    {
        Console.WriteLine("  Boiling water for pasta");
        Console.WriteLine("  Cutting bacon into small pieces");
        Console.WriteLine("  Whisking eggs with parmesan");
    }

    protected override void Cook()
    {
        Console.WriteLine("  Cooking pasta for 10 minutes");
        Console.WriteLine("  Frying bacon until crispy");
        Console.WriteLine("  Mixing hot pasta with egg mixture");
        Console.WriteLine("  Adding bacon and pepper");
    }

    protected override void Plate()
    {
        Console.WriteLine("  Twirling pasta on plate");
        Console.WriteLine("  Garnishing with parsley and extra parmesan");
    }
}

public class SteakRecipe : RecipeTemplate
{
    protected override string GetRecipeName() => "Grilled Steak";

    protected override void GatherIngredients()
    {
        Console.WriteLine("  Gathering: Ribeye steak, salt, pepper, butter, garlic");
    }

    protected override void PrepareIngredients()
    {
        Console.WriteLine("  Bringing steak to room temperature");
        Console.WriteLine("  Seasoning generously with salt and pepper");
        Console.WriteLine("  Mincing garlic");
    }

    protected override void Cook()
    {
        Console.WriteLine("  Preheating grill to high heat");
        Console.WriteLine("  Grilling steak 4 minutes per side");
        Console.WriteLine("  Basting with butter and garlic");
        Console.WriteLine("  Resting for 5 minutes");
    }

    protected override void Plate()
    {
        Console.WriteLine("  Slicing against the grain");
        Console.WriteLine("  Arranging slices on plate");
        Console.WriteLine("  Drizzling with pan juices");
    }
}

public class SmoothieRecipe : RecipeTemplate
{
    protected override string GetRecipeName() => "Berry Smoothie";

    protected override void GatherIngredients()
    {
        Console.WriteLine("  Gathering: Mixed berries, banana, yogurt, honey, ice");
    }

    protected override void PrepareIngredients()
    {
        Console.WriteLine("  Washing berries");
        Console.WriteLine("  Peeling and slicing banana");
    }

    protected override void Cook()
    {
        Console.WriteLine("  Adding all ingredients to blender");
        Console.WriteLine("  Blending until smooth (30 seconds)");
    }

    protected override bool RequiresPlating()
    {
        return false; // Smoothies don't need plating
    }

    protected override void Plate()
    {
        // Not called due to RequiresPlating returning false
    }
}

#endregion

#region Example 3: Unit Test Framework

public abstract class TestCase
{
    // Template method
    public void Run()
    {
        Console.WriteLine($"\n--- Running {GetTestName()} ---");
        
        try
        {
            SetUp();
            RunTest();
            Console.WriteLine("  ✓ Test passed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ✗ Test failed: {ex.Message}");
        }
        finally
        {
            TearDown();
        }
    }

    protected abstract string GetTestName();
    protected abstract void RunTest();

    // Hook methods with default implementation
    protected virtual void SetUp()
    {
        Console.WriteLine("  Setting up test environment...");
    }

    protected virtual void TearDown()
    {
        Console.WriteLine("  Cleaning up test environment...");
    }

    protected void Assert(bool condition, string message)
    {
        if (!condition)
            throw new Exception(message);
    }
}

public class DatabaseTest : TestCase
{
    private object? _database;
    private object? _connection;

    protected override string GetTestName() => "Database Connection Test";

    protected override void SetUp()
    {
        base.SetUp();
        Console.WriteLine("  Initializing test database...");
        Console.WriteLine("  Creating test tables...");
        _database = new { Name = "TestDB" };
        _connection = new { Status = "Connected" };
    }

    protected override void RunTest()
    {
        Console.WriteLine("  Testing database connection...");
        Assert(_database != null, "Database not initialized");
        Assert(_connection != null, "Connection not established");
        
        Console.WriteLine("  Executing test query...");
        Console.WriteLine("  Verifying results...");
        Assert(true, "Query results valid");
    }

    protected override void TearDown()
    {
        Console.WriteLine("  Dropping test tables...");
        Console.WriteLine("  Closing database connection...");
        _database = null;
        _connection = null;
        base.TearDown();
    }
}

public class ApiTest : TestCase
{
    protected override string GetTestName() => "API Endpoint Test";

    protected override void SetUp()
    {
        base.SetUp();
        Console.WriteLine("  Starting test server...");
        Console.WriteLine("  Loading test configuration...");
    }

    protected override void RunTest()
    {
        Console.WriteLine("  Sending GET request to /api/users...");
        Console.WriteLine("  Received response: 200 OK");
        
        Assert(true, "Status code is 200");
        Console.WriteLine("  Validating response body...");
        Assert(true, "Response contains expected data");
    }

    protected override void TearDown()
    {
        Console.WriteLine("  Stopping test server...");
        base.TearDown();
    }
}

public class PerformanceTest : TestCase
{
    private DateTime _startTime;

    protected override string GetTestName() => "Performance Test";

    protected override void SetUp()
    {
        base.SetUp();
        Console.WriteLine("  Warming up system...");
        _startTime = DateTime.Now;
    }

    protected override void RunTest()
    {
        Console.WriteLine("  Running performance benchmark...");
        Console.WriteLine("  Processing 10,000 operations...");
        
        // Simulate work
        Thread.Sleep(100);
        
        var duration = DateTime.Now - _startTime;
        Console.WriteLine($"  Completed in {duration.TotalMilliseconds}ms");
        Assert(duration.TotalSeconds < 5, "Performance within acceptable range");
    }
}

#endregion

public static class TemplateMethodDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Template Method Pattern Demo ===\n");

        // Example 1: Data Mining
        Console.WriteLine("--- Example 1: Data Mining ---");
        
        DataMiner pdfMiner = new PdfDataMiner();
        pdfMiner.Mine("report.pdf");

        DataMiner csvMiner = new CsvDataMiner();
        csvMiner.Mine("data.csv");

        DataMiner excelMiner = new ExcelDataMiner();
        excelMiner.Mine("spreadsheet.xlsx");

        // Example 2: Food Recipes
        Console.WriteLine("\n--- Example 2: Food Recipes ---");
        
        RecipeTemplate pasta = new PastaRecipe();
        pasta.PrepareRecipe();

        RecipeTemplate steak = new SteakRecipe();
        steak.PrepareRecipe();

        RecipeTemplate smoothie = new SmoothieRecipe();
        smoothie.PrepareRecipe();

        // Example 3: Unit Testing
        Console.WriteLine("\n--- Example 3: Unit Testing Framework ---");
        
        TestCase dbTest = new DatabaseTest();
        dbTest.Run();

        TestCase apiTest = new ApiTest();
        apiTest.Run();

        TestCase perfTest = new PerformanceTest();
        perfTest.Run();
    }
}
