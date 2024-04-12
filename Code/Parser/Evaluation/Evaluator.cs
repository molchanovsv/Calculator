using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Calculator.Code.Parser.Evaluation;

public class Evaluator<T> where T : IFloatingPoint<T>, IPowerFunctions<T>
{
    private readonly Dictionary<string, OperationDefinition<T>> _operations = new Dictionary<string, OperationDefinition<T>>
    {
        { "+", new OperationDefinition<T>((x, y) => x + y, 0) },
        { "-", new OperationDefinition<T>((x, y) => x - y, 0) },
        { "*", new OperationDefinition<T>((x, y) => x * y, 1) },
        { "/", new OperationDefinition<T>((x, y) => {
            if (T.IsZero(y))
                throw new ArgumentException("Number cannot be divided by zero!");
            return x / y;
        }, 1) },
        { "^", new OperationDefinition<T>((x, y) => {
            if (T.IsZero(x) && T.IsZero(y))
                throw new ArgumentException("Zero to power of zero is ambiguous!");
            return T.Pow(x, y);
        }, 2) }
    };

    [return: NotNull]
    public T Parse<TExpression>(TExpression expression) =>
        T.TryParse(expression?.ToString(), CultureInfo.InvariantCulture,  out T? result) ? result : 
            throw new ArgumentException($"Expression cannot be parsed to {typeof(T).FullName}");

    public T Evaluate(string operation, T operand1, T operand2)
    {
        // Debug.WriteLine($"{operand1} {operation} {operand2} = {_operations[operation].Invoke(operand1, operand2)}");
        return _operations[operation].Invoke(operand1, operand2);
    }

    public int GetPrecedence(string operation) => 
        _operations.TryGetValue(operation, out OperationDefinition<T>? result) ? result.Precedence : 0;
}