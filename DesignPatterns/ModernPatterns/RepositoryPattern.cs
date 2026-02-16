using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DesignPatterns.ModernPatterns;

#region Domain Models

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}

public class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public List<string> Tags { get; set; } = new();
    public int ViewCount { get; set; }
}

#endregion

#region Repository Interfaces

// Generic repository interface
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<int> CountAsync();
}

// Specific repository with custom methods
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<IEnumerable<User>> GetRecentUsersAsync(int count);
}

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
    Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
    Task<decimal> GetTotalRevenueAsync();
    Task<IEnumerable<Order>> GetOrdersInDateRangeAsync(DateTime startDate, DateTime endDate);
}

public interface IBlogPostRepository : IRepository<BlogPost>
{
    Task<IEnumerable<BlogPost>> GetByAuthorAsync(string author);
    Task<IEnumerable<BlogPost>> GetByTagAsync(string tag);
    Task<IEnumerable<BlogPost>> GetMostViewedAsync(int count);
    Task IncrementViewCountAsync(int id);
}

#endregion

#region Example 1: Generic Repository Implementation

public class GenericRepository<T> : IRepository<T> where T : class
{
    protected readonly List<T> _data;
    protected int _nextId;

    public GenericRepository()
    {
        _data = new List<T>();
        _nextId = 1;
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        await Task.Delay(10); // Simulate async database operation
        var idProperty = typeof(T).GetProperty("Id");
        return _data.FirstOrDefault(item =>
        {
            var itemId = idProperty?.GetValue(item);
            return itemId?.Equals(id) == true;
        });
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        await Task.Delay(10);
        return _data.ToList();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        await Task.Delay(10);
        return _data.Where(predicate.Compile()).ToList();
    }

    public virtual async Task AddAsync(T entity)
    {
        await Task.Delay(10);
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null)
        {
            idProperty.SetValue(entity, _nextId++);
        }
        _data.Add(entity);
    }

    public virtual async Task UpdateAsync(T entity)
    {
        await Task.Delay(10);
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null)
        {
            var id = (int)idProperty.GetValue(entity)!;
            var existing = await GetByIdAsync(id);
            if (existing != null)
            {
                var index = _data.IndexOf(existing);
                _data[index] = entity;
            }
        }
    }

    public virtual async Task DeleteAsync(int id)
    {
        await Task.Delay(10);
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _data.Remove(entity);
        }
    }

    public virtual async Task<int> CountAsync()
    {
        await Task.Delay(10);
        return _data.Count;
    }
}

#endregion

#region Example 2: Specialized User Repository

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository()
    {
        // Seed some initial data
        _data.AddRange(new[]
        {
            new User { Id = 1, Username = "alice", Email = "alice@example.com", CreatedAt = DateTime.Now.AddDays(-30), IsActive = true },
            new User { Id = 2, Username = "bob", Email = "bob@example.com", CreatedAt = DateTime.Now.AddDays(-20), IsActive = true },
            new User { Id = 3, Username = "charlie", Email = "charlie@example.com", CreatedAt = DateTime.Now.AddDays(-10), IsActive = false },
            new User { Id = 4, Username = "diana", Email = "diana@example.com", CreatedAt = DateTime.Now.AddDays(-5), IsActive = true }
        });
        _nextId = 5;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        await Task.Delay(10);
        return _data.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        await Task.Delay(10);
        return _data.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        await Task.Delay(10);
        return _data.Where(u => u.IsActive).ToList();
    }

    public async Task<IEnumerable<User>> GetRecentUsersAsync(int count)
    {
        await Task.Delay(10);
        return _data.OrderByDescending(u => u.CreatedAt).Take(count).ToList();
    }
}

#endregion

#region Example 3: Order Repository with Business Logic

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository()
    {
        // Seed initial orders
        _data.AddRange(new[]
        {
            new Order { Id = 1, UserId = 1, TotalAmount = 99.99m, OrderDate = DateTime.Now.AddDays(-15), Status = OrderStatus.Delivered },
            new Order { Id = 2, UserId = 1, TotalAmount = 149.99m, OrderDate = DateTime.Now.AddDays(-10), Status = OrderStatus.Delivered },
            new Order { Id = 3, UserId = 2, TotalAmount = 79.99m, OrderDate = DateTime.Now.AddDays(-5), Status = OrderStatus.Shipped },
            new Order { Id = 4, UserId = 2, TotalAmount = 199.99m, OrderDate = DateTime.Now.AddDays(-2), Status = OrderStatus.Confirmed },
            new Order { Id = 5, UserId = 3, TotalAmount = 49.99m, OrderDate = DateTime.Now.AddDays(-1), Status = OrderStatus.Pending }
        });
        _nextId = 6;
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
    {
        await Task.Delay(10);
        return _data.Where(o => o.UserId == userId).OrderByDescending(o => o.OrderDate).ToList();
    }

    public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        await Task.Delay(10);
        return _data.Where(o => o.Status == status).ToList();
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        await Task.Delay(10);
        return _data.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.TotalAmount);
    }

    public async Task<IEnumerable<Order>> GetOrdersInDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        await Task.Delay(10);
        return _data.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                   .OrderBy(o => o.OrderDate)
                   .ToList();
    }
}

#endregion

#region Blog Post Repository

public class BlogPostRepository : GenericRepository<BlogPost>, IBlogPostRepository
{
    public BlogPostRepository()
    {
        // Seed blog posts
        _data.AddRange(new[]
        {
            new BlogPost
            {
                Id = 1,
                Title = "Getting Started with C#",
                Content = "C# is a powerful language...",
                Author = "alice",
                PublishedDate = DateTime.Now.AddDays(-30),
                Tags = new List<string> { "csharp", "programming", "beginners" },
                ViewCount = 1500
            },
            new BlogPost
            {
                Id = 2,
                Title = "Design Patterns in .NET",
                Content = "Design patterns are reusable solutions...",
                Author = "bob",
                PublishedDate = DateTime.Now.AddDays(-20),
                Tags = new List<string> { "csharp", "design-patterns", "architecture" },
                ViewCount = 2300
            },
            new BlogPost
            {
                Id = 3,
                Title = "Async/Await Best Practices",
                Content = "Asynchronous programming in C#...",
                Author = "alice",
                PublishedDate = DateTime.Now.AddDays(-10),
                Tags = new List<string> { "csharp", "async", "best-practices" },
                ViewCount = 3200
            }
        });
        _nextId = 4;
    }

    public async Task<IEnumerable<BlogPost>> GetByAuthorAsync(string author)
    {
        await Task.Delay(10);
        return _data.Where(p => p.Author.Equals(author, StringComparison.OrdinalIgnoreCase))
                   .OrderByDescending(p => p.PublishedDate)
                   .ToList();
    }

    public async Task<IEnumerable<BlogPost>> GetByTagAsync(string tag)
    {
        await Task.Delay(10);
        return _data.Where(p => p.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                   .OrderByDescending(p => p.PublishedDate)
                   .ToList();
    }

    public async Task<IEnumerable<BlogPost>> GetMostViewedAsync(int count)
    {
        await Task.Delay(10);
        return _data.OrderByDescending(p => p.ViewCount).Take(count).ToList();
    }

    public async Task IncrementViewCountAsync(int id)
    {
        await Task.Delay(10);
        var post = await GetByIdAsync(id);
        if (post != null)
        {
            post.ViewCount++;
        }
    }
}

#endregion

#region Service Layer Using Repositories

public class RepoUserService
{
    private readonly IUserRepository _userRepository;

    public RepoUserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> CreateUserAsync(string username, string email)
    {
        // Check if username already exists
        var existingUser = await _userRepository.GetByUsernameAsync(username);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"Username '{username}' already exists");
        }

        var user = new User
        {
            Username = username,
            Email = email,
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        await _userRepository.AddAsync(user);
        return user;
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _userRepository.GetActiveUsersAsync();
    }
}

public class BlogService
{
    private readonly IBlogPostRepository _blogRepository;

    public BlogService(IBlogPostRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public async Task<BlogPost> GetPostByIdAsync(int id)
    {
        var post = await _blogRepository.GetByIdAsync(id);
        if (post != null)
        {
            await _blogRepository.IncrementViewCountAsync(id);
        }
        return post ?? throw new KeyNotFoundException($"Post {id} not found");
    }

    public async Task<IEnumerable<BlogPost>> GetPopularPostsAsync(int count = 5)
    {
        return await _blogRepository.GetMostViewedAsync(count);
    }
}

#endregion

#region Demo

public static class RepositoryPatternDemo
{
    public static async Task Run()
    {
        Console.WriteLine("=== Repository Pattern Demo ===\n");

        Console.WriteLine("--- Example 1: Generic Repository ---");
        await GenericRepositoryExample();

        Console.WriteLine("\n--- Example 2: User Repository with Custom Methods ---");
        await UserRepositoryExample();

        Console.WriteLine("\n--- Example 3: Order Repository with Business Queries ---");
        await OrderRepositoryExample();

        Console.WriteLine("\n--- Example 4: Blog Post Repository ---");
        await BlogPostRepositoryExample();
    }

    private static async Task GenericRepositoryExample()
    {
        var userRepo = new UserRepository();

        // Get all users
        var allUsers = await userRepo.GetAllAsync();
        Console.WriteLine($"Total users: {await userRepo.CountAsync()}");

        // Find users with LINQ expression
        var activeUsers = await userRepo.FindAsync(u => u.IsActive);
        Console.WriteLine($"Active users: {activeUsers.Count()}");

        // Add new user
        var newUser = new User
        {
            Username = "eve",
            Email = "eve@example.com",
            CreatedAt = DateTime.Now,
            IsActive = true
        };
        await userRepo.AddAsync(newUser);
        Console.WriteLine($"Added user: {newUser.Username} (ID: {newUser.Id})");
    }

    private static async Task UserRepositoryExample()
    {
        var userRepo = new UserRepository();
        var userService = new RepoUserService(userRepo);

        // Get user by username
        var user = await userRepo.GetByUsernameAsync("alice");
        Console.WriteLine($"Found user: {user?.Username} ({user?.Email})");

        // Get recent users
        var recentUsers = await userRepo.GetRecentUsersAsync(3);
        Console.WriteLine($"\nRecent users:");
        foreach (var u in recentUsers)
        {
            Console.WriteLine($"  - {u.Username} (joined {u.CreatedAt:yyyy-MM-dd})");
        }

        // Create new user through service
        try
        {
            var newUser = await userService.CreateUserAsync("frank", "frank@example.com");
            Console.WriteLine($"\nCreated user: {newUser.Username}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static async Task OrderRepositoryExample()
    {
        var orderRepo = new OrderRepository();

        // Get orders by user
        var userOrders = await orderRepo.GetOrdersByUserIdAsync(1);
        Console.WriteLine($"User 1 orders: {userOrders.Count()}");
        foreach (var order in userOrders)
        {
            Console.WriteLine($"  Order #{order.Id}: ${order.TotalAmount} - {order.Status}");
        }

        // Get total revenue
        var revenue = await orderRepo.GetTotalRevenueAsync();
        Console.WriteLine($"\nTotal revenue (delivered orders): ${revenue:F2}");

        // Get orders by status
        var pendingOrders = await orderRepo.GetOrdersByStatusAsync(OrderStatus.Pending);
        Console.WriteLine($"\nPending orders: {pendingOrders.Count()}");

        // Get orders in date range
        var startDate = DateTime.Now.AddDays(-7);
        var endDate = DateTime.Now;
        var recentOrders = await orderRepo.GetOrdersInDateRangeAsync(startDate, endDate);
        Console.WriteLine($"Orders in last 7 days: {recentOrders.Count()}");
    }

    private static async Task BlogPostRepositoryExample()
    {
        var blogRepo = new BlogPostRepository();
        var blogService = new BlogService(blogRepo);

        // Get most viewed posts
        var popularPosts = await blogService.GetPopularPostsAsync(3);
        Console.WriteLine("Most viewed posts:");
        foreach (var post in popularPosts)
        {
            Console.WriteLine($"  - {post.Title} by {post.Author} ({post.ViewCount} views)");
        }

        // Get posts by author
        var alicePosts = await blogRepo.GetByAuthorAsync("alice");
        Console.WriteLine($"\nPosts by alice: {alicePosts.Count()}");

        // Get posts by tag
        var csharpPosts = await blogRepo.GetByTagAsync("csharp");
        Console.WriteLine($"Posts tagged 'csharp': {csharpPosts.Count()}");

        // View a post (increments view count)
        var viewedPost = await blogService.GetPostByIdAsync(1);
        Console.WriteLine($"\nViewed post: {viewedPost.Title} (now has {viewedPost.ViewCount} views)");
    }
}

#endregion
