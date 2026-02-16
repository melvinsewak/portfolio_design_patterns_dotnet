# Observer Pattern

## Intent
The Observer pattern defines a one-to-many dependency between objects so that when one object changes state, all its dependents are notified and updated automatically.

## Problem
When you need to:
- Maintain consistency between related objects
- Notify multiple objects about state changes
- Avoid tight coupling between objects
- Support broadcast communication

**Without the pattern:**
- Objects must poll for changes
- Tight coupling between objects
- Hard to add/remove observers
- Difficult to maintain consistency

## Solution
Define a subject (observable) that maintains a list of observers and notifies them automatically of state changes. Observers register with the subject to receive updates.

## UML Diagram
```
┌────────────────────┐          ┌────────────────────┐
│   <<interface>>    │          │  <<interface>>     │
│      Subject       │          │     Observer       │
├────────────────────┤          ├────────────────────┤
│+ Attach(Observer)  │          │+ Update(data)      │
│+ Detach(Observer)  │          └─────────△──────────┘
│+ Notify()          │                    │
└──────────△─────────┘                    │
           │                              │implements
           │implements                    │
┌──────────┴─────────┐          ┌────────┴───────────┐
│  ConcreteSubject   │          │ ConcreteObserver   │
├────────────────────┤          ├────────────────────┤
│- state             │ notifies │- observerState     │
│- observers[]       │─────────→│+ Update(data)      │
│+ GetState()        │          └────────────────────┘
│+ SetState()        │
└────────────────────┘
```

## When to Use

### Use When:
- **State Changes** - Multiple objects need to react to state changes
- **Loose Coupling** - Want to decouple objects that depend on each other
- **Dynamic Subscription** - Objects subscribe/unsubscribe at runtime
- **Broadcast** - One object must notify many others
- **Event Systems** - Implementing event-driven architectures
- **MVC Pattern** - Views observe model changes

### Avoid When:
- **Simple Dependencies** - Only one or two simple dependencies
- **Performance Critical** - Notification overhead is too high
- **Complex Chains** - Updates trigger cascading changes
- **Synchronization Issues** - Concurrent updates cause problems

## Real-World Examples

### 1. **Stock Market**
   - Stock price changes notify all traders
   - Multiple displays update simultaneously
   - Real-time price feeds

### 2. **Social Media**
   - Post notifications to followers
   - Like/comment notifications
   - Activity feeds

### 3. **Weather Station**
   - Temperature changes notify all displays
   - Current conditions, statistics, forecast
   - Multiple display formats

### 4. **GUI Events**
   - Button clicks notify event handlers
   - Multiple listeners per event
   - Event bubbling in UI frameworks

### 5. **Newsletter Subscriptions**
   - New content notifies all subscribers
   - Unsubscribe anytime
   - Different subscriber types

### 6. **Spreadsheet Cells**
   - Formula cells observe referenced cells
   - Automatic recalculation
   - Dependency tracking

## Advantages

1. **Loose Coupling** - Subject and observers are loosely coupled
2. **Dynamic Relationships** - Can add/remove observers at runtime
3. **Broadcast Communication** - One-to-many notifications
4. **Open/Closed Principle** - Add observers without modifying subject
5. **Separation of Concerns** - Subject doesn't know observer details

## Disadvantages

1. **Unexpected Updates** - Observers don't know about each other
2. **Memory Leaks** - Forgot to unsubscribe can cause leaks
3. **Update Order** - No guarantee of notification order
4. **Performance** - Can be slow with many observers
5. **Complex Updates** - Cascading updates hard to track

## Best Practices

### 1. **Define Clear Observer Interface**
```csharp
public interface IObserver<T>
{
    void Update(T data);
}

public interface ISubject<T>
{
    void Attach(IObserver<T> observer);
    void Detach(IObserver<T> observer);
    void Notify();
}
```

### 2. **Use C# Events**
```csharp
public class Stock
{
    public event EventHandler<PriceChangedEventArgs>? PriceChanged;
    
    private decimal _price;
    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                _price = value;
                OnPriceChanged(new PriceChangedEventArgs(value));
            }
        }
    }
    
    protected virtual void OnPriceChanged(PriceChangedEventArgs e)
    {
        PriceChanged?.Invoke(this, e);
    }
}
```

### 3. **Implement IObservable<T> and IObserver<T>**
```csharp
public class DataPublisher : IObservable<string>
{
    private readonly List<IObserver<string>> _observers = new();
    
    public IDisposable Subscribe(IObserver<string> observer)
    {
        _observers.Add(observer);
        return new Unsubscriber(_observers, observer);
    }
    
    public void PublishData(string data)
    {
        foreach (var observer in _observers)
            observer.OnNext(data);
    }
}
```

### 4. **Prevent Memory Leaks**
```csharp
public class WeakObserver<T>
{
    private readonly WeakReference<IObserver<T>> _observer;
    
    public bool Update(T data)
    {
        if (_observer.TryGetTarget(out var observer))
        {
            observer.Update(data);
            return true;
        }
        return false; // Observer was garbage collected
    }
}
```

### 5. **Use Reactive Extensions (Rx)**
```csharp
var observable = Observable.Interval(TimeSpan.FromSeconds(1));
var subscription = observable.Subscribe(
    onNext: value => Console.WriteLine($"Value: {value}"),
    onError: ex => Console.WriteLine($"Error: {ex}"),
    onCompleted: () => Console.WriteLine("Completed")
);
```

### 6. **Implement Push vs Pull Model**
```csharp
// Push model - subject pushes data to observers
public void Notify(Data data)
{
    foreach (var observer in _observers)
        observer.Update(data);
}

// Pull model - observers pull data from subject
public void Notify()
{
    foreach (var observer in _observers)
        observer.Update(this); // Observer pulls data from this
}
```

### 7. **Thread Safety**
```csharp
public class ThreadSafeSubject
{
    private readonly ConcurrentBag<IObserver> _observers = new();
    
    public void Notify()
    {
        foreach (var observer in _observers.ToArray())
            observer.Update();
    }
}
```

## Related Patterns

- **Mediator**: Both define communication, but Mediator encapsulates it
- **Singleton**: Observable subjects often implemented as singletons
- **Command**: Observers can use commands to process updates

## C# Specific Considerations

### PropertyChanged Pattern (INotifyPropertyChanged)
```csharp
public class Person : INotifyPropertyChanged
{
    private string _name = string.Empty;
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }
    
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, 
            new PropertyChangedEventArgs(propertyName));
    }
}
```

### Using Delegates
```csharp
public delegate void DataChangedHandler(object sender, DataEventArgs e);

public class DataSource
{
    public event DataChangedHandler? DataChanged;
    
    protected virtual void OnDataChanged(DataEventArgs e)
    {
        DataChanged?.Invoke(this, e);
    }
}
```

### Async Observers
```csharp
public interface IAsyncObserver<T>
{
    Task UpdateAsync(T data);
}

public class AsyncSubject<T>
{
    private readonly List<IAsyncObserver<T>> _observers = new();
    
    public async Task NotifyAsync(T data)
    {
        var tasks = _observers.Select(o => o.UpdateAsync(data));
        await Task.WhenAll(tasks);
    }
}
```

## Summary

The Observer pattern is fundamental for event-driven programming and maintaining consistency between related objects. C# provides built-in support through events, IObservable/IObserver interfaces, and INotifyPropertyChanged. Use it for implementing reactive UIs, event systems, and maintaining data consistency across multiple views.
