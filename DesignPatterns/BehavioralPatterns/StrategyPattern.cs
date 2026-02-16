namespace DesignPatterns.BehavioralPatterns;

/// <summary>
/// Strategy Pattern - Define a family of algorithms, encapsulate each one, and make them interchangeable
/// Strategy lets the algorithm vary independently from clients that use it
/// </summary>

#region Example 1: Payment Processing

public interface IPaymentStrategy
{
    bool ProcessPayment(decimal amount);
    string GetPaymentMethod();
}

public class CreditCardPayment : IPaymentStrategy
{
    private readonly string _cardNumber;
    private readonly string _cvv;

    public CreditCardPayment(string cardNumber, string cvv)
    {
        _cardNumber = cardNumber;
        _cvv = cvv;
    }

    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"\n[Credit Card] Processing payment of ${amount:F2}");
        Console.WriteLine($"  Card: **** **** **** {_cardNumber[^4..]}");
        Console.WriteLine("  Validating card details...");
        Console.WriteLine("  Contacting payment gateway...");
        Console.WriteLine("  ✓ Payment successful!");
        return true;
    }

    public string GetPaymentMethod() => "Credit Card";
}

public class PayPalPayment : IPaymentStrategy
{
    private readonly string _email;

    public PayPalPayment(string email)
    {
        _email = email;
    }

    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"\n[PayPal] Processing payment of ${amount:F2}");
        Console.WriteLine($"  Account: {_email}");
        Console.WriteLine("  Redirecting to PayPal...");
        Console.WriteLine("  Authenticating user...");
        Console.WriteLine("  ✓ Payment successful!");
        return true;
    }

    public string GetPaymentMethod() => "PayPal";
}

public class CryptocurrencyPayment : IPaymentStrategy
{
    private readonly string _walletAddress;
    private readonly string _cryptoType;

    public CryptocurrencyPayment(string walletAddress, string cryptoType)
    {
        _walletAddress = walletAddress;
        _cryptoType = cryptoType;
    }

    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"\n[{_cryptoType}] Processing payment of ${amount:F2}");
        Console.WriteLine($"  Wallet: {_walletAddress[..8]}...{_walletAddress[^8..]}");
        Console.WriteLine("  Broadcasting transaction to blockchain...");
        Console.WriteLine("  Waiting for confirmation...");
        Console.WriteLine("  ✓ Payment confirmed!");
        return true;
    }

    public string GetPaymentMethod() => $"{_cryptoType} Cryptocurrency";
}

public class ShoppingCart
{
    private IPaymentStrategy? _paymentStrategy;
    private readonly List<(string item, decimal price)> _items = new();

    public void AddItem(string item, decimal price)
    {
        _items.Add((item, price));
        Console.WriteLine($"Added to cart: {item} - ${price:F2}");
    }

    public void SetPaymentStrategy(IPaymentStrategy strategy)
    {
        _paymentStrategy = strategy;
        Console.WriteLine($"\nPayment method set to: {strategy.GetPaymentMethod()}");
    }

    public void Checkout()
    {
        if (_paymentStrategy == null)
        {
            Console.WriteLine("Please select a payment method first");
            return;
        }

        var total = _items.Sum(item => item.price);
        Console.WriteLine($"\n=== Checkout ===");
        Console.WriteLine($"Items: {_items.Count}");
        Console.WriteLine($"Total: ${total:F2}");

        if (_paymentStrategy.ProcessPayment(total))
        {
            Console.WriteLine("\n🎉 Order completed successfully!");
            _items.Clear();
        }
    }
}

#endregion

#region Example 2: Sorting Strategies

public interface ISortStrategy<T>
{
    void Sort(List<T> list);
    string GetAlgorithmName();
}

public class BubbleSort<T> : ISortStrategy<T> where T : IComparable<T>
{
    public void Sort(List<T> list)
    {
        Console.WriteLine("  Using Bubble Sort algorithm...");
        int n = list.Count;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (list[j].CompareTo(list[j + 1]) > 0)
                {
                    (list[j], list[j + 1]) = (list[j + 1], list[j]);
                }
            }
        }
    }

    public string GetAlgorithmName() => "Bubble Sort (O(n²))";
}

public class QuickSort<T> : ISortStrategy<T> where T : IComparable<T>
{
    public void Sort(List<T> list)
    {
        Console.WriteLine("  Using Quick Sort algorithm...");
        QuickSortRecursive(list, 0, list.Count - 1);
    }

    private void QuickSortRecursive(List<T> list, int low, int high)
    {
        if (low < high)
        {
            int pi = Partition(list, low, high);
            QuickSortRecursive(list, low, pi - 1);
            QuickSortRecursive(list, pi + 1, high);
        }
    }

    private int Partition(List<T> list, int low, int high)
    {
        T pivot = list[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if (list[j].CompareTo(pivot) <= 0)
            {
                i++;
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        (list[i + 1], list[high]) = (list[high], list[i + 1]);
        return i + 1;
    }

    public string GetAlgorithmName() => "Quick Sort (O(n log n))";
}

public class MergeSort<T> : ISortStrategy<T> where T : IComparable<T>
{
    public void Sort(List<T> list)
    {
        Console.WriteLine("  Using Merge Sort algorithm...");
        var temp = new T[list.Count];
        MergeSortRecursive(list, temp, 0, list.Count - 1);
    }

    private void MergeSortRecursive(List<T> list, T[] temp, int left, int right)
    {
        if (left < right)
        {
            int mid = (left + right) / 2;
            MergeSortRecursive(list, temp, left, mid);
            MergeSortRecursive(list, temp, mid + 1, right);
            Merge(list, temp, left, mid, right);
        }
    }

    private void Merge(List<T> list, T[] temp, int left, int mid, int right)
    {
        int i = left, j = mid + 1, k = left;

        while (i <= mid && j <= right)
        {
            if (list[i].CompareTo(list[j]) <= 0)
                temp[k++] = list[i++];
            else
                temp[k++] = list[j++];
        }

        while (i <= mid)
            temp[k++] = list[i++];

        while (j <= right)
            temp[k++] = list[j++];

        for (i = left; i <= right; i++)
            list[i] = temp[i];
    }

    public string GetAlgorithmName() => "Merge Sort (O(n log n))";
}

public class SortContext<T> where T : IComparable<T>
{
    private ISortStrategy<T>? _strategy;

    public void SetStrategy(ISortStrategy<T> strategy)
    {
        _strategy = strategy;
        Console.WriteLine($"Strategy set to: {strategy.GetAlgorithmName()}");
    }

    public void Sort(List<T> list)
    {
        if (_strategy == null)
        {
            Console.WriteLine("No sorting strategy set");
            return;
        }

        Console.WriteLine($"Sorting {list.Count} elements...");
        _strategy.Sort(list);
        Console.WriteLine($"✓ Sorted: {string.Join(", ", list)}");
    }
}

#endregion

#region Example 3: Compression Strategies

public interface ICompressionStrategy
{
    byte[] Compress(byte[] data);
    byte[] Decompress(byte[] data);
    string GetAlgorithmName();
}

public class ZipCompression : ICompressionStrategy
{
    public byte[] Compress(byte[] data)
    {
        Console.WriteLine($"  [ZIP] Compressing {data.Length} bytes...");
        // Simplified simulation
        var compressed = new byte[data.Length / 2]; // Simulated compression
        Console.WriteLine($"  [ZIP] Compressed to {compressed.Length} bytes ({(compressed.Length * 100.0 / data.Length):F1}%)");
        return compressed;
    }

    public byte[] Decompress(byte[] data)
    {
        Console.WriteLine($"  [ZIP] Decompressing {data.Length} bytes...");
        var decompressed = new byte[data.Length * 2]; // Simulated decompression
        Console.WriteLine($"  [ZIP] Decompressed to {decompressed.Length} bytes");
        return decompressed;
    }

    public string GetAlgorithmName() => "ZIP Compression";
}

public class GzipCompression : ICompressionStrategy
{
    public byte[] Compress(byte[] data)
    {
        Console.WriteLine($"  [GZIP] Compressing {data.Length} bytes...");
        var compressed = new byte[(int)(data.Length * 0.4)]; // Better compression ratio
        Console.WriteLine($"  [GZIP] Compressed to {compressed.Length} bytes ({(compressed.Length * 100.0 / data.Length):F1}%)");
        return compressed;
    }

    public byte[] Decompress(byte[] data)
    {
        Console.WriteLine($"  [GZIP] Decompressing {data.Length} bytes...");
        var decompressed = new byte[(int)(data.Length / 0.4)];
        Console.WriteLine($"  [GZIP] Decompressed to {decompressed.Length} bytes");
        return decompressed;
    }

    public string GetAlgorithmName() => "GZIP Compression";
}

public class LzmaCompression : ICompressionStrategy
{
    public byte[] Compress(byte[] data)
    {
        Console.WriteLine($"  [LZMA] Compressing {data.Length} bytes...");
        Console.WriteLine("  [LZMA] Using advanced compression (slower)...");
        var compressed = new byte[(int)(data.Length * 0.3)]; // Best compression
        Console.WriteLine($"  [LZMA] Compressed to {compressed.Length} bytes ({(compressed.Length * 100.0 / data.Length):F1}%)");
        return compressed;
    }

    public byte[] Decompress(byte[] data)
    {
        Console.WriteLine($"  [LZMA] Decompressing {data.Length} bytes...");
        var decompressed = new byte[(int)(data.Length / 0.3)];
        Console.WriteLine($"  [LZMA] Decompressed to {decompressed.Length} bytes");
        return decompressed;
    }

    public string GetAlgorithmName() => "LZMA Compression";
}

public class FileCompressor
{
    private ICompressionStrategy _strategy;

    public FileCompressor(ICompressionStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(ICompressionStrategy strategy)
    {
        _strategy = strategy;
        Console.WriteLine($"\nCompression strategy changed to: {strategy.GetAlgorithmName()}");
    }

    public void CompressFile(string fileName, int fileSize)
    {
        Console.WriteLine($"\nCompressing file: {fileName}");
        var data = new byte[fileSize];
        var compressed = _strategy.Compress(data);
        Console.WriteLine($"✓ Compression complete");
    }

    public void DecompressFile(string fileName, int compressedSize)
    {
        Console.WriteLine($"\nDecompressing file: {fileName}");
        var data = new byte[compressedSize];
        var decompressed = _strategy.Decompress(data);
        Console.WriteLine($"✓ Decompression complete");
    }
}

#endregion

public static class StrategyDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Strategy Pattern Demo ===\n");

        // Example 1: Payment Processing
        Console.WriteLine("--- Example 1: Payment Processing ---");
        var cart = new ShoppingCart();
        
        cart.AddItem("Laptop", 999.99m);
        cart.AddItem("Mouse", 29.99m);
        cart.AddItem("Keyboard", 79.99m);

        // Try credit card
        cart.SetPaymentStrategy(new CreditCardPayment("1234567812345678", "123"));
        cart.Checkout();

        // New purchase with PayPal
        Console.WriteLine("\n--- New Purchase ---");
        cart.AddItem("Monitor", 299.99m);
        cart.SetPaymentStrategy(new PayPalPayment("user@example.com"));
        cart.Checkout();

        // Cryptocurrency payment
        Console.WriteLine("\n--- New Purchase ---");
        cart.AddItem("Headphones", 149.99m);
        cart.SetPaymentStrategy(new CryptocurrencyPayment("0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb5", "Bitcoin"));
        cart.Checkout();

        // Example 2: Sorting Strategies
        Console.WriteLine("\n\n--- Example 2: Sorting Strategies ---");
        var sorter = new SortContext<int>();

        // Small dataset - Bubble sort is acceptable
        var smallList = new List<int> { 5, 2, 8, 1, 9 };
        Console.WriteLine($"\nOriginal: {string.Join(", ", smallList)}");
        sorter.SetStrategy(new BubbleSort<int>());
        sorter.Sort(new List<int>(smallList));

        // Medium dataset - Quick sort is better
        var mediumList = new List<int> { 64, 34, 25, 12, 22, 11, 90, 88, 45, 50 };
        Console.WriteLine($"\nOriginal: {string.Join(", ", mediumList)}");
        sorter.SetStrategy(new QuickSort<int>());
        sorter.Sort(new List<int>(mediumList));

        // Large dataset - Merge sort for stability
        var largeList = new List<int> { 38, 27, 43, 3, 9, 82, 10, 1, 54, 23, 67, 15 };
        Console.WriteLine($"\nOriginal: {string.Join(", ", largeList)}");
        sorter.SetStrategy(new MergeSort<int>());
        sorter.Sort(new List<int>(largeList));

        // Example 3: Compression Strategies
        Console.WriteLine("\n\n--- Example 3: File Compression ---");
        
        var compressor = new FileCompressor(new ZipCompression());
        compressor.CompressFile("document.txt", 10000);

        // Switch to GZIP for better compression
        compressor.SetStrategy(new GzipCompression());
        compressor.CompressFile("image.png", 50000);

        // Use LZMA for archival (best compression)
        compressor.SetStrategy(new LzmaCompression());
        compressor.CompressFile("archive.tar", 100000);
        compressor.DecompressFile("archive.tar.lzma", 30000);
    }
}
