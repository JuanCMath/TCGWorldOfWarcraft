using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;

namespace Compiler
{
    public class Parsing
    {
        private int currentIndex;
        private List<Token> tokens;

        public void Parse(string input)
        {
            tokens = Lexer.Tokenize(input);
            currentIndex = 0;

            var card = ParseCard();
            Console.WriteLine("Parsed card:");
            Console.WriteLine($"Type: {card.Type}");
            Console.WriteLine($"Name: {card.Name}");
            Console.WriteLine($"Faction: {card.Faction}");
            Console.WriteLine($"Power: {card.Power}");
            Console.WriteLine($"Range: {string.Join(", ", card.Range)}");
            Console.WriteLine("OnActivation:");
            foreach (var action in card.OnActivation)
            {
                Console.WriteLine("\tEffect:");
                Console.WriteLine($"\t\tName: {action.Effect.Name}");
                if (action.Effect.Params.ContainsKey(TokenType.EffectAmount))
                {
                    Console.WriteLine($"\t\tAmount: {action.Effect.Params[TokenType.EffectAmount]}");
                }
                Console.WriteLine("\t\tSelector:");
                Console.WriteLine($"\t\t\tSource: {action.Selector.Source}");
                Console.WriteLine($"\t\t\tSingle: {action.Selector.Single}");
                Console.WriteLine($"\t\t\tPredicate: {action.Selector.Predicate}");
                Console.WriteLine("\t\tPostAction:");
                Console.WriteLine($"\t\t\tType: {action.PostAction.Type}");
                Console.WriteLine("\t\t\tSelector:");
                Console.WriteLine($"\t\t\t\tSource: {action.PostAction.Selector.Source}");
                Console.WriteLine($"\t\t\t\tSingle: {action.PostAction.Selector.Single}");
                Console.WriteLine($"\t\t\t\tPredicate: {action.PostAction.Selector.Predicate}");
            }
        }

        private Card ParseCard()
        {
            var card = new Card();

            Expect(TokenType.CardDeclaration);
            Expect(TokenType.BraceL);

            while (!Match(TokenType.BraceR))
            {
                var propertyName = ExpectIdentifier();
                Expect(TokenType.Colon);

                switch (propertyName)
                {
                    case TokenType.TypeParam:
                        card.Type = ExpectString();
                        break;
                    case TokenType.Name:
                        card.Name = ExpectString();
                        break;
                    case TokenType.CardFaction:
                        card.Faction = ExpectString();
                        break;
                    case TokenType.CardPower:
                        card.Power = ExpectNumber();
                        break;
                    case TokenType.CardRange:
                        card.Range = ExpectStringArray();
                        break;
                    case TokenType.OnActivation:
                        card.OnActivation = ParseActionArray();
                        break;
                    default:
                        throw new Exception($"Unexpected property name: {propertyName}");
                }
            }

            Expect(TokenType.BraceR);

            return card;
        }

        private List<Action> ParseActionArray()
        {
            var actions = new List<Action>();

            Expect(TokenType.BracketL);

            while (!Match(TokenType.BracketR))
            {
                var action = new Action();

                Expect(TokenType.BraceL);

                while (!Match(TokenType.BraceR))
                {
                    var propertyName = ExpectIdentifier();
                    Expect(TokenType.Colon);

                    switch (propertyName)
                    {
                        case TokenType.EffectDeclaration:
                            action.Effect = ParseEffect();
                            break;
                        case TokenType.Selector:
                            action.Selector = ParseSelector();
                            break;
                        case TokenType.PostActionDeclaration:
                            action.PostAction = ParsePostAction();
                            break;
                        default:
                            throw new Exception($"Unexpected property name: {propertyName}");
                    }
                }

                actions.Add(action);
            }

            Expect(TokenType.BracketR);

            return actions;
        }

        private Effect ParseEffect()
        {
            var effect = new Effect();

            Expect(TokenType.BraceL);

            while (!Match(TokenType.BraceR))
            {
                var propertyName = ExpectIdentifier();
                Expect(TokenType.Colon);

                switch (propertyName)
                {
                    case TokenType.Name:
                        effect.Name = ExpectString();
                        break;
                    case TokenType.TypeParam:
                        effect.Params = ParseParams();
                        break;
                    case TokenType.EffectAction:
                        effect.Action = ExpectString();
                        break;
                    default:
                        throw new Exception($"Unexpected property name: {propertyName}");
                }
            }

            Expect(TokenType.BraceR);

            return effect;
        }

        private Dictionary<TokenType, string> ParseParams()
        {
            var parameters = new Dictionary<TokenType, string>();

            Expect(TokenType.BraceL);

            while (!Match(TokenType.BraceR))
            {
                var paramName = ExpectIdentifier();
                Expect(TokenType.Colon);
                var paramValue = ExpectString();

                parameters.Add(paramName, paramValue);
            }

            Expect(TokenType.BraceR);

            return parameters;
        }

        private Selector ParseSelector()
        {
            var selector = new Selector();

            Expect(TokenType.BraceL);

            while (!Match(TokenType.BraceR))
            {
                var propertyName = ExpectIdentifier();
                Expect(TokenType.Colon);

                switch (propertyName)
                {
                    case TokenType.Source:
                        selector.Source = ExpectString(); //Aqui Deberia ir una referencia de unity que aun no he hecho
                        break;
                    case TokenType.Single:
                        selector.Single = ExpectBoolean();
                        break;
                    case TokenType.Predicate:
                        selector.Predicate = ExpectString();
                        break;
                    default:
                        throw new Exception($"Unexpected property name: {propertyName}");
                }
            }

            Expect(TokenType.BraceR);

            return selector;
        }

        private PostAction ParsePostAction()
        {
            var postAction = new PostAction();

            Expect(TokenType.BraceL);

            while (!Match(TokenType.BraceR))
            {
                var propertyName = ExpectIdentifier();
                Expect(TokenType.Colon);

                switch (propertyName)
                {
                    case TokenType.TypeParam:
                        postAction.Type = ExpectString();
                        break;
                    case TokenType.Selector:
                        postAction.Selector = ParseSelector();
                        break;
                    default:
                        throw new Exception($"Unexpected property name: {propertyName}");
                }
            }

            Expect(TokenType.BraceR);

            return postAction;
        }

        //Recive el token que estas esperando, te lo devuelve si es correcto lo que estabas esperando y se mueve al proximo token
        private Token Expect(TokenType expectedToken)
        {
            if (currentIndex >= tokens.Count)
            {
                throw new Exception($"Expected token '{expectedToken}', but reached end of input.");
            }

            var token = tokens[currentIndex];

            if (token.type != expectedToken)
            {
                throw new Exception($"Expected token '{expectedToken}', but found '{token}'.");
            }

            currentIndex++;

            return token;
        }

        private TokenType ExpectIdentifier()
        {
            var token = Expect(tokens[currentIndex].type);

            if (!Match(TokenType.Identifier))
            {
                throw new Exception($"Expected identifier, but found '{token}'.");
            }

            return token.type;
        }

        private string ExpectString()
        {
            var token = Expect(tokens[currentIndex].type);

            if (token.type != TokenType.String)
            {
                throw new Exception($"Expected string, but found '{token}'.");
            }

            return token.lexeme;
        }

        private int ExpectNumber()
        {
            var token = Expect(tokens[currentIndex].type);

            if (token.type != TokenType.Number)
            {
                throw new Exception($"Expected number, but found '{token}'.");
            }

            return  int.Parse(token.lexeme);
        }

        private bool ExpectBoolean()
        {
            var token = Expect(tokens[currentIndex].type);

            if (token.type != TokenType.True && token.type != TokenType.False)
            {
                throw new Exception($"Expected boolean, but found '{token}'.");
            }

            return true;
        }

         private string[] ExpectStringArray()
         {
            Queue<string> temporalRanges = new Queue<string>();

            Expect(TokenType.BracketL);
            while (!Match(TokenType.BracketR))
            {
                temporalRanges.Enqueue(ExpectString());
                Expect(TokenType.Colon);
            }
            Expect(TokenType.BracketR);
            
            string[] rangeList = new string[temporalRanges.Count];

            for (int i = 0; i < temporalRanges.Count; i++)
            {
                rangeList[i] = temporalRanges.Dequeue();
            }   
            return rangeList;
        }

        private bool Match(TokenType expectedToken)
        {
            if (currentIndex >= tokens.Count)
            {
                return false;
            }

            return tokens[currentIndex].type == expectedToken;
        }
    }

    public class Card
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Faction { get; set; }
        public int Power { get; set; }
        public string[] Range { get; set; }
        public List<Action> OnActivation { get; set; }
    }

    public class Action
    {
        public Effect Effect { get; set; }
        public Selector Selector { get; set; }
        public PostAction PostAction { get; set; }
    }

    public class Effect
    {
        public string Name { get; set; }
        public Dictionary<TokenType, string> Params { get; set; }
        public string Action { get; set; }
    }

    public class Selector
    {
        public string Source { get; set; }
        public bool Single { get; set; }
        public string Predicate { get; set; }
    }

    public class PostAction
    {
        public string Type { get; set; }
        public Selector Selector { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var input = @"
card {
    Type: ""Oro"",
    Name: ""Beluga"",
    Faction: ""Northern Realms"",
    Power: 10,
    Range: [""Melee"", ""Ranged""],
    OnActivation: 
    [   
        {
            Effect: 
            {
                Name: ""Damage"",
                Params: {
                    Amount: 5
                },
                Action: ""(targets, context) => {
                    for target in targets {
                        i = 0;
                        while (i++ < Amount)
                            target.Power -=1:
                    };
                }""
            },
            Selector: 
            {
                Source: ""board"",
                Single: false,
                Predicate: ""(unit) => unit.Faction == \""Northern Realms\""""
            },
            PostAction: 
            {
                Type: ""ReturnToDeck"",
                Selector: 
                {
                    Source: ""parent"",
                    Single: false,
                    Predicate: ""(unit) => unit.Power < 1""
                }
            }
        },
        {
            Effect: ""Draw"",
            Action: ""(targets, context) => {
                topCard = context.Deck.Pop();
                context.Hand.Add(topCard);
                conext.Hand.Shuffle();
            }""
        }
    ]
}";

            var parser = new Parsing();
            parser.Parse(input);
        }
    }
} 