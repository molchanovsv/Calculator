using System.Diagnostics;
using System.Globalization;
using Calculator.Code.Lexer;
using Calculator.Code.Parser.Evaluation;

namespace Calculator.Code.Parser;

public class Parser
{
    private readonly Stack<Token> _operationStack = new Stack<Token>();
    private readonly Stack<double> _numberStack = new Stack<double>();
    private readonly Evaluator<double> _evaluator = new Evaluator<double>();

    public double GetExpressionValue(Token[] expression)
    {
        // Replace predefined constants
        for (int index = 0; index < expression.Length; index++)
        {
            if (expression[index].Type == TokenType.Predefined)
                expression[index] = new Token(
                    Value: _evaluator.GetConstant(expression[index].Value).ToString(CultureInfo.InvariantCulture),
                    Type: TokenType.Number);
        }

        //Apply unary minus
        if (expression[0].Value == "-")
        {
            expression[1] = expression[1].Negate();
            expression[0] = expression[0] with { Type = TokenType.Empty };
        }
        for (int index = 0; index < expression.Length; index++)
        {
            if (expression[index].Value == "-" && expression[index - 1].Value == "(" &&
                expression[index + 1].Type == TokenType.Number)
            {
                expression[index] = expression[index] with { Type = TokenType.Empty };
                expression[index + 1] = expression[index + 1].Negate();
            }
        }

        //Parse the expression
        foreach (Token token in expression)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                    _numberStack.Push(_evaluator.Parse(token.Value));
                    break;
                case TokenType.Empty:
                    break;
                case TokenType.Operation:
                    if (_operationStack.Count != 0 && _evaluator.GetPrecedence(token.Value) <= _evaluator.GetPrecedence(_operationStack.Peek().Value))
                        ResolveOperator();
                    _operationStack.Push(token);
                    break;
                case TokenType.Function:
                case TokenType.Bracket when token.Value == "(":
                    _operationStack.Push(token);
                    break;
                case TokenType.Bracket when token.Value == ")":
                    while (_operationStack.Peek().Value != "(")
                        ResolveOperator();
                    _operationStack.Pop();
                    if (_operationStack.Count != 0 && _operationStack.Peek().Type == TokenType.Function)
                        ResolveFunction();
                    break;
                case TokenType.Predefined:
                default:
                    throw new ArgumentException("Unknown type met!");
            }
        }

        //Solve unresolved operations
        while (_operationStack.Count != 0)
        {
            ResolveOperator();
        }

        return _numberStack.Pop();
    }

    private void ResolveOperator()
    {
        double secondOperand = _numberStack.Pop();
        double firstOperand = _numberStack.Pop();
        string operation = _operationStack.Pop().Value;
        
        Debug.WriteLine($"Resolved: {firstOperand} {operation} {secondOperand} = {_evaluator.Evaluate(operation, firstOperand, secondOperand)}");
        
        _numberStack.Push(_evaluator.Evaluate(operation, firstOperand, secondOperand));
    }
    private void ResolveFunction()
    {
        double operand = _numberStack.Pop();
        string function = _operationStack.Pop().Value;
        
        Debug.WriteLine($"Resolved: {function}({operand}) = {_evaluator.Evaluate(function, operand)}");
        
        _numberStack.Push(_evaluator.Evaluate(function, operand));
    }
}