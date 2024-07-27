using System.Collections.Generic;
using System;
using System.Data;
using Enums;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.SocialPlatforms.Impl;
using System.Runtime.InteropServices.WindowsRuntime;
#nullable enable

namespace Compiler
{
    public class Evaluator
    {
        public Stack<Dictionary<string, object>> scopes;
        public Dictionary<string, EffectDeclarationNode> availableEffects;

        public void EvaluateMain(MainProgramNode node)
        {
            Evaluate(node);
        }

        public object? Evaluate(ASTNode node)
        {
            switch(node)
            {
                case MainProgramNode mainNode:
                    EnterScope();
                    EvaluateBlock(mainNode.Body);
                    ExitScope();
                    return null;

                case CardDeclarationNode cardNode:
                    return EvaluateCardDeclarationNode(cardNode);
                
                case EffectDeclarationNode effectNode:
                    EvaluateEffectDeclarationNode(effectNode);
                    return null;

                case StringNode stringNode:
                    return EvaluateString(stringNode);

                case NumberNode numberNode:
                    return EvaluateNumber(numberNode);
                    
                case BooleanNode boolNode:
                    return EvaluateBool(boolNode);

                case GameObjectReferenceNode objectReferenceNode:
                    return EvaluateObjectReference(objectReferenceNode);

                case VariableAssignementNode variableNode:
                    string variableName = EvaluateString(variableNode.Name);
                    object value = Evaluate(variableNode.Value);

                    if (scopes.Peek().ContainsKey(variableName))
                    {
                        throw new Exception($"Variable '{variableName}' already exists.");
                    }
                    else
                    {
                        scopes.Peek().Add(variableName, value);
                        break;
                    }

                case VariableReferenceNode referenceNode:
                    string variableReference = EvaluateString(referenceNode.Name);

                    if (FindVariableScope(variableReference) != null)
                    {
                        return FindVariableScope(variableReference)[variableReference];
                    }
                    else
                    {
                        throw new Exception($"Variable '{variableReference}' does not exist.");
                    }
                case WhileNode whileNode:
                    object? loopConditionValue = Evaluate(whileNode.Condition);
                    while (loopConditionValue is bool loopConditionBool && loopConditionBool)
                    {
                        EnterScope();
                        EvaluateBlock(whileNode.BodyStatements);
                        ExitScope();
                        loopConditionValue = Evaluate(whileNode.Condition);
                    }
                    return null;
                case ForNode forNode:
                    EnterScope();
                    List<GameObject> temp = new List<GameObject>();

                    object forObject = Evaluate(forNode.Objetcs);

                    if (forObject is List<GameObject> objects)
                    {
                        foreach (GameObject obj in objects)
                        {
                            scopes.Peek().Add(EvaluateString(forNode.GameObject.GameObject), obj);
                            EvaluateBlock(forNode.Body);
                        }
                    }
                    else
                    {
                        throw new Exception("For loop must iterate over a list of GameObjects.");
                    }
                    
                    ExitScope();
                    return null;
                case BlockNode blockNode:
                    EnterScope();
                    foreach (ASTNode statement in blockNode.Statements)
                    {
                        Evaluate(statement);
                    }
                    ExitScope();
                    return null;

                case UnaryExpressionNode unaryExpression:
                    return EvaluateUnaryExpressionNode(unaryExpression);

                case BinaryExpressionNode binaryExpressionNode:
                    return EvaluateBinaryExpressionNode(binaryExpressionNode);

                case PropertyCallNode propertyNode:
                    return EvaluatePropertyCallNode(propertyNode);
                    
                case MethodCallNode methodCallNode:
                    return EvaluateMethodCallNode(methodCallNode);

                default:
                    throw new System.Exception("Unknown node type");
            }
            return null;
        }

        public CardData EvaluateCardDeclarationNode(CardDeclarationNode cardnode)
        {
            CardData data = new CardData();

            data.cardType = EvaluateType(cardnode.Type);
            data.isHero = EvaluateIsHero(cardnode.Type);
            data.name = (string)Evaluate(cardnode.Name);
            data.cardFaction = (string)Evaluate(cardnode.Faction);
            data.attackPower = (int)Evaluate(cardnode.Power);
            data.slots = EvaluateRanges(cardnode.Ranges);
            data.effect = SaveOnActivationBlock(cardnode.OnActivation);

            return data;
        }

        public string EvaluateString(StringNode node)
        {
            return node.Value;
        }

        public double EvaluateNumber(NumberNode node)
        {
            return node.Value;
        }

        public bool EvaluateBool(BooleanNode node)
        {
            return node.Value;
        }

        public type EvaluateType(StringNode node)
        {
            string value = EvaluateString(node);    

            switch(value)
            {
                case "Oro":
                    return type.Unidad;
                case "Plata":
                    return type.Unidad;
                case "Despeje":
                    return type.Despeje;
                case "Aumento":
                    return type.Aumento;
                case "Clima":
                    return type.Clima;
                case "Señuelo":
                    return type.Señuelo;
                default:
                    throw new System.Exception("Unknown type");
            }
        }

        public string[] EvaluateRanges(StringNode[] nodes)
        {
            string[] ranges = new string[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                ranges[i] = EvaluateString(nodes[i]);
            }
            return ranges;
        }

        public bool EvaluateIsHero(StringNode node)
        {
            string value = EvaluateString(node);
            return value == "Oro";
        }

        public GameObject EvaluateObjectReference(GameObjectReferenceNode node)
        {
            return GameObject.Find(EvaluateString(node.GameObject));
        }

        public OnActivationNode SaveOnActivationBlock(OnActivationNode node)
        {
            return node;
        }

        public void EvaluateOnActivationBlock(OnActivationNode node)
        {
            foreach (EffectsToBeActivateNode effect in node.effectActivations)
            {
                EvaluateEffectsToBeActivate(effect);
            }
        }

        public void EvaluateEffectsToBeActivate(EffectsToBeActivateNode node)
        {
            EvaluateSelector(node.Selector);
            EvaluateEffect(node.Effect);

            if(node.PostAction != null)
            {
                EvaluatePostActionNode(node.PostAction);
            }
        }

        public void EvaluateSelector(SelectorNode node)
        {
            GameObject Source = (GameObject)Evaluate(node.source);

            List<GameObject> cards = new List<GameObject>();

            foreach (Transform child in Source.transform)
            {
                if (child.gameObject.GetComponent<Card>() != null)
                {
                    cards.Add(child.gameObject);
                }
            }

            bool getOne = (bool)Evaluate(node.single);

            scopes.Peek().Add("Sinlge", getOne);
            scopes.Peek().Add("CardsToPredicate", cards);
            
            EvaluatePredicate(node.predicate);
        }

        public void EvaluatePredicate(PredicateNode node)
        {
            EnterScope();
            Queue<GameObject> filteredCards = new Queue<GameObject>();
            
            string temp = (string)Evaluate(node.identifier.GameObject);

            foreach (GameObject card in (List<GameObject>)FindVariableScope("CardToPredicate")["CardsToPredicate"])
            {
                node.identifier.GameObject.Value = card.gameObject.name;
                scopes.Peek().Add(temp, Evaluate(node.identifier));
                
                if ((bool)Evaluate(node.Condition))
                {
                    filteredCards.Enqueue(card);
                }
                scopes.Peek().Remove(temp);
            }
            ExitScope();
            scopes.Peek().Remove("CardsToPredicate");
            scopes.Peek().Add("FilteredCards", filteredCards);
        }

        public void EvaluatePostActionNode(PostActionNode node)
        {
            EvaluateSelector(node.selector);
            EvaluateEffect(node.parameters);

            if(node.postAction != null)
            {
                EvaluatePostActionNode(node.postAction);
            }
        }

        public void EvaluateEffect(EffectParametersAssignementNode node)
        {
            string effectname = EvaluateString(node.Name);
            foreach (VariableAssignementNode parameter in node.Parameters)
            {
                Evaluate(parameter);
            }
            Evaluate(availableEffects[effectname]);
        }

        public void EvaluateEffectDeclarationNode(EffectDeclarationNode node)
        {
            foreach (VariableAssignementNode parameter in node.Params.Params)
            {
                if (scopes.Peek().ContainsKey(EvaluateString(parameter.Name)))
                {
                    continue;
                }
                else
                {
                    throw new Exception($"Variable '{EvaluateString(parameter.Name)}' does not exist, and we need it for the effect execution.");
                }
            }

            EvaluateActionNode(node.Action);
        }   

        public void EvaluateActionNode(ActionDeclarationNode node)
        {
            EnterScope();
            
            string temp = (string)Evaluate(node.Targets.GameObject);

            foreach (GameObject card in (List<GameObject>)FindVariableScope("FilteredCards")["FilteredCards"])
            {
                node.Targets.GameObject.Value = card.gameObject.name;
                scopes.Peek().Add(temp, Evaluate(node.Targets));

                //Context TODO, crear un objecto context que tenga todos los datos necesarios en unity
                foreach (StatementNodes statement in node.Body.Statements)
                {
                    Evaluate(statement);
                }
            }
            ExitScope();
        }

        public double EvaluateUnaryExpressionNode(UnaryExpressionNode node)
        {
            object operand = Evaluate(node.Expression);

            switch(node.Operator.type)
            {
                case TokenType.MinusOne:
                    if (operand is double operandDouble)
                    {
                        return operandDouble - 1;
                    }
                    else 
                    {
                        throw new Exception("Unary operator - is only defined for numbers.");
                    }
                case TokenType.PlusOne:
                    if (operand is double operandDouble2)
                    {
                        return operandDouble2 + 1;
                    }
                    else 
                    {
                        throw new Exception("Unary operator + is only defined for numbers.");
                    }
                default:
                    throw new System.Exception("Unknown unary operator");
            }
        }

        public object EvaluateBinaryExpressionNode(BinaryExpressionNode node)
        {
            object left = Evaluate(node.Left);
            object right = Evaluate(node.Right);

            switch(node.Operator.type)
            {
                case TokenType.Plus:
                    if (left is double leftDoublePlus && right is double rightDoublePlus)
                    {
                        return leftDoublePlus + rightDoublePlus;
                    }
                    else
                    {
                        throw new Exception("Binary operator + is only defined for numbers and strings.");
                    }
                case TokenType.Sub:
                    if (left is double leftDoubleSub && right is double rightDoubleSub)
                    {
                        return leftDoubleSub - rightDoubleSub;
                    }
                    else
                    {
                        throw new Exception("Binary operator - is only defined for numbers.");
                    }
                case TokenType.Multiplication:
                    if (left is double leftDoubleMult && right is double rightDoubleMult)
                    {
                        return leftDoubleMult * rightDoubleMult;
                    }
                    else
                    {
                        throw new Exception("Binary operator * is only defined for numbers.");
                    }
                case TokenType.Div:
                    if (left is double leftDoubleDiv && right is double rightDoubleDiv)
                    {
                        return leftDoubleDiv / rightDoubleDiv;
                    }
                    else
                    {
                        throw new Exception("Binary operator / is only defined for numbers.");
                    }
                case TokenType.Pow:
                    if (left is double leftDoublePow && right is double rightDoublePow)
                    {
                        return Math.Pow(leftDoublePow, rightDoublePow);
                    }
                    else
                    {
                        throw new Exception("Binary operator ^ is only defined for numbers.");
                    }
                case TokenType.Equal:
                    if (left is double leftDouble && right is double rightDouble)
                    {
                        return leftDouble == rightDouble;
                    }
                    else if (left is string leftString && right is string rightString)
                    {
                        return leftString == rightString;
                    }
                    else if (left is bool leftBool && right is bool rightBool)
                    {
                        return leftBool == rightBool;
                    }
                    else
                    {
                        throw new Exception("Binary operator == is only defined for numbers, strings and booleans.");
                    }
                case TokenType.NotEqual:
                    if (left is double leftDoubleNotEqual && right is double rightDoubleNotEqual)
                    {
                        return leftDoubleNotEqual != rightDoubleNotEqual;
                    }
                    else if (left is string leftString && right is string rightString)
                    {
                        return leftString != rightString;
                    }
                    else
                    {
                        throw new Exception("Binary operator != is only defined for numbers, strings and booleans.");
                    }
                case TokenType.GreaterThan:
                    if (left is double leftDoubleGreater && right is double rightDoubleGreater)
                    {
                        return leftDoubleGreater > rightDoubleGreater;
                    }
                    else
                    {
                        throw new Exception("Binary operator > is only defined for numbers.");
                    }
                case TokenType.GreatherOrEqual:
                    if (left is double leftDoubleGreaterEqual && right is double rightDoubleGreaterEqual)
                    {
                        return leftDoubleGreaterEqual >= rightDoubleGreaterEqual;
                    }
                    else
                    {
                        throw new Exception("Binary operator >= is only defined for numbers.");
                    }
                case TokenType.LessOrEqual:
                    if (left is double leftDoubleLessEqual && right is double rightDoubleLessEqual)
                    {
                        return leftDoubleLessEqual <= rightDoubleLessEqual;
                    }
                    else
                    {
                        throw new Exception("Binary operator <= is only defined for numbers.");
                    }
                case TokenType.LessThan:
                    if (left is double leftDoubleLess && right is double rightDoubleLess)
                    {
                        return leftDoubleLess < rightDoubleLess;
                    }
                    else
                    {
                        throw new Exception("Binary operator < is only defined for numbers.");
                    }
                case TokenType.LogicalAnd:
                    if (left is bool leftBoolAnd && right is bool rightBoolAnd)
                    {
                        return leftBoolAnd && rightBoolAnd;
                    }
                    else
                    {
                        throw new Exception("Binary operator && is only defined for booleans.");
                    }
                case TokenType.LogicalOr:
                    if (left is bool leftBoolOr && right is bool rightBoolOr)
                    {
                        return leftBoolOr || rightBoolOr;
                    }
                    else
                    {
                        throw new Exception("Binary operator || is only defined for booleans.");
                    }
                case TokenType.Concatenate:
                    if (left is string leftStringConcat && right is string rightStringConcat)
                    {
                        return leftStringConcat + " " + rightStringConcat;
                    }
                    else
                    {
                        throw new Exception("Binary operator + is only defined for strings.");
                    }
                case TokenType.At:
                    if (left is string leftStringAt && right is string rightStringAt)
                    {
                        return leftStringAt + rightStringAt;
                    }
                    else
                    {
                        throw new Exception("Binary operator + is only defined for strings.");
                    }
                default :
                    throw new System.Exception("Unknown binary operator");
                }
        }

        public void EvaluateBlock(List<ASTNode> statements)
        {
            foreach (ASTNode statement in statements)
            {
                Evaluate(statement);
            }
        }

        public object EvaluatePropertyCallNode(PropertyCallNode node)
        {
            object obj = Evaluate(node.Target) is GameObject objectReferenceNode ? objectReferenceNode : throw new Exception("Target of property call must be a GameObject.");
            object propertyName = node.PropertyName is StringNode stringNode ? EvaluateString(stringNode) : throw new Exception("Property name must be a PropertyName.");

            if(objectReferenceNode.GetComponent<Card>() != null) //Entonces es una carta
            {
                switch(propertyName)
                {
                    case "Power": 
                        return objectReferenceNode.GetComponent<Card>().attackPower;
                    case "Owner":
                        if(objectReferenceNode.tag == "Card Player1")
                            return "player 1";
                        else
                            return "player2";
                    default:
                        throw new Exception("Unknown property name");
                }
            }
            else if(objectReferenceNode.GetComponent<Context>() != null) //TODO, crear el object context con su script asociado
            {
                switch(propertyName)
                {
                    case "TriggerPlayer":
                        if (GameObject.Find("Game Manger").GetComponent<GameManager>().state == gameTracker.Player1Turn)
                            return GameObject.Find("Player1 Manager");
                        else
                            return GameObject.Find("Player2 Manager");
                            
                    case "Board":
                        List<GameObject> cards = new List<GameObject>();

                        foreach (Transform child in GameObject.Find("Panels p1").transform)
                        {
                            if(child.GetComponent<Card>() != null) cards.Add(child.gameObject);
                        }
                        foreach (Transform child in GameObject.Find("Panels p2").transform)
                        {
                            if(child.GetComponent<Card>() != null) cards.Add(child.gameObject);
                        }

                        return cards;
                    default:
                        throw new Exception("Unknown property name");
                }
            }
            else 
            {
                throw new Exception("This GameObject cant be called with a property");
            }
        }

        public object EvaluateMethodCallNode(MethodCallNode node)
        {
            object obj = Evaluate(node.Target) is GameObject objectReferenceNode ? objectReferenceNode : throw new Exception("Target of property call must be a GameObject.");
            object MethodName = node.MethodName is StringNode stringNode ? EvaluateString(stringNode) : throw new Exception("Property name must be a PropertyName.");
            GameObject argument = EvaluateObjectReference(node.Arguments);

            
            if(objectReferenceNode.GetComponent<Context>() != null) //TODO, crear el object context con su script asociado
            {
                switch(MethodName)
                {
                    case "HandOfPlayer":
                        List<GameObject> cardsOnHand = GetCardsInObject(argument.GetComponent<PlayerManager>().hand);
                        return cardsOnHand;

                    case "FieldOfPlayer":
                        List<GameObject> cardsOnField = GetCardsInObject(argument.GetComponent<PlayerManager>().field);
                        return cardsOnField;

                    case "GraveyardOfPlayer":
                        List<GameObject> cardsOnGraveyard = GetCardsInObject(argument.GetComponent<PlayerManager>().graveyard);
                        return cardsOnGraveyard;

                    case "DeckOfplayer":
                        List<GameObject> cardsOnDeck = GetCardsInObject(argument.GetComponent<PlayerManager>().deck);
                        return cardsOnDeck;

                    default:
                        throw new Exception("Unknown Method name");
                }
            }
            else if(objectReferenceNode.GetComponent<CardContainer>() != null)
            {
                List<GameObject> cards = new List<GameObject>();

                foreach (Transform child in objectReferenceNode.transform)
                {
                    if(child.GetComponent<Card>() != null) cards.Add(child.gameObject);
                }

                switch(MethodName)
                {
                    case "Find": //En el proximo capitulo
                    case "Push":
                    case "SendBottom":
                    case "Pop":
                    case "Remove":
                    case "Shuffle":
                    default:
                        throw new Exception("Unknown Method name");
                }
            }
            else 
            {
                throw new Exception("This GameObject cant be called with a Method");
            }
        }
    




        public List<GameObject> GetCardsInObject (GameObject panel)
        {
            List<GameObject> cards = new List<GameObject>();

            foreach (Transform transform in panel.transform)
            {
                if (transform.GetComponent<Card>() != null) cards.Add(transform.gameObject);
            }

            return cards;
        }

        public void EnterScope()
        {
            scopes.Push(new Dictionary<string, object>());
        }

        public void ExitScope()
        {
            scopes.Pop();
        }

        private Dictionary<string, object> FindVariableScope(string variableName)
        {
            foreach (Dictionary<string, object> scope in scopes)
            {
                if (scope.ContainsKey(variableName))
                {
                    return scope;
                }
            }

            // If the variable is not found in the local scope,
            // recursively search in the parent scope (the previous scope in the stack).
            // if (scopes.Count > 1)
            // {
            //     scopes.Pop();
            //     Dictionary<string, object> parentScope = FindVariableScope(variableName);
            //     scopes.Push(parentScope);
            //     return parentScope;
            // }

            return null;
        }
    }
}