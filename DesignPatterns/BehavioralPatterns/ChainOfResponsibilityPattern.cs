namespace DesignPatterns.BehavioralPatterns;

/// <summary>
/// Chain of Responsibility Pattern - Pass requests along a chain of handlers
/// Each handler decides either to process the request or pass it to the next handler
/// </summary>

#region Example 1: Support Ticket System

// Base handler
public abstract class SupportHandler
{
    protected SupportHandler? NextHandler;

    public SupportHandler SetNext(SupportHandler handler)
    {
        NextHandler = handler;
        return handler;
    }

    public abstract void HandleTicket(SupportTicket ticket);
}

public class SupportTicket
{
    public string Issue { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class Level1Support : SupportHandler
{
    public override void HandleTicket(SupportTicket ticket)
    {
        if (ticket.Priority <= 3 && ticket.Category == "General")
        {
            Console.WriteLine($"[Level 1 Support] Handling ticket: {ticket.Issue}");
            Console.WriteLine("  → Providing basic troubleshooting steps");
        }
        else if (NextHandler != null)
        {
            Console.WriteLine("[Level 1 Support] Escalating to next level...");
            NextHandler.HandleTicket(ticket);
        }
    }
}

public class Level2Support : SupportHandler
{
    public override void HandleTicket(SupportTicket ticket)
    {
        if (ticket.Priority <= 7 && ticket.Category is "Technical" or "General")
        {
            Console.WriteLine($"[Level 2 Support] Handling ticket: {ticket.Issue}");
            Console.WriteLine("  → Running advanced diagnostics and fixes");
        }
        else if (NextHandler != null)
        {
            Console.WriteLine("[Level 2 Support] Escalating to senior team...");
            NextHandler.HandleTicket(ticket);
        }
    }
}

public class SeniorSupport : SupportHandler
{
    public override void HandleTicket(SupportTicket ticket)
    {
        if (ticket.Priority <= 10)
        {
            Console.WriteLine($"[Senior Support] Handling critical ticket: {ticket.Issue}");
            Console.WriteLine("  → Providing expert-level resolution");
        }
        else
        {
            Console.WriteLine($"[Senior Support] Unable to handle: {ticket.Issue}");
        }
    }
}

#endregion

#region Example 2: Expense Approval System

public abstract class ExpenseApprover
{
    protected ExpenseApprover? Successor;
    public string ApproverName { get; }  // Changed to public
    protected decimal ApprovalLimit;

    protected ExpenseApprover(string name, decimal limit)
    {
        ApproverName = name;
        ApprovalLimit = limit;
    }

    public void SetSuccessor(ExpenseApprover successor)
    {
        Successor = successor;
    }

    public abstract void ApproveExpense(ExpenseRequest request);
}

public class ExpenseRequest
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
}

public class TeamLead : ExpenseApprover
{
    public TeamLead() : base("Team Lead", 1000) { }

    public override void ApproveExpense(ExpenseRequest request)
    {
        if (request.Amount <= ApprovalLimit)
        {
            Console.WriteLine($"[{ApproverName}] Approved ${request.Amount:N2} for: {request.Description}");
        }
        else if (Successor != null)
        {
            Console.WriteLine($"[{ApproverName}] Cannot approve ${request.Amount:N2}. Forwarding to {Successor.ApproverName}");
            Successor.ApproveExpense(request);
        }
    }
}

public class Manager : ExpenseApprover
{
    public Manager() : base("Manager", 5000) { }

    public override void ApproveExpense(ExpenseRequest request)
    {
        if (request.Amount <= ApprovalLimit)
        {
            Console.WriteLine($"[{ApproverName}] Approved ${request.Amount:N2} for: {request.Description}");
        }
        else if (Successor != null)
        {
            Console.WriteLine($"[{ApproverName}] Cannot approve ${request.Amount:N2}. Forwarding to {Successor.ApproverName}");
            Successor.ApproveExpense(request);
        }
    }
}

public class Director : ExpenseApprover
{
    public Director() : base("Director", 20000) { }

    public override void ApproveExpense(ExpenseRequest request)
    {
        if (request.Amount <= ApprovalLimit)
        {
            Console.WriteLine($"[{ApproverName}] Approved ${request.Amount:N2} for: {request.Description}");
        }
        else
        {
            Console.WriteLine($"[{ApproverName}] Amount ${request.Amount:N2} exceeds my authority. Requires board approval.");
        }
    }
}

#endregion

#region Example 3: Request Validation Pipeline

public interface IRequestValidator
{
    IRequestValidator? Next { get; set; }
    bool Validate(HttpRequestData request);
}

public class HttpRequestData
{
    public string Method { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public string Body { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
}

public abstract class BaseValidator : IRequestValidator
{
    public IRequestValidator? Next { get; set; }

    public abstract bool Validate(HttpRequestData request);

    protected bool PassToNext(HttpRequestData request)
    {
        return Next?.Validate(request) ?? true;
    }
}

public class AuthenticationValidator : BaseValidator
{
    public override bool Validate(HttpRequestData request)
    {
        Console.WriteLine("→ Checking authentication...");
        if (!request.Headers.ContainsKey("Authorization"))
        {
            Console.WriteLine("  ✗ Authentication failed: No Authorization header");
            return false;
        }

        Console.WriteLine("  ✓ Authentication successful");
        return PassToNext(request);
    }
}

public class RateLimitValidator : BaseValidator
{
    private readonly Dictionary<string, int> _requestCounts = new();
    private const int MaxRequests = 100;

    public override bool Validate(HttpRequestData request)
    {
        Console.WriteLine("→ Checking rate limit...");
        
        if (!_requestCounts.ContainsKey(request.IpAddress))
            _requestCounts[request.IpAddress] = 0;

        _requestCounts[request.IpAddress]++;

        if (_requestCounts[request.IpAddress] > MaxRequests)
        {
            Console.WriteLine($"  ✗ Rate limit exceeded for {request.IpAddress}");
            return false;
        }

        Console.WriteLine($"  ✓ Rate limit OK ({_requestCounts[request.IpAddress]}/{MaxRequests})");
        return PassToNext(request);
    }
}

public class InputSanitizationValidator : BaseValidator
{
    public override bool Validate(HttpRequestData request)
    {
        Console.WriteLine("→ Sanitizing input...");
        
        if (request.Body.Contains("<script>") || request.Body.Contains("DROP TABLE"))
        {
            Console.WriteLine("  ✗ Malicious content detected");
            return false;
        }

        Console.WriteLine("  ✓ Input is safe");
        return PassToNext(request);
    }
}

#endregion

public static class ChainOfResponsibilityDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Chain of Responsibility Pattern Demo ===\n");

        // Example 1: Support Ticket System
        Console.WriteLine("--- Example 1: Support Ticket System ---");
        var level1 = new Level1Support();
        var level2 = new Level2Support();
        var senior = new SeniorSupport();

        level1.SetNext(level2).SetNext(senior);

        var tickets = new[]
        {
            new SupportTicket { Issue = "Password reset", Priority = 2, Category = "General" },
            new SupportTicket { Issue = "Database connection error", Priority = 6, Category = "Technical" },
            new SupportTicket { Issue = "System architecture review", Priority = 9, Category = "Critical" }
        };

        foreach (var ticket in tickets)
        {
            level1.HandleTicket(ticket);
            Console.WriteLine();
        }

        // Example 2: Expense Approval System
        Console.WriteLine("\n--- Example 2: Expense Approval System ---");
        var teamLead = new TeamLead();
        var manager = new Manager();
        var director = new Director();

        teamLead.SetSuccessor(manager);
        manager.SetSuccessor(director);

        var expenses = new[]
        {
            new ExpenseRequest { Description = "Office supplies", Amount = 500, RequestedBy = "John" },
            new ExpenseRequest { Description = "New laptops", Amount = 3500, RequestedBy = "Sarah" },
            new ExpenseRequest { Description = "Conference sponsorship", Amount = 15000, RequestedBy = "Mike" },
            new ExpenseRequest { Description = "Office renovation", Amount = 50000, RequestedBy = "CEO" }
        };

        foreach (var expense in expenses)
        {
            teamLead.ApproveExpense(expense);
            Console.WriteLine();
        }

        // Example 3: Request Validation Pipeline
        Console.WriteLine("\n--- Example 3: Request Validation Pipeline ---");
        var authValidator = new AuthenticationValidator();
        var rateLimitValidator = new RateLimitValidator();
        var sanitizationValidator = new InputSanitizationValidator();

        authValidator.Next = rateLimitValidator;
        rateLimitValidator.Next = sanitizationValidator;

        var requests = new[]
        {
            new HttpRequestData 
            { 
                Method = "POST",
                Headers = new() { ["Authorization"] = "Bearer token123" },
                Body = "Valid content",
                IpAddress = "192.168.1.1"
            },
            new HttpRequestData 
            { 
                Method = "POST",
                Body = "Normal request",
                IpAddress = "192.168.1.2"
            },
            new HttpRequestData 
            { 
                Method = "POST",
                Headers = new() { ["Authorization"] = "Bearer token456" },
                Body = "<script>alert('xss')</script>",
                IpAddress = "192.168.1.3"
            }
        };

        foreach (var request in requests)
        {
            Console.WriteLine($"\nValidating request from {request.IpAddress}:");
            bool isValid = authValidator.Validate(request);
            Console.WriteLine($"Final result: {(isValid ? "✓ ACCEPTED" : "✗ REJECTED")}");
        }
    }
}
