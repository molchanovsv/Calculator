namespace Calculator.Code.Parser.Evaluation;

public record OperationDefinition<T>(Func<T, T, T> Invoke, int Precedence);