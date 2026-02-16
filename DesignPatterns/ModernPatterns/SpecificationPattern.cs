using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DesignPatterns.ModernPatterns;

#region Core Specification Pattern

// Base Specification Interface
public interface ISpecification<T>
{
    bool IsSatisfiedBy(T entity);
    Expression<Func<T, bool>> ToExpression();
}

// Abstract base class for specifications
public abstract class Specification<T> : ISpecification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();

    public bool IsSatisfiedBy(T entity)
    {
        var predicate = ToExpression().Compile();
        return predicate(entity);
    }

    // Combine specifications with AND
    public Specification<T> And(Specification<T> other)
    {
        return new AndSpecification<T>(this, other);
    }

    // Combine specifications with OR
    public Specification<T> Or(Specification<T> other)
    {
        return new OrSpecification<T>(this, other);
    }

    // Negate specification
    public Specification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}

// Composite Specifications
public class AndSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

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

public class OrSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public OrSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpr = _left.ToExpression();
        var rightExpr = _right.ToExpression();
        
        var parameter = Expression.Parameter(typeof(T));
        var combined = Expression.OrElse(
            Expression.Invoke(leftExpr, parameter),
            Expression.Invoke(rightExpr, parameter)
        );
        
        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}

public class NotSpecification<T> : Specification<T>
{
    private readonly Specification<T> _specification;

    public NotSpecification(Specification<T> specification)
    {
        _specification = specification;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        var expr = _specification.ToExpression();
        var parameter = Expression.Parameter(typeof(T));
        var negated = Expression.Not(Expression.Invoke(expr, parameter));
        
        return Expression.Lambda<Func<T, bool>>(negated, parameter);
    }
}

#endregion

#region Example 1: Product Filtering Specifications

public class ProductSpec
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public int Rating { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
}

// Concrete Specifications for Products
public class ProductByPriceRangeSpecification : Specification<ProductSpec>
{
    private readonly decimal _minPrice;
    private readonly decimal _maxPrice;

    public ProductByPriceRangeSpecification(decimal minPrice, decimal maxPrice)
    {
        _minPrice = minPrice;
        _maxPrice = maxPrice;
    }

    public override Expression<Func<ProductSpec, bool>> ToExpression()
    {
        return product => product.Price >= _minPrice && product.Price <= _maxPrice;
    }
}

public class ProductByCategorySpecification : Specification<ProductSpec>
{
    private readonly string _category;

    public ProductByCategorySpecification(string category)
    {
        _category = category;
    }

    public override Expression<Func<ProductSpec, bool>> ToExpression()
    {
        return product => product.Category == _category;
    }
}

public class AvailableProductSpecification : Specification<ProductSpec>
{
    public override Expression<Func<ProductSpec, bool>> ToExpression()
    {
        return product => product.IsAvailable;
    }
}

public class ProductByMinRatingSpecification : Specification<ProductSpec>
{
    private readonly int _minRating;

    public ProductByMinRatingSpecification(int minRating)
    {
        _minRating = minRating;
    }

    public override Expression<Func<ProductSpec, bool>> ToExpression()
    {
        return product => product.Rating >= _minRating;
    }
}

public class NewProductSpecification : Specification<ProductSpec>
{
    private readonly int _daysOld;

    public NewProductSpecification(int daysOld = 30)
    {
        _daysOld = daysOld;
    }

    public override Expression<Func<ProductSpec, bool>> ToExpression()
    {
        var cutoffDate = DateTime.Now.AddDays(-_daysOld);
        return product => product.ReleaseDate >= cutoffDate;
    }
}

public class ProductByTagSpecification : Specification<ProductSpec>
{
    private readonly string _tag;

    public ProductByTagSpecification(string tag)
    {
        _tag = tag;
    }

    public override Expression<Func<ProductSpec, bool>> ToExpression()
    {
        return product => product.Tags.Contains(_tag);
    }
}

// Product Repository with Specification support
public class ProductSpecRepository
{
    private readonly List<ProductSpec> _products;

    public ProductSpecRepository()
    {
        _products = new List<ProductSpec>
        {
            new() { Id = 1, Name = "Laptop Pro", Price = 1299.99m, Category = "Electronics", IsAvailable = true, Rating = 5, ReleaseDate = DateTime.Now.AddDays(-15), Tags = new[] { "premium", "new" } },
            new() { Id = 2, Name = "Wireless Mouse", Price = 29.99m, Category = "Electronics", IsAvailable = true, Rating = 4, ReleaseDate = DateTime.Now.AddDays(-60), Tags = new[] { "accessory" } },
            new() { Id = 3, Name = "Office Chair", Price = 199.99m, Category = "Furniture", IsAvailable = false, Rating = 3, ReleaseDate = DateTime.Now.AddDays(-120), Tags = new[] { "office" } },
            new() { Id = 4, Name = "Desk Lamp", Price = 49.99m, Category = "Furniture", IsAvailable = true, Rating = 5, ReleaseDate = DateTime.Now.AddDays(-10), Tags = new[] { "new", "lighting" } },
            new() { Id = 5, Name = "Gaming Keyboard", Price = 149.99m, Category = "Electronics", IsAvailable = true, Rating = 5, ReleaseDate = DateTime.Now.AddDays(-5), Tags = new[] { "gaming", "new", "premium" } }
        };
    }

    public async Task<IEnumerable<ProductSpec>> FindAsync(ISpecification<ProductSpec> specification)
    {
        await Task.Delay(10);
        var predicate = specification.ToExpression().Compile();
        return _products.Where(predicate).ToList();
    }

    public async Task<IEnumerable<ProductSpec>> GetAllAsync()
    {
        await Task.Delay(10);
        return _products.ToList();
    }
}

#endregion

#region Example 2: User Eligibility Specifications

public class UserEligibility
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Country { get; set; } = string.Empty;
    public decimal AccountBalance { get; set; }
    public bool IsVerified { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool HasActiveSubscription { get; set; }
    public int OrderCount { get; set; }
}

// User Specifications
public class AdultUserSpecification : Specification<UserEligibility>
{
    public override Expression<Func<UserEligibility, bool>> ToExpression()
    {
        return user => user.Age >= 18;
    }
}

public class VerifiedUserSpecification : Specification<UserEligibility>
{
    public override Expression<Func<UserEligibility, bool>> ToExpression()
    {
        return user => user.IsVerified;
    }
}

public class UserFromCountrySpecification : Specification<UserEligibility>
{
    private readonly string _country;

    public UserFromCountrySpecification(string country)
    {
        _country = country;
    }

    public override Expression<Func<UserEligibility, bool>> ToExpression()
    {
        return user => user.Country == _country;
    }
}

public class PremiumUserSpecification : Specification<UserEligibility>
{
    private readonly decimal _minBalance;

    public PremiumUserSpecification(decimal minBalance = 1000m)
    {
        _minBalance = minBalance;
    }

    public override Expression<Func<UserEligibility, bool>> ToExpression()
    {
        return user => user.AccountBalance >= _minBalance && user.HasActiveSubscription;
    }
}

public class LoyalCustomerSpecification : Specification<UserEligibility>
{
    private readonly int _minOrders;
    private readonly int _minDaysSinceRegistration;

    public LoyalCustomerSpecification(int minOrders = 10, int minDaysSinceRegistration = 90)
    {
        _minOrders = minOrders;
        _minDaysSinceRegistration = minDaysSinceRegistration;
    }

    public override Expression<Func<UserEligibility, bool>> ToExpression()
    {
        var cutoffDate = DateTime.Now.AddDays(-_minDaysSinceRegistration);
        return user => user.OrderCount >= _minOrders && user.RegistrationDate <= cutoffDate;
    }
}

public class UserByIdRangeSpec : Specification<UserEligibility>
{
    private readonly int _minId;
    private readonly int _maxId;

    public UserByIdRangeSpec(int minId, int maxId)
    {
        _minId = minId;
        _maxId = maxId;
    }

    public override Expression<Func<UserEligibility, bool>> ToExpression()
    {
        return user => user.Id >= _minId && user.Id <= _maxId;
    }
}

// User Service with Business Rules
public class UserEligibilityService
{
    private readonly List<UserEligibility> _users;

    public UserEligibilityService()
    {
        _users = new List<UserEligibility>
        {
            new() { Id = 1, Name = "Alice", Age = 25, Country = "USA", AccountBalance = 1500m, IsVerified = true, RegistrationDate = DateTime.Now.AddDays(-120), HasActiveSubscription = true, OrderCount = 15 },
            new() { Id = 2, Name = "Bob", Age = 17, Country = "USA", AccountBalance = 100m, IsVerified = false, RegistrationDate = DateTime.Now.AddDays(-30), HasActiveSubscription = false, OrderCount = 2 },
            new() { Id = 3, Name = "Charlie", Age = 30, Country = "UK", AccountBalance = 2500m, IsVerified = true, RegistrationDate = DateTime.Now.AddDays(-200), HasActiveSubscription = true, OrderCount = 25 },
            new() { Id = 4, Name = "Diana", Age = 22, Country = "Canada", AccountBalance = 500m, IsVerified = true, RegistrationDate = DateTime.Now.AddDays(-45), HasActiveSubscription = false, OrderCount = 5 }
        };
    }

    public async Task<IEnumerable<UserEligibility>> FindUsersAsync(ISpecification<UserEligibility> specification)
    {
        await Task.Delay(10);
        var predicate = specification.ToExpression().Compile();
        return _users.Where(predicate).ToList();
    }

    public async Task<bool> IsEligibleForPromotionAsync(int userId)
    {
        await Task.Delay(10);
        var user = _users.FirstOrDefault(u => u.Id == userId);
        if (user == null) return false;

        // Promotion eligibility: Adult, Verified, and either Premium or Loyal Customer
        var adultSpec = new AdultUserSpecification();
        var verifiedSpec = new VerifiedUserSpecification();
        var premiumSpec = new PremiumUserSpecification();
        var loyalSpec = new LoyalCustomerSpecification();

        var eligibilitySpec = adultSpec
            .And(verifiedSpec)
            .And(premiumSpec.Or(loyalSpec));

        return eligibilitySpec.IsSatisfiedBy(user);
    }
}

#endregion

#region Example 3: Invoice Filtering Specifications

public class InvoiceSpec
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsPaid { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

// Invoice Specifications
public class OverdueInvoiceSpecification : Specification<InvoiceSpec>
{
    public override Expression<Func<InvoiceSpec, bool>> ToExpression()
    {
        var now = DateTime.Now;
        return invoice => !invoice.IsPaid && invoice.DueDate.HasValue && invoice.DueDate.Value < now;
    }
}

public class UnpaidInvoiceSpecification : Specification<InvoiceSpec>
{
    public override Expression<Func<InvoiceSpec, bool>> ToExpression()
    {
        return invoice => !invoice.IsPaid;
    }
}

public class InvoiceByAmountRangeSpecification : Specification<InvoiceSpec>
{
    private readonly decimal _minAmount;
    private readonly decimal _maxAmount;

    public InvoiceByAmountRangeSpecification(decimal minAmount, decimal maxAmount)
    {
        _minAmount = minAmount;
        _maxAmount = maxAmount;
    }

    public override Expression<Func<InvoiceSpec, bool>> ToExpression()
    {
        return invoice => invoice.Amount >= _minAmount && invoice.Amount <= _maxAmount;
    }
}

public class InvoiceByCustomerSpecification : Specification<InvoiceSpec>
{
    private readonly string _customerName;

    public InvoiceByCustomerSpecification(string customerName)
    {
        _customerName = customerName;
    }

    public override Expression<Func<InvoiceSpec, bool>> ToExpression()
    {
        return invoice => invoice.CustomerName == _customerName;
    }
}

public class RecentInvoiceSpecification : Specification<InvoiceSpec>
{
    private readonly int _days;

    public RecentInvoiceSpecification(int days = 30)
    {
        _days = days;
    }

    public override Expression<Func<InvoiceSpec, bool>> ToExpression()
    {
        var cutoffDate = DateTime.Now.AddDays(-_days);
        return invoice => invoice.InvoiceDate >= cutoffDate;
    }
}

public class InvoiceRepository
{
    private readonly List<InvoiceSpec> _invoices;

    public InvoiceRepository()
    {
        _invoices = new List<InvoiceSpec>
        {
            new() { Id = 1, Amount = 1500m, InvoiceDate = DateTime.Now.AddDays(-45), DueDate = DateTime.Now.AddDays(-15), IsPaid = false, CustomerName = "Acme Corp", Status = "Overdue" },
            new() { Id = 2, Amount = 750m, InvoiceDate = DateTime.Now.AddDays(-20), DueDate = DateTime.Now.AddDays(10), IsPaid = false, CustomerName = "Tech Solutions", Status = "Pending" },
            new() { Id = 3, Amount = 2000m, InvoiceDate = DateTime.Now.AddDays(-60), DueDate = DateTime.Now.AddDays(-30), IsPaid = true, CustomerName = "Acme Corp", Status = "Paid" },
            new() { Id = 4, Amount = 500m, InvoiceDate = DateTime.Now.AddDays(-5), DueDate = DateTime.Now.AddDays(25), IsPaid = false, CustomerName = "StartUp Inc", Status = "Pending" }
        };
    }

    public async Task<IEnumerable<InvoiceSpec>> FindAsync(ISpecification<InvoiceSpec> specification)
    {
        await Task.Delay(10);
        var predicate = specification.ToExpression().Compile();
        return _invoices.Where(predicate).ToList();
    }

    public async Task<decimal> GetTotalAsync(ISpecification<InvoiceSpec> specification)
    {
        await Task.Delay(10);
        var invoices = await FindAsync(specification);
        return invoices.Sum(i => i.Amount);
    }
}

#endregion

#region Demo

public static class SpecificationPatternDemo
{
    public static async Task Run()
    {
        Console.WriteLine("=== Specification Pattern Demo ===\n");

        Console.WriteLine("--- Example 1: Product Filtering ---");
        await ProductFilteringExample();

        Console.WriteLine("\n--- Example 2: User Eligibility Rules ---");
        await UserEligibilityExample();

        Console.WriteLine("\n--- Example 3: Invoice Filtering ---");
        await InvoiceFilteringExample();

        Console.WriteLine("\n--- Example 4: Complex Specification Combinations ---");
        await ComplexSpecificationExample();
    }

    private static async Task ProductFilteringExample()
    {
        var repository = new ProductSpecRepository();

        // Simple specifications
        var electronicsSpec = new ProductByCategorySpecification("Electronics");
        var electronics = await repository.FindAsync(electronicsSpec);
        Console.WriteLine($"Electronics products: {electronics.Count()}");

        // Combined specifications
        var affordableElectronics = electronicsSpec.And(new ProductByPriceRangeSpecification(0, 100));
        var affordable = await repository.FindAsync(affordableElectronics);
        Console.WriteLine($"Affordable electronics (< $100): {affordable.Count()}");

        // Complex combinations
        var premiumNewProducts = new ProductByMinRatingSpecification(5)
            .And(new NewProductSpecification(30))
            .And(new AvailableProductSpecification());
        
        var premium = await repository.FindAsync(premiumNewProducts);
        Console.WriteLine($"\nPremium new products (5-star, new, available):");
        foreach (var product in premium)
        {
            Console.WriteLine($"  - {product.Name}: ${product.Price} (Released: {product.ReleaseDate:yyyy-MM-dd})");
        }
    }

    private static async Task UserEligibilityExample()
    {
        var service = new UserEligibilityService();

        // Find verified adults
        var verifiedAdults = new AdultUserSpecification().And(new VerifiedUserSpecification());
        var users = await service.FindUsersAsync(verifiedAdults);
        Console.WriteLine($"Verified adult users: {users.Count()}");

        // Find premium customers
        var premiumSpec = new PremiumUserSpecification();
        var premiumUsers = await service.FindUsersAsync(premiumSpec);
        Console.WriteLine($"Premium users: {premiumUsers.Count()}");
        foreach (var user in premiumUsers)
        {
            Console.WriteLine($"  - {user.Name}: ${user.AccountBalance}");
        }

        // Check promotion eligibility
        Console.WriteLine("\nPromotion eligibility check:");
        var allUsers = await service.FindUsersAsync(new UserByIdRangeSpec(1, 4));
        foreach (var user in allUsers)
        {
            var eligible = await service.IsEligibleForPromotionAsync(user.Id);
            Console.WriteLine($"  User {user.Id} ({user.Name}): {(eligible ? "ELIGIBLE" : "NOT ELIGIBLE")}");
        }
    }

    private static async Task InvoiceFilteringExample()
    {
        var repository = new InvoiceRepository();

        // Find overdue invoices
        var overdueSpec = new OverdueInvoiceSpecification();
        var overdue = await repository.FindAsync(overdueSpec);
        Console.WriteLine($"Overdue invoices: {overdue.Count()}");
        foreach (var invoice in overdue)
        {
            Console.WriteLine($"  - Invoice #{invoice.Id}: ${invoice.Amount} from {invoice.CustomerName} (Due: {invoice.DueDate:yyyy-MM-dd})");
        }

        // Find large unpaid invoices
        var largeUnpaid = new UnpaidInvoiceSpecification()
            .And(new InvoiceByAmountRangeSpecification(1000, decimal.MaxValue));
        var large = await repository.FindAsync(largeUnpaid);
        var largeTotal = await repository.GetTotalAsync(largeUnpaid);
        Console.WriteLine($"\nLarge unpaid invoices (>$1000): {large.Count()} (Total: ${largeTotal:F2})");

        // Recent invoices for specific customer
        var acmeRecent = new InvoiceByCustomerSpecification("Acme Corp")
            .And(new RecentInvoiceSpecification(60));
        var acmeInvoices = await repository.FindAsync(acmeRecent);
        Console.WriteLine($"\nRecent Acme Corp invoices (last 60 days): {acmeInvoices.Count()}");
    }

    private static async Task ComplexSpecificationExample()
    {
        var repository = new ProductSpecRepository();

        // Build complex specification: 
        // (Electronics OR Furniture) AND (Price < $200) AND Available AND (Rating >= 4)
        var categorySpec = new ProductByCategorySpecification("Electronics")
            .Or(new ProductByCategorySpecification("Furniture"));
        
        var qualitySpec = new ProductByPriceRangeSpecification(0, 200)
            .And(new AvailableProductSpecification())
            .And(new ProductByMinRatingSpecification(4));

        var finalSpec = categorySpec.And(qualitySpec);

        var products = await repository.FindAsync(finalSpec);
        Console.WriteLine($"Matching products: {products.Count()}");
        foreach (var product in products)
        {
            Console.WriteLine($"  - {product.Name} ({product.Category}): ${product.Price} - {product.Rating}★");
        }

        // Using NOT specification
        var notElectronics = new ProductByCategorySpecification("Electronics").Not();
        var nonElectronics = await repository.FindAsync(notElectronics);
        Console.WriteLine($"\nNon-electronics products: {nonElectronics.Count()}");
    }
}

#endregion
