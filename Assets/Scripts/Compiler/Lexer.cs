using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UIElements.Experimental;
namespace Lexer
{
// Token class represents a token in the lexer
public class Token
{
    public int Line { get; }
    public int Column { get; }
    public TokenType Type { get; }
    public string Lexeme { get; }

    public Token(int line, int column, TokenType type, string lexeme)
    {
        Line = line;
        Column = column;
        Type = type;
        Lexeme = lexeme;   
    }

    public override string ToString()
    {
        return $"Token({Line}, {Column}, {Type}, {Lexeme})";
    }
}

// Enum class for the token types
public enum TokenType
{
    //General Comands
    Number, String, Identifier, Semicolon, BraceL, BraceR, ParenL, ParenR, BracketL, BracketR, Comma, Equal,
    Plus, Sub, Star, Div, Rem, Pow, Dot, Colon, At, And, Or, Lambda, Not, End, Concatenate,
    GreaterThan, LessThan,
    ForCycle, WhileCycle,   

    //Card Comands
    Name, TypeParam, Selector, Source, Predicate, Single, 
    CardDeclaration,  CardFaction, CardPower, CardRange, OnActivation, 
    EffectDeclaration, EffectAmount, EffectUnit, 
    PostActionDeclaration, PostActionType, PostActionSelector, PostActionSource, PostActionPredicate, PostActionUnit, 
}

// Lexer class for tokenizing the input string
class Lexer
{
    private static Dictionary<string, TokenType> operators = new Dictionary<string, TokenType>()
    {
        { ";", TokenType.Semicolon },
        { "{", TokenType.BraceL },
        { "}", TokenType.BraceR },
        { "(", TokenType.ParenL },
        { ")", TokenType.ParenR },
        { "[", TokenType.BracketL },
        { "]", TokenType.BracketR },
        { ",", TokenType.Comma },
        { "=", TokenType.Equal },
        { "+", TokenType.Plus },
        { "-", TokenType.Sub },
        { "*", TokenType.Star },
        { "/", TokenType.Div },
        { "%", TokenType.Rem },
        { "^", TokenType.Pow },
        { ".", TokenType.Dot },
        { ":", TokenType.Colon },
        { "@", TokenType.At },
        { "&", TokenType.And },
        { "|", TokenType.Or },
        { "=>", TokenType.Lambda },
        { "!", TokenType.Not },
        { ">", TokenType.GreaterThan },
        { "<", TokenType.LessThan },
        { "$", TokenType.End},
        { "@@", TokenType.Concatenate}

    };

    private static Dictionary<string, TokenType> cardStuffs = new Dictionary<string, TokenType>()
    {
        {"card", TokenType.CardDeclaration},
        {"Type", TokenType.TypeParam},
        {"Name", TokenType.Name},
        {"Faction", TokenType.CardFaction},
        {"Power", TokenType.CardPower},
        {"Range", TokenType.CardRange},
        {"OnActivation", TokenType.OnActivation},
        {"Effect", TokenType.EffectDeclaration},
        {"Amount", TokenType.EffectAmount},
        {"Selector", TokenType.Selector},
        {"Source", TokenType.Source},
        {"Predicate", TokenType.Predicate},
        {"unit", TokenType.EffectUnit},
        {"PostAction", TokenType.PostActionDeclaration},
        {"Single", TokenType.Single},
        {"for", TokenType.ForCycle},
        {"while", TokenType.WhileCycle}
    }; 

    private static Regex symbols = new Regex(@"\{|\}|@@|=>|\(|\)|\[|\]|,|=|\+|-|\*|/|%|\^|\.|:|@|&|\||!|>|<", RegexOptions.IgnoreCase);
    private static Regex number = new Regex(@"[0-9]+|[0-9]*\.[0-9]+");
    private static Regex identifier = new Regex(@"[_a-zA-Z][_a-zA-Z0-9]*");

    public static List<Token> Tokenize(string input)
    {
        List<Token> tokens = new List<Token>();
        input = input.Trim();

        int line = 1;
        int column = 1;

        while (input.Length > 0)
        {
            // Skip whitespace characters
            while (char.IsWhiteSpace(input[0]))
            {
                if (input[0] == '\n')
                {
                    line++;
                    column = 1;
                }
                else
                {
                    column++;
                }
                input = input.Substring(1);
            }

            // Match symbols
            Match symbolMatch = symbols.Match(input);
            if (symbolMatch.Success && symbolMatch.Index == 0)
            {
                string lexeme = symbolMatch.Value;
                tokens.Add(new Token(line, column, operators[lexeme], lexeme));
                column += symbolMatch.Length;
                input = input.Substring(symbolMatch.Length);
                continue;
            }

            // Match numbers
            Match numberMatch = number.Match(input);
            if (numberMatch.Success && numberMatch.Index == 0)
            {
                string lexeme = numberMatch.Value;
                tokens.Add(new Token(line, column, TokenType.Number, lexeme));
                column += numberMatch.Length;
                input = input.Substring(numberMatch.Length);
                continue;
            }

            // Match identifiers
            Match identifierMatch = identifier.Match(input);
            if (identifierMatch.Success && identifierMatch.Index == 0)
            {
                string lexeme = identifierMatch.Value;
                if (cardStuffs.ContainsKey(lexeme))
                {
                    tokens.Add(new Token(line, column, cardStuffs[lexeme], lexeme));
                }
                else
                {
                    tokens.Add(new Token(line, column, TokenType.Identifier, lexeme));
                }
                column += identifierMatch.Length;
                input = input.Substring(identifierMatch.Length);
                continue;
            }

            // Match strings
            if (input[0] == '"')
            {
                int i = 1;
                while (input[i] != '"')
                {
                    if (input[i] == '\\')
                    {
                        input = input.Remove(i, 1);
                        i++;
                    }   
                    i++;
                }
                tokens.Add(new Token(line, column, TokenType.String, input.Substring(1, i - 1)));
                column += i + 1;
                input = input.Substring(i + 1);
                continue;
            }

            // Skip comments
            if (input[0] == '/' && input[1] == '/')
            {
                while (input[0] != '\n')
                {
                    column++;
                    input = input.Substring(1);
                }
                continue;
            }

            // Raise syntax error for invalid syntax
            throw new SyntaxErrorException($"Invalid syntax at {line}:{column}");
        }

        // Append end-of-file token
        tokens.Add(new Token(line, column, TokenType.End, "$"));
        return tokens;
    }
}
}
