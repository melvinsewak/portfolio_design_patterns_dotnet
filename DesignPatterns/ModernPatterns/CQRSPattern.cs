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

#region Example 3: E-Commerce with Azure SQL, Event Sourcing & Service Bus

/*
 * ARCHITECTURE OVERVIEW:
 * ======================
 * 
 * ┌─────────────────────────────────────────────────────────────────────────────┐
 * │                          COMMAND PATH (Write Side)                          │
 * ├─────────────────────────────────────────────────────────────────────────────┤
 * │                                                                              │
 * │  Client Request → Command → Command Handler → Write to WRITE DB             │
 * │                                       ↓                                      │
 * │                              Save Event to Event Store                       │
 * │                                       ↓                                      │
 * │                       Publish Event to Azure Service Bus                     │
 * │                                                                              │
 * └─────────────────────────────────────────────────────────────────────────────┘
 * 
 * ┌─────────────────────────────────────────────────────────────────────────────┐
 * │                      MESSAGE QUEUE (Azure Service Bus)                      │
 * ├─────────────────────────────────────────────────────────────────────────────┤
 * │                                                                              │
 * │  Event Messages Queue (with metadata: timestamp, version, correlation id)   │
 * │                                                                              │
 * └─────────────────────────────────────────────────────────────────────────────┘
 * 
 * ┌─────────────────────────────────────────────────────────────────────────────┐
 * │                          QUERY PATH (Read Side)                             │
 * ├─────────────────────────────────────────────────────────────────────────────┤
 * │                                                                              │
 * │  Event Processor (Background Worker) → Consume Event from Service Bus       │
 * │                                       ↓                                      │
 * │                             Update READ Database                             │
 * │                        (Denormalized, Query-Optimized)                       │
 * │                                       ↓                                      │
 * │                    Client Query → Query Handler → READ DB                    │
 * │                                                                              │
 * └─────────────────────────────────────────────────────────────────────────────┘
 * 
 * KEY FEATURES:
 * - Complete separation of Write and Read databases
 * - Event Sourcing for full audit trail
 * - Asynchronous communication via Service Bus
 * - Eventual consistency model
 * - Optimistic locking with version control
 * - Denormalized read models for performance
 * 
 * DATABASE SCHEMAS:
 * 
 * WRITE DATABASE (ProductWriteDB):
 * --------------------------------
 * Products Table:
 *   ProductId (PK, int)
 *   Name (nvarchar(255))
 *   Description (nvarchar(max))
 *   Price (decimal(18,2))
 *   Stock (int)
 *   Category (nvarchar(100))
 *   Version (int) -- for optimistic locking
 *   CreatedAt (datetime2)
 *   UpdatedAt (datetime2)
 * 
 * EventStore Table:
 *   EventId (PK, uniqueidentifier)
 *   AggregateId (int) -- Product ID
 *   AggregateType (nvarchar(100)) -- "Product"
 *   EventType (nvarchar(255))
 *   EventData (nvarchar(max)) -- JSON
 *   Version (int)
 *   Timestamp (datetime2)
 *   CorrelationId (uniqueidentifier)
 *   UserId (nvarchar(100))
 * 
 * READ DATABASE (ProductReadDB):
 * ------------------------------
 * ProductReadModels Table:
 *   ProductId (PK, int)
 *   Name (nvarchar(255))
 *   Description (nvarchar(max))
 *   Price (decimal(18,2))
 *   Stock (int)
 *   Category (nvarchar(100))
 *   IsAvailable (bit) -- computed: Stock > 0
 *   PriceFormatted (nvarchar(50)) -- pre-formatted
 *   LastUpdated (datetime2)
 *   SearchVector (nvarchar(max)) -- for full-text search
 */

// Domain Events
public interface IDomainEvent
{
    Guid EventId { get; }
    int AggregateId { get; }
    string AggregateType { get; }
    DateTime Timestamp { get; }
    int Version { get; }
    Guid CorrelationId { get; }
}

public abstract class DomainEventBase : IDomainEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public int AggregateId { get; set; }
    public string AggregateType { get; set; } = "Product";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int Version { get; set; }
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
}

public class ProductCreatedEvent : DomainEventBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class ProductPriceUpdatedEvent : DomainEventBase
{
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
}

public class ProductStockUpdatedEvent : DomainEventBase
{
    public int OldStock { get; set; }
    public int NewStock { get; set; }
    public int Quantity { get; set; }
}

public class ProductDeletedEvent : DomainEventBase
{
    public string ProductName { get; set; } = string.Empty;
}

// Event Store Service
public interface IEventStore
{
    Task SaveEventAsync(IDomainEvent domainEvent);
    Task<IEnumerable<IDomainEvent>> GetEventsAsync(int aggregateId);
    Task<IEnumerable<IDomainEvent>> GetAllEventsAsync();
}

public class InMemoryEventStore : IEventStore
{
    private readonly List<IDomainEvent> _events = new();

    public Task SaveEventAsync(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
        Console.WriteLine($"[EVENT STORE] Saved event: {domainEvent.GetType().Name} (ID: {domainEvent.EventId}, Version: {domainEvent.Version})");
        return Task.CompletedTask;
    }

    public Task<IEnumerable<IDomainEvent>> GetEventsAsync(int aggregateId)
    {
        return Task.FromResult(_events.Where(e => e.AggregateId == aggregateId).OrderBy(e => e.Version).AsEnumerable());
    }

    public Task<IEnumerable<IDomainEvent>> GetAllEventsAsync()
    {
        return Task.FromResult(_events.OrderBy(e => e.Timestamp).AsEnumerable());
    }
}

// Event Publisher (Azure Service Bus Mock)
public interface IEventPublisher
{
    Task PublishAsync(IDomainEvent domainEvent);
}

public class MockServiceBusPublisher : IEventPublisher
{
    private readonly List<IDomainEvent> _publishedEvents = new();

    public Task PublishAsync(IDomainEvent domainEvent)
    {
        _publishedEvents.Add(domainEvent);
        Console.WriteLine($"[SERVICE BUS] Published event: {domainEvent.GetType().Name} to queue");
        Console.WriteLine($"  - EventId: {domainEvent.EventId}");
        Console.WriteLine($"  - AggregateId: {domainEvent.AggregateId}");
        Console.WriteLine($"  - Timestamp: {domainEvent.Timestamp:yyyy-MM-dd HH:mm:ss}");
        return Task.CompletedTask;
    }

    public IEnumerable<IDomainEvent> GetPublishedEvents() => _publishedEvents;
}

// Write-Side Domain Model with Versioning
public class ProductAggregate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Write Store (Transactional Database)
public class AzureProductWriteRepository
{
    private readonly List<ProductAggregate> _products = new();
    private int _nextId = 1;

    public async Task<int> CreateAsync(ProductAggregate product)
    {
        await Task.Delay(15); // Simulate Azure SQL write latency
        product.Id = _nextId++;
        product.Version = 1;
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;
        _products.Add(product);
        return product.Id;
    }

    public async Task<ProductAggregate?> GetByIdAsync(int id)
    {
        await Task.Delay(10);
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public async Task UpdateAsync(ProductAggregate product)
    {
        await Task.Delay(15);
        var existing = _products.FirstOrDefault(p => p.Id == product.Id);
        if (existing == null)
            throw new InvalidOperationException($"Product {product.Id} not found");
        
        // Optimistic locking check
        if (existing.Version != product.Version)
            throw new InvalidOperationException($"Concurrency conflict: Product version mismatch");
        
        product.Version++; // Increment version
        product.UpdatedAt = DateTime.UtcNow;
        var index = _products.IndexOf(existing);
        _products[index] = product;
    }

    public async Task DeleteAsync(int id)
    {
        await Task.Delay(15);
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            _products.Remove(product);
        }
    }
}

// Read-Side Denormalized Model
public class ProductReadModel
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public string PriceFormatted { get; set; } = string.Empty;
    public string SearchVector { get; set; } = string.Empty; // For search optimization
    public DateTime LastUpdated { get; set; }
}

// Read Store (Query-Optimized Database)
public class AzureProductReadRepository
{
    private readonly List<ProductReadModel> _readModels = new();

    public async Task UpsertAsync(ProductReadModel model)
    {
        await Task.Delay(8); // Reads are faster
        var existing = _readModels.FirstOrDefault(p => p.ProductId == model.ProductId);
        if (existing != null)
        {
            _readModels.Remove(existing);
        }
        _readModels.Add(model);
        Console.WriteLine($"[READ DB] Updated read model for Product {model.ProductId}");
    }

    public async Task<ProductReadModel?> GetByIdAsync(int id)
    {
        await Task.Delay(5);
        return _readModels.FirstOrDefault(p => p.ProductId == id);
    }

    public async Task<IEnumerable<ProductReadModel>> GetAllAsync()
    {
        await Task.Delay(5);
        return _readModels.ToList();
    }

    public async Task<IEnumerable<ProductReadModel>> GetAvailableAsync()
    {
        await Task.Delay(5);
        return _readModels.Where(p => p.IsAvailable).ToList();
    }

    public async Task<IEnumerable<ProductReadModel>> SearchAsync(string searchTerm)
    {
        await Task.Delay(5);
        var lower = searchTerm.ToLower();
        return _readModels
            .Where(p => p.SearchVector.Contains(lower))
            .ToList();
    }

    public async Task DeleteAsync(int id)
    {
        await Task.Delay(8);
        var model = _readModels.FirstOrDefault(p => p.ProductId == id);
        if (model != null)
        {
            _readModels.Remove(model);
            Console.WriteLine($"[READ DB] Deleted read model for Product {id}");
        }
    }
}

// Commands with Event Sourcing
public class CreateProductCommandV3 : ICommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class UpdateProductPriceCommandV3 : ICommand
{
    public int ProductId { get; set; }
    public decimal NewPrice { get; set; }
}

public class UpdateProductStockCommandV3 : ICommand
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class DeleteProductCommandV3 : ICommand
{
    public int ProductId { get; set; }
}

// Command Handlers with Event Sourcing
public class CreateProductCommandHandlerV3 : ICommandHandler<CreateProductCommandV3>
{
    private readonly AzureProductWriteRepository _writeRepo;
    private readonly IEventStore _eventStore;
    private readonly IEventPublisher _eventPublisher;

    public CreateProductCommandHandlerV3(
        AzureProductWriteRepository writeRepo,
        IEventStore eventStore,
        IEventPublisher eventPublisher)
    {
        _writeRepo = writeRepo;
        _eventStore = eventStore;
        _eventPublisher = eventPublisher;
    }

    public async Task HandleAsync(CreateProductCommandV3 command)
    {
        // 1. Create product in write database
        var product = new ProductAggregate
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            Stock = command.Stock,
            Category = command.Category
        };

        var productId = await _writeRepo.CreateAsync(product);
        Console.WriteLine($"[WRITE DB] Created product: {command.Name} (ID: {productId})");

        // 2. Create and save event
        var @event = new ProductCreatedEvent
        {
            AggregateId = productId,
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            Stock = command.Stock,
            Category = command.Category,
            Version = 1
        };

        await _eventStore.SaveEventAsync(@event);

        // 3. Publish event to Service Bus
        await _eventPublisher.PublishAsync(@event);
    }
}

public class UpdateProductPriceCommandHandlerV3 : ICommandHandler<UpdateProductPriceCommandV3>
{
    private readonly AzureProductWriteRepository _writeRepo;
    private readonly IEventStore _eventStore;
    private readonly IEventPublisher _eventPublisher;

    public UpdateProductPriceCommandHandlerV3(
        AzureProductWriteRepository writeRepo,
        IEventStore eventStore,
        IEventPublisher eventPublisher)
    {
        _writeRepo = writeRepo;
        _eventStore = eventStore;
        _eventPublisher = eventPublisher;
    }

    public async Task HandleAsync(UpdateProductPriceCommandV3 command)
    {
        var product = await _writeRepo.GetByIdAsync(command.ProductId);
        if (product == null)
            throw new InvalidOperationException($"Product {command.ProductId} not found");

        var oldPrice = product.Price;
        product.Price = command.NewPrice;
        
        await _writeRepo.UpdateAsync(product);
        Console.WriteLine($"[WRITE DB] Updated price: {product.Name} ${oldPrice} → ${command.NewPrice}");

        var @event = new ProductPriceUpdatedEvent
        {
            AggregateId = command.ProductId,
            OldPrice = oldPrice,
            NewPrice = command.NewPrice,
            Version = product.Version
        };

        await _eventStore.SaveEventAsync(@event);
        await _eventPublisher.PublishAsync(@event);
    }
}

public class UpdateProductStockCommandHandlerV3 : ICommandHandler<UpdateProductStockCommandV3>
{
    private readonly AzureProductWriteRepository _writeRepo;
    private readonly IEventStore _eventStore;
    private readonly IEventPublisher _eventPublisher;

    public UpdateProductStockCommandHandlerV3(
        AzureProductWriteRepository writeRepo,
        IEventStore eventStore,
        IEventPublisher eventPublisher)
    {
        _writeRepo = writeRepo;
        _eventStore = eventStore;
        _eventPublisher = eventPublisher;
    }

    public async Task HandleAsync(UpdateProductStockCommandV3 command)
    {
        var product = await _writeRepo.GetByIdAsync(command.ProductId);
        if (product == null)
            throw new InvalidOperationException($"Product {command.ProductId} not found");

        var oldStock = product.Stock;
        product.Stock += command.Quantity;
        
        await _writeRepo.UpdateAsync(product);
        Console.WriteLine($"[WRITE DB] Updated stock: {product.Name} {oldStock} → {product.Stock} units");

        var @event = new ProductStockUpdatedEvent
        {
            AggregateId = command.ProductId,
            OldStock = oldStock,
            NewStock = product.Stock,
            Quantity = command.Quantity,
            Version = product.Version
        };

        await _eventStore.SaveEventAsync(@event);
        await _eventPublisher.PublishAsync(@event);
    }
}

public class DeleteProductCommandHandlerV3 : ICommandHandler<DeleteProductCommandV3>
{
    private readonly AzureProductWriteRepository _writeRepo;
    private readonly IEventStore _eventStore;
    private readonly IEventPublisher _eventPublisher;

    public DeleteProductCommandHandlerV3(
        AzureProductWriteRepository writeRepo,
        IEventStore eventStore,
        IEventPublisher eventPublisher)
    {
        _writeRepo = writeRepo;
        _eventStore = eventStore;
        _eventPublisher = eventPublisher;
    }

    public async Task HandleAsync(DeleteProductCommandV3 command)
    {
        var product = await _writeRepo.GetByIdAsync(command.ProductId);
        if (product == null)
            throw new InvalidOperationException($"Product {command.ProductId} not found");

        var productName = product.Name;
        await _writeRepo.DeleteAsync(command.ProductId);
        Console.WriteLine($"[WRITE DB] Deleted product: {productName} (ID: {command.ProductId})");

        var @event = new ProductDeletedEvent
        {
            AggregateId = command.ProductId,
            ProductName = productName,
            Version = product.Version
        };

        await _eventStore.SaveEventAsync(@event);
        await _eventPublisher.PublishAsync(@event);
    }
}

// Read Model Updater (Background Worker - processes events from Service Bus)
public class ReadModelUpdater
{
    private readonly AzureProductReadRepository _readRepo;

    public ReadModelUpdater(AzureProductReadRepository readRepo)
    {
        _readRepo = readRepo;
    }

    public async Task ProcessEventAsync(IDomainEvent domainEvent)
    {
        Console.WriteLine($"[EVENT PROCESSOR] Processing event: {domainEvent.GetType().Name}");
        
        switch (domainEvent)
        {
            case ProductCreatedEvent created:
                await HandleProductCreatedAsync(created);
                break;
            
            case ProductPriceUpdatedEvent priceUpdated:
                await HandleProductPriceUpdatedAsync(priceUpdated);
                break;
            
            case ProductStockUpdatedEvent stockUpdated:
                await HandleProductStockUpdatedAsync(stockUpdated);
                break;
            
            case ProductDeletedEvent deleted:
                await HandleProductDeletedAsync(deleted);
                break;
            
            default:
                Console.WriteLine($"[EVENT PROCESSOR] Unknown event type: {domainEvent.GetType().Name}");
                break;
        }
    }

    private async Task HandleProductCreatedAsync(ProductCreatedEvent @event)
    {
        var readModel = new ProductReadModel
        {
            ProductId = @event.AggregateId,
            Name = @event.Name,
            Description = @event.Description,
            Price = @event.Price,
            Stock = @event.Stock,
            Category = @event.Category,
            IsAvailable = @event.Stock > 0,
            PriceFormatted = $"${@event.Price:F2}",
            SearchVector = $"{@event.Name} {@event.Description} {@event.Category}".ToLower(),
            LastUpdated = DateTime.UtcNow
        };

        await _readRepo.UpsertAsync(readModel);
    }

    private async Task HandleProductPriceUpdatedAsync(ProductPriceUpdatedEvent @event)
    {
        var readModel = await _readRepo.GetByIdAsync(@event.AggregateId);
        if (readModel != null)
        {
            readModel.Price = @event.NewPrice;
            readModel.PriceFormatted = $"${@event.NewPrice:F2}";
            readModel.LastUpdated = DateTime.UtcNow;
            await _readRepo.UpsertAsync(readModel);
        }
    }

    private async Task HandleProductStockUpdatedAsync(ProductStockUpdatedEvent @event)
    {
        var readModel = await _readRepo.GetByIdAsync(@event.AggregateId);
        if (readModel != null)
        {
            readModel.Stock = @event.NewStock;
            readModel.IsAvailable = @event.NewStock > 0;
            readModel.LastUpdated = DateTime.UtcNow;
            await _readRepo.UpsertAsync(readModel);
        }
    }

    private async Task HandleProductDeletedAsync(ProductDeletedEvent @event)
    {
        await _readRepo.DeleteAsync(@event.AggregateId);
    }
}

// Queries
public class GetProductByIdQueryV3 : IQuery<ProductReadModel?>
{
    public int ProductId { get; set; }
}

public class GetAllProductsQueryV3 : IQuery<IEnumerable<ProductReadModel>>
{
}

public class GetAvailableProductsQueryV3 : IQuery<IEnumerable<ProductReadModel>>
{
}

public class SearchProductsQueryV3 : IQuery<IEnumerable<ProductReadModel>>
{
    public string SearchTerm { get; set; } = string.Empty;
}

// Query Handlers
public class GetProductByIdQueryHandlerV3 : IQueryHandler<GetProductByIdQueryV3, ProductReadModel?>
{
    private readonly AzureProductReadRepository _readRepo;

    public GetProductByIdQueryHandlerV3(AzureProductReadRepository readRepo)
    {
        _readRepo = readRepo;
    }

    public async Task<ProductReadModel?> HandleAsync(GetProductByIdQueryV3 query)
    {
        Console.WriteLine($"[READ DB QUERY] Getting product details for ID: {query.ProductId}");
        return await _readRepo.GetByIdAsync(query.ProductId);
    }
}

public class GetAllProductsQueryHandlerV3 : IQueryHandler<GetAllProductsQueryV3, IEnumerable<ProductReadModel>>
{
    private readonly AzureProductReadRepository _readRepo;

    public GetAllProductsQueryHandlerV3(AzureProductReadRepository readRepo)
    {
        _readRepo = readRepo;
    }

    public async Task<IEnumerable<ProductReadModel>> HandleAsync(GetAllProductsQueryV3 query)
    {
        Console.WriteLine("[READ DB QUERY] Getting all products");
        return await _readRepo.GetAllAsync();
    }
}

public class GetAvailableProductsQueryHandlerV3 : IQueryHandler<GetAvailableProductsQueryV3, IEnumerable<ProductReadModel>>
{
    private readonly AzureProductReadRepository _readRepo;

    public GetAvailableProductsQueryHandlerV3(AzureProductReadRepository readRepo)
    {
        _readRepo = readRepo;
    }

    public async Task<IEnumerable<ProductReadModel>> HandleAsync(GetAvailableProductsQueryV3 query)
    {
        Console.WriteLine("[READ DB QUERY] Getting available products");
        return await _readRepo.GetAvailableAsync();
    }
}

public class SearchProductsQueryHandlerV3 : IQueryHandler<SearchProductsQueryV3, IEnumerable<ProductReadModel>>
{
    private readonly AzureProductReadRepository _readRepo;

    public SearchProductsQueryHandlerV3(AzureProductReadRepository readRepo)
    {
        _readRepo = readRepo;
    }

    public async Task<IEnumerable<ProductReadModel>> HandleAsync(SearchProductsQueryV3 query)
    {
        Console.WriteLine($"[READ DB QUERY] Searching products: '{query.SearchTerm}'");
        return await _readRepo.SearchAsync(query.SearchTerm);
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

        Console.WriteLine("\n--- Example 3: Azure SQL with Event Sourcing & Service Bus ---");
        await AzureCQRSWithEventSourcingExample();

        Console.WriteLine("\n--- Example 4: Read vs Write Performance ---");
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

    private static async Task AzureCQRSWithEventSourcingExample()
    {
        /*
         * This example demonstrates:
         * 1. Separate Write and Read databases
         * 2. Event Sourcing for audit trail
         * 3. Azure Service Bus for async communication
         * 4. Eventual consistency
         * 5. Denormalized read models
         */

        Console.WriteLine("\n🔧 Setting up infrastructure...");
        
        // Setup infrastructure
        var writeRepo = new AzureProductWriteRepository();
        var readRepo = new AzureProductReadRepository();
        var eventStore = new InMemoryEventStore();
        var eventPublisher = new MockServiceBusPublisher();
        var readModelUpdater = new ReadModelUpdater(readRepo);

        // Setup command handlers
        var createHandler = new CreateProductCommandHandlerV3(writeRepo, eventStore, eventPublisher);
        var updatePriceHandler = new UpdateProductPriceCommandHandlerV3(writeRepo, eventStore, eventPublisher);
        var updateStockHandler = new UpdateProductStockCommandHandlerV3(writeRepo, eventStore, eventPublisher);
        var deleteHandler = new DeleteProductCommandHandlerV3(writeRepo, eventStore, eventPublisher);

        // Setup query handlers
        var getProductHandler = new GetProductByIdQueryHandlerV3(readRepo);
        var getAllProductsHandler = new GetAllProductsQueryHandlerV3(readRepo);
        var getAvailableHandler = new GetAvailableProductsQueryHandlerV3(readRepo);
        var searchHandler = new SearchProductsQueryHandlerV3(readRepo);

        Console.WriteLine("\n📝 STEP 1: Execute Commands (Write to WRITE DB + Publish Events)");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        // Create products
        await createHandler.HandleAsync(new CreateProductCommandV3
        {
            Name = "Gaming Laptop",
            Description = "High-performance gaming laptop with RTX 4090",
            Price = 2499.99m,
            Stock = 15,
            Category = "Electronics"
        });

        await createHandler.HandleAsync(new CreateProductCommandV3
        {
            Name = "Mechanical Keyboard",
            Description = "RGB mechanical keyboard with Cherry MX switches",
            Price = 149.99m,
            Stock = 50,
            Category = "Accessories"
        });

        await createHandler.HandleAsync(new CreateProductCommandV3
        {
            Name = "4K Monitor",
            Description = "32-inch 4K HDR monitor",
            Price = 599.99m,
            Stock = 0,
            Category = "Electronics"
        });

        Console.WriteLine("\n⚡ STEP 2: Process Events (Simulate Service Bus Event Processing)");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        // Simulate event processing from Service Bus (in real world, this would be a background worker)
        var publishedEvents = eventPublisher.GetPublishedEvents();
        foreach (var evt in publishedEvents)
        {
            await readModelUpdater.ProcessEventAsync(evt);
            await Task.Delay(50); // Simulate network latency
        }

        Console.WriteLine("\n🔍 STEP 3: Query Read Database (After Synchronization)");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        // Query read database
        var allProducts = await getAllProductsHandler.HandleAsync(new GetAllProductsQueryV3());
        Console.WriteLine($"\n📊 Total Products in Read DB: {allProducts.Count()}");
        foreach (var product in allProducts)
        {
            Console.WriteLine($"  • {product.Name} - {product.PriceFormatted} (Stock: {product.Stock}, Available: {product.IsAvailable})");
        }

        var availableProducts = await getAvailableHandler.HandleAsync(new GetAvailableProductsQueryV3());
        Console.WriteLine($"\n✅ Available Products: {availableProducts.Count()}");
        foreach (var product in availableProducts)
        {
            Console.WriteLine($"  • {product.Name}");
        }

        Console.WriteLine("\n🔄 STEP 4: Update Commands + Event Processing");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        // Update price
        await updatePriceHandler.HandleAsync(new UpdateProductPriceCommandV3
        {
            ProductId = 1,
            NewPrice = 2299.99m
        });

        // Update stock
        await updateStockHandler.HandleAsync(new UpdateProductStockCommandV3
        {
            ProductId = 3,
            Quantity = 25 // Restock the monitor
        });

        Console.WriteLine("\n⏱️  Demonstrating Eventual Consistency...");
        Console.WriteLine("   (In production, there would be a delay before read DB is updated)");
        await Task.Delay(100);

        // Process new events
        var newEvents = eventPublisher.GetPublishedEvents().Skip(3);
        foreach (var evt in newEvents)
        {
            await readModelUpdater.ProcessEventAsync(evt);
        }

        Console.WriteLine("\n🔍 STEP 5: Query After Updates");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        var updatedProduct = await getProductHandler.HandleAsync(new GetProductByIdQueryV3 { ProductId = 1 });
        Console.WriteLine($"📦 Updated Product: {updatedProduct?.Name} - {updatedProduct?.PriceFormatted}");

        var nowAvailable = await getAvailableHandler.HandleAsync(new GetAvailableProductsQueryV3());
        Console.WriteLine($"\n✅ Available Products After Restock: {nowAvailable.Count()}");

        Console.WriteLine("\n🔎 STEP 6: Search Functionality");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        var searchResults = await searchHandler.HandleAsync(new SearchProductsQueryV3 { SearchTerm = "laptop" });
        Console.WriteLine($"🔍 Search results for 'laptop': {searchResults.Count()} found");
        foreach (var product in searchResults)
        {
            Console.WriteLine($"  • {product.Name} - {product.PriceFormatted}");
        }

        Console.WriteLine("\n🗑️ STEP 7: Delete Product + Event Processing");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        await deleteHandler.HandleAsync(new DeleteProductCommandV3 { ProductId = 2 });

        // Process delete event
        var deleteEvent = eventPublisher.GetPublishedEvents().Last();
        await readModelUpdater.ProcessEventAsync(deleteEvent);

        var remainingProducts = await getAllProductsHandler.HandleAsync(new GetAllProductsQueryV3());
        Console.WriteLine($"\n📊 Remaining Products: {remainingProducts.Count()}");

        Console.WriteLine("\n📜 STEP 8: Event Store Audit Trail");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        var allEvents = await eventStore.GetAllEventsAsync();
        Console.WriteLine($"📋 Total Events in Event Store: {allEvents.Count()}");
        Console.WriteLine("\nEvent History:");
        foreach (var evt in allEvents)
        {
            Console.WriteLine($"  [{evt.Timestamp:HH:mm:ss}] {evt.GetType().Name} - Aggregate {evt.AggregateId} (Version: {evt.Version})");
        }

        Console.WriteLine("\n✨ KEY TAKEAWAYS:");
        Console.WriteLine("══════════════════════════════════════════════════════════════");
        Console.WriteLine("✓ Write and Read databases are completely separate");
        Console.WriteLine("✓ Every state change is captured as an event");
        Console.WriteLine("✓ Events are published to Service Bus for async processing");
        Console.WriteLine("✓ Read models are denormalized and optimized for queries");
        Console.WriteLine("✓ Full audit trail available through event store");
        Console.WriteLine("✓ Eventual consistency model demonstrated");
        Console.WriteLine("✓ Optimistic locking prevents concurrent update conflicts");
        Console.WriteLine("\n💡 PRODUCTION CONSIDERATIONS:");
        Console.WriteLine("══════════════════════════════════════════════════════════════");
        Console.WriteLine("• Use Azure SQL for both Write and Read databases");
        Console.WriteLine("• Use Azure Service Bus for event publishing");
        Console.WriteLine("• Implement retry logic with exponential backoff");
        Console.WriteLine("• Add idempotency checks in event processors");
        Console.WriteLine("• Monitor event processing lag");
        Console.WriteLine("• Implement dead-letter queue handling");
        Console.WriteLine("• Consider event versioning for schema evolution");
        Console.WriteLine("• Use distributed tracing (Application Insights)");
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
