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
        new TokenDefinition(TokenType.Operation, @"[\^]"),
        new TokenDefinition(TokenType.Predefined, @"(?:pi)|(?:pI)|(?:Pi)|(?:PI)"),
        new TokenDefinition(TokenType.Predefined, @"(?:e)|(?:E)"),
        new TokenDefinition(TokenType.Bracket, @"(?:\()|(?:\))"),
        new TokenDefinition(TokenType.Function, "(?:sin)|(?:cos)|(?:tan)|(?:cot)"),
        new TokenDefinition(TokenType.Function, "(?:abs)|(?:ln)|(?:log2)|(?:lg)|(?:exp)|(?:sqrt)|(?:sign)")
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

        return orderedTokens;
    }
}