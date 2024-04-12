using Calculator.Code.Lexer;
using Calculator.Code.Parser.Evaluation;

namespace Calculator.Code.Parser;

public class Parser
{
    private readonly Stack<double> _numberStack = [];
    private readonly Stack<string> _operationsStack = [];
    private readonly Evaluator<double> _evaluator = new Evaluator<double>();

    public double ParseExpression(IEnumerable<Token> tokens)
    {
        foreach (Token token in tokens)
        {
            if (token.Type == TokenType.Number)
            {
                _numberStack.Push(_evaluator.Parse(token.Value));
            }
            else if (token is { Type: TokenType.Operation })
            {
                if (_operationsStack.Count > 0 
                    && _evaluator.GetPrecedence(token.Value) <= _evaluator.GetPrecedence(_operationsStack.Peek()))
                    ApplyMath();
                _operationsStack.Push(token.Value);
            }
        }

        while (_operationsStack.Count > 0)
            ApplyMath();
        
        return _evaluator.Parse(_numberStack.Pop());
    }

    private void ApplyMath()
    {
        double secondOperand = _numberStack.Pop();
        double firstOperand = _numberStack.Pop();
        string operation = _operationsStack.Pop();

        double result = _evaluator.Evaluate(operation, firstOperand, secondOperand);
        _numberStack.Push(result);
    }
}