namespace DesignPatterns.BehavioralPatterns;

/// <summary>
/// Observer Pattern - Define a one-to-many dependency between objects
/// When one object changes state, all dependents are notified and updated automatically
/// </summary>

#region Example 1: Stock Market Monitoring

public interface IObserver
{
    void Update(string stockSymbol, decimal price);
}

public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify();
}

public class Stock : ISubject
{
    private readonly List<IObserver> _observers = new();
    private decimal _price;
    
    public string Symbol { get; }
    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                _price = value;
                Notify();
            }
        }
    }

    public Stock(string symbol, decimal initialPrice)
    {
        Symbol = symbol;
        _price = initialPrice;
    }

    public void Attach(IObserver observer)
    {
        _observers.Add(observer);
        Console.WriteLine($"  Observer subscribed to {Symbol}");
    }

    public void Detach(IObserver observer)
    {
        _observers.Remove(observer);
        Console.WriteLine($"  Observer unsubscribed from {Symbol}");
    }

    public void Notify()
    {
        Console.WriteLine($"\n[{Symbol}] Price changed to ${Price:F2} - Notifying {_observers.Count} observers");
        foreach (var observer in _observers)
        {
            observer.Update(Symbol, Price);
        }
    }
}

public class StockTrader : IObserver
{
    private readonly string _name;
    private readonly Dictionary<string, decimal> _portfolio = new();

    public StockTrader(string name)
    {
        _name = name;
    }

    public void Update(string stockSymbol, decimal price)
    {
        Console.WriteLine($"  [{_name}] Notified: {stockSymbol} is now ${price:F2}");
        
        // Trading logic
        if (price < 100m && !_portfolio.ContainsKey(stockSymbol))
        {
            _portfolio[stockSymbol] = price;
            Console.WriteLine($"  [{_name}] 💰 BUY {stockSymbol} at ${price:F2}");
        }
        else if (price > 150m && _portfolio.ContainsKey(stockSymbol))
        {
            var buyPrice = _portfolio[stockSymbol];
            var profit = price - buyPrice;
            _portfolio.Remove(stockSymbol);
            Console.WriteLine($"  [{_name}] 💵 SELL {stockSymbol} at ${price:F2} (Profit: ${profit:F2})");
        }
    }
}

public class StockDisplay : IObserver
{
    private readonly string _displayName;

    public StockDisplay(string displayName)
    {
        _displayName = displayName;
    }

    public void Update(string stockSymbol, decimal price)
    {
        Console.WriteLine($"  [{_displayName}] Display updated: {stockSymbol} = ${price:F2}");
    }
}

#endregion

#region Example 2: Weather Station

public class WeatherData
{
    public float Temperature { get; set; }
    public float Humidity { get; set; }
    public float Pressure { get; set; }
}

public interface IWeatherObserver
{
    void Update(WeatherData data);
}

public class WeatherStation : ISubject
{
    private readonly List<IWeatherObserver> _observers = new();
    private WeatherData _currentData = new();

    public void Attach(IWeatherObserver observer)  // Changed parameter type
    {
        _observers.Add(observer);
        Console.WriteLine("  Weather observer registered");
    }

    public void Detach(IWeatherObserver observer)  // Changed parameter type
    {
        _observers.Remove(observer);
    }

    // Implement ISubject methods for compatibility
    void ISubject.Attach(IObserver observer)
    {
        if (observer is IWeatherObserver weatherObserver)
            Attach(weatherObserver);
    }

    void ISubject.Detach(IObserver observer)
    {
        if (observer is IWeatherObserver weatherObserver)
            Detach(weatherObserver);
    }

    public void SetMeasurements(float temperature, float humidity, float pressure)
    {
        Console.WriteLine($"\n[Weather Station] New measurements received");
        _currentData = new WeatherData
        {
            Temperature = temperature,
            Humidity = humidity,
            Pressure = pressure
        };
        Notify();
    }

    public void Notify()
    {
        Console.WriteLine($"  Notifying {_observers.Count} weather displays...");
        foreach (var observer in _observers)
        {
            observer.Update(_currentData);
        }
    }
}

public class CurrentConditionsDisplay : IWeatherObserver
{
    public void Update(WeatherData data)
    {
        Console.WriteLine($"  [Current Conditions] {data.Temperature}°F, {data.Humidity}% humidity");
    }
}

public class StatisticsDisplay : IWeatherObserver
{
    private readonly List<float> _temperatures = new();

    public void Update(WeatherData data)
    {
        _temperatures.Add(data.Temperature);
        var avg = _temperatures.Average();
        var max = _temperatures.Max();
        var min = _temperatures.Min();
        
        Console.WriteLine($"  [Statistics] Avg: {avg:F1}°F, Max: {max:F1}°F, Min: {min:F1}°F");
    }
}

public class ForecastDisplay : IWeatherObserver
{
    private float _lastPressure = 0f;

    public void Update(WeatherData data)
    {
        string forecast;
        
        if (data.Pressure > _lastPressure)
            forecast = "Improving weather on the way!";
        else if (data.Pressure < _lastPressure)
            forecast = "Watch out for cooler, rainy weather";
        else
            forecast = "More of the same";

        Console.WriteLine($"  [Forecast] {forecast}");
        _lastPressure = data.Pressure;
    }
}

#endregion

#region Example 3: Event System (C# Events)

// Using built-in C# event system
public class Button
{
    public string Name { get; }
    
    // Event declaration
    public event EventHandler<ButtonClickEventArgs>? Clicked;

    public Button(string name)
    {
        Name = name;
    }

    public void Click()
    {
        Console.WriteLine($"\n[{Name}] Button clicked!");
        OnClicked(new ButtonClickEventArgs { ClickTime = DateTime.Now });
    }

    protected virtual void OnClicked(ButtonClickEventArgs e)
    {
        Clicked?.Invoke(this, e);
    }
}

public class ButtonClickEventArgs : EventArgs
{
    public DateTime ClickTime { get; set; }
}

public class ClickLogger
{
    private readonly string _logName;

    public ClickLogger(string logName)
    {
        _logName = logName;
    }

    public void OnButtonClicked(object? sender, ButtonClickEventArgs e)
    {
        if (sender is Button button)
        {
            Console.WriteLine($"  [{_logName}] Log: {button.Name} clicked at {e.ClickTime:HH:mm:ss}");
        }
    }
}

public class ClickCounter
{
    private int _count = 0;

    public void OnButtonClicked(object? sender, ButtonClickEventArgs e)
    {
        _count++;
        if (sender is Button button)
        {
            Console.WriteLine($"  [Counter] {button.Name} has been clicked {_count} time(s)");
        }
    }
}

public class ClickAnalytics
{
    private DateTime _lastClickTime = DateTime.MinValue;

    public void OnButtonClicked(object? sender, ButtonClickEventArgs e)
    {
        if (_lastClickTime != DateTime.MinValue)
        {
            var timeSinceLastClick = e.ClickTime - _lastClickTime;
            Console.WriteLine($"  [Analytics] Time since last click: {timeSinceLastClick.TotalSeconds:F2}s");
        }
        _lastClickTime = e.ClickTime;
    }
}

// Modern Observable pattern using IObservable<T> and IObserver<T>
public class NewsPublisher : IObservable<string>
{
    private readonly List<IObserver<string>> _observers = new();
    public string Name { get; }

    public NewsPublisher(string name)
    {
        Name = name;
    }

    public IDisposable Subscribe(IObserver<string> observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
            Console.WriteLine($"  Subscriber added to {Name}");
        }
        return new Unsubscriber(_observers, observer);
    }

    public void PublishNews(string news)
    {
        Console.WriteLine($"\n[{Name}] Publishing: {news}");
        foreach (var observer in _observers)
        {
            observer.OnNext(news);
        }
    }

    public void EndTransmission()
    {
        foreach (var observer in _observers.ToArray())
        {
            observer.OnCompleted();
        }
        _observers.Clear();
    }

    private class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<string>> _observers;
        private readonly IObserver<string> _observer;

        public Unsubscriber(List<IObserver<string>> observers, IObserver<string> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null && _observers.Contains(_observer))
            {
                _observers.Remove(_observer);
                Console.WriteLine("  Subscriber removed");
            }
        }
    }
}

public class NewsSubscriber : IObserver<string>
{
    private readonly string _name;
    private IDisposable? _unsubscriber;

    public NewsSubscriber(string name)
    {
        _name = name;
    }

    public virtual void Subscribe(IObservable<string> provider)
    {
        _unsubscriber = provider.Subscribe(this);
    }

    public void OnNext(string value)
    {
        Console.WriteLine($"  [{_name}] Received: {value}");
    }

    public void OnError(Exception error)
    {
        Console.WriteLine($"  [{_name}] Error: {error.Message}");
    }

    public void OnCompleted()
    {
        Console.WriteLine($"  [{_name}] No more news");
        Unsubscribe();
    }

    public void Unsubscribe()
    {
        _unsubscriber?.Dispose();
    }
}

#endregion

public static class ObserverDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Observer Pattern Demo ===\n");

        // Example 1: Stock Market
        Console.WriteLine("--- Example 1: Stock Market Monitoring ---");
        var appleStock = new Stock("AAPL", 120.00m);
        var googleStock = new Stock("GOOGL", 140.00m);

        var trader1 = new StockTrader("John");
        var trader2 = new StockTrader("Sarah");
        var display = new StockDisplay("Main Board");

        appleStock.Attach(trader1);
        appleStock.Attach(trader2);
        appleStock.Attach(display);
        googleStock.Attach(trader1);

        appleStock.Price = 95.00m;  // Should trigger buys
        appleStock.Price = 155.00m; // Should trigger sells
        
        googleStock.Price = 95.00m;

        // Example 2: Weather Station
        Console.WriteLine("\n\n--- Example 2: Weather Station ---");
        var weatherStation = new WeatherStation();
        
        var currentDisplay = new CurrentConditionsDisplay();
        var statisticsDisplay = new StatisticsDisplay();
        var forecastDisplay = new ForecastDisplay();

        weatherStation.Attach(currentDisplay);
        weatherStation.Attach(statisticsDisplay);
        weatherStation.Attach(forecastDisplay);

        weatherStation.SetMeasurements(80, 65, 30.4f);
        weatherStation.SetMeasurements(82, 70, 29.2f);
        weatherStation.SetMeasurements(78, 90, 29.2f);

        // Example 3: Event System
        Console.WriteLine("\n\n--- Example 3a: C# Events ---");
        var loginButton = new Button("Login");
        var submitButton = new Button("Submit");

        var logger = new ClickLogger("Main Logger");
        var counter = new ClickCounter();
        var analytics = new ClickAnalytics();

        loginButton.Clicked += logger.OnButtonClicked;
        loginButton.Clicked += counter.OnButtonClicked;
        loginButton.Clicked += analytics.OnButtonClicked;

        submitButton.Clicked += logger.OnButtonClicked;
        submitButton.Clicked += counter.OnButtonClicked;

        loginButton.Click();
        Thread.Sleep(1000);
        loginButton.Click();
        submitButton.Click();

        Console.WriteLine("\n--- Example 3b: IObservable/IObserver ---");
        var newsPublisher = new NewsPublisher("Tech News");
        
        var subscriber1 = new NewsSubscriber("Alice");
        var subscriber2 = new NewsSubscriber("Bob");

        subscriber1.Subscribe(newsPublisher);
        subscriber2.Subscribe(newsPublisher);

        newsPublisher.PublishNews("New smartphone released!");
        newsPublisher.PublishNews("Tech conference announced!");

        Console.WriteLine("\n  Alice unsubscribing...");
        subscriber1.Unsubscribe();

        newsPublisher.PublishNews("Stock market update!");
    }
}
