using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Calculator.Code.Parser.Evaluation;

public class Evaluator<T>
    where T : IFloatingPoint<T>, IPowerFunctions<T>, ITrigonometricFunctions<T>, ILogarithmicFunctions<T>
{
    private readonly Dictionary<string, OperationDefinition<T>> _operations =
        new Dictionary<string, OperationDefinition<T>> 
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
    private readonly Dictionary<string, FunctionDefinition<T>> _functions =
        new Dictionary<string, FunctionDefinition<T>>
        {
            { "sin",  new FunctionDefinition<T>(T.Sin)},
            { "cos",  new FunctionDefinition<T>(T.Cos)},
            { "tan",  new FunctionDefinition<T>(T.Tan)},
            { "cot",  new FunctionDefinition<T>(point => T.Cos(point) / T.Sin(point))},
            { "abs",  new FunctionDefinition<T>(T.Abs)},
            { "ln",   new FunctionDefinition<T>(T.Log)},
            { "log2", new FunctionDefinition<T>(T.Log2)},
            { "lg",   new FunctionDefinition<T>(T.Log10)},
            { "exp",  new FunctionDefinition<T>(x => T.Pow(T.E, x))},
            { "sqrt", new FunctionDefinition<T>(x => T.Pow(x, T.One / (T.One + T.One)))},
            { "sign", new FunctionDefinition<T>(x => T.IsPositive(x) && !T.IsZero(x) ? T.One : T.IsZero(x) ? T.Zero : -T.One)}
        };
    private readonly Dictionary<string, T> _constants =
        new Dictionary<string, T> 
        { 
            { "pi", T.Pi }, 
            { "e", T.E } 
        };

    [return: NotNull]
    public T Parse<TExpression>(TExpression expression) =>
        T.TryParse(expression?.ToString(), CultureInfo.InvariantCulture, out T? result)
            ? result
            : throw new ArgumentException($"Expression cannot be parsed to {typeof(T).FullName}");
    public T Evaluate(string operation, T operand1, T operand2) =>
        _operations[operation].Invoke(operand1, operand2);
    public T Evaluate(string function, T operand) =>
        _functions[function].Invoke(operand);
    public T GetConstant(string constantName) =>
        _constants.TryGetValue(constantName.Trim().ToLower(), out T? value)
            ? value
            : throw new ArgumentException($"Constant is not defined in {typeof(T).FullName}");
    public int GetPrecedence(string operation) =>
        _operations.TryGetValue(operation, out OperationDefinition<T>? result) ? result.Precedence : int.MinValue;
}