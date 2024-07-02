using System.Collections.Generic;
using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using Enums;

namespace Compiler
{
    public class Parser
    {
        private int currentIndex;
        private List<Token> tokens;

        public void Parse (List<Token> tokens)
        {
            this.tokens = tokens;
            currentIndex = 0;
            MainProgramNode mainProgram = MainProgram();
        }

        public MainProgramNode MainProgram ()
        {
            List<ASTNode> body = new List<ASTNode>();

            while(!Match(TokenType.EOF))
            {
                if (tokens[0].type == TokenType.CardDeclaration)
                    body.Add(CardDeclarationParser());
                else if (tokens[0].type == TokenType.EffectDeclaration)
                    body.Add(EffectDeclarationParser());
            }
            return new MainProgramNode(body);
        }

        public CardDeclarationNode CardDeclarationParser()
        {
            StringNode name = null;
            StringNode type = null;
            StringNode faction = null;
            NumberNode power = null;
            StringNode[] ranges = null;
            List<OnActivationNode> onActivation = null;

            Expect(TokenType.CardDeclaration);
            Expect(TokenType.BracketL);

            while (!Match(TokenType.BracketR))
            {
                
                var propertyName = ExpectIdentifier();
                Expect(TokenType.Colon);

                switch (propertyName)
                {
                    case TokenType.Name:
                        name = new StringNode(ExpectString());
                        break;
                    case TokenType.TypeParam:
                        type = new StringNode(ExpectString());
                        break;
                    case TokenType.CardFaction:
                        faction = new StringNode(ExpectString());
                        break;
                    case TokenType.CardPower:
                        power = new NumberNode(ExpectNumber());
                        break;
                    case TokenType.CardRange:
                        ranges = ExpectStringArray();
                        break;
                    case TokenType.OnActivation:
                        onActivation = new List<OnActivationNode>();
                        while (!Match(TokenType.BracketR))
                        {
                            onActivation.Add(OnActivationParser());
                        }
                        break;
                    default:
                        throw new Exception($"Unexpected property '{propertyName}'.");
                }
            }
            return new CardDeclarationNode(name, type, faction, power, ranges, onActivation);
        }
        public OnActivationNode OnActivationParser()
        {
            EffectActivationNode effectActivation = null;
            SelectorNode selector = null;
            OnActivationNode onActivation = null;

            Expect(TokenType.BraceL);

            while (!Match(TokenType.BraceR))
            {
                var propertyName = ExpectIdentifier();
                Expect(TokenType.BraceL);

                switch (propertyName)
                {
                    case TokenType.EffectDeclaration:
                        effectActivation = EffectActivationParser();
                        break;
                    case TokenType.Selector:
                        selector = SelectorParser();
                        break;
                    case TokenType.PostActionDeclaration:
                        onActivation = OnActivationParser();
                        break;
                    default:
                        throw new Exception($"Unexpected property '{propertyName}'.");
                }
            }
            return new OnActivationNode(effectActivation, selector, onActivation);
        }

        public EffectActivationNode EffectActivationParser()
        {
            StringNode name = null;
            List<VariableAssignementNode> parameters = null;

            Expect(TokenType.BraceL);

            while (!Match(TokenType.BraceR))
            {
                var propertyName = ExpectIdentifier();
                Expect(TokenType.Colon);
                
                VariableAssignementNode parameter = null;
                switch (propertyName)
                {
                    case TokenType.Name:
                        name = new StringNode(ExpectString());
                        break;
                    case TokenType.String:
                        parameters.Add(new VariableAssignementNode(ExpectString(), );
                        break;
                    
                    default:
                        throw new Exception($"Unexpected property '{propertyName}'.");
                }
            }
            return new EffectActivationNode(name, );
        }
        public EffectDeclarationNode EffectDeclarationParser()
        {
            
        }









        // This method is used to check if the current token is the expected token (return token, current++)
        private Token Expect(TokenType expectedToken) 
        {
            if (currentIndex >= tokens.Count)
            {
                throw new Exception($"Expected token '{expectedToken}', but reached end of input.");
            }

            var token = tokens[currentIndex];

            if (token.type != expectedToken)
            {
                throw new Exception($"Expected token '{expectedToken}', but found '{token}'. On line {token.line}, column {token.column}.");
            }

            currentIndex++;

            return token;
        }

        // This method is used to check if the current token is Identifier token (return token.type, current++)
        private TokenType ExpectIdentifier()
        {
            var token = Expect(tokens[currentIndex].type);

            if (!Match(TokenType.Identifier))
            {
                throw new Exception($"Expected identifier, but found '{token}'.");
            }

            return token.type;
        }

        // This method is used to check if the current token is the expected token (return token.lexeme, current++)
        private string ExpectString()
        {
            var token = Expect(tokens[currentIndex].type);

            if (token.type != TokenType.String)
            {
                throw new Exception($"Expected string, but found '{token}'.");
            }

            return token.lexeme;
        }

        // This method is used to check if the current token is the expected token (return int.Parse(token.lexeme), current++)
        private int ExpectNumber()
        {
            var token = Expect(tokens[currentIndex].type);

            if (token.type != TokenType.Number)
            {
                throw new Exception($"Expected number, but found '{token}'.");
            }

            return  int.Parse(token.lexeme);
        }

        // This method is used to check if the current token is the expected token (return true, current++)
        private bool ExpectBoolean()
        {
            var token = Expect(tokens[currentIndex].type);

            if (token.type != TokenType.True && token.type != TokenType.False)
            {
                throw new Exception($"Expected boolean, but found '{token}'.");
            }

            return true;
        }

        // This method is used to check if the current token is the expected token (return rangeList, current++)
         private StringNode[] ExpectStringArray()
         {
            Queue<StringNode> temporalRanges = new Queue<StringNode>();

            Expect(TokenType.BracketL);
            while (!Match(TokenType.BracketR))
            {
                temporalRanges.Enqueue(new StringNode(ExpectString()));
                Expect(TokenType.Colon);
            }
            Expect(TokenType.BracketR);
            
            StringNode[] rangeList = new StringNode[temporalRanges.Count];

            for (int i = 0; i < temporalRanges.Count; i++)
            {
                rangeList[i] = temporalRanges.Dequeue();
            }   
            return rangeList;
        }

        // This method is used to check if the current token is the expected token (return True if CurrentToken.Type == expectedToken, current++)
        private bool Match(TokenType expectedToken)
        {
            if (currentIndex >= tokens.Count)
            {
                return false;
            }

            return tokens[currentIndex].type == expectedToken;
        }
    }
}