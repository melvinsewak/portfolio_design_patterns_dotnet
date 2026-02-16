namespace DesignPatterns.BehavioralPatterns;

/// <summary>
/// Interpreter Pattern - Define a representation for a grammar along with an interpreter
/// Used for interpreting sentences in a language or implementing domain-specific languages
/// </summary>

#region Example 1: Simple Boolean Expression Interpreter

public interface IBooleanExpression
{
    bool Interpret(Dictionary<string, bool> context);
}

public class VariableExpression : IBooleanExpression
{
    private readonly string _name;

    public VariableExpression(string name)
    {
        _name = name;
    }

    public bool Interpret(Dictionary<string, bool> context)
    {
        return context.GetValueOrDefault(_name, false);
    }

    public override string ToString() => _name;
}

public class AndExpression : IBooleanExpression
{
    private readonly IBooleanExpression _left;
    private readonly IBooleanExpression _right;

    public AndExpression(IBooleanExpression left, IBooleanExpression right)
    {
        _left = left;
        _right = right;
    }

    public bool Interpret(Dictionary<string, bool> context)
    {
        return _left.Interpret(context) && _right.Interpret(context);
    }

    public override string ToString() => $"({_left} AND {_right})";
}

public class OrExpression : IBooleanExpression
{
    private readonly IBooleanExpression _left;
    private readonly IBooleanExpression _right;

    public OrExpression(IBooleanExpression left, IBooleanExpression right)
    {
        _left = left;
        _right = right;
    }

    public bool Interpret(Dictionary<string, bool> context)
    {
        return _left.Interpret(context) || _right.Interpret(context);
    }

    public override string ToString() => $"({_left} OR {_right})";
}

public class NotExpression : IBooleanExpression
{
    private readonly IBooleanExpression _expression;

    public NotExpression(IBooleanExpression expression)
    {
        _expression = expression;
    }

    public bool Interpret(Dictionary<string, bool> context)
    {
        return !_expression.Interpret(context);
    }

    public override string ToString() => $"(NOT {_expression})";
}

#endregion

#region Example 2: SQL-like Query Interpreter

public interface IExpression
{
    string Interpret();
}

public class SelectExpression : IExpression
{
    private readonly List<string> _columns;

    public SelectExpression(params string[] columns)
    {
        _columns = new List<string>(columns);
    }

    public string Interpret()
    {
        return $"SELECT {string.Join(", ", _columns)}";
    }
}

public class FromExpression : IExpression
{
    private readonly string _table;

    public FromExpression(string table)
    {
        _table = table;
    }

    public string Interpret()
    {
        return $"FROM {_table}";
    }
}

public class WhereExpression : IExpression
{
    private readonly string _condition;

    public WhereExpression(string condition)
    {
        _condition = condition;
    }

    public string Interpret()
    {
        return $"WHERE {_condition}";
    }
}

public class QueryBuilder
{
    private readonly List<IExpression> _expressions = new();

    public QueryBuilder Select(params string[] columns)
    {
        _expressions.Add(new SelectExpression(columns));
        return this;
    }

    public QueryBuilder From(string table)
    {
        _expressions.Add(new FromExpression(table));
        return this;
    }

    public QueryBuilder Where(string condition)
    {
        _expressions.Add(new WhereExpression(condition));
        return this;
    }

    public string Build()
    {
        var query = string.Join(" ", _expressions.Select(e => e.Interpret()));
        Console.WriteLine($"  Built query: {query}");
        return query;
    }
}

#endregion

#region Example 3: Mathematical Expression Parser

public interface IMathExpression
{
    double Interpret();
}

public class NumberExpression : IMathExpression
{
    private readonly double _value;

    public NumberExpression(double value)
    {
        _value = value;
    }

    public double Interpret()
    {
        return _value;
    }

    public override string ToString() => _value.ToString();
}

public class AddExpression : IMathExpression
{
    private readonly IMathExpression _left;
    private readonly IMathExpression _right;

    public AddExpression(IMathExpression left, IMathExpression right)
    {
        _left = left;
        _right = right;
    }

    public double Interpret()
    {
        return _left.Interpret() + _right.Interpret();
    }

    public override string ToString() => $"({_left} + {_right})";
}

public class SubtractExpression : IMathExpression
{
    private readonly IMathExpression _left;
    private readonly IMathExpression _right;

    public SubtractExpression(IMathExpression left, IMathExpression right)
    {
        _left = left;
        _right = right;
    }

    public double Interpret()
    {
        return _left.Interpret() - _right.Interpret();
    }

    public override string ToString() => $"({_left} - {_right})";
}

public class MultiplyExpression : IMathExpression
{
    private readonly IMathExpression _left;
    private readonly IMathExpression _right;

    public MultiplyExpression(IMathExpression left, IMathExpression right)
    {
        _left = left;
        _right = right;
    }

    public double Interpret()
    {
        return _left.Interpret() * _right.Interpret();
    }

    public override string ToString() => $"({_left} * {_right})";
}

public class DivideExpression : IMathExpression
{
    private readonly IMathExpression _left;
    private readonly IMathExpression _right;

    public DivideExpression(IMathExpression left, IMathExpression right)
    {
        _left = left;
        _right = right;
    }

    public double Interpret()
    {
        var divisor = _right.Interpret();
        if (Math.Abs(divisor) < 0.0001)
            throw new DivideByZeroException();
        return _left.Interpret() / divisor;
    }

    public override string ToString() => $"({_left} / {_right})";
}

// Simple RPN (Reverse Polish Notation) Calculator
public class RpnCalculator
{
    public double Evaluate(string expression)
    {
        var tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var stack = new Stack<IMathExpression>();

        foreach (var token in tokens)
        {
            if (double.TryParse(token, out var number))
            {
                stack.Push(new NumberExpression(number));
            }
            else
            {
                if (stack.Count < 2)
                    throw new InvalidOperationException("Invalid expression");

                var right = stack.Pop();
                var left = stack.Pop();

                IMathExpression result = token switch
                {
                    "+" => new AddExpression(left, right),
                    "-" => new SubtractExpression(left, right),
                    "*" => new MultiplyExpression(left, right),
                    "/" => new DivideExpression(left, right),
                    _ => throw new InvalidOperationException($"Unknown operator: {token}")
                };

                stack.Push(result);
            }
        }

        if (stack.Count != 1)
            throw new InvalidOperationException("Invalid expression");

        var finalExpression = stack.Pop();
        Console.WriteLine($"  Expression tree: {finalExpression}");
        return finalExpression.Interpret();
    }
}

#endregion

#region Example 4: Simple Regex-like Pattern Matcher

public interface IPattern
{
    bool Matches(string text, int position);
    int MatchLength { get; }
}

public class LiteralPattern : IPattern
{
    private readonly string _literal;
    public int MatchLength { get; private set; }

    public LiteralPattern(string literal)
    {
        _literal = literal;
    }

    public bool Matches(string text, int position)
    {
        if (position + _literal.Length > text.Length)
        {
            MatchLength = 0;
            return false;
        }

        var matches = text.Substring(position, _literal.Length) == _literal;
        MatchLength = matches ? _literal.Length : 0;
        return matches;
    }

    public override string ToString() => $"'{_literal}'";
}

public class WildcardPattern : IPattern
{
    public int MatchLength { get; private set; }

    public bool Matches(string text, int position)
    {
        if (position >= text.Length)
        {
            MatchLength = 0;
            return false;
        }

        MatchLength = 1;
        return true;
    }

    public override string ToString() => "?";
}

public class SequencePattern : IPattern
{
    private readonly List<IPattern> _patterns;
    public int MatchLength { get; private set; }

    public SequencePattern(params IPattern[] patterns)
    {
        _patterns = new List<IPattern>(patterns);
    }

    public bool Matches(string text, int position)
    {
        int currentPos = position;
        int totalLength = 0;

        foreach (var pattern in _patterns)
        {
            if (!pattern.Matches(text, currentPos))
            {
                MatchLength = 0;
                return false;
            }

            totalLength += pattern.MatchLength;
            currentPos += pattern.MatchLength;
        }

        MatchLength = totalLength;
        return true;
    }

    public override string ToString() => string.Join("", _patterns);
}

public class OrPattern : IPattern
{
    private readonly IPattern _left;
    private readonly IPattern _right;
    public int MatchLength { get; private set; }

    public OrPattern(IPattern left, IPattern right)
    {
        _left = left;
        _right = right;
    }

    public bool Matches(string text, int position)
    {
        if (_left.Matches(text, position))
        {
            MatchLength = _left.MatchLength;
            return true;
        }

        if (_right.Matches(text, position))
        {
            MatchLength = _right.MatchLength;
            return true;
        }

        MatchLength = 0;
        return false;
    }

    public override string ToString() => $"({_left}|{_right})";
}

public class PatternMatcher
{
    public bool Match(string text, IPattern pattern)
    {
        Console.WriteLine($"  Pattern: {pattern}");
        Console.WriteLine($"  Text: '{text}'");
        
        for (int i = 0; i <= text.Length; i++)
        {
            if (pattern.Matches(text, i) && i + pattern.MatchLength == text.Length)
            {
                Console.WriteLine($"  ✓ Match found at position {i}");
                return true;
            }
        }

        Console.WriteLine($"  ✗ No match found");
        return false;
    }
}

#endregion

public static class InterpreterDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Interpreter Pattern Demo ===\n");

        // Example 1: Boolean Expressions
        Console.WriteLine("--- Example 1: Boolean Expression Interpreter ---");
        var context = new Dictionary<string, bool>
        {
            ["A"] = true,
            ["B"] = false,
            ["C"] = true
        };

        // (A AND B) OR C
        var a = new VariableExpression("A");
        var b = new VariableExpression("B");
        var c = new VariableExpression("C");
        var expr1 = new OrExpression(new AndExpression(a, b), c);

        Console.WriteLine($"Expression: {expr1}");
        Console.WriteLine($"Context: A={context["A"]}, B={context["B"]}, C={context["C"]}");
        Console.WriteLine($"Result: {expr1.Interpret(context)}\n");

        // NOT (A OR B) AND C
        var expr2 = new AndExpression(new NotExpression(new OrExpression(a, b)), c);
        Console.WriteLine($"Expression: {expr2}");
        Console.WriteLine($"Context: A={context["A"]}, B={context["B"]}, C={context["C"]}");
        Console.WriteLine($"Result: {expr2.Interpret(context)}");

        // Example 2: SQL Query Builder
        Console.WriteLine("\n\n--- Example 2: SQL Query Builder ---");
        
        var query1 = new QueryBuilder()
            .Select("name", "email")
            .From("users")
            .Where("age > 18")
            .Build();

        Console.WriteLine();

        var query2 = new QueryBuilder()
            .Select("*")
            .From("products")
            .Where("price < 100 AND category = 'electronics'")
            .Build();

        // Example 3: Mathematical Expression Parser (RPN)
        Console.WriteLine("\n\n--- Example 3: Mathematical Expression Parser (RPN) ---");
        var calculator = new RpnCalculator();

        var expressions = new[]
        {
            "3 4 +",           // 3 + 4 = 7
            "15 7 1 1 + - /",  // 15 / (7 - (1 + 1)) = 3
            "5 1 2 + 4 * + 3 -" // 5 + ((1 + 2) * 4) - 3 = 14
        };

        foreach (var expr in expressions)
        {
            Console.WriteLine($"\nEvaluating: {expr}");
            try
            {
                var result = calculator.Evaluate(expr);
                Console.WriteLine($"  Result: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error: {ex.Message}");
            }
        }

        // Example 4: Pattern Matcher
        Console.WriteLine("\n\n--- Example 4: Pattern Matcher ---");
        var matcher = new PatternMatcher();

        // Match literal
        Console.WriteLine("\nTest 1: Literal pattern");
        var pattern1 = new LiteralPattern("hello");
        matcher.Match("hello", pattern1);

        // Match with wildcard
        Console.WriteLine("\nTest 2: Pattern with wildcard");
        var pattern2 = new SequencePattern(
            new LiteralPattern("he"),
            new WildcardPattern(),
            new WildcardPattern(),
            new LiteralPattern("o")
        );
        matcher.Match("hello", pattern2);

        // Match with OR
        Console.WriteLine("\nTest 3: OR pattern");
        var pattern3 = new OrPattern(
            new LiteralPattern("cat"),
            new LiteralPattern("dog")
        );
        matcher.Match("cat", pattern3);
        matcher.Match("dog", pattern3);
        matcher.Match("bird", pattern3);
    }
}
