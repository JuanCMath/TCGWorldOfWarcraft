using System.Collections.Generic;
using System;

namespace Compiler
{
    public class Parser
    {
        private int currentIndex;
        private List<Token> tokens;
        Dictionary<TokenType, int> precedence = new Dictionary<TokenType, int>
            {
                { TokenType.Equal, 1 },
                { TokenType.NotEqual, 1 },
                { TokenType.LogicalAnd,1},
                { TokenType.LogicalOr,1},
                { TokenType.LessThan, 2 },
                { TokenType.LessOrEqual, 2 },
                { TokenType.GreaterThan, 2 },
                { TokenType.GreatherOrEqual, 2 },
                { TokenType.Plus, 3 },
                { TokenType.Sub, 3 },
                { TokenType.At,3},
                { TokenType.Concatenate, 3},
                { TokenType.Dot, 3},
                { TokenType.Multiplication, 4 },
                { TokenType.Div, 4 },
                { TokenType.Pow,5},
            };

        public void Parse (List<Token> tokens, Dictionary<TokenType, int> precedence)
        {
            this.tokens = tokens;
            currentIndex = 0;
            this.precedence = precedence;
            MainProgramNode mainProgram = MainProgram();
        }

        public MainProgramNode MainProgram ()
        {
            List<ASTNode> body = new List<ASTNode>();

            while(!Match(TokenType.EOF))
            {
                var propertyName = ExpectIdentifier();

                switch(propertyName)
                {
                    case TokenType.CardDeclaration:
                        body.Add(CardDeclarationParser());
                        break;
                    case TokenType.EffectDeclaration:
                        body.Add(EffectDeclarationParser());
                        break;
                    default:
                        throw new Exception($"Unexpected property '{propertyName}'.");
                }
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
            OnActivationNode onActivation = null;

            Expect(TokenType.BracketL);

            while (!Match(TokenType.BracketR))
            {
                var propertyName = ExpectIdentifier();
                Expect(TokenType.Colon);

                switch (propertyName)
                {
                    case TokenType.Name:
                        name = new StringNode(ExpectString());
                        Expect(TokenType.Comma);
                        break;
                    case TokenType.TypeParam:
                        type = new StringNode(ExpectString());
                        Expect(TokenType.Comma);
                        break;
                    case TokenType.CardFaction:
                        faction = new StringNode(ExpectString());
                        Expect(TokenType.Comma);
                        break;
                    case TokenType.CardPower:
                        power = new NumberNode(ExpectNumber());
                        Expect(TokenType.Comma);
                        break;
                    case TokenType.CardRange:
                        ranges = ExpectStringArray();
                        Expect(TokenType.Comma);
                        break;
                    case TokenType.OnActivation:
                        onActivation = OnActivationParser();
                        break;
                    default:
                        throw new Exception($"Unexpected property '{propertyName}'.");
                }
            }
            return new CardDeclarationNode(name, type, faction, power, ranges, onActivation);
        }

        public OnActivationNode OnActivationParser()
        {
            List<EffectsToBeActivateNode> effectActivation = null;

            Expect(TokenType.BracketL);

            while (!Match(TokenType.BracketR))
            {   
                effectActivation.Add(EffectsToBeActivateParser());
                if (Match(TokenType.Comma))
                {
                    continue;
                }
                break;
            }
            Expect(TokenType.BracketR);
            return new OnActivationNode(effectActivation);
        }

        public EffectsToBeActivateNode EffectsToBeActivateParser()
        {
            EffectParametersAssignementNode effect = null;
            SelectorNode selector = null;
            PostActionNode postAction = null;

            Expect(TokenType.BraceL);

            while (!Match(TokenType.BraceR))
            {
                var propertyName = ExpectIdentifier();
                Expect(TokenType.Colon);

                switch (propertyName)
                {
                    case TokenType.EffectsUsage:
                        effect = EffectParametersAssignementParser();
                        Expect(TokenType.Comma);
                        break;
                    case TokenType.Selector:
                        selector = SelectorParser();
                        Expect(TokenType.Comma);
                        break;
                    case TokenType.PostActionDeclaration:
                        postAction = PostActionParser();
                        break;
                    default:
                        throw new Exception($"Unexpected property '{propertyName}'.");
                }
            }
            return new EffectsToBeActivateNode(effect, selector, postAction);
        }
        public EffectParametersAssignementNode EffectParametersAssignementParser()
        {
            
            List<VariableAssignementNode> parameters = null;

            TerminalNodes value = null; //Parametro a pasarle a parameters
            StringNode name = null;

            Expect(TokenType.BraceL);
            while (!Match(TokenType.BraceR))
            {
                var propertyName = ExpectIdentifier();
                
                name = new StringNode(Expect(TokenType.Identifier).lexeme);
                        
                Expect(TokenType.Colon);
                if (Match(TokenType.String))
                {
                    value = new StringNode(ExpectString());
                }
                else if (Match(TokenType.Number))
                {
                    value = new NumberNode(ExpectNumber());
                }
                else if (Match(TokenType.True) || Match(TokenType.False))
                {
                   value = new BooleanNode(ExpectBoolean());
                }

                parameters.Add(new VariableAssignementNode(name, value));
            }
            Expect(TokenType.BraceR);

            return new EffectParametersAssignementNode(parameters);
        }

        public SelectorNode SelectorParser()
        {
            GameObjectReferenceNode source = null;
            BooleanNode single = null;
            PredicateNode predicate = null;

            Expect(TokenType.BraceL);

            while(!Match(TokenType.BraceR))
            {
                var propertyName = ExpectIdentifier();

                switch (propertyName)
                {
                    case TokenType.Source:
                    source = new GameObjectReferenceNode(new StringNode(ExpectString()));
                    Expect(TokenType.Comma);
                    break;
                    case TokenType.Single:
                    single = new BooleanNode(ExpectBoolean());
                    Expect(TokenType.Comma);
                    break;
                    case TokenType.Predicate:
                    predicate = PredicateParser();
                    break;
                }
            }
            return new SelectorNode(source, single, predicate);
        }
        public PredicateNode PredicateParser()
        {
            
            GameObjectReferenceNode indentifier = null;
            ExpresionNodes condition = null;

            Expect(TokenType.Colon);

            Expect(TokenType.ParenL);
            indentifier = new GameObjectReferenceNode(new StringNode(Expect(TokenType.Identifier).lexeme));
            Expect(TokenType.ParenR);

            Expect(TokenType.Lambda);
            condition = ParseExpresion();

            return new PredicateNode(indentifier, condition);
        }
        public PostActionNode PostActionParser()
        {
            EffectParametersAssignementNode parameters = null;
            SelectorNode selector = null;

            Expect(TokenType.BraceL);

            while (!Match(TokenType.BraceR))
            {
                var propertyName = ExpectIdentifier();
                Expect(TokenType.Colon);

                switch (propertyName)
                {
                    case TokenType.TypeParam:
                        parameters = EffectParametersAssignementParser();
                        break;
                    case TokenType.EffectsUsage:
                        parameters = EffectParametersAssignementParser();
                        Expect(TokenType.Comma);
                        break;
                    case TokenType.Selector:
                        selector = SelectorParser();
                        break;
                    default:
                        throw new Exception($"Unexpected property '{propertyName}'.");
                }
            }
            return new PostActionNode(parameters, selector);
        }

        public EffectDeclarationNode EffectDeclarationParser()
        {
            StringNode name = null;
            EffectParamsReferenceNode param = null;
            ActionDeclarationNode action = null;

            Expect(TokenType.BracketL);

            while (!Match(TokenType.BracketR))
            {
                var propertyName = ExpectIdentifier();
                Expect(TokenType.Colon);

                switch (propertyName)
                {
                    case TokenType.Name:
                        name = new StringNode(ExpectString());
                        Expect(TokenType.Comma);
                        break;
                    case TokenType.EffectParams:
                        param = EffectParamsReferenceParser();
                        Expect(TokenType.Comma);
                        break;
                    case TokenType.EffectAction:
                        action = ActionDeclarationParser();
                        break;
                    default:
                        throw new Exception($"Unexpected property '{propertyName}'.");
                }
            }
            return new EffectDeclarationNode(name, param, action);
        }

        public EffectParamsReferenceNode EffectParamsReferenceParser()
        {
            List<VariableReferenceNode> param = null;

            Expect(TokenType.BracketL);

            while (!Match(TokenType.BracketR))
            {
                param.Add(new VariableReferenceNode(new StringNode(Expect(TokenType.Identifier).lexeme)));
                if (Match(TokenType.Comma))
                {
                    Expect(TokenType.Comma);
                    continue;
                }
                break;
            }
            Expect(TokenType.BracketR);
            return new EffectParamsReferenceNode(param);
        }

        public ActionDeclarationNode ActionDeclarationParser()
        {
            Expect(TokenType.ParenL);
            GameObjectReferenceNode target = new GameObjectReferenceNode(new StringNode(Expect(TokenType.Identifier).lexeme));
            Expect(TokenType.Comma);
            GameObjectReferenceNode context = new GameObjectReferenceNode(new StringNode(Expect(TokenType.Identifier).lexeme));
            Expect(TokenType.ParenR);
            Expect(TokenType.Lambda);

            StatementNodes body = new BlockNode(ParseBlock());

            return new ActionDeclarationNode(target, context, body);
        }

        
        public ASTNode StatementParser()
        {
            if (Match(TokenType.WhileCycle))
            {
                Expect(TokenType.WhileCycle);
                Expect(TokenType.ParenL);
                ExpresionNodes condition = ParseExpresion();
                Expect(TokenType.ParenR);
                List<ASTNode> bodyStatements = ParseBlock();

                return new WhileNode(condition, bodyStatements);
            }
            else if (Match(TokenType.Identifier))
            {
                Token identifier = Expect(TokenType.Identifier);
                if (Match(TokenType.Assignement))
                {
                    Expect(TokenType.Assignement);
                    ExpresionNodes value = ParseExpresion();
                    return new VariableAssignementNode (new StringNode(identifier.lexeme), value);
                }
                return new VariableReferenceNode(new StringNode(identifier.lexeme));
            }
            else if (Match(TokenType.ForCycle))
            {
                Expect(TokenType.ForCycle);
                GameObjectReferenceNode singleObect= new GameObjectReferenceNode(new StringNode(Expect(TokenType.Identifier).lexeme));
                Expect(TokenType.inForCycle);
                VariableReferenceNode objectReference = new VariableReferenceNode(new StringNode(Expect(TokenType.Identifier).lexeme));
                List<ASTNode> bodyStatements = ParseBlock();

                return new ForNode(singleObect, objectReference, bodyStatements);
            }
            else if (Match(TokenType.BraceL))
            {
                return new BlockNode(ParseBlock());
            }
            else
            {  
                ASTNode exp = ParseExpresion();
                return exp;
            }
        }

        public List<ASTNode> ParseBlock()
        {
            List<ASTNode> bodyStatements = new List<ASTNode>();
            Expect(TokenType.BraceL);
            while (!Match(TokenType.BraceR))
            {
                bodyStatements.Add(StatementParser());
                Expect(TokenType.Semicolon);
            }
            Expect(TokenType.BraceR);
            
            return bodyStatements;
        }

        public ExpresionNodes ParseExpresion()
        {
            return ParseBinaryExpresion(0);
        }

        public ExpresionNodes ParseBinaryExpresion(int minPrecedence)
        {
            ExpresionNodes left = ParseUnaryExpresion();

            while (true)
            {
                Token op = Peek();
                TokenType opType = op.type;
                if (!precedence.ContainsKey(opType) || precedence[opType] < minPrecedence)
                {
                    return left;
                }
                else if (opType == TokenType.Dot)
                {
                    if (tokens[currentIndex + 1].type == TokenType.Identifier)
                    {
                        currentIndex++;
                        Token identifier = Expect(TokenType.Identifier);
                        left = new PropertyCallNode(new StringNode(identifier.lexeme), (GameObjectReferenceNode)left);
                    }
                    else if (tokens[currentIndex + 1].type == TokenType.ParenL)
                    {
                        currentIndex++;
                        Expect(TokenType.ParenL);
                        List<ExpresionNodes> arguments = new List<ExpresionNodes>();
                        while (!Match(TokenType.ParenR))
                        {
                            arguments.Add(ParseExpresion());
                            if (Match(TokenType.Comma))
                            {
                                Expect(TokenType.Comma);
                                continue;
                            }
                            break;
                        }
                        Expect(TokenType.ParenR);
                        left = new MethodCallNode(new StringNode(op.lexeme), (GameObjectReferenceNode)left, arguments.ToArray());
                    }
                }
                ExpresionNodes right = ParseBinaryExpresion(precedence[opType] + 1);
                left = new BinaryExpressionNode(left, op, right);
            }
        }

        public ExpresionNodes ParseUnaryExpresion()
        {
            if(tokens[currentIndex + 1].type == TokenType.MinusOne)
            {
                ExpresionNodes exp = ParseUnaryExpresion();
                Token op = Expect(TokenType.MinusOne);
                return new UnaryExpressionNode(op, exp);
            }
            else if (tokens[currentIndex + 1].type == TokenType.PlusOne)
            {
                ExpresionNodes exp = ParseUnaryExpresion();
                Token op = Expect(TokenType.PlusOne);
                return new UnaryExpressionNode(op, exp);
            }
            else
            {
                return ParsePrimaryExpresion();
            }
        }

        public ExpresionNodes ParsePrimaryExpresion()
        {
            if (Match(TokenType.Number))
            {
                return new NumberNode(ExpectNumber());
            }
            else if (Match(TokenType.String))
            {
                return new StringNode(ExpectString());
            }
            else if (Match(TokenType.True) || Match(TokenType.False))
            {
                return new BooleanNode(ExpectBoolean());
            }
            else if (Match(TokenType.ParenL))
            {
                Expect(TokenType.ParenL);
                ExpresionNodes exp = ParseExpresion();
                Expect(TokenType.ParenR); 
                return new GroupingNode(exp);
            }
            else if (Match(TokenType.Identifier))
            {
                Token identifier = Expect(TokenType.Identifier);     
                return new VariableReferenceNode(new StringNode(identifier.lexeme));
            }
            else
            {
                throw new Exception("Expected expression but get " + Peek().type + " ("+Peek().lexeme+")"); 
            }
        }

        // This method is used to check if the current token is the expected token (return rangeList, current++)
         private StringNode[] ExpectStringArray()
         {
            Queue<StringNode> temporalRanges = new Queue<StringNode>();

            Expect(TokenType.BracketL);
            while (!Match(TokenType.BracketR))
            {
                temporalRanges.Enqueue(new StringNode(ExpectString()));
                if (!Match(TokenType.Comma)) break;
                else Expect(TokenType.Comma); continue;
            }
            Expect(TokenType.BracketR);
            
            StringNode[] rangeList = new StringNode[temporalRanges.Count];

            for (int i = 0; i < temporalRanges.Count; i++)
            {
                rangeList[i] = temporalRanges.Dequeue();
            }   
            return rangeList;
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

        

        // This method is used to check if the current token is the expected token (return True if CurrentToken.Type == expectedToken, current++)
        private bool Match(TokenType expectedToken)
        {
            if (currentIndex >= tokens.Count)
            {
                return false;
            }

            return tokens[currentIndex].type == expectedToken;
        }

        private Token Peek()
        { 
            currentIndex++;
            return tokens[currentIndex - 1];
        }
    }
}