# Specification Pattern

## Intent
The Specification Pattern encapsulates business rules and query criteria as reusable objects that can be combined, tested, and maintained independently. It provides a way to express complex business logic in a composable, testable, and readable manner.

## Problem
When implementing business rules and query logic, several issues arise:
- **Scattered business logic** across repositories, services, and controllers
- **Hard-coded query criteria** in data access code
- **Duplicate validation logic** in multiple places
- **Complex conditional logic** that's hard to read and maintain
- **Difficult to test** business rules in isolation
- **Inflexible queries** that can't be easily combined
- **Tight coupling** between business rules and data access

Example of problematic code:
```csharp
public class ProductRepository
{
    public async Task<IEnumerable<Product>> GetPremiumProducts()
    {
        return await _context.Products
            .Where(p => p.Price > 100 && p.Rating >= 4 && p.IsAvailable)
            .ToListAsync();
        // Business logic hard-coded in repository
        // Can't reuse these rules elsewhere
    }
    
    public async Task<IEnumerable<Product>> GetAffordablePremiumProducts()
    {
        return await _context.Products
            .Where(p => p.Price > 100 && p.Price < 500 && p.Rating >= 4 && p.IsAvailable)
            .ToListAsync();
        // Duplicate logic with slight variations
    }
}
```

## Solution
The Specification Pattern provides:
1. **Encapsulated business rules** in reusable specification classes
2. **Composable specifications** using AND, OR, NOT operations
3. **Testable logic** - specifications can be unit tested
4. **Readable code** - business rules expressed clearly
5. **Flexible queries** - combine specifications dynamically

Key benefits:
- Business rules separated from data access
- Specifications reusable across the application
- Complex criteria built from simple building blocks
- Easy to test business logic in isolation

## Structure

```
┌─────────────────────────────────────────────────────────────┐
│                  ISpecification<T>                           │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  + IsSatisfiedBy(T entity) : bool                     │  │
│  │  + ToExpression() : Expression<Func<T, bool>>        │  │
│  │  + And(ISpecification<T>) : ISpecification<T>        │  │
│  │  + Or(ISpecification<T>) : ISpecification<T>         │  │
│  │  + Not() : ISpecification<T>                         │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                         ▲
                         │ Implements
        ┌────────────────┼────────────────┐
        │                │                │
        │                │                │
┌───────┴────────┐ ┌────┴─────┐ ┌────────┴────────┐
│ Specification1 │ │  Spec2   │ │ CompositeSpec   │
│                │ │          │ │                 │
│ PriceRange     │ │Category  │ │ AndSpec         │
│ Specification  │ │  Spec    │ │ OrSpec          │
│                │ │          │ │ NotSpec         │
└────────────────┘ └──────────┘ └─────────────────┘

Usage Flow:
┌─────────────────────────────────────────────────────────────┐
│  // Create specifications                                    │
│  var priceSpec = new PriceRangeSpec(100, 500);              │
│  var categorySpec = new CategorySpec("Electronics");         │
│  var availableSpec = new AvailableSpec();                    │
│                                                              │
│  // Compose them                                             │
│  var combinedSpec = priceSpec                                │
│      .And(categorySpec)                                      │
│      .And(availableSpec);                                    │
│                                                              │
│  // Use with repository                                      │
│  var products = await repository.FindAsync(combinedSpec);    │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                     Repository                               │
│                                                              │
│  public async Task<IEnumerable<T>> FindAsync(               │
│      ISpecification<T> specification)                        │
│  {                                                           │
│      var predicate = specification.ToExpression().Compile(); │
│      return _data.Where(predicate).ToList();                │
│  }                                                           │
└─────────────────────────────────────────────────────────────┘
```

## Key Components

### 1. Specification Interface
```csharp
public interface ISpecification<T>
{
    // Check if entity satisfies specification
    bool IsSatisfiedBy(T entity);
    
    // Convert to LINQ expression for querying
    Expression<Func<T, bool>> ToExpression();
}
```

### 2. Abstract Base Specification
```csharp
public abstract class Specification<T> : ISpecification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();
    
    public bool IsSatisfiedBy(T entity)
    {
        var predicate = ToExpression().Compile();
        return predicate(entity);
    }
    
    // Combinator methods
    public Specification<T> And(Specification<T> other)
    {
        return new AndSpecification<T>(this, other);
    }
    
    public Specification<T> Or(Specification<T> other)
    {
        return new OrSpecification<T>(this, other);
    }
    
    public Specification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}
```

### 3. Concrete Specifications
```csharp
public class PriceRangeSpecification : Specification<Product>
{
    private readonly decimal _minPrice;
    private readonly decimal _maxPrice;
    
    public PriceRangeSpecification(decimal minPrice, decimal maxPrice)
    {
        _minPrice = minPrice;
        _maxPrice = maxPrice;
    }
    
    public override Expression<Func<Product, bool>> ToExpression()
    {
        return product => product.Price >= _minPrice && product.Price <= _maxPrice;
    }
}
```

### 4. Composite Specifications
```csharp
public class AndSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;
    
    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpr = _left.ToExpression();
        var rightExpr = _right.ToExpression();
        
        var parameter = Expression.Parameter(typeof(T));
        var combined = Expression.AndAlso(
            Expression.Invoke(leftExpr, parameter),
            Expression.Invoke(rightExpr, parameter)
        );
        
        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}
```

## When to Use

### Use Specification Pattern When:
- ✅ You have **complex business rules** that need to be reusable
- ✅ Building **filter/search functionality** with many combinations
- ✅ You need to **test business logic** independently
- ✅ Query criteria need to be **composed dynamically**
- ✅ Working with **Domain-Driven Design** (DDD)
- ✅ Business rules are **scattered** and need centralization
- ✅ You want **readable business logic** instead of complex LINQ
- ✅ Validating **domain objects** against business rules

### Avoid When:
- ❌ Building **simple CRUD operations** with basic filters
- ❌ Queries are **simple and unlikely to change**
- ❌ You don't need to **combine or reuse** business rules
- ❌ The added **abstraction doesn't provide value**
- ❌ Your team is **unfamiliar with the pattern**
- ❌ Working on a **prototype or MVP**

## Real-World Use Cases

### 1. **E-Commerce Product Filtering**
```csharp
// Individual specifications
var priceSpec = new PriceRangeSpec(50, 200);
var categorySpec = new CategorySpec("Electronics");
var inStockSpec = new InStockSpec();
var highRatingSpec = new MinRatingSpec(4);

// Combine for complex filter
var premiumElectronics = categorySpec
    .And(priceSpec)
    .And(inStockSpec)
    .And(highRatingSpec);

var products = await repository.FindAsync(premiumElectronics);
```

### 2. **User Eligibility Rules**
```csharp
// Specifications for promotion eligibility
var adultSpec = new AdultUserSpec();            // Age >= 18
var verifiedSpec = new VerifiedUserSpec();      // Email verified
var premiumSpec = new PremiumUserSpec();        // Has subscription
var loyalSpec = new LoyalCustomerSpec();        // 10+ orders

// Complex eligibility rule
var promotionEligible = adultSpec
    .And(verifiedSpec)
    .And(premiumSpec.Or(loyalSpec));  // Premium OR Loyal

if (promotionEligible.IsSatisfiedBy(user))
{
    await ApplyPromotionAsync(user);
}
```

### 3. **Invoice Processing**
```csharp
// Different invoice states
var overdueSpec = new OverdueInvoiceSpec();
var unpaidSpec = new UnpaidInvoiceSpec();
var largeAmountSpec = new MinAmountSpec(1000);

// Find invoices needing attention
var criticalInvoices = overdueSpec
    .And(unpaidSpec)
    .And(largeAmountSpec);

var invoices = await repository.FindAsync(criticalInvoices);
foreach (var invoice in invoices)
{
    await SendReminderAsync(invoice);
}
```

### 4. **Loan Approval System**
```csharp
// Loan approval specifications
var minIncomeSpec = new MinIncomeSpec(50000);
var goodCreditSpec = new MinCreditScoreSpec(700);
var employedSpec = new EmploymentSpec();
var lowDebtSpec = new MaxDebtRatioSpec(0.3);

// Approval criteria
var loanApprovalSpec = minIncomeSpec
    .And(goodCreditSpec)
    .And(employedSpec)
    .And(lowDebtSpec);

var approved = loanApprovalSpec.IsSatisfiedBy(applicant);
```

### 5. **Content Moderation**
```csharp
// Moderation specifications
var containsProfanitySpec = new ProfanitySpec();
var tooShortSpec = new MinLengthSpec(10);
var spamSpec = new SpamDetectionSpec();

// Content should NOT satisfy these
var invalidContent = containsProfanitySpec
    .Or(tooShortSpec)
    .Or(spamSpec);

if (invalidContent.IsSatisfiedBy(post))
{
    await RejectPostAsync(post);
}
```

## Advantages

1. **Reusable Business Logic**
   - Write once, use everywhere
   - Consistent business rules across application
   - Easy to share between layers

2. **Composable**
   - Build complex rules from simple ones
   - Use AND, OR, NOT operators
   - Dynamic query construction

3. **Testable**
   - Unit test specifications in isolation
   - Test complex combinations
   - Mock-free testing of business rules

4. **Readable**
   - Business intent clear from code
   - Self-documenting
   - Non-technical stakeholders can understand

5. **Maintainable**
   - Single location for business rules
   - Changes affect all usages automatically
   - Easy to modify without breaking code

6. **Separation of Concerns**
   - Business logic separated from data access
   - Domain objects don't know about queries
   - Clean architecture

7. **Type-Safe**
   - Compile-time checking
   - IntelliSense support
   - Refactoring-friendly

## Disadvantages

1. **Complexity**
   - Additional classes for each specification
   - Learning curve for team
   - More code to maintain

2. **Performance Overhead**
   - Expression tree building
   - Potential for inefficient queries if not careful
   - May generate complex SQL

3. **Over-Engineering Risk**
   - Can be overkill for simple scenarios
   - May add unnecessary abstraction
   - Temptation to create too many specifications

4. **Expression Tree Limitations**
   - Not all logic can be expressed in expressions
   - Some operations don't translate to SQL
   - Debugging can be harder

5. **Potential for Misuse**
   - Specifications that are too generic
   - Too many small specifications
   - Unclear naming conventions

## Best Practices

1. **Name Specifications Clearly**
   ```csharp
   // ✅ Good - Clear intent
   public class ActiveUserSpecification { }
   public class PremiumProductSpecification { }
   public class OverdueInvoiceSpecification { }
   
   // ❌ Bad - Vague names
   public class UserSpec { }
   public class ProductFilter { }
   ```

2. **Keep Specifications Focused**
   ```csharp
   // ✅ Good - Single responsibility
   public class AdultUserSpec : Specification<User>
   {
       public override Expression<Func<User, bool>> ToExpression()
       {
           return user => user.Age >= 18;
       }
   }
   
   // ❌ Bad - Too much logic
   public class ComplexUserSpec : Specification<User>
   {
       public override Expression<Func<User, bool>> ToExpression()
       {
           return user => user.Age >= 18 
               && user.IsVerified 
               && user.Balance > 1000
               && user.OrderCount > 10;
           // Too many concerns in one spec
       }
   }
   ```

3. **Use Composition Over Complex Specifications**
   ```csharp
   // ✅ Good - Compose simple specs
   var eligibleUser = new AdultSpec()
       .And(new VerifiedSpec())
       .And(new PremiumSpec());
   
   // ❌ Bad - One complex spec
   var eligibleUser = new ComplexEligibilitySpec();
   ```

4. **Parameterize Specifications**
   ```csharp
   // ✅ Good - Parameterized
   public class PriceRangeSpec : Specification<Product>
   {
       public PriceRangeSpec(decimal min, decimal max) { }
   }
   
   var affordableProducts = new PriceRangeSpec(0, 100);
   var premiumProducts = new PriceRangeSpec(1000, 10000);
   ```

5. **Use Expression<Func<T, bool>> for Database Queries**
   ```csharp
   // ✅ Good - Returns expression for EF Core
   public override Expression<Func<Product, bool>> ToExpression()
   {
       return product => product.Price > 100;
   }
   
   // ❌ Bad - Returns Func (can't translate to SQL)
   public Func<Product, bool> GetPredicate()
   {
       return product => product.Price > 100;
   }
   ```

6. **Provide Both In-Memory and Database Methods**
   ```csharp
   public abstract class Specification<T> : ISpecification<T>
   {
       // For database queries
       public abstract Expression<Func<T, bool>> ToExpression();
       
       // For in-memory validation
       public bool IsSatisfiedBy(T entity)
       {
           return ToExpression().Compile()(entity);
       }
   }
   ```

7. **Test Specifications Independently**
   ```csharp
   [Test]
   public void AdultSpec_User18OrOlder_ReturnsTrue()
   {
       var spec = new AdultUserSpec();
       var user = new User { Age = 18 };
       
       Assert.IsTrue(spec.IsSatisfiedBy(user));
   }
   
   [Test]
   public void AdultSpec_User17OrYounger_ReturnsFalse()
   {
       var spec = new AdultUserSpec();
       var user = new User { Age = 17 };
       
       Assert.IsFalse(spec.IsSatisfiedBy(user));
   }
   ```

8. **Consider Caching Complex Specifications**
   ```csharp
   public static class Specifications
   {
       // Reuse common specifications
       public static readonly Specification<User> ActiveUser = new ActiveUserSpec();
       public static readonly Specification<User> VerifiedUser = new VerifiedUserSpec();
   }
   
   // Usage
   var eligibleUsers = Specifications.ActiveUser.And(Specifications.VerifiedUser);
   ```

## Integration with .NET Ecosystem

### Entity Framework Core Integration
```csharp
public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext _context;
    
    public async Task<IEnumerable<T>> FindAsync(ISpecification<T> specification)
    {
        // Specification expression translates to SQL
        return await _context.Set<T>()
            .Where(specification.ToExpression())
            .ToListAsync();
    }
}

// Usage
var products = await repository.FindAsync(new AvailableProductsSpec());
```

### LINQ Integration
```csharp
public static class SpecificationExtensions
{
    public static IQueryable<T> Where<T>(
        this IQueryable<T> query, 
        ISpecification<T> specification)
    {
        return query.Where(specification.ToExpression());
    }
}

// Usage
var products = _context.Products
    .Where(new PriceRangeSpec(100, 500))
    .Where(new InStockSpec())
    .ToList();
```

### FluentValidation Integration
```csharp
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        var adultSpec = new AdultUserSpec();
        var verifiedSpec = new VerifiedUserSpec();
        
        RuleFor(user => user)
            .Must(u => adultSpec.IsSatisfiedBy(u))
            .WithMessage("User must be 18 or older");
            
        RuleFor(user => user)
            .Must(u => verifiedSpec.IsSatisfiedBy(u))
            .WithMessage("User must be verified");
    }
}
```

### AutoMapper Integration
```csharp
public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(
                dest => dest.IsPremium,
                opt => opt.MapFrom(src => new PremiumProductSpec().IsSatisfiedBy(src))
            );
    }
}
```

### Dependency Injection
```csharp
// Register specifications as services
builder.Services.AddSingleton<ISpecification<User>>(new ActiveUserSpec());
builder.Services.AddTransient<ISpecification<Product>>(sp => 
    new PriceRangeSpec(100, 500));

// Inject into services
public class UserService
{
    private readonly ISpecification<User> _activeUserSpec;
    
    public UserService(ISpecification<User> activeUserSpec)
    {
        _activeUserSpec = activeUserSpec;
    }
}
```

### Repository Pattern
```csharp
public interface IRepository<T>
{
    Task<IEnumerable<T>> FindAsync(ISpecification<T> specification);
    Task<int> CountAsync(ISpecification<T> specification);
    Task<bool> AnyAsync(ISpecification<T> specification);
}

public class Repository<T> : IRepository<T> where T : class
{
    public async Task<int> CountAsync(ISpecification<T> specification)
    {
        return await _context.Set<T>()
            .CountAsync(specification.ToExpression());
    }
}
```

## Related Patterns

- **Repository Pattern**: Specifications used with repositories for querying
- **Strategy Pattern**: Specifications are strategies for filtering
- **Composite Pattern**: AND, OR, NOT create composite specifications
- **Chain of Responsibility**: Similar chaining of business rules
- **Builder Pattern**: Fluent API for building specifications
- **Visitor Pattern**: Can traverse specification trees

## Common Variations

### 1. **Parameterless Specifications**
```csharp
public class ActiveUserSpec : Specification<User>
{
    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.IsActive;
    }
}
```

### 2. **Parameterized Specifications**
```csharp
public class UserOlderThanSpec : Specification<User>
{
    private readonly int _age;
    
    public UserOlderThanSpec(int age)
    {
        _age = age;
    }
    
    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.Age > _age;
    }
}
```

### 3. **Fluent Specification Builder**
```csharp
var spec = SpecificationBuilder<Product>
    .Create()
    .Where(p => p.Price > 100)
    .And(p => p.IsAvailable)
    .OrWhere(p => p.Rating >= 5)
    .Build();
```

## Summary

The Specification Pattern is a powerful way to encapsulate business rules and query criteria in reusable, composable, and testable objects. It promotes clean code by separating business logic from data access, making complex rules readable and maintainable.

The pattern excels in scenarios with complex filtering, dynamic query building, and domain-driven design. It's particularly valuable for e-commerce filters, eligibility rules, content moderation, and any domain with rich business logic. When combined with Repository Pattern and Entity Framework Core, it provides a clean, testable architecture for data access.

However, for simple applications with basic queries, the Specification Pattern may introduce unnecessary complexity. Use it when business rules are complex, reusable, and need to be composed dynamically. The benefits of testability, reusability, and readability make it worthwhile for enterprise applications with rich domain logic.
