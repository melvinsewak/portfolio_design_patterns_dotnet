# CQRS Pattern (Command Query Responsibility Segregation)

## Intent
CQRS (Command Query Responsibility Segregation) is an architectural pattern that separates read operations (Queries) from write operations (Commands). It uses different models for updating information (Commands) than for reading information (Queries), allowing for optimized and scalable data access patterns.

## Problem
Traditional CRUD applications use the same model for both reading and writing data, leading to:
- **Complex models** trying to serve both read and write scenarios
- **Performance bottlenecks** - reads and writes have different optimization needs
- **Scalability issues** - can't scale reads and writes independently
- **Inefficient queries** - write-optimized models aren't ideal for complex reads
- **Conflicting requirements** - DTOs for display vs. validation for updates
- **Difficult optimization** - can't optimize reads without affecting writes
- **Security concerns** - same model exposes write operations in read scenarios

Example of problematic code:
```csharp
public class ProductService
{
    // Same model for both read and write
    public Product GetProduct(int id) { }           // Read
    public void UpdateProduct(Product product) { }  // Write
    // Product model serves both purposes poorly
}
```

## Solution
CQRS separates concerns by:
1. **Commands** - Modify state, don't return data
2. **Queries** - Return data, never modify state
3. **Separate models** - Different models for read and write
4. **Independent optimization** - Optimize each side separately
5. **Scalability** - Scale reads and writes independently

Key principles:
- **Commands**: Task-based operations (CreateOrder, UpdatePrice)
- **Queries**: Data retrieval operations (GetOrderById, SearchProducts)
- **No overlap**: Commands don't return data, queries don't modify data
- **Different stores**: Can use separate read/write databases

## Structure

```
┌─────────────────────────────────────────────────────────────┐
│                        Client/UI                             │
│                                                              │
│  ┌──────────────────┐              ┌──────────────────┐    │
│  │  Send Commands   │              │  Send Queries    │    │
│  │  (Write Intent)  │              │  (Read Intent)   │    │
│  └──────────────────┘              └──────────────────┘    │
└─────────────────────────────────────────────────────────────┘
           │                                    │
           │                                    │
           ▼                                    ▼
┌─────────────────────────┐       ┌─────────────────────────┐
│    Command Side         │       │     Query Side          │
│    (Write Model)        │       │     (Read Model)        │
│                         │       │                         │
│  ┌──────────────────┐  │       │  ┌──────────────────┐  │
│  │ ICommand         │  │       │  │ IQuery<TResult>  │  │
│  │                  │  │       │  │                  │  │
│  │ CreateProduct    │  │       │  │ GetProductById   │  │
│  │ UpdatePrice      │  │       │  │ SearchProducts   │  │
│  │ DeleteProduct    │  │       │  │ GetAllProducts   │  │
│  └──────────────────┘  │       │  └──────────────────┘  │
│           ▼             │       │           ▼             │
│  ┌──────────────────┐  │       │  ┌──────────────────┐  │
│  │ Command Handler  │  │       │  │ Query Handler    │  │
│  │                  │  │       │  │                  │  │
│  │ • Validate       │  │       │  │ • Fetch data     │  │
│  │ • Apply business │  │       │  │ • Transform      │  │
│  │   rules          │  │       │  │ • Return DTO     │  │
│  │ • Persist changes│  │       │  │                  │  │
│  │ • Raise events   │  │       │  │ Fast reads!      │  │
│  └──────────────────┘  │       │  └──────────────────┘  │
│           ▼             │       │           ▼             │
│  ┌──────────────────┐  │       │  ┌──────────────────┐  │
│  │  Write Store     │  │       │  │  Read Store      │  │
│  │  (Normalized)    │  │       │  │  (Denormalized)  │  │
│  │                  │  │       │  │                  │  │
│  │  • Entities      │  │       │  │  • Projections   │  │
│  │  • Relationships │  │       │  │  • Flat views    │  │
│  │  • Constraints   │  │       │  │  • Cached data   │  │
│  └──────────────────┘  │       │  └──────────────────┘  │
└─────────────────────────┘       └─────────────────────────┘
           │                                    ▲
           │                                    │
           └─────── Eventual Consistency ───────┘
                   (Event handlers, sync jobs)
```

## Key Components

### 1. Commands (Write Operations)
```csharp
// Command interface - no return value
public interface ICommand { }

// Specific command
public class CreateProductCommand : ICommand
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

// Command handler
public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    public async Task HandleAsync(CreateProductCommand command)
    {
        // Validate, apply business logic, persist
        var product = new Product { Name = command.Name, ... };
        await _writeStore.AddAsync(product);
    }
}
```

### 2. Queries (Read Operations)
```csharp
// Query interface - returns data
public interface IQuery<TResult> { }

// Specific query
public class GetProductByIdQuery : IQuery<ProductDto>
{
    public int ProductId { get; set; }
}

// Query handler
public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> HandleAsync(GetProductByIdQuery query)
    {
        // Fetch from optimized read store
        return await _readStore.GetProductAsync(query.ProductId);
    }
}
```

### 3. Separate Stores
```csharp
// Write Store - Normalized, transactional
public class ProductWriteStore
{
    public async Task AddAsync(Product product) { }
    public async Task UpdateAsync(Product product) { }
}

// Read Store - Denormalized, optimized for queries
public class ProductReadStore
{
    public async Task<ProductDto> GetProductAsync(int id) { }
    public async Task<IEnumerable<ProductDto>> SearchAsync(string term) { }
}
```

## When to Use

### Use CQRS Pattern When:
- ✅ **Different optimization needs** for reads vs. writes
- ✅ Building **high-traffic applications** needing separate scaling
- ✅ **Complex business logic** on write side, simple reads
- ✅ Working with **event-driven architectures**
- ✅ You need **different security models** for read/write
- ✅ Building **collaborative systems** with many concurrent users
- ✅ Read and write **workloads are vastly different**
- ✅ Using **Event Sourcing** (CQRS is a natural fit)
- ✅ You want to **optimize queries independently**

### Avoid When:
- ❌ Building **simple CRUD applications**
- ❌ Reads and writes have **similar complexity**
- ❌ You don't need **independent scaling**
- ❌ The added **complexity isn't justified**
- ❌ Your team is **unfamiliar with the pattern**
- ❌ You're working on a **small project** or MVP
- ❌ **Immediate consistency** is required everywhere

## Real-World Use Cases

### 1. **E-Commerce Product Catalog**
```csharp
// Commands - Update product information
public class UpdateProductPriceCommand : ICommand
{
    public int ProductId { get; set; }
    public decimal NewPrice { get; set; }
}

// Queries - Fast product searches
public class SearchProductsQuery : IQuery<IEnumerable<ProductDto>>
{
    public string SearchTerm { get; set; }
    public string Category { get; set; }
    public decimal? MaxPrice { get; set; }
}

// Read model optimized for search
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool InStock { get; set; }
    public string ImageUrl { get; set; }  // Denormalized for fast display
}
```

### 2. **Social Media Feed**
```csharp
// Command - Create post
public class CreatePostCommand : ICommand
{
    public string UserId { get; set; }
    public string Content { get; set; }
    public List<string> Tags { get; set; }
}

// Query - Get personalized feed (complex, optimized read)
public class GetUserFeedQuery : IQuery<IEnumerable<PostDto>>
{
    public string UserId { get; set; }
    public int PageSize { get; set; }
}

// Read model includes denormalized data for fast feeds
public class PostDto
{
    public string PostId { get; set; }
    public string AuthorName { get; set; }      // Denormalized
    public string AuthorAvatar { get; set; }    // Denormalized
    public string Content { get; set; }
    public int LikesCount { get; set; }         // Pre-calculated
    public int CommentsCount { get; set; }      // Pre-calculated
}
```

### 3. **Banking System**
```csharp
// Commands - Money transfers (complex validation)
public class TransferFundsCommand : ICommand
{
    public string FromAccount { get; set; }
    public string ToAccount { get; set; }
    public decimal Amount { get; set; }
}

// Queries - Account statements (optimized reads)
public class GetAccountStatementQuery : IQuery<StatementDto>
{
    public string AccountId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

// Read model with pre-calculated balances
public class StatementDto
{
    public string AccountId { get; set; }
    public decimal CurrentBalance { get; set; }
    public List<TransactionDto> Transactions { get; set; }
}
```

### 4. **Order Management**
```csharp
// Command - Place order (validation, inventory check, payment)
public class PlaceOrderCommand : ICommand
{
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
    public PaymentInfo Payment { get; set; }
}

// Query - Order history (fast, cached)
public class GetOrderHistoryQuery : IQuery<IEnumerable<OrderSummaryDto>>
{
    public int CustomerId { get; set; }
}

// Denormalized read model
public class OrderSummaryDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; }
    public int ItemCount { get; set; }  // Pre-calculated
}
```

## Advantages

1. **Independent Scalability**
   - Scale read and write databases separately
   - More read replicas for query-heavy apps
   - Smaller write database for transactional operations

2. **Performance Optimization**
   - Optimize write model for transactions and consistency
   - Optimize read model for query performance (denormalization, caching)
   - Different database technologies (SQL for writes, NoSQL for reads)

3. **Simplified Models**
   - Write model focuses on business logic and validation
   - Read model focuses on efficient data retrieval
   - No compromise between read and write needs

4. **Security**
   - Separate permissions for commands and queries
   - Read-only users can't execute commands
   - Fine-grained access control

5. **Flexibility**
   - Different databases for read and write
   - Multiple read models for different views
   - Easy to add new query models without affecting writes

6. **Clear Intent**
   - Commands express business intent (PlaceOrder vs. UpdateOrder)
   - Queries clearly define data needs
   - Better documentation of system behavior

7. **Event-Driven Integration**
   - Natural fit for Event Sourcing
   - Easy to publish domain events
   - Supports eventual consistency

## Disadvantages

1. **Increased Complexity**
   - More code (commands, queries, handlers, models)
   - Steeper learning curve
   - More files to maintain

2. **Eventual Consistency**
   - Read model may be slightly behind write model
   - Need to handle stale data scenarios
   - Complex synchronization logic

3. **Data Duplication**
   - Same data stored in multiple formats
   - Synchronization overhead
   - Storage costs

4. **More Infrastructure**
   - May need message queues for sync
   - Multiple databases to manage
   - Deployment complexity

5. **Debugging Challenges**
   - Harder to trace through separate handlers
   - Async operations complicate troubleshooting
   - More moving parts

6. **Overkill for Simple Apps**
   - Not justified for basic CRUD
   - Adds unnecessary complexity
   - Slower initial development

## Best Practices

1. **Commands Don't Return Data**
   ```csharp
   // ✅ Good - Command returns nothing
   public interface ICommandHandler<TCommand>
   {
       Task HandleAsync(TCommand command);
   }
   
   // ❌ Bad - Command returns data
   public interface ICommandHandler<TCommand, TResult>
   {
       Task<TResult> HandleAsync(TCommand command);
   }
   ```

2. **Queries Never Modify State**
   ```csharp
   // ✅ Good - Pure query
   public class GetProductsQueryHandler
   {
       public async Task<IEnumerable<ProductDto>> HandleAsync(GetProductsQuery query)
       {
           return await _readStore.GetProductsAsync();  // Read only
       }
   }
   
   // ❌ Bad - Query modifies data
   public async Task<ProductDto> HandleAsync(GetProductQuery query)
   {
       var product = await _readStore.GetProductAsync(query.Id);
       product.ViewCount++;  // DON'T DO THIS
       return product;
   }
   ```

3. **Use Task-Based Command Names**
   ```csharp
   // ✅ Good - Business intent clear
   CreateOrderCommand
   CancelOrderCommand
   UpdateProductPriceCommand
   
   // ❌ Bad - Generic names
   OrderCommand
   ProductCommand
   ```

4. **Keep Handlers Focused**
   ```csharp
   // ✅ Good - Single responsibility
   public class CreateProductCommandHandler
   {
       public async Task HandleAsync(CreateProductCommand command)
       {
           // Only handles product creation
       }
   }
   
   // ❌ Bad - Handler does too much
   public class ProductCommandHandler
   {
       public async Task HandleAsync(ICommand command)
       {
           if (command is CreateProductCommand) { }
           else if (command is UpdateProductCommand) { }
           // Too many responsibilities
       }
   }
   ```

5. **Denormalize Read Models**
   ```csharp
   // ✅ Good - Denormalized for fast reads
   public class OrderDto
   {
       public int OrderId { get; set; }
       public string CustomerName { get; set; }      // Denormalized
       public string CustomerEmail { get; set; }     // Denormalized
       public List<OrderItemDto> Items { get; set; } // Everything in one query
   }
   ```

6. **Use MediatR or Similar Libraries**
   ```csharp
   // Simplifies command/query dispatching
   await _mediator.Send(new CreateProductCommand { ... });
   var product = await _mediator.Send(new GetProductByIdQuery { Id = 1 });
   ```

7. **Handle Eventual Consistency**
   ```csharp
   // Show user feedback while syncing
   public async Task<IActionResult> CreateOrder(CreateOrderCommand command)
   {
       await _mediator.Send(command);
       return Ok(new { Message = "Order is being processed..." });
       // Don't immediately query - may not be in read store yet
   }
   ```

8. **Consider Using Separate Databases**
   ```csharp
   // Write database - SQL Server (ACID, consistency)
   services.AddDbContext<WriteDbContext>(options =>
       options.UseSqlServer(writeConnectionString));
   
   // Read database - MongoDB (fast queries, denormalized)
   services.AddSingleton<IMongoClient>(new MongoClient(readConnectionString));
   ```

## Integration with .NET Ecosystem

### MediatR Integration
```csharp
// Install MediatR
// Install-Package MediatR
// Install-Package MediatR.Extensions.Microsoft.DependencyInjection

// Commands
public class CreateProductCommand : IRequest
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
{
    public async Task<Unit> Handle(CreateProductCommand request, CancellationToken ct)
    {
        // Handle command
        return Unit.Value;
    }
}

// Queries
public class GetProductQuery : IRequest<ProductDto>
{
    public int ProductId { get; set; }
}

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken ct)
    {
        // Handle query
        return new ProductDto();
    }
}

// Registration
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Usage
await _mediator.Send(new CreateProductCommand { ... });
var product = await _mediator.Send(new GetProductQuery { ProductId = 1 });
```

### ASP.NET Core Integration
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    // Command endpoint
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        await _mediator.Send(command);
        return Accepted();  // Command returns nothing
    }
    
    // Query endpoint
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> Get(int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery { ProductId = id });
        return Ok(product);
    }
}
```

### Event Sourcing Integration
```csharp
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
{
    private readonly IEventStore _eventStore;
    
    public async Task<Unit> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var @event = new ProductCreatedEvent
        {
            ProductId = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price
        };
        
        await _eventStore.AppendAsync(@event);
        return Unit.Value;
    }
}
```

### SignalR for Real-Time Updates
```csharp
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
{
    private readonly IHubContext<OrderHub> _hubContext;
    
    public async Task<Unit> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        // Process order
        
        // Notify clients in real-time
        await _hubContext.Clients.All.SendAsync("OrderCreated", orderId);
        return Unit.Value;
    }
}
```

## Related Patterns

- **Event Sourcing**: CQRS is often used with Event Sourcing
- **Repository Pattern**: Separate read and write repositories
- **Mediator Pattern**: MediatR dispatches commands and queries
- **Domain-Driven Design**: CQRS aligns with DDD aggregates
- **Event-Driven Architecture**: Commands can raise domain events
- **Saga Pattern**: Coordinates multiple commands in distributed systems

## Summary

CQRS (Command Query Responsibility Segregation) is a powerful architectural pattern that separates read and write operations, allowing for independent optimization and scaling. By using different models for commands (writes) and queries (reads), applications can achieve better performance, scalability, and maintainability.

The pattern is most beneficial in complex, high-traffic applications where read and write workloads differ significantly. It enables independent scaling, optimized data models, and clear separation of concerns. However, it introduces complexity through eventual consistency, data duplication, and additional infrastructure requirements.

For simple CRUD applications, CQRS may be overkill. But for enterprise systems, microservices, event-driven architectures, and applications requiring high scalability, CQRS provides significant benefits. When combined with tools like MediatR, Event Sourcing, and modern .NET features, CQRS becomes a natural fit for building robust, scalable applications.
