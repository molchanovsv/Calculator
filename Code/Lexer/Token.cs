using System.Text.RegularExpressions;

namespace Calculator.Code.Lexer;

public record Token(TokenType Type, string Value)
{
    public Token Negate()
    {
        return this with { Value = "-" + Value };
    }
}

public record TokenDefinition(TokenType Type, Regex Regex)
{
    public TokenDefinition(TokenType type, string regexPattern)
        : this(type, new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled))
    {
    }

    public IEnumerable<KeyValuePair<int, Token>> GetMatches(string input)
    {
        foreach (Match match in Regex.Matches(input))
            yield return new KeyValuePair<int, Token>(match.Index, new Token(Type, match.Value));
    }
}