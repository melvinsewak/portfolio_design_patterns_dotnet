# Unit of Work Pattern

## Intent
The Unit of Work Pattern maintains a list of objects affected by a business transaction and coordinates the writing out of changes and the resolution of concurrency problems. It treats a series of operations as a single transaction, ensuring all operations succeed together or fail together (ACID properties).

## Problem
When working with multiple repositories or data operations, several issues arise:
- **Multiple database round trips** - each repository saves independently
- **Transaction management complexity** - hard to ensure atomicity
- **Inconsistent state** - partial updates when one operation fails
- **No centralized commit** - calling SaveChanges() scattered throughout code
- **Difficult rollback** - hard to undo multiple operations
- **Concurrency issues** - race conditions with multiple updates
- **Resource management** - database connections not properly managed

Example of problematic code:
```csharp
public async Task ProcessOrderAsync(Order order)
{
    await _orderRepository.AddAsync(order);        // Saves immediately
    await _inventoryRepository.UpdateAsync(item);  // Another save
    await _paymentRepository.AddAsync(payment);    // Third save
    // What if payment fails? Order and inventory already saved!
}
```

## Solution
Unit of Work Pattern provides:
1. **Single transaction** for multiple operations
2. **Coordinated commit** - all changes saved together
3. **Rollback capability** - undo all changes on failure
4. **Change tracking** - monitors modified objects
5. **Optimized database calls** - batch operations together

Key principles:
- One transaction per business operation
- All repositories share same context
- Explicit commit/rollback control
- Ensures data consistency

## Structure

```
┌─────────────────────────────────────────────────────────────┐
│                    Service Layer                             │
│                                                              │
│  public async Task ProcessOrder()                            │
│  {                                                           │
│      unitOfWork.BeginTransaction();                          │
│      await unitOfWork.Orders.AddAsync(order);                │
│      await unitOfWork.Inventory.UpdateAsync(item);           │
│      await unitOfWork.Payments.AddAsync(payment);            │
│      await unitOfWork.CommitAsync();  // Single commit       │
│  }                                                           │
└─────────────────────────────────────────────────────────────┘
                         │
                         │ Uses
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                   IUnitOfWork Interface                      │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  IUnitOfWork                                          │  │
│  │  ┌────────────────────────────────────────────────┐  │  │
│  │  │ + IOrderRepository Orders { get; }             │  │  │
│  │  │ + IInventoryRepository Inventory { get; }      │  │  │
│  │  │ + IPaymentRepository Payments { get; }         │  │  │
│  │  │ + void BeginTransaction()                      │  │  │
│  │  │ + Task<int> CommitAsync()                      │  │  │
│  │  │ + Task RollbackAsync()                         │  │  │
│  │  │ + void Dispose()                               │  │  │
│  │  └────────────────────────────────────────────────┘  │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                         ▲
                         │ Implements
                         │
┌─────────────────────────────────────────────────────────────┐
│                  UnitOfWork Implementation                   │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  UnitOfWork                                           │  │
│  │  ┌────────────────────────────────────────────────┐  │  │
│  │  │ - DbContext _context                           │  │  │
│  │  │ - DbTransaction _transaction                   │  │  │
│  │  │ - IOrderRepository _orders                     │  │  │
│  │  │ - IInventoryRepository _inventory              │  │  │
│  │  │                                                 │  │  │
│  │  │ + BeginTransaction()                           │  │  │
│  │  │   {                                            │  │  │
│  │  │       _transaction = _context.BeginTransaction()│  │  │
│  │  │   }                                            │  │  │
│  │  │                                                 │  │  │
│  │  │ + CommitAsync()                                │  │  │
│  │  │   {                                            │  │  │
│  │  │       await _context.SaveChangesAsync()       │  │  │
│  │  │       await _transaction.CommitAsync()        │  │  │
│  │  │   }                                            │  │  │
│  │  └────────────────────────────────────────────────┘  │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                         │
                         │ Shares with repositories
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    Repositories                              │
│                                                              │
│  ┌──────────────┐  ┌────────────────┐  ┌────────────────┐ │
│  │OrderRepository│  │InventoryRepo   │  │PaymentRepo     │ │
│  │              │  │                │  │                │ │
│  │Uses same     │  │Uses same       │  │Uses same       │ │
│  │DbContext     │  │DbContext       │  │DbContext       │ │
│  └──────────────┘  └────────────────┘  └────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                         │
                         │ All share
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                  Shared DbContext/Transaction                │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  DbContext                                            │  │
│  │  • Change Tracker                                     │  │
│  │  • Transaction                                        │  │
│  │  • Connection                                         │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## Key Components

### 1. Unit of Work Interface
```csharp
public interface IUnitOfWork : IDisposable
{
    IOrderRepository Orders { get; }
    ICustomerRepository Customers { get; }
    IPaymentRepository Payments { get; }
    
    void BeginTransaction();
    Task<int> CommitAsync();
    Task RollbackAsync();
}
```

### 2. Unit of Work Implementation
```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private IDbContextTransaction? _transaction;
    
    public UnitOfWork(DbContext context)
    {
        _context = context;
        Orders = new OrderRepository(_context);
        Customers = new CustomerRepository(_context);
    }
    
    public void BeginTransaction()
    {
        _transaction = _context.Database.BeginTransaction();
    }
    
    public async Task<int> CommitAsync()
    {
        try
        {
            var result = await _context.SaveChangesAsync();
            await _transaction?.CommitAsync();
            return result;
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
    }
    
    public async Task RollbackAsync()
    {
        await _transaction?.RollbackAsync();
        _transaction?.Dispose();
    }
}
```

### 3. Repositories Sharing Context
```csharp
public class OrderRepository : IOrderRepository
{
    private readonly DbContext _context;  // Shared context
    
    public OrderRepository(DbContext context)
    {
        _context = context;
    }
}
```

## When to Use

### Use Unit of Work Pattern When:
- ✅ You need **multiple database operations** in a single transaction
- ✅ Working with **complex business operations** affecting multiple entities
- ✅ You need **ACID guarantees** (Atomicity, Consistency, Isolation, Durability)
- ✅ Building **financial or banking applications** requiring strict consistency
- ✅ You want to **batch operations** for performance
- ✅ Using **multiple repositories** that need coordinated saves
- ✅ Working with **Domain-Driven Design** aggregate roots
- ✅ You need **explicit transaction control**

### Avoid When:
- ❌ Using **Entity Framework Core** alone (DbContext is already a Unit of Work)
- ❌ Building **simple CRUD operations** with single entity updates
- ❌ Working with **read-only scenarios**
- ❌ You're adding **unnecessary abstraction** over EF Core
- ❌ The added complexity **doesn't provide value**
- ❌ Performance requirements favor **micro-transactions**

## Real-World Use Cases

### 1. **Banking Transfer Transaction**
```csharp
public async Task<bool> TransferFundsAsync(int fromAccount, int toAccount, decimal amount)
{
    _unitOfWork.BeginTransaction();
    try
    {
        var from = await _unitOfWork.Accounts.GetByIdAsync(fromAccount);
        var to = await _unitOfWork.Accounts.GetByIdAsync(toAccount);
        
        from.Balance -= amount;
        to.Balance += amount;
        
        await _unitOfWork.Accounts.UpdateAsync(from);
        await _unitOfWork.Accounts.UpdateAsync(to);
        await _unitOfWork.AuditLogs.AddAsync(new AuditLog { ... });
        
        await _unitOfWork.CommitAsync();  // All or nothing
        return true;
    }
    catch
    {
        await _unitOfWork.RollbackAsync();
        return false;
    }
}
```

### 2. **E-Commerce Order Processing**
```csharp
public async Task<Order> PlaceOrderAsync(CreateOrderDto dto)
{
    _unitOfWork.BeginTransaction();
    try
    {
        // Create order
        var order = new Order { CustomerId = dto.CustomerId };
        await _unitOfWork.Orders.AddAsync(order);
        
        // Update inventory for each item
        foreach (var item in dto.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
            product.Stock -= item.Quantity;
            await _unitOfWork.Products.UpdateAsync(product);
        }
        
        // Create payment record
        var payment = new Payment { OrderId = order.Id, Amount = dto.Total };
        await _unitOfWork.Payments.AddAsync(payment);
        
        // Send notification
        await _unitOfWork.Notifications.AddAsync(new Notification { ... });
        
        await _unitOfWork.CommitAsync();
        return order;
    }
    catch
    {
        await _unitOfWork.RollbackAsync();
        throw;
    }
}
```

### 3. **Multi-Tenant Data Isolation**
```csharp
public class TenantUnitOfWork : IUnitOfWork
{
    private readonly string _tenantId;
    
    public async Task<int> CommitAsync()
    {
        // Ensure all changes are for correct tenant
        ValidateTenantIsolation();
        return await _context.SaveChangesAsync();
    }
}
```

### 4. **Audit Trail with Every Transaction**
```csharp
public async Task<int> CommitAsync()
{
    // Automatically add audit logs for all changes
    var entries = _context.ChangeTracker.Entries()
        .Where(e => e.State == EntityState.Modified);
    
    foreach (var entry in entries)
    {
        await _auditRepository.AddAsync(new AuditLog
        {
            Entity = entry.Entity.GetType().Name,
            Action = "Modified",
            Timestamp = DateTime.Now
        });
    }
    
    return await _context.SaveChangesAsync();
}
```

## Advantages

1. **Transaction Management**
   - All operations succeed or fail together
   - ACID guarantees maintained
   - Prevents partial updates

2. **Centralized Save Logic**
   - Single point for SaveChanges()
   - Easier to add cross-cutting concerns (audit, validation)
   - Consistent transaction handling

3. **Performance Optimization**
   - Batch multiple operations
   - Reduced database round trips
   - Single connection for all operations

4. **Simplified Error Handling**
   - Easy to rollback on errors
   - Consistent error recovery
   - Clean transaction boundaries

5. **Coordinated Repositories**
   - All repositories share same context
   - Consistent view of data
   - No conflicts between repositories

6. **Testing Benefits**
   - Easy to mock unit of work
   - Test transactions in isolation
   - Verify commit/rollback behavior

## Disadvantages

1. **Complexity**
   - Additional abstraction layer
   - More code to write and maintain
   - Learning curve for developers

2. **Redundant with EF Core**
   - DbContext already implements Unit of Work
   - May be unnecessary abstraction
   - Duplicates EF Core functionality

3. **Long-Running Transactions**
   - Can cause database locks
   - Performance issues with large transactions
   - Concurrency problems

4. **Memory Overhead**
   - Tracks all changes in memory
   - Large transactions consume resources
   - Potential for memory leaks

5. **Tight Coupling to Repositories**
   - Unit of Work knows about all repositories
   - Changes require UoW interface updates
   - Violates Open/Closed Principle

## Best Practices

1. **Keep Transactions Short**
   ```csharp
   // ✅ Good - Short transaction
   _uow.BeginTransaction();
   await _uow.Orders.AddAsync(order);
   await _uow.CommitAsync();
   
   // ❌ Bad - Long-running transaction
   _uow.BeginTransaction();
   await LongRunningOperation();  // Minutes or hours
   await _uow.CommitAsync();
   ```

2. **Always Use Try-Catch with Rollback**
   ```csharp
   try
   {
       _uow.BeginTransaction();
       // Operations
       await _uow.CommitAsync();
   }
   catch
   {
       await _uow.RollbackAsync();  // Always rollback on error
       throw;
   }
   ```

3. **Use Using Statement for Disposal**
   ```csharp
   using (var uow = new UnitOfWork())
   {
       uow.BeginTransaction();
       // Operations
       await uow.CommitAsync();
   }  // Automatically disposed
   ```

4. **Don't Nest Unit of Work**
   ```csharp
   // ❌ Bad - Nested UoW
   using (var uow1 = new UnitOfWork())
   {
       using (var uow2 = new UnitOfWork())  // Don't do this
       {
           // Confusing transaction boundaries
       }
   }
   ```

5. **Register as Scoped in DI**
   ```csharp
   // One instance per request/scope
   services.AddScoped<IUnitOfWork, UnitOfWork>();
   ```

6. **Consider Explicit Commits**
   ```csharp
   // ✅ Good - Explicit commit
   await _uow.CommitAsync();
   
   // ❌ Bad - Implicit save in repository
   await repository.SaveAsync();  // Bypasses UoW
   ```

7. **Handle Concurrency Conflicts**
   ```csharp
   try
   {
       await _uow.CommitAsync();
   }
   catch (DbUpdateConcurrencyException ex)
   {
       // Handle optimistic concurrency conflicts
       await _uow.RollbackAsync();
   }
   ```

8. **Separate Read and Write Operations**
   ```csharp
   // Don't need UoW for reads
   var users = await _userRepository.GetAllAsync();
   
   // Use UoW for writes
   _uow.BeginTransaction();
   await _uow.Users.UpdateAsync(user);
   await _uow.CommitAsync();
   ```

## Integration with .NET Ecosystem

### Entity Framework Core Integration
```csharp
public class EfCoreUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public EfCoreUnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Orders = new OrderRepository(_context);
    }
    
    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
}

// Registration
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddScoped<IUnitOfWork, EfCoreUnitOfWork>();
```

### Dapper Integration
```csharp
public class DapperUnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private IDbTransaction? _transaction;
    
    public void BeginTransaction()
    {
        _connection.Open();
        _transaction = _connection.BeginTransaction();
    }
    
    public async Task CommitAsync()
    {
        _transaction?.Commit();
        _connection.Close();
    }
}
```

### ASP.NET Core Integration
```csharp
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    
    public OrdersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
    {
        try
        {
            _unitOfWork.BeginTransaction();
            var order = await ProcessOrder(dto);
            await _unitOfWork.CommitAsync();
            return Ok(order);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            return BadRequest();
        }
    }
}
```

### MediatR Integration (CQRS)
```csharp
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Order> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var order = new Order { ... };
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CommitAsync();
            return order;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
```

## Related Patterns

- **Repository Pattern**: UoW coordinates multiple repositories
- **Transaction Script**: UoW implements transactional operations
- **Identity Map**: Ensures single instance per transaction
- **Lazy Load**: Defers loading until commit
- **Data Mapper**: Separates domain from persistence
- **CQRS**: UoW typically used on command side

## Common Misconceptions

1. **"EF Core needs Unit of Work wrapper"**
   - FALSE: DbContext IS already a Unit of Work
   - Only add wrapper if you need abstraction for testing or multiple data sources

2. **"Always use Unit of Work with Repository"**
   - FALSE: Simple apps don't need both
   - Consider your specific needs

3. **"Unit of Work improves performance automatically"**
   - PARTIALLY TRUE: Batching helps, but can cause locks
   - Long transactions can hurt performance

## Summary

The Unit of Work Pattern ensures that multiple database operations are treated as a single atomic transaction, providing ACID guarantees and preventing inconsistent state. It centralizes transaction management and coordinates multiple repositories.

However, when using Entity Framework Core, be aware that DbContext already implements the Unit of Work pattern. Adding an additional wrapper may be unnecessary unless you need abstraction for testing, want to support multiple data sources, or need to add cross-cutting concerns like auditing.

The pattern is most valuable in complex business scenarios requiring strict transactional consistency, such as financial applications, e-commerce order processing, or any system where partial updates could lead to data corruption. For simpler applications, direct use of DbContext or repositories without an explicit Unit of Work wrapper may be more appropriate.
