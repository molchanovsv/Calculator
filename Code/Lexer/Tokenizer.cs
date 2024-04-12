namespace Calculator.Code.Lexer;

public class Tokenizer
{
    private readonly List<TokenDefinition> _tokenDefinitions =
    [
        new TokenDefinition(TokenType.Number, "([0-9]*[.])?[0-9]+"),
        new TokenDefinition(TokenType.Operation, "[+]"),
        new TokenDefinition(TokenType.Operation, "[*]"),
        new TokenDefinition(TokenType.Operation, "[-]"),
        new TokenDefinition(TokenType.Operation, "[/]"),
        new TokenDefinition(TokenType.Operation, "[\\^]")
    ];

    public IEnumerable<Token> Tokenize(string input)
    {
        List<KeyValuePair<int, Token>> tokens = [];

        foreach (TokenDefinition definition in _tokenDefinitions)
            tokens.AddRange(definition.GetMatches(input));
        
        List<Token> orderedTokens = tokens
            .OrderBy(x => x.Key)
            .Select(x => x.Value)
            .ToList();

        if (orderedTokens[0].Value == "-")
        {
            orderedTokens[1] = orderedTokens[1].Negate();
            orderedTokens.RemoveAt(0);
        }

        return orderedTokens;
    }
}