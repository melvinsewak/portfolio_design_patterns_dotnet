using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesignPatterns.ModernPatterns;

#region Core Interfaces

public interface IUnitOfWork : IDisposable
{
    Task<int> CommitAsync();
    Task RollbackAsync();
    void BeginTransaction();
}

#endregion

#region Domain Models for Unit of Work

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}

public class Invoice
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime InvoiceDate { get; set; }
    public bool IsPaid { get; set; }
}

public class Payment
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}

public class AuditLog
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
}

#endregion

#region Repository Interfaces for Unit of Work

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
}

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(int id);
    Task<IEnumerable<Invoice>> GetByCustomerIdAsync(int customerId);
    Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync();
    Task AddAsync(Invoice invoice);
    Task UpdateAsync(Invoice invoice);
}

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id);
    Task<IEnumerable<Payment>> GetByCustomerIdAsync(int customerId);
    Task<IEnumerable<Payment>> GetByInvoiceIdAsync(int invoiceId);
    Task AddAsync(Payment payment);
}

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, int entityId);
    Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count);
}

#endregion

#region Repository Implementations

public class CustomerRepository : ICustomerRepository
{
    private readonly List<Customer> _customers;

    public CustomerRepository(List<Customer> customers)
    {
        _customers = customers;
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        await Task.Delay(10);
        return _customers.FirstOrDefault(c => c.Id == id);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        await Task.Delay(10);
        return _customers.ToList();
    }

    public async Task AddAsync(Customer customer)
    {
        await Task.Delay(10);
        customer.Id = _customers.Any() ? _customers.Max(c => c.Id) + 1 : 1;
        _customers.Add(customer);
    }

    public async Task UpdateAsync(Customer customer)
    {
        await Task.Delay(10);
        var existing = _customers.FirstOrDefault(c => c.Id == customer.Id);
        if (existing != null)
        {
            var index = _customers.IndexOf(existing);
            _customers[index] = customer;
        }
    }

    public async Task DeleteAsync(int id)
    {
        await Task.Delay(10);
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        if (customer != null)
        {
            _customers.Remove(customer);
        }
    }
}

public class UoWInvoiceRepository : IInvoiceRepository
{
    private readonly List<Invoice> _invoices;

    public UoWInvoiceRepository(List<Invoice> invoices)
    {
        _invoices = invoices;
    }

    public async Task<Invoice?> GetByIdAsync(int id)
    {
        await Task.Delay(10);
        return _invoices.FirstOrDefault(i => i.Id == id);
    }

    public async Task<IEnumerable<Invoice>> GetByCustomerIdAsync(int customerId)
    {
        await Task.Delay(10);
        return _invoices.Where(i => i.CustomerId == customerId).ToList();
    }

    public async Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync()
    {
        await Task.Delay(10);
        return _invoices.Where(i => !i.IsPaid).ToList();
    }

    public async Task AddAsync(Invoice invoice)
    {
        await Task.Delay(10);
        invoice.Id = _invoices.Any() ? _invoices.Max(i => i.Id) + 1 : 1;
        _invoices.Add(invoice);
    }

    public async Task UpdateAsync(Invoice invoice)
    {
        await Task.Delay(10);
        var existing = _invoices.FirstOrDefault(i => i.Id == invoice.Id);
        if (existing != null)
        {
            var index = _invoices.IndexOf(existing);
            _invoices[index] = invoice;
        }
    }
}

public class PaymentRepository : IPaymentRepository
{
    private readonly List<Payment> _payments;

    public PaymentRepository(List<Payment> payments)
    {
        _payments = payments;
    }

    public async Task<Payment?> GetByIdAsync(int id)
    {
        await Task.Delay(10);
        return _payments.FirstOrDefault(p => p.Id == id);
    }

    public async Task<IEnumerable<Payment>> GetByCustomerIdAsync(int customerId)
    {
        await Task.Delay(10);
        return _payments.Where(p => p.CustomerId == customerId).ToList();
    }

    public async Task<IEnumerable<Payment>> GetByInvoiceIdAsync(int invoiceId)
    {
        await Task.Delay(10);
        return _payments.Where(p => p.InvoiceId == invoiceId).ToList();
    }

    public async Task AddAsync(Payment payment)
    {
        await Task.Delay(10);
        payment.Id = _payments.Any() ? _payments.Max(p => p.Id) + 1 : 1;
        _payments.Add(payment);
    }
}

public class AuditLogRepository : IAuditLogRepository
{
    private readonly List<AuditLog> _auditLogs;

    public AuditLogRepository(List<AuditLog> auditLogs)
    {
        _auditLogs = auditLogs;
    }

    public async Task AddAsync(AuditLog log)
    {
        await Task.Delay(10);
        log.Id = _auditLogs.Any() ? _auditLogs.Max(a => a.Id) + 1 : 1;
        log.Timestamp = DateTime.Now;
        _auditLogs.Add(log);
    }

    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, int entityId)
    {
        await Task.Delay(10);
        return _auditLogs.Where(a => a.EntityType == entityType && a.EntityId == entityId).ToList();
    }

    public async Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count)
    {
        await Task.Delay(10);
        return _auditLogs.OrderByDescending(a => a.Timestamp).Take(count).ToList();
    }
}

#endregion

#region Example 1: Simple Unit of Work Implementation

public class UnitOfWork : IUnitOfWork
{
    private readonly List<Customer> _customers;
    private readonly List<Invoice> _invoices;
    private readonly List<Payment> _payments;
    private readonly List<AuditLog> _auditLogs;

    private bool _isInTransaction;
    private List<Action> _operations;

    public ICustomerRepository Customers { get; }
    public IInvoiceRepository Invoices { get; }
    public IPaymentRepository Payments { get; }
    public IAuditLogRepository AuditLogs { get; }

    public UnitOfWork()
    {
        // Shared data stores
        _customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "John Doe", Email = "john@example.com", Balance = 1000m },
            new Customer { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Balance = 2000m }
        };

        _invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerId = 1, Amount = 500m, InvoiceDate = DateTime.Now.AddDays(-10), IsPaid = false },
            new Invoice { Id = 2, CustomerId = 2, Amount = 750m, InvoiceDate = DateTime.Now.AddDays(-5), IsPaid = false }
        };

        _payments = new List<Payment>();
        _auditLogs = new List<AuditLog>();
        _operations = new List<Action>();

        // Initialize repositories with shared data
        Customers = new CustomerRepository(_customers);
        Invoices = new UoWInvoiceRepository(_invoices);
        Payments = new PaymentRepository(_payments);
        AuditLogs = new AuditLogRepository(_auditLogs);
    }

    public void BeginTransaction()
    {
        _isInTransaction = true;
        _operations.Clear();
        Console.WriteLine("[UoW] Transaction started");
    }

    public async Task<int> CommitAsync()
    {
        if (!_isInTransaction)
        {
            throw new InvalidOperationException("No active transaction");
        }

        try
        {
            Console.WriteLine($"[UoW] Committing transaction with {_operations.Count} pending operations");
            await Task.Delay(50); // Simulate database commit
            _isInTransaction = false;
            _operations.Clear();
            Console.WriteLine("[UoW] Transaction committed successfully");
            return 1; // Success
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UoW] Commit failed: {ex.Message}");
            await RollbackAsync();
            throw;
        }
    }

    public async Task RollbackAsync()
    {
        Console.WriteLine("[UoW] Rolling back transaction");
        await Task.Delay(20);
        _isInTransaction = false;
        _operations.Clear();
        Console.WriteLine("[UoW] Transaction rolled back");
    }

    public void Dispose()
    {
        if (_isInTransaction)
        {
            RollbackAsync().Wait();
        }
    }
}

#endregion

#region Example 2: Banking Service Using Unit of Work

public class BankingService
{
    private readonly UnitOfWork _unitOfWork;

    public BankingService(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // Process payment with multiple coordinated operations
    public async Task<bool> ProcessPaymentAsync(int invoiceId, decimal amount, string paymentMethod)
    {
        _unitOfWork.BeginTransaction();

        try
        {
            // 1. Get invoice
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                throw new InvalidOperationException($"Invoice {invoiceId} not found");
            }

            if (invoice.IsPaid)
            {
                throw new InvalidOperationException($"Invoice {invoiceId} is already paid");
            }

            // 2. Get customer
            var customer = await _unitOfWork.Customers.GetByIdAsync(invoice.CustomerId);
            if (customer == null)
            {
                throw new InvalidOperationException($"Customer {invoice.CustomerId} not found");
            }

            // 3. Validate amount
            if (amount != invoice.Amount)
            {
                throw new InvalidOperationException($"Payment amount ${amount} doesn't match invoice amount ${invoice.Amount}");
            }

            // 4. Check customer balance
            if (customer.Balance < amount)
            {
                throw new InvalidOperationException($"Insufficient balance. Customer has ${customer.Balance}, needs ${amount}");
            }

            // 5. Create payment record
            var payment = new Payment
            {
                InvoiceId = invoiceId,
                CustomerId = customer.Id,
                Amount = amount,
                PaymentDate = DateTime.Now,
                PaymentMethod = paymentMethod
            };
            await _unitOfWork.Payments.AddAsync(payment);

            // 6. Update invoice status
            invoice.IsPaid = true;
            await _unitOfWork.Invoices.UpdateAsync(invoice);

            // 7. Update customer balance
            customer.Balance -= amount;
            await _unitOfWork.Customers.UpdateAsync(customer);

            // 8. Add audit log
            await _unitOfWork.AuditLogs.AddAsync(new AuditLog
            {
                Action = "PaymentProcessed",
                EntityType = "Payment",
                EntityId = payment.Id,
                Details = $"Payment of ${amount} processed for invoice {invoiceId}"
            });

            // Commit all changes atomically
            await _unitOfWork.CommitAsync();

            Console.WriteLine($"Payment processed: ${amount} from customer {customer.Name}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Payment processing failed: {ex.Message}");
            await _unitOfWork.RollbackAsync();
            return false;
        }
    }

    // Transfer balance between customers
    public async Task<bool> TransferBalanceAsync(int fromCustomerId, int toCustomerId, decimal amount)
    {
        _unitOfWork.BeginTransaction();

        try
        {
            // Get both customers
            var fromCustomer = await _unitOfWork.Customers.GetByIdAsync(fromCustomerId);
            var toCustomer = await _unitOfWork.Customers.GetByIdAsync(toCustomerId);

            if (fromCustomer == null || toCustomer == null)
            {
                throw new InvalidOperationException("One or both customers not found");
            }

            // Validate balance
            if (fromCustomer.Balance < amount)
            {
                throw new InvalidOperationException($"Insufficient balance for transfer");
            }

            // Update balances
            fromCustomer.Balance -= amount;
            toCustomer.Balance += amount;

            await _unitOfWork.Customers.UpdateAsync(fromCustomer);
            await _unitOfWork.Customers.UpdateAsync(toCustomer);

            // Audit logs
            await _unitOfWork.AuditLogs.AddAsync(new AuditLog
            {
                Action = "BalanceTransfer",
                EntityType = "Customer",
                EntityId = fromCustomerId,
                Details = $"Transferred ${amount} to customer {toCustomerId}"
            });

            await _unitOfWork.AuditLogs.AddAsync(new AuditLog
            {
                Action = "BalanceTransfer",
                EntityType = "Customer",
                EntityId = toCustomerId,
                Details = $"Received ${amount} from customer {fromCustomerId}"
            });

            // Commit transaction
            await _unitOfWork.CommitAsync();

            Console.WriteLine($"Transfer successful: ${amount} from {fromCustomer.Name} to {toCustomer.Name}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Transfer failed: {ex.Message}");
            await _unitOfWork.RollbackAsync();
            return false;
        }
    }
}

#endregion

#region Example 3: E-Commerce Order Service

public class OrderItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class OrderProcessingService
{
    private readonly UnitOfWork _unitOfWork;

    public OrderProcessingService(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> CreateOrderWithInvoiceAsync(int customerId, List<OrderItem> items)
    {
        _unitOfWork.BeginTransaction();

        try
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new InvalidOperationException($"Customer {customerId} not found");
            }

            // Calculate total
            var total = items.Sum(i => i.Price * i.Quantity);

            // Create invoice
            var invoice = new Invoice
            {
                CustomerId = customerId,
                Amount = total,
                InvoiceDate = DateTime.Now,
                IsPaid = false
            };
            await _unitOfWork.Invoices.AddAsync(invoice);

            // Log order creation
            await _unitOfWork.AuditLogs.AddAsync(new AuditLog
            {
                Action = "OrderCreated",
                EntityType = "Invoice",
                EntityId = invoice.Id,
                Details = $"Order created for {customer.Name} with {items.Count} items, total ${total:F2}"
            });

            // Log each item
            foreach (var item in items)
            {
                await _unitOfWork.AuditLogs.AddAsync(new AuditLog
                {
                    Action = "OrderItemAdded",
                    EntityType = "Invoice",
                    EntityId = invoice.Id,
                    Details = $"{item.Quantity}x {item.ProductName} @ ${item.Price}"
                });
            }

            await _unitOfWork.CommitAsync();

            Console.WriteLine($"Order created successfully. Invoice ID: {invoice.Id}, Total: ${total:F2}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Order creation failed: {ex.Message}");
            await _unitOfWork.RollbackAsync();
            return false;
        }
    }
}

#endregion

#region Demo

public static class UnitOfWorkDemo
{
    public static async Task Run()
    {
        Console.WriteLine("=== Unit of Work Pattern Demo ===\n");

        Console.WriteLine("--- Example 1: Payment Processing ---");
        await PaymentProcessingExample();

        Console.WriteLine("\n--- Example 2: Balance Transfer ---");
        await BalanceTransferExample();

        Console.WriteLine("\n--- Example 3: Order with Invoice Creation ---");
        await OrderCreationExample();

        Console.WriteLine("\n--- Example 4: Rollback on Error ---");
        await RollbackExample();
    }

    private static async Task PaymentProcessingExample()
    {
        var unitOfWork = new UnitOfWork();
        var bankingService = new BankingService(unitOfWork);

        // Display initial state
        var customer = await unitOfWork.Customers.GetByIdAsync(1);
        var invoice = await unitOfWork.Invoices.GetByIdAsync(1);
        Console.WriteLine($"Customer: {customer!.Name}, Balance: ${customer.Balance}");
        Console.WriteLine($"Invoice: #{invoice!.Id}, Amount: ${invoice.Amount}, Paid: {invoice.IsPaid}");

        // Process payment
        var success = await bankingService.ProcessPaymentAsync(1, 500m, "Credit Card");

        // Display final state
        customer = await unitOfWork.Customers.GetByIdAsync(1);
        invoice = await unitOfWork.Invoices.GetByIdAsync(1);
        Console.WriteLine($"\nAfter payment:");
        Console.WriteLine($"Customer Balance: ${customer!.Balance}");
        Console.WriteLine($"Invoice Paid: {invoice!.IsPaid}");
    }

    private static async Task BalanceTransferExample()
    {
        var unitOfWork = new UnitOfWork();
        var bankingService = new BankingService(unitOfWork);

        var customer1 = await unitOfWork.Customers.GetByIdAsync(1);
        var customer2 = await unitOfWork.Customers.GetByIdAsync(2);

        Console.WriteLine($"Before transfer:");
        Console.WriteLine($"  {customer1!.Name}: ${customer1.Balance}");
        Console.WriteLine($"  {customer2!.Name}: ${customer2.Balance}");

        await bankingService.TransferBalanceAsync(1, 2, 200m);

        customer1 = await unitOfWork.Customers.GetByIdAsync(1);
        customer2 = await unitOfWork.Customers.GetByIdAsync(2);

        Console.WriteLine($"\nAfter transfer:");
        Console.WriteLine($"  {customer1!.Name}: ${customer1.Balance}");
        Console.WriteLine($"  {customer2!.Name}: ${customer2.Balance}");
    }

    private static async Task OrderCreationExample()
    {
        var unitOfWork = new UnitOfWork();
        var orderService = new OrderProcessingService(unitOfWork);

        var items = new List<OrderItem>
        {
            new() { ProductId = 1, ProductName = "Laptop", Price = 999.99m, Quantity = 1 },
            new() { ProductId = 2, ProductName = "Mouse", Price = 29.99m, Quantity = 2 }
        };

        await orderService.CreateOrderWithInvoiceAsync(1, items);

        // Show audit logs
        var logs = await unitOfWork.AuditLogs.GetRecentLogsAsync(5);
        Console.WriteLine("\nRecent audit logs:");
        foreach (var log in logs)
        {
            Console.WriteLine($"  [{log.Timestamp:HH:mm:ss}] {log.Action}: {log.Details}");
        }
    }

    private static async Task RollbackExample()
    {
        var unitOfWork = new UnitOfWork();
        var bankingService = new BankingService(unitOfWork);

        var customer = await unitOfWork.Customers.GetByIdAsync(1);
        var initialBalance = customer!.Balance;

        Console.WriteLine($"Initial balance: ${initialBalance}");

        // Try to process payment with insufficient amount (should fail and rollback)
        var invoice = await unitOfWork.Invoices.GetByIdAsync(1);
        await bankingService.ProcessPaymentAsync(1, invoice!.Amount + 10000m, "Credit Card");

        customer = await unitOfWork.Customers.GetByIdAsync(1);
        Console.WriteLine($"Balance after failed payment: ${customer!.Balance}");
        Console.WriteLine($"Balance unchanged: {customer.Balance == initialBalance}");
    }
}

#endregion
