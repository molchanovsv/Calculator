using System.Globalization;
using Calculator.Code.Lexer;
using Calculator.Code.Parser;

namespace Calculator;

internal static class Program
{
    private static void Main()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        string expression = Console.ReadLine() ?? "";

        Tokenizer tokenizer = new Tokenizer();
        Parser parser = new Parser();

        IEnumerable<Token> tokenizedExpression = tokenizer.Tokenize(expression);
        
        double result = parser.ParseExpression(tokenizedExpression);
        Console.WriteLine(result);
    }
}
