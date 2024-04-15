namespace Calculator.Code.Parser.Evaluation;

public record OperationDefinition<T>(Func<T, T, T> Invoke, int Precedence);
public record FunctionDefinition<T>(Func<T, T> Invoke);