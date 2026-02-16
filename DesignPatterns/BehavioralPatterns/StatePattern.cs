namespace DesignPatterns.BehavioralPatterns;

/// <summary>
/// State Pattern - Allow an object to alter its behavior when its internal state changes
/// The object will appear to change its class
/// </summary>

#region Example 1: Document Workflow

public interface IDocumentState
{
    void Publish(Document document);
    void Review(Document document);
    void Approve(Document document);
    void Reject(Document document);
}

public class Document
{
    private IDocumentState _state;
    public string Title { get; }
    public string Content { get; set; } = string.Empty;

    public Document(string title)
    {
        Title = title;
        _state = new DraftState();
        Console.WriteLine($"[{Title}] Created in Draft state");
    }

    public void SetState(IDocumentState state)
    {
        _state = state;
    }

    public void Publish() => _state.Publish(this);
    public void Review() => _state.Review(this);
    public void Approve() => _state.Approve(this);
    public void Reject() => _state.Reject(this);
}

public class DraftState : IDocumentState
{
    public void Publish(Document document)
    {
        Console.WriteLine($"[{document.Title}] Cannot publish from Draft. Submit for review first.");
    }

    public void Review(Document document)
    {
        Console.WriteLine($"[{document.Title}] Submitted for review");
        document.SetState(new ModerationState());
    }

    public void Approve(Document document)
    {
        Console.WriteLine($"[{document.Title}] Cannot approve a draft");
    }

    public void Reject(Document document)
    {
        Console.WriteLine($"[{document.Title}] Draft discarded");
    }
}

public class ModerationState : IDocumentState
{
    public void Publish(Document document)
    {
        Console.WriteLine($"[{document.Title}] Cannot publish during moderation. Needs approval.");
    }

    public void Review(Document document)
    {
        Console.WriteLine($"[{document.Title}] Already in moderation");
    }

    public void Approve(Document document)
    {
        Console.WriteLine($"[{document.Title}] ✓ Approved! Moving to Published state");
        document.SetState(new PublishedState());
    }

    public void Reject(Document document)
    {
        Console.WriteLine($"[{document.Title}] ✗ Rejected. Returning to Draft");
        document.SetState(new DraftState());
    }
}

public class PublishedState : IDocumentState
{
    public void Publish(Document document)
    {
        Console.WriteLine($"[{document.Title}] Already published");
    }

    public void Review(Document document)
    {
        Console.WriteLine($"[{document.Title}] Taking offline for review");
        document.SetState(new ModerationState());
    }

    public void Approve(Document document)
    {
        Console.WriteLine($"[{document.Title}] Already approved and published");
    }

    public void Reject(Document document)
    {
        Console.WriteLine($"[{document.Title}] Unpublishing document");
        document.SetState(new DraftState());
    }
}

#endregion

#region Example 2: Vending Machine

public interface IVendingMachineState
{
    void InsertCoin(VendingMachine machine, decimal amount);
    void SelectProduct(VendingMachine machine, string product);
    void DispenseProduct(VendingMachine machine);
    void ReturnMoney(VendingMachine machine);
}

public class VendingMachine
{
    private IVendingMachineState _state;
    private decimal _balance = 0;
    private readonly Dictionary<string, decimal> _products = new()
    {
        ["Soda"] = 1.50m,
        ["Chips"] = 2.00m,
        ["Candy"] = 1.00m,
        ["Water"] = 1.25m
    };
    private string _selectedProduct = string.Empty;

    public VendingMachine()
    {
        _state = new NoCoinState();
        Console.WriteLine("Vending Machine initialized - Ready for coins");
    }

    public void SetState(IVendingMachineState state) => _state = state;
    
    public decimal GetBalance() => _balance;
    public void AddBalance(decimal amount) => _balance += amount;
    public void ResetBalance() => _balance = 0;
    
    public decimal GetProductPrice(string product) => _products.GetValueOrDefault(product, 0);
    public void SetSelectedProduct(string product) => _selectedProduct = product;
    public string GetSelectedProduct() => _selectedProduct;

    public void ShowProducts()
    {
        Console.WriteLine("\n  Available Products:");
        foreach (var product in _products)
        {
            Console.WriteLine($"    - {product.Key}: ${product.Value:F2}");
        }
    }

    public void InsertCoin(decimal amount) => _state.InsertCoin(this, amount);
    public void SelectProduct(string product) => _state.SelectProduct(this, product);
    public void Dispense() => _state.DispenseProduct(this);
    public void ReturnMoney() => _state.ReturnMoney(this);
}

public class NoCoinState : IVendingMachineState
{
    public void InsertCoin(VendingMachine machine, decimal amount)
    {
        Console.WriteLine($"💰 Coin inserted: ${amount:F2}");
        machine.AddBalance(amount);
        Console.WriteLine($"   Balance: ${machine.GetBalance():F2}");
        machine.SetState(new HasCoinState());
    }

    public void SelectProduct(VendingMachine machine, string product)
    {
        Console.WriteLine("Please insert coins first");
    }

    public void DispenseProduct(VendingMachine machine)
    {
        Console.WriteLine("Please insert coins first");
    }

    public void ReturnMoney(VendingMachine machine)
    {
        Console.WriteLine("No money to return");
    }
}

public class HasCoinState : IVendingMachineState
{
    public void InsertCoin(VendingMachine machine, decimal amount)
    {
        Console.WriteLine($"💰 Coin inserted: ${amount:F2}");
        machine.AddBalance(amount);
        Console.WriteLine($"   Balance: ${machine.GetBalance():F2}");
    }

    public void SelectProduct(VendingMachine machine, string product)
    {
        var price = machine.GetProductPrice(product);
        
        if (price == 0)
        {
            Console.WriteLine($"Product '{product}' not available");
            return;
        }

        if (machine.GetBalance() >= price)
        {
            Console.WriteLine($"Selected: {product} (${price:F2})");
            machine.SetSelectedProduct(product);
            machine.SetState(new DispensingState());
            machine.Dispense();
        }
        else
        {
            var needed = price - machine.GetBalance();
            Console.WriteLine($"Insufficient funds. Need ${needed:F2} more");
        }
    }

    public void DispenseProduct(VendingMachine machine)
    {
        Console.WriteLine("Please select a product first");
    }

    public void ReturnMoney(VendingMachine machine)
    {
        Console.WriteLine($"💵 Returning ${machine.GetBalance():F2}");
        machine.ResetBalance();
        machine.SetState(new NoCoinState());
    }
}

public class DispensingState : IVendingMachineState
{
    public void InsertCoin(VendingMachine machine, decimal amount)
    {
        Console.WriteLine("Please wait, dispensing product...");
    }

    public void SelectProduct(VendingMachine machine, string product)
    {
        Console.WriteLine("Already dispensing a product");
    }

    public void DispenseProduct(VendingMachine machine)
    {
        var product = machine.GetSelectedProduct();
        var price = machine.GetProductPrice(product);
        var change = machine.GetBalance() - price;

        Console.WriteLine($"📦 Dispensing {product}...");
        
        if (change > 0)
        {
            Console.WriteLine($"💵 Returning change: ${change:F2}");
        }

        machine.ResetBalance();
        machine.SetState(new NoCoinState());
        Console.WriteLine("Thank you! Ready for next customer\n");
    }

    public void ReturnMoney(VendingMachine machine)
    {
        Console.WriteLine("Cannot return money while dispensing");
    }
}

#endregion

#region Example 3: TCP Connection

public interface IConnectionState
{
    void Open(TcpConnection connection);
    void Close(TcpConnection connection);
    void Send(TcpConnection connection, string data);
    void Receive(TcpConnection connection);
}

public class TcpConnection
{
    private IConnectionState _state;
    public string RemoteAddress { get; }

    public TcpConnection(string remoteAddress)
    {
        RemoteAddress = remoteAddress;
        _state = new ClosedState();
        Console.WriteLine($"[{RemoteAddress}] Connection created (Closed)");
    }

    public void SetState(IConnectionState state) => _state = state;

    public void Open() => _state.Open(this);
    public void Close() => _state.Close(this);
    public void Send(string data) => _state.Send(this, data);
    public void Receive() => _state.Receive(this);
}

public class ClosedState : IConnectionState
{
    public void Open(TcpConnection connection)
    {
        Console.WriteLine($"[{connection.RemoteAddress}] Opening connection...");
        connection.SetState(new ListenState());
    }

    public void Close(TcpConnection connection)
    {
        Console.WriteLine($"[{connection.RemoteAddress}] Already closed");
    }

    public void Send(TcpConnection connection, string data)
    {
        Console.WriteLine($"[{connection.RemoteAddress}] Error: Cannot send data - connection closed");
    }

    public void Receive(TcpConnection connection)
    {
        Console.WriteLine($"[{connection.RemoteAddress}] Error: Cannot receive data - connection closed");
    }
}

public class ListenState : IConnectionState
{
    public void Open(TcpConnection connection)
    {
        Console.WriteLine($"[{connection.RemoteAddress}] Connection established!");
        connection.SetState(new EstablishedState());
    }

    public void Close(TcpConnection connection)
    {
        Console.WriteLine($"[{connection.RemoteAddress}] Closing connection");
        connection.SetState(new ClosedState());
    }

    public void Send(TcpConnection connection, string data)
    {
        Console.WriteLine($"[{connection.RemoteAddress}] Error: Connection not established yet");
    }

    public void Receive(TcpConnection connection)
    {
        Console.WriteLine($"[{connection.RemoteAddress}] Waiting for connection to establish...");
    }
}

public class EstablishedState : IConnectionState
{
    public void Open(TcpConnection connection)
    {
        Console.WriteLine($"[{connection.RemoteAddress}] Connection already established");
    }

    public void Close(TcpConnection connection)
    {
        Console.WriteLine($"[{connection.RemoteAddress}] Closing established connection");
        connection.SetState(new ClosedState());
    }

    public void Send(TcpConnection connection, string data)
    {
        Console.WriteLine($"[{connection.RemoteAddress}] ➜ Sending: {data}");
    }

    public void Receive(TcpConnection connection)
    {
        Console.WriteLine($"[{connection.RemoteAddress}] ← Receiving data...");
    }
}

#endregion

public static class StateDemo
{
    public static void Run()
    {
        Console.WriteLine("=== State Pattern Demo ===\n");

        // Example 1: Document Workflow
        Console.WriteLine("--- Example 1: Document Workflow ---");
        var document = new Document("Project Proposal");
        
        document.Publish();  // Cannot publish from draft
        document.Review();   // Submit for review
        document.Review();   // Already in moderation
        document.Approve();  // Approve and publish
        document.Publish();  // Already published
        document.Reject();   // Unpublish

        Console.WriteLine("\nNew document workflow:");
        var article = new Document("Tech Article");
        article.Review();
        article.Reject();    // Send back to draft
        article.Review();    // Resubmit
        article.Approve();   // Publish

        // Example 2: Vending Machine
        Console.WriteLine("\n\n--- Example 2: Vending Machine ---");
        var vendingMachine = new VendingMachine();
        vendingMachine.ShowProducts();

        Console.WriteLine("\nTransaction 1:");
        vendingMachine.SelectProduct("Soda");     // No coins
        vendingMachine.InsertCoin(1.00m);
        vendingMachine.SelectProduct("Soda");     // Not enough
        vendingMachine.InsertCoin(1.00m);
        vendingMachine.SelectProduct("Soda");     // Should dispense with change

        Console.WriteLine("Transaction 2:");
        vendingMachine.InsertCoin(2.00m);
        vendingMachine.InsertCoin(0.50m);
        vendingMachine.SelectProduct("Chips");    // Exact amount

        Console.WriteLine("Transaction 3:");
        vendingMachine.InsertCoin(5.00m);
        vendingMachine.ReturnMoney();             // Cancel transaction

        // Example 3: TCP Connection
        Console.WriteLine("\n--- Example 3: TCP Connection ---");
        var connection = new TcpConnection("192.168.1.100:8080");
        
        connection.Send("Hello");           // Cannot send - closed
        connection.Open();                  // Start opening
        connection.Send("Hello");           // Cannot send - not established
        connection.Open();                  // Complete establishment
        connection.Send("Hello Server");    // Should work
        connection.Receive();               // Should work
        connection.Send("Goodbye");
        connection.Close();
        connection.Send("After close");     // Cannot send - closed

        Console.WriteLine("\nNew connection:");
        var connection2 = new TcpConnection("example.com:443");
        connection2.Open();
        connection2.Open(); // Establish
        connection2.Send("GET /api/data");
        connection2.Receive();
        connection2.Close();
    }
}
