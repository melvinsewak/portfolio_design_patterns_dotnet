# Repository Pattern

## Intent
The Repository Pattern mediates between the domain and data mapping layers, acting like an in-memory collection of domain objects. It provides a more object-oriented view of the persistence layer and encapsulates the logic required to access data sources.

## Problem
Direct data access in application code leads to several issues:
- **Scattered data access logic** throughout the application
- **Tight coupling** to specific database technologies (SQL Server, MongoDB, etc.)
- **Difficult testing** - can't easily mock database operations
- **Code duplication** - same queries repeated in multiple places
- **Business logic mixed with data access** code
- **Hard to change data sources** without affecting business logic
- **No centralized location** for data access modifications

Example of problematic code:
```csharp
public class OrderService
{
    public Order GetOrder(int id)
    {
        using var connection = new SqlConnection(connectionString);
        var command = new SqlCommand("SELECT * FROM Orders WHERE Id = @Id", connection);
        // Direct SQL in business logic - hard to test, maintain, change
    }
}
```

## Solution
The Repository Pattern provides:
1. **Abstraction layer** over data access
2. **Collection-like interface** for accessing domain objects
3. **Centralized data access logic** in repository classes
4. **Separation of concerns** between business and data layers
5. **Testability** through interface-based design

Key benefits:
- Business logic doesn't know about database details
- Easy to swap data sources (SQL → NoSQL, file system, cloud storage)
- Consistent API for data operations across the application
- Simplified unit testing with mock repositories

## Structure

```
┌─────────────────────────────────────────────────────────────┐
│                   Business/Service Layer                     │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  OrderService                                         │  │
│  │  • ProcessOrder()                                     │  │
│  │  • CancelOrder()                                      │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                         │
                         │ Uses
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                  Repository Interface                        │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  IRepository<T>                                       │  │
│  │  • GetByIdAsync(int id)                              │  │
│  │  • GetAllAsync()                                     │  │
│  │  • AddAsync(T entity)                                │  │
│  │  • UpdateAsync(T entity)                             │  │
│  │  • DeleteAsync(int id)                               │  │
│  │  • FindAsync(Expression<Func<T, bool>>)              │  │
│  └──────────────────────────────────────────────────────┘  │
│                         ▲                                    │
│                         │                                    │
│  ┌──────────────────────┴───────────────────────────────┐  │
│  │  IUserRepository : IRepository<User>                  │  │
│  │  • GetByUsernameAsync(string username)               │  │
│  │  • GetActiveUsersAsync()                             │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                         ▲
                         │ Implements
                         │
┌─────────────────────────────────────────────────────────────┐
│                Repository Implementation                     │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  UserRepository : IUserRepository                     │  │
│  │  • Data access implementation                         │  │
│  │  • Query logic                                        │  │
│  │  • Database operations                                │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                         │
                         │ Accesses
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    Data Source                               │
│                                                              │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐           │
│  │ SQL Server │  │  MongoDB   │  │   Cache    │           │
│  └────────────┘  └────────────┘  └────────────┘           │
└─────────────────────────────────────────────────────────────┘
```

## Repository Types

### 1. Generic Repository
**Provides common CRUD operations for all entities**
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

### 2. Specific Repository
**Extends generic repository with entity-specific methods**
```csharp
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetActiveUsersAsync();
}
```

### 3. Read-Only Repository
**For query-only scenarios (CQRS pattern)**
```csharp
public interface IReadOnlyRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
}
```

## When to Use

### Use Repository Pattern When:
- ✅ You want to **decouple business logic from data access**
- ✅ Building applications that may **switch data sources** (SQL, NoSQL, cloud)
- ✅ You need **testable code** with mock repositories
- ✅ Working with **Domain-Driven Design** (DDD)
- ✅ You want **centralized data access logic**
- ✅ Building **enterprise applications** with complex data access
- ✅ You need to **cache or optimize** data access in one place
- ✅ Multiple services need to **access the same data**

### Avoid When:
- ❌ Using **Entity Framework Core with DbContext** (it's already a repository)
- ❌ Building very **simple CRUD applications**
- ❌ You're creating **unnecessary abstraction** over ORMs
- ❌ The pattern adds **complexity without value**
- ❌ Working with **read-only or reporting scenarios** (use queries directly)
- ❌ Your team is **unfamiliar with the pattern** and time is constrained

## Real-World Use Cases

### 1. **E-Commerce Application**
```csharp
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    
    public async Task<Order> CreateOrderAsync(int userId, List<int> productIds)
    {
        var products = await _productRepository.GetByIdsAsync(productIds);
        var order = new Order { UserId = userId, Products = products };
        await _orderRepository.AddAsync(order);
        return order;
    }
}
```

### 2. **Multi-Tenant SaaS Application**
```csharp
public class TenantUserRepository : IUserRepository
{
    private readonly string _tenantId;
    private readonly DbContext _context;
    
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        // Automatically filters by tenant
        return await _context.Users
            .Where(u => u.TenantId == _tenantId)
            .ToListAsync();
    }
}
```

### 3. **Caching Layer**
```csharp
public class CachedUserRepository : IUserRepository
{
    private readonly IUserRepository _innerRepository;
    private readonly ICache _cache;
    
    public async Task<User?> GetByIdAsync(int id)
    {
        var cacheKey = $"user:{id}";
        var cached = await _cache.GetAsync<User>(cacheKey);
        if (cached != null) return cached;
        
        var user = await _innerRepository.GetByIdAsync(id);
        await _cache.SetAsync(cacheKey, user);
        return user;
    }
}
```

### 4. **Hybrid Data Sources**
```csharp
public class HybridProductRepository : IProductRepository
{
    private readonly SqlProductRepository _sqlRepo;
    private readonly RedisProductRepository _cacheRepo;
    
    public async Task<Product?> GetByIdAsync(int id)
    {
        // Try cache first, fallback to database
        return await _cacheRepo.GetByIdAsync(id) 
            ?? await _sqlRepo.GetByIdAsync(id);
    }
}
```

## Advantages

1. **Separation of Concerns**
   - Business logic separated from data access
   - Domain objects don't depend on database

2. **Testability**
   - Easy to create mock repositories for unit tests
   - No need for actual database in tests

3. **Centralized Data Logic**
   - All data access in one place
   - Easy to modify queries globally

4. **Flexibility**
   - Easy to swap data sources (SQL → NoSQL)
   - Can add caching, logging, validation in one place

5. **Reusability**
   - Same repository used across multiple services
   - Consistent API for data operations

6. **Domain-Driven Design Support**
   - Repositories work with aggregate roots
   - Supports DDD patterns and practices

7. **Query Optimization**
   - Centralized location for performance tuning
   - Can implement query caching strategies

## Disadvantages

1. **Additional Abstraction Layer**
   - More code to write and maintain
   - Can be over-engineering for simple apps

2. **Potential for Generic Repository Anti-Pattern**
   - Generic repositories may expose too much functionality
   - Can lead to repositories with hundreds of methods

3. **Complexity**
   - Learning curve for developers
   - More files and interfaces

4. **Performance Concerns**
   - Additional layer may impact performance slightly
   - Need to be careful with eager/lazy loading

5. **Redundancy with ORMs**
   - Entity Framework Core's DbContext is already a repository
   - May create unnecessary abstraction

6. **Maintenance Overhead**
   - Need to keep repository interfaces in sync with requirements
   - Changes to data model may require repository updates

## Best Practices

1. **Use Interfaces**
   ```csharp
   // ✅ Good - Interface-based
   public interface IUserRepository { }
   public class UserRepository : IUserRepository { }
   
   // ❌ Bad - Concrete class only
   public class UserRepository { }
   ```

2. **Avoid Generic Repository Anti-Pattern**
   ```csharp
   // ❌ Bad - Too generic
   IRepository<User> repo;
   
   // ✅ Good - Specific interface with domain methods
   public interface IUserRepository : IRepository<User>
   {
       Task<User?> GetByEmailAsync(string email);
   }
   ```

3. **Return Domain Objects, Not DTOs**
   ```csharp
   // ✅ Good - Returns domain entity
   Task<User> GetByIdAsync(int id);
   
   // ❌ Bad - Repository returns DTO
   Task<UserDto> GetByIdAsync(int id);  // DTOs belong in service layer
   ```

4. **Use Async Methods**
   ```csharp
   Task<IEnumerable<User>> GetAllAsync();  // ✅ Async
   IEnumerable<User> GetAll();             // ❌ Synchronous
   ```

5. **Keep Business Logic Out**
   ```csharp
   // ❌ Bad - Business logic in repository
   public async Task<bool> CanUserPlaceOrder(int userId)
   {
       var user = await GetByIdAsync(userId);
       return user.Balance > 0 && user.IsVerified;  // Business rule
   }
   
   // ✅ Good - Data access only
   public async Task<User?> GetByIdAsync(int userId)
   {
       return await _context.Users.FindAsync(userId);
   }
   ```

6. **Use Expression<Func<T, bool>> for Flexible Queries**
   ```csharp
   Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate);
   
   // Usage:
   await repo.FindAsync(u => u.Age > 18 && u.IsActive);
   ```

7. **Consider Unit of Work Pattern**
   ```csharp
   // Combine with Unit of Work for transaction management
   using var uow = new UnitOfWork();
   await uow.Users.AddAsync(user);
   await uow.Orders.AddAsync(order);
   await uow.CommitAsync();  // Single transaction
   ```

8. **Don't Expose IQueryable**
   ```csharp
   // ❌ Bad - Exposes EF Core details
   IQueryable<User> GetAll();
   
   // ✅ Good - Returns materialized collection
   Task<IEnumerable<User>> GetAllAsync();
   ```

## Integration with .NET Ecosystem

### Entity Framework Core Integration
```csharp
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
    
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}
```

### Dependency Injection
```csharp
// Register repositories in DI container
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Use in service
public class UserService
{
    private readonly IUserRepository _repository;
    
    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }
}
```

### Dapper Integration
```csharp
public class UserRepository : IUserRepository
{
    private readonly IDbConnection _connection;
    
    public async Task<User?> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM Users WHERE Id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
    }
}
```

### AutoMapper Integration
```csharp
public class UserRepository : IUserRepository
{
    private readonly DbContext _context;
    private readonly IMapper _mapper;
    
    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        var entities = await _context.Users
            .Where(u => u.IsActive)
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<User>>(entities);
    }
}
```

### Specification Pattern
```csharp
public class UserRepository : IUserRepository
{
    public async Task<IEnumerable<User>> FindAsync(ISpecification<User> spec)
    {
        return await _context.Users
            .Where(spec.ToExpression())
            .ToListAsync();
    }
}

// Usage:
var adults = await repo.FindAsync(new AdultUserSpecification());
```

## Common Patterns with Repository

### Repository + Unit of Work
Manage transactions across multiple repositories
```csharp
public class UnitOfWork : IUnitOfWork
{
    public IUserRepository Users { get; }
    public IOrderRepository Orders { get; }
    
    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
```

### Repository + Specification
Encapsulate query logic in reusable specifications
```csharp
var spec = new ActiveUsersSpecification()
    .And(new PremiumUsersSpecification());
var users = await repository.FindAsync(spec);
```

### Repository + CQRS
Separate read and write repositories
```csharp
public interface IUserReadRepository { }   // Query side
public interface IUserWriteRepository { }  // Command side
```

## Related Patterns

- **Unit of Work**: Manages transactions across multiple repositories
- **Specification**: Encapsulates query logic
- **Data Mapper**: Maps between domain objects and database records
- **Identity Map**: Ensures single instance of each object
- **Lazy Load**: Delays loading of related data
- **Service Layer**: Uses repositories to implement business logic

## Summary

The Repository Pattern is a fundamental data access pattern that provides an abstraction layer between business logic and data sources. It centralizes data access logic, improves testability, and enables flexibility in switching data sources. While it adds an extra layer of abstraction, the benefits in terms of maintainability, testability, and clean architecture make it valuable for enterprise applications. 

However, be cautious about adding unnecessary abstraction over ORMs like Entity Framework Core, which already provide repository-like functionality through DbContext. The pattern works best when combined with Domain-Driven Design, Unit of Work, and Specification patterns for complex enterprise applications. For simple CRUD applications, the added complexity may not be justified.
