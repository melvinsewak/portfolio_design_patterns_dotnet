using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesignPatterns.ModernPatterns;

#region CQRS Core Concepts

// Commands - Change state
public interface ICommand { }

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command);
}

// Queries - Read state
public interface IQuery<TResult> { }

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query);
}

#endregion

#region Example 1: E-Commerce Product Catalog

// Domain Model
public class ProductEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Read Model (optimized for queries)
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable => Stock > 0;
}

public class ProductDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public string PriceFormatted => $"${Price:F2}";
}

// Commands
public class CreateProductCommand : ICommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class UpdateProductPriceCommand : ICommand
{
    public int ProductId { get; set; }
    public decimal NewPrice { get; set; }
}

public class UpdateProductStockCommand : ICommand
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class DeleteProductCommand : ICommand
{
    public int ProductId { get; set; }
}

// Queries
public class GetProductByIdQuery : IQuery<ProductDetailDto?>
{
    public int ProductId { get; set; }
}

public class GetAllProductsQuery : IQuery<IEnumerable<ProductDto>>
{
    public string? Category { get; set; }
}

public class GetAvailableProductsQuery : IQuery<IEnumerable<ProductDto>>
{
}

public class SearchProductsQuery : IQuery<IEnumerable<ProductDto>>
{
    public string SearchTerm { get; set; } = string.Empty;
}

// Write Store (Commands modify this)
public class ProductWriteStore
{
    private readonly List<ProductEntity> _products = new();
    private int _nextId = 1;

    public async Task<int> AddProductAsync(ProductEntity product)
    {
        await Task.Delay(10);
        product.Id = _nextId++;
        product.CreatedAt = DateTime.Now;
        product.UpdatedAt = DateTime.Now;
        _products.Add(product);
        return product.Id;
    }

    public async Task<ProductEntity?> GetProductAsync(int id)
    {
        await Task.Delay(10);
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public async Task UpdateProductAsync(ProductEntity product)
    {
        await Task.Delay(10);
        var existing = _products.FirstOrDefault(p => p.Id == product.Id);
        if (existing != null)
        {
            product.UpdatedAt = DateTime.Now;
            var index = _products.IndexOf(existing);
            _products[index] = product;
        }
    }

    public async Task DeleteProductAsync(int id)
    {
        await Task.Delay(10);
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            _products.Remove(product);
        }
    }

    public IEnumerable<ProductEntity> GetAllProducts() => _products.ToList();
}

// Read Store (Queries use this - optimized for reads)
public class ProductReadStore
{
    private readonly ProductWriteStore _writeStore;

    public ProductReadStore(ProductWriteStore writeStore)
    {
        _writeStore = writeStore;
    }

    public async Task<ProductDetailDto?> GetProductDetailsAsync(int id)
    {
        await Task.Delay(5); // Read operations are faster
        var product = _writeStore.GetAllProducts().FirstOrDefault(p => p.Id == id);
        
        return product == null ? null : new ProductDetailDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Category = product.Category
        };
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(string? category = null)
    {
        await Task.Delay(5);
        var products = _writeStore.GetAllProducts();
        
        if (!string.IsNullOrEmpty(category))
        {
            products = products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Stock = p.Stock
        }).ToList();
    }

    public async Task<IEnumerable<ProductDto>> GetAvailableProductsAsync()
    {
        await Task.Delay(5);
        return _writeStore.GetAllProducts()
            .Where(p => p.Stock > 0)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock
            }).ToList();
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        await Task.Delay(5);
        var lowerSearch = searchTerm.ToLower();
        return _writeStore.GetAllProducts()
            .Where(p => p.Name.ToLower().Contains(lowerSearch) || 
                       p.Description.ToLower().Contains(lowerSearch))
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock
            }).ToList();
    }
}

// Command Handlers
public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    private readonly ProductWriteStore _writeStore;

    public CreateProductCommandHandler(ProductWriteStore writeStore)
    {
        _writeStore = writeStore;
    }

    public async Task HandleAsync(CreateProductCommand command)
    {
        var product = new ProductEntity
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            Stock = command.Stock,
            Category = command.Category
        };

        var id = await _writeStore.AddProductAsync(product);
        Console.WriteLine($"[COMMAND] Product created: {command.Name} (ID: {id})");
    }
}

public class UpdateProductPriceCommandHandler : ICommandHandler<UpdateProductPriceCommand>
{
    private readonly ProductWriteStore _writeStore;

    public UpdateProductPriceCommandHandler(ProductWriteStore writeStore)
    {
        _writeStore = writeStore;
    }

    public async Task HandleAsync(UpdateProductPriceCommand command)
    {
        var product = await _writeStore.GetProductAsync(command.ProductId);
        if (product == null)
        {
            throw new InvalidOperationException($"Product {command.ProductId} not found");
        }

        product.Price = command.NewPrice;
        await _writeStore.UpdateProductAsync(product);
        Console.WriteLine($"[COMMAND] Product price updated: {product.Name} -> ${command.NewPrice}");
    }
}

public class UpdateProductStockCommandHandler : ICommandHandler<UpdateProductStockCommand>
{
    private readonly ProductWriteStore _writeStore;

    public UpdateProductStockCommandHandler(ProductWriteStore writeStore)
    {
        _writeStore = writeStore;
    }

    public async Task HandleAsync(UpdateProductStockCommand command)
    {
        var product = await _writeStore.GetProductAsync(command.ProductId);
        if (product == null)
        {
            throw new InvalidOperationException($"Product {command.ProductId} not found");
        }

        product.Stock += command.Quantity;
        await _writeStore.UpdateProductAsync(product);
        Console.WriteLine($"[COMMAND] Stock updated: {product.Name} -> {product.Stock} units");
    }
}

// Query Handlers
public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDetailDto?>
{
    private readonly ProductReadStore _readStore;

    public GetProductByIdQueryHandler(ProductReadStore readStore)
    {
        _readStore = readStore;
    }

    public async Task<ProductDetailDto?> HandleAsync(GetProductByIdQuery query)
    {
        Console.WriteLine($"[QUERY] Getting product details for ID: {query.ProductId}");
        return await _readStore.GetProductDetailsAsync(query.ProductId);
    }
}

public class GetAllProductsQueryHandler : IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly ProductReadStore _readStore;

    public GetAllProductsQueryHandler(ProductReadStore readStore)
    {
        _readStore = readStore;
    }

    public async Task<IEnumerable<ProductDto>> HandleAsync(GetAllProductsQuery query)
    {
        Console.WriteLine($"[QUERY] Getting all products" + (query.Category != null ? $" in category: {query.Category}" : ""));
        return await _readStore.GetAllProductsAsync(query.Category);
    }
}

public class GetAvailableProductsQueryHandler : IQueryHandler<GetAvailableProductsQuery, IEnumerable<ProductDto>>
{
    private readonly ProductReadStore _readStore;

    public GetAvailableProductsQueryHandler(ProductReadStore readStore)
    {
        _readStore = readStore;
    }

    public async Task<IEnumerable<ProductDto>> HandleAsync(GetAvailableProductsQuery query)
    {
        Console.WriteLine("[QUERY] Getting available products");
        return await _readStore.GetAvailableProductsAsync();
    }
}

#endregion

#region Example 2: Order Management System

public class OrderEntity
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<OrderLineItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public OrderState Status { get; set; }
    public DateTime OrderDate { get; set; }
}

public class OrderLineItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public enum OrderState
{
    Created,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}

public class OrderSummaryDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public OrderState Status { get; set; }
    public int ItemCount { get; set; }
}

// Commands
public class CreateOrderCommand : ICommand
{
    public string CustomerName { get; set; } = string.Empty;
    public List<OrderLineItem> Items { get; set; } = new();
}

public class ConfirmOrderCommand : ICommand
{
    public int OrderId { get; set; }
}

public class CancelOrderCommand : ICommand
{
    public int OrderId { get; set; }
}

// Queries
public class GetOrderByIdQuery : IQuery<OrderEntity?>
{
    public int OrderId { get; set; }
}

public class GetOrdersByCustomerQuery : IQuery<IEnumerable<OrderSummaryDto>>
{
    public string CustomerName { get; set; } = string.Empty;
}

public class GetOrdersByStatusQuery : IQuery<IEnumerable<OrderSummaryDto>>
{
    public OrderState Status { get; set; }
}

// Stores
public class OrderWriteStore
{
    private readonly List<OrderEntity> _orders = new();
    private int _nextId = 1;

    public async Task<int> CreateOrderAsync(OrderEntity order)
    {
        await Task.Delay(10);
        order.Id = _nextId++;
        order.OrderDate = DateTime.Now;
        _orders.Add(order);
        return order.Id;
    }

    public async Task<OrderEntity?> GetOrderAsync(int id)
    {
        await Task.Delay(10);
        return _orders.FirstOrDefault(o => o.Id == id);
    }

    public async Task UpdateOrderAsync(OrderEntity order)
    {
        await Task.Delay(10);
        var existing = _orders.FirstOrDefault(o => o.Id == order.Id);
        if (existing != null)
        {
            var index = _orders.IndexOf(existing);
            _orders[index] = order;
        }
    }

    public IEnumerable<OrderEntity> GetAllOrders() => _orders.ToList();
}

public class OrderReadStore
{
    private readonly OrderWriteStore _writeStore;

    public OrderReadStore(OrderWriteStore writeStore)
    {
        _writeStore = writeStore;
    }

    public async Task<OrderEntity?> GetOrderAsync(int id)
    {
        await Task.Delay(5);
        return _writeStore.GetAllOrders().FirstOrDefault(o => o.Id == id);
    }

    public async Task<IEnumerable<OrderSummaryDto>> GetOrdersByCustomerAsync(string customerName)
    {
        await Task.Delay(5);
        return _writeStore.GetAllOrders()
            .Where(o => o.CustomerName.Equals(customerName, StringComparison.OrdinalIgnoreCase))
            .Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                ItemCount = o.Items.Count
            }).ToList();
    }

    public async Task<IEnumerable<OrderSummaryDto>> GetOrdersByStatusAsync(OrderState status)
    {
        await Task.Delay(5);
        return _writeStore.GetAllOrders()
            .Where(o => o.Status == status)
            .Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                ItemCount = o.Items.Count
            }).ToList();
    }
}

// Command Handlers
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly OrderWriteStore _writeStore;

    public CreateOrderCommandHandler(OrderWriteStore writeStore)
    {
        _writeStore = writeStore;
    }

    public async Task HandleAsync(CreateOrderCommand command)
    {
        var order = new OrderEntity
        {
            CustomerName = command.CustomerName,
            Items = command.Items,
            TotalAmount = command.Items.Sum(i => i.Price * i.Quantity),
            Status = OrderState.Created
        };

        var id = await _writeStore.CreateOrderAsync(order);
        Console.WriteLine($"[COMMAND] Order created for {command.CustomerName} (ID: {id}, Total: ${order.TotalAmount:F2})");
    }
}

public class ConfirmOrderCommandHandler : ICommandHandler<ConfirmOrderCommand>
{
    private readonly OrderWriteStore _writeStore;

    public ConfirmOrderCommandHandler(OrderWriteStore writeStore)
    {
        _writeStore = writeStore;
    }

    public async Task HandleAsync(ConfirmOrderCommand command)
    {
        var order = await _writeStore.GetOrderAsync(command.OrderId);
        if (order == null)
        {
            throw new InvalidOperationException($"Order {command.OrderId} not found");
        }

        order.Status = OrderState.Confirmed;
        await _writeStore.UpdateOrderAsync(order);
        Console.WriteLine($"[COMMAND] Order {command.OrderId} confirmed");
    }
}

// Query Handler
public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderEntity?>
{
    private readonly OrderReadStore _readStore;

    public GetOrderByIdQueryHandler(OrderReadStore readStore)
    {
        _readStore = readStore;
    }

    public async Task<OrderEntity?> HandleAsync(GetOrderByIdQuery query)
    {
        Console.WriteLine($"[QUERY] Getting order ID: {query.OrderId}");
        return await _readStore.GetOrderAsync(query.OrderId);
    }
}

public class GetOrdersByStatusQueryHandler : IQueryHandler<GetOrdersByStatusQuery, IEnumerable<OrderSummaryDto>>
{
    private readonly OrderReadStore _readStore;

    public GetOrdersByStatusQueryHandler(OrderReadStore readStore)
    {
        _readStore = readStore;
    }

    public async Task<IEnumerable<OrderSummaryDto>> HandleAsync(GetOrdersByStatusQuery query)
    {
        Console.WriteLine($"[QUERY] Getting orders with status: {query.Status}");
        return await _readStore.GetOrdersByStatusAsync(query.Status);
    }
}

#endregion

#region Demo

public static class CQRSPatternDemo
{
    public static async Task Run()
    {
        Console.WriteLine("=== CQRS Pattern Demo ===\n");

        Console.WriteLine("--- Example 1: Product Catalog (Commands & Queries) ---");
        await ProductCatalogExample();

        Console.WriteLine("\n--- Example 2: Order Management ---");
        await OrderManagementExample();

        Console.WriteLine("\n--- Example 3: Read vs Write Performance ---");
        await PerformanceExample();
    }

    private static async Task ProductCatalogExample()
    {
        // Setup
        var writeStore = new ProductWriteStore();
        var readStore = new ProductReadStore(writeStore);

        var createHandler = new CreateProductCommandHandler(writeStore);
        var updatePriceHandler = new UpdateProductPriceCommandHandler(writeStore);
        var updateStockHandler = new UpdateProductStockCommandHandler(writeStore);
        var getProductHandler = new GetProductByIdQueryHandler(readStore);
        var getAllHandler = new GetAllProductsQueryHandler(readStore);
        var getAvailableHandler = new GetAvailableProductsQueryHandler(readStore);

        // Execute Commands
        await createHandler.HandleAsync(new CreateProductCommand
        {
            Name = "Laptop",
            Description = "High-performance laptop",
            Price = 1299.99m,
            Stock = 10,
            Category = "Electronics"
        });

        await createHandler.HandleAsync(new CreateProductCommand
        {
            Name = "Mouse",
            Description = "Wireless mouse",
            Price = 29.99m,
            Stock = 0,
            Category = "Electronics"
        });

        // Execute Queries
        var product = await getProductHandler.HandleAsync(new GetProductByIdQuery { ProductId = 1 });
        Console.WriteLine($"\nProduct Details: {product?.Name} - {product?.PriceFormatted}");

        var allProducts = await getAllHandler.HandleAsync(new GetAllProductsQuery());
        Console.WriteLine($"\nAll Products: {allProducts.Count()}");

        var availableProducts = await getAvailableHandler.HandleAsync(new GetAvailableProductsQuery());
        Console.WriteLine($"Available Products: {availableProducts.Count()}");

        // Update commands
        await updatePriceHandler.HandleAsync(new UpdateProductPriceCommand { ProductId = 1, NewPrice = 1199.99m });
        await updateStockHandler.HandleAsync(new UpdateProductStockCommand { ProductId = 2, Quantity = 50 });
    }

    private static async Task OrderManagementExample()
    {
        var writeStore = new OrderWriteStore();
        var readStore = new OrderReadStore(writeStore);

        var createHandler = new CreateOrderCommandHandler(writeStore);
        var confirmHandler = new ConfirmOrderCommandHandler(writeStore);
        var getOrderHandler = new GetOrderByIdQueryHandler(readStore);
        var getByStatusHandler = new GetOrdersByStatusQueryHandler(readStore);

        // Create orders
        await createHandler.HandleAsync(new CreateOrderCommand
        {
            CustomerName = "Alice",
            Items = new List<OrderLineItem>
            {
                new() { ProductId = 1, ProductName = "Laptop", Quantity = 1, Price = 1299.99m }
            }
        });

        await createHandler.HandleAsync(new CreateOrderCommand
        {
            CustomerName = "Bob",
            Items = new List<OrderLineItem>
            {
                new() { ProductId = 2, ProductName = "Mouse", Quantity = 2, Price = 29.99m }
            }
        });

        // Confirm first order
        await confirmHandler.HandleAsync(new ConfirmOrderCommand { OrderId = 1 });

        // Query orders
        var order = await getOrderHandler.HandleAsync(new GetOrderByIdQuery { OrderId = 1 });
        Console.WriteLine($"\nOrder Details: {order?.CustomerName} - {order?.Items.Count} items - ${order?.TotalAmount:F2}");

        var createdOrders = await getByStatusHandler.HandleAsync(new GetOrdersByStatusQuery { Status = OrderState.Created });
        Console.WriteLine($"\nCreated Orders: {createdOrders.Count()}");

        var confirmedOrders = await getByStatusHandler.HandleAsync(new GetOrdersByStatusQuery { Status = OrderState.Confirmed });
        Console.WriteLine($"Confirmed Orders: {confirmedOrders.Count()}");
    }

    private static async Task PerformanceExample()
    {
        var writeStore = new ProductWriteStore();
        var readStore = new ProductReadStore(writeStore);

        // Measure command execution time
        var commandStart = DateTime.Now;
        var createHandler = new CreateProductCommandHandler(writeStore);
        for (int i = 0; i < 10; i++)
        {
            await createHandler.HandleAsync(new CreateProductCommand
            {
                Name = $"Product {i}",
                Description = "Test product",
                Price = 99.99m,
                Stock = 10,
                Category = "Test"
            });
        }
        var commandTime = (DateTime.Now - commandStart).TotalMilliseconds;

        // Measure query execution time
        var queryStart = DateTime.Now;
        var getAllHandler = new GetAllProductsQueryHandler(readStore);
        for (int i = 0; i < 10; i++)
        {
            await getAllHandler.HandleAsync(new GetAllProductsQuery());
        }
        var queryTime = (DateTime.Now - queryStart).TotalMilliseconds;

        Console.WriteLine($"10 Commands execution time: {commandTime:F2}ms");
        Console.WriteLine($"10 Queries execution time: {queryTime:F2}ms");
        Console.WriteLine($"Queries are ~{commandTime / queryTime:F1}x faster (optimized for reads)");
    }
}

#endregion
