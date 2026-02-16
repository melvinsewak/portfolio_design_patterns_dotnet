# Interpreter Pattern

## Intent
The Interpreter pattern defines a representation for a grammar along with an interpreter that uses the representation to interpret sentences in the language.

## Problem
When you need to:
- Interpret a language or notation
- Parse and evaluate expressions
- Implement domain-specific languages (DSL)
- Process structured text or commands

**Without the pattern:**
- Complex parsing logic scattered throughout code
- Difficult to extend grammar
- Hard to maintain expression evaluation
- No clear representation of grammar rules

## Solution
Define a class for each grammar rule, creating an abstract syntax tree (AST). Each class implements an interpret method that evaluates the expression.

## UML Diagram
```
┌──────────────────────┐
│      Context         │
├──────────────────────┤
│- variables: Map      │
│+ Lookup(name)        │
└──────────────────────┘
           │
           │
┌──────────┴───────────┐
│  <<interface>>       │
│  AbstractExpression  │
├──────────────────────┤
│+ Interpret(Context)  │
└──────────△───────────┘
           │
    ┌──────┴──────┬──────────────┐
    │             │              │
┌───┴──────┐ ┌───┴────────┐ ┌───┴──────────┐
│Terminal  │ │NonTerminal │ │NonTerminal   │
│Expression│ │Expression1 │ │Expression2   │
├──────────┤ ├────────────┤ ├──────────────┤
│+Interpret│ │-expr1      │ │-expr2        │
│          │ │-expr2      │ │-expr3        │
│          │ │+Interpret()│ │+Interpret()  │
└──────────┘ └────────────┘ └──────────────┘
```

## When to Use

### Use When:
- **Simple Grammar** - Grammar is simple and well-defined
- **Performance Not Critical** - Efficiency is not a primary concern
- **Extensibility** - Grammar may change or extend
- **DSL Implementation** - Creating domain-specific language
- **Expression Evaluation** - Mathematical or logical expressions
- **Rule Engine** - Implementing business rules

### Avoid When:
- **Complex Grammar** - Grammar is too complex (use parser generators)
- **Performance Critical** - Interpretation overhead is unacceptable
- **Frequent Changes** - Grammar changes very frequently
- **Large Language** - Better tools available (ANTLR, Lex/Yacc)

## Real-World Examples

### 1. **Regular Expressions**
   - Pattern matching engine
   - Each regex operator is an expression
   - Compose complex patterns

### 2. **SQL Query Parsing**
   - SELECT, FROM, WHERE as expressions
   - Build query object model
   - Execute queries

### 3. **Calculator**
   - Mathematical expressions: +, -, *, /
   - Reverse Polish Notation (RPN)
   - Expression trees

### 4. **Boolean Logic**
   - AND, OR, NOT operators
   - Truth table evaluation
   - Rule evaluation engines

### 5. **Configuration Language**
   - Parse config files
   - Domain-specific settings
   - Custom markup languages

### 6. **Music Notation**
   - Notes, rests, dynamics
   - Parse sheet music
   - MIDI generation

## Advantages

1. **Easy to Change Grammar** - Add new expressions easily
2. **Easy to Implement** - Each rule is a class
3. **Extensible** - Add new operations by adding classes
4. **Reusable** - Expressions can be reused
5. **Testable** - Test each expression independently

## Disadvantages

1. **Complex Grammars** - Many classes for complex grammars
2. **Performance** - Slower than compiled solutions
3. **Maintenance** - Large number of classes to maintain
4. **No Error Recovery** - Poor error handling
5. **Limited Optimization** - Hard to optimize

## Best Practices

### 1. **Define Clear Expression Interface**
```csharp
public interface IExpression
{
    bool Interpret(Dictionary<string, bool> context);
}
```

### 2. **Implement Terminal Expressions**
```csharp
public class VariableExpression : IExpression
{
    private readonly string _name;
    
    public VariableExpression(string name) => _name = name;
    
    public bool Interpret(Dictionary<string, bool> context)
    {
        return context.GetValueOrDefault(_name, false);
    }
}
```

### 3. **Implement Non-Terminal Expressions**
```csharp
public class AndExpression : IExpression
{
    private readonly IExpression _left;
    private readonly IExpression _right;
    
    public AndExpression(IExpression left, IExpression right)
    {
        _left = left;
        _right = right;
    }
    
    public bool Interpret(Dictionary<string, bool> context)
    {
        return _left.Interpret(context) && _right.Interpret(context);
    }
}
```

### 4. **Use Context Object**
```csharp
public class Context
{
    private readonly Dictionary<string, object> _variables = new();
    
    public void SetVariable(string name, object value)
    {
        _variables[name] = value;
    }
    
    public object GetVariable(string name)
    {
        return _variables.GetValueOrDefault(name)!;
    }
}
```

### 5. **Build Expression Trees**
```csharp
// (A AND B) OR C
var a = new VariableExpression("A");
var b = new VariableExpression("B");
var c = new VariableExpression("C");
var expr = new OrExpression(
    new AndExpression(a, b),
    c
);
```

### 6. **Use Composite Pattern for Structure**
```csharp
public abstract class Expression
{
    public abstract TResult Accept<TResult>(IVisitor<TResult> visitor);
}

public class CompositeExpression : Expression
{
    protected readonly List<Expression> _children = new();
    
    public void Add(Expression expr) => _children.Add(expr);
}
```

### 7. **Implement Parser for Complex Grammar**
```csharp
public class ExpressionParser
{
    public IExpression Parse(string input)
    {
        var tokens = Tokenize(input);
        return BuildExpression(tokens);
    }
    
    private IExpression BuildExpression(Queue<string> tokens)
    {
        // Build expression tree from tokens
    }
}
```

## Related Patterns

- **Composite**: Expression trees use Composite structure
- **Visitor**: Can traverse expression tree with Visitor
- **Flyweight**: Can share terminal symbols
- **Iterator**: Traverse expression structure

## C# Specific Considerations

### Using Expression Trees (System.Linq.Expressions)
```csharp
using System.Linq.Expressions;

// Build expression: (x, y) => x + y
var x = Expression.Parameter(typeof(int), "x");
var y = Expression.Parameter(typeof(int), "y");
var add = Expression.Add(x, y);
var lambda = Expression.Lambda<Func<int, int, int>>(add, x, y);

// Compile and execute
var func = lambda.Compile();
var result = func(3, 4); // 7
```

### Dynamic Code Generation
```csharp
public class CodeGenerator
{
    public Func<T, TResult> Compile<T, TResult>(IExpression expr)
    {
        // Generate IL or use Expression Trees
        var parameter = Expression.Parameter(typeof(T));
        var body = ConvertToExpression(expr, parameter);
        return Expression.Lambda<Func<T, TResult>>(body, parameter).Compile();
    }
}
```

### Pattern Matching for Parsing (C# 8+)
```csharp
public IExpression Parse(string token) => token switch
{
    "+" => new AddExpression(),
    "-" => new SubtractExpression(),
    "*" => new MultiplyExpression(),
    "/" => new DivideExpression(),
    _ => new NumberExpression(double.Parse(token))
};
```

## Implementation Variations

### 1. **Simple Interpreter**
```csharp
public interface IExpression<T>
{
    T Interpret();
}

public class Constant : IExpression<int>
{
    private readonly int _value;
    public int Interpret() => _value;
}
```

### 2. **Context-Based Interpreter**
```csharp
public interface IExpression
{
    object Interpret(Context context);
}
```

### 3. **Visitor-Based Interpreter**
```csharp
public interface IExpressionVisitor<TResult>
{
    TResult Visit(AddExpression expr);
    TResult Visit(NumberExpression expr);
}

public abstract class Expression
{
    public abstract TResult Accept<TResult>(
        IExpressionVisitor<TResult> visitor);
}
```

### 4. **Stack-Based Interpreter (RPN)**
```csharp
public class RpnInterpreter
{
    public double Evaluate(string expression)
    {
        var stack = new Stack<double>();
        var tokens = expression.Split(' ');
        
        foreach (var token in tokens)
        {
            if (double.TryParse(token, out var number))
            {
                stack.Push(number);
            }
            else
            {
                var b = stack.Pop();
                var a = stack.Pop();
                stack.Push(ApplyOperator(token, a, b));
            }
        }
        
        return stack.Pop();
    }
}
```

## Summary

The Interpreter pattern is useful for implementing simple languages and expression evaluators. While powerful for DSLs and expression parsing, it can become unwieldy for complex grammars. For production systems with complex languages, consider using parser generators like ANTLR. The pattern shines in calculator applications, rule engines, and simple scripting languages.
