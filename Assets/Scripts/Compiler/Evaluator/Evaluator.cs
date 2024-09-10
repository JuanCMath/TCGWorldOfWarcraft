using System.Collections.Generic;
using System;
using Enums;
using UnityEngine;
using System.Linq;
#nullable enable


namespace Compiler
{
    public class Evaluator : MonoBehaviour
    {
        private Stack<Dictionary<string, object>> scopes =  new Stack<Dictionary<string, object>>();
        
        public object? Evaluate(ASTNode node)
        {
            scopes.Push(new Dictionary<string, object>());
            scopes.Peek().Add("context", GameObject.Find("Context"));
            scopes.Peek().Add("player1", GameObject.Find("Player1"));
            scopes.Peek().Add("player2", GameObject.Find("Player2"));
            
            switch(node)
            {
                case CardDeclarationNode cardNode:
                    return EvaluateCardDeclarationNode(cardNode);
                
                case EffectDeclarationNode effectNode:
                    EvaluateEffectDeclarationNode(effectNode);
                    return null;

                case OnActivationNode onActivationNode:
                    EvaluateOnActivationBlock(onActivationNode);
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
                    object value = Evaluate(variableNode.Value) ?? throw new ArgumentNullException(nameof(value));

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

                    object forObject = Evaluate(forNode.Objetcs) ?? throw new ArgumentNullException(nameof(forObject));

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
                    throw new Exception("Unknown node type");
            }
            return null;
        }

        private CardData EvaluateCardDeclarationNode(CardDeclarationNode cardnode)
        {
            CardData data = new CardData();

            data.cardType = EvaluateType(cardnode.Type);
            data.isHero = EvaluateIsHero(cardnode.Type);
            data.cardName = Evaluate(cardnode.Name) as string;
            data.cardFaction = Evaluate(cardnode.Faction) as string;
            data.attackPower = Evaluate(cardnode.Power) is int number ? number: 0;
            data.slots = EvaluateRanges(cardnode.Ranges);
            data.effect = SaveOnActivationBlock(cardnode.OnActivation);

            data.cardDescription = cardnode.description != null ? Evaluate(cardnode.description) as string : null;
            data.art = cardnode.artName != null ? Resources.Load<Sprite>(EvaluateString(cardnode.artName)) : null;

            return data;
        }

        private string EvaluateString(StringNode node)
        {
            return node.Value;
        }

        private int EvaluateNumber(NumberNode node)
        {
            return node.Value;
        }

        private bool EvaluateBool(BooleanNode node)
        {
            return node.Value;
        }

        private type EvaluateType(StringNode node)
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
                case "Lider":
                    return type.Lider;
                default:
                    throw new System.Exception("Unknown type");
            }
        }

        private string[] EvaluateRanges(StringNode[] nodes)
        {
            string[] ranges = new string[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                ranges[i] = EvaluateString(nodes[i]);
            }
            
            return ranges;
        }

        private bool EvaluateIsHero(StringNode node)
        {
            string value = EvaluateString(node);
            return value == "Oro";
        }

        private GameObject EvaluateObjectReference(GameObjectReferenceNode node)
        {
            return GameObject.Find(EvaluateString(node.GameObject));
        }

        private OnActivationNode SaveOnActivationBlock(OnActivationNode node)
        {
            return node;
        }

        private void EvaluateOnActivationBlock(OnActivationNode node)
        {
            foreach (EffectsToBeActivateNode effect in node.effectActivations)
            {
                EvaluateEffectsToBeActivate(effect);
            }
        }

        private void EvaluateEffectsToBeActivate(EffectsToBeActivateNode node)
        {
            if (node.Selector !=null) EvaluateSelector(node.Selector);
            EvaluateEffect(node.Effect);

            if(node.PostAction != null)
            {
                EvaluatePostActionNode(node.PostAction);
            }
        }

        private void EvaluateSelector(SelectorNode node)
        {
            List<GameObject> cards = new List<GameObject>();

            string sourcefount = Evaluate(node.source) as string;

            switch (sourcefount)
            {
                case "hand":
                    if (GameObject.Find("Game Manager").GetComponent<GameManager>().state == gameTracker.Player1Turn)
                        cards = GetCardsInObject(GameObject.Find("Player1").GetComponent<PlayerManager>().hand);
                    else
                        cards = GetCardsInObject(GameObject.Find("Player2").GetComponent<PlayerManager>().hand);
                    
                    if (FindVariableScope("CardsToPredicate") != null) FindVariableScope("CardsToPredicate")["CardsToPredicate"] = cards;
                    
                    break;

                case "otherHand":
                    if (GameObject.Find("Game Manager").GetComponent<GameManager>().state == gameTracker.Player1Turn)
                        cards = GetCardsInObject(GameObject.Find("Player2").GetComponent<PlayerManager>().hand);
                    else
                        cards = GetCardsInObject(GameObject.Find("Player1").GetComponent<PlayerManager>().hand);

                    if (FindVariableScope("CardsToPredicate") != null) FindVariableScope("CardsToPredicate")["CardsToPredicate"] = cards;    
                    break;

                case "field":
                    if (GameObject.Find("Game Manager").GetComponent<GameManager>().state == gameTracker.Player1Turn)
                        cards = GetCardsInObject(GameObject.Find("Player1").GetComponent<PlayerManager>().field);
                    else
                        cards = GetCardsInObject(GameObject.Find("Player2").GetComponent<PlayerManager>().field);

                    if (FindVariableScope("CardsToPredicate") != null) FindVariableScope("CardsToPredicate")["CardsToPredicate"] = cards;
                    break;
                
                case "otherField":
                    if (GameObject.Find("Game Manager").GetComponent<GameManager>().state == gameTracker.Player1Turn)
                        cards = GetCardsInObject(GameObject.Find("Player2").GetComponent<PlayerManager>().field);
                    else
                        cards = GetCardsInObject(GameObject.Find("Player1").GetComponent<PlayerManager>().field); 
                    
                    if (FindVariableScope("CardsToPredicate") != null) FindVariableScope("CardsToPredicate")["CardsToPredicate"] = cards;
                    break;

                case "deck":
                    if (GameObject.Find("Game Manager").GetComponent<GameManager>().state == gameTracker.Player1Turn)
                        cards = GetCardsInObject(GameObject.Find("Player1").GetComponent<PlayerManager>().deck);
                    else
                        cards = GetCardsInObject(GameObject.Find("Player2").GetComponent<PlayerManager>().deck); 
                    
                    if (FindVariableScope("CardsToPredicate") != null) FindVariableScope("CardsToPredicate")["CardsToPredicate"] = cards;
                    break;

                case "otherDeck":
                    if (GameObject.Find("Game Manager").GetComponent<GameManager>().state == gameTracker.Player1Turn)
                        cards = GetCardsInObject(GameObject.Find("Player2").GetComponent<PlayerManager>().deck);
                    else
                        cards = GetCardsInObject(GameObject.Find("Player1").GetComponent<PlayerManager>().deck); 
                    
                    if (FindVariableScope("CardsToPredicate") != null) FindVariableScope("CardsToPredicate")["CardsToPredicate"] = cards;
                    break;

                case "board":
                    foreach (Transform child in GameObject.Find("Field p1").transform)
                    {
                        foreach (Transform child2 in child.gameObject.transform)
                        {
                            if(child2.GetComponent<Card>() != null) cards.Add(child2.gameObject);
                        }
                    }
                    foreach (Transform child in GameObject.Find("Field p2").transform)
                    {
                        foreach (Transform child2 in child.gameObject.transform)
                        {
                            if(child2.GetComponent<Card>() != null) cards.Add(child2.gameObject);
                        }
                    }

                    if (FindVariableScope("CardsToPredicate") != null) FindVariableScope("CardsToPredicate")["CardsToPredicate"] = cards;
                    break;

                case "parent":
                    cards = (List<GameObject>)FindVariableScope("FilteredCards")["FilteredCards"];
                    break;

                default:
                    throw new Exception("Unknown source");
            }

            bool getOne = (bool?)Evaluate(node.single) ?? false;

            scopes.Peek().Add("Single", getOne);
            if(FindVariableScope("CardsToPredicate") == null) scopes.Peek().Add("CardsToPredicate", cards);
        
            EvaluatePredicate(node.predicate);
        }

        private void EvaluatePredicate(PredicateNode node)
        {
            
            EnterScope();
            Queue<GameObject> filteredCards = new Queue<GameObject>();
            bool getOne = (bool)FindVariableScope("Single")["Single"];
            
            List<GameObject> cards = FindVariableScope("CardsToPredicate")["CardsToPredicate"] as List<GameObject> ?? throw new Exception("Cards to predicate evaluation returned null.");

            //Guardo el nombre del identificador del predicate para la evluacion de la Expresion
            string temp = Evaluate(node.identifier.GameObject) as string ?? throw new Exception("Identifier evaluation returned null.");
            //Itero sobre todas las cartas a evaluar
            foreach (GameObject card in cards)
            {
                //Guardar la referencia al gameObject actual para llamarlo en la evaluacion de la expresion
                node.identifier.GameObject.Value = card.gameObject.name;

                //Evaluo Para que me devuelva el gameobject guardado en el paso anterior
                var value = Evaluate(node.identifier);

                //Añado la referencia al gameObject usando el identifier del predicate
                if (value != null)
                {
                    scopes.Peek().Add(temp, value);
                }
                else throw new Exception("Predicate evaluation returned null.");
                
                //Evaluo la condicion, si true añado la carta como que paso la evaluacion del predicate
                if (Evaluate(node.Condition) is bool condition && condition)
                {
                    filteredCards.Enqueue(card);

                    if (getOne) break;
                }

                //Elimino la referencia actual del objeto
                scopes.Peek().Remove(temp);
            }
            ExitScope();

            //Acualizo las cartas del predicado
            scopes.Peek().Remove("CardsToPredicate");
            scopes.Peek().Add("FilteredCards", filteredCards.ToList());
        }

        private void EvaluatePostActionNode(PostActionNode node)
        {
            if (node.selector !=null) EvaluateSelector(node.selector);

            EvaluateEffect(node.parameters);

            if(node.postAction != null)
            {
                EvaluatePostActionNode(node.postAction);
            }
        }

        private void EvaluateEffect(EffectParametersAssignementNode node)
        {
            string effectname = EvaluateString(node.Name);
            if (node.Parameters != null)
            {
                foreach (VariableAssignementNode parameter in node.Parameters)
                {
                    Evaluate(parameter);
                }
            }
            Evaluate(Effects.availableEffects[effectname]);
        }

        private void EvaluateEffectDeclarationNode(EffectDeclarationNode node)
        {
            if (node.Params != null)
            {
                foreach (VariableAssignementNode parameter in node.Params.Params)
                {
                    if (FindVariableScope(EvaluateString(parameter.Name)) != null)
                    {
                        continue;
                    }
                    else
                    {
                        throw new Exception($"Variable '{EvaluateString(parameter.Name)}' does not exist, and we need it for the effect execution.");
                    }
                }
            }

            EvaluateActionNode(node.Action);
        }   

        private void EvaluateActionNode(ActionDeclarationNode node)
        {
            EnterScope();
            
            string temp = Evaluate(node.Targets.GameObject) as string ?? throw new Exception("Target evaluation returned null.");

            if ( FindVariableScope("FilteredCards") != null)
            {
                scopes.Peek().Add(temp, (List<GameObject>)FindVariableScope("FilteredCards")["FilteredCards"]);
            }
            foreach (ASTNode statement in node.Body.Statements)
            {
                Evaluate(statement);
            }
            
            ExitScope();
        }

        private int EvaluateUnaryExpressionNode(UnaryExpressionNode node)
        {
            object? operand = Evaluate(node.Expression);

            string variableReference = "";

            if(node.Expression is VariableReferenceNode variableReferenceNode)
            {
                variableReference = EvaluateString(variableReferenceNode.Name);
            }

            switch(node.Operator.type)
            {
                case TokenType.MinusOne:
                    if (operand is int operandDouble)
                    {
                        if(FindVariableScope(variableReference) != null) FindVariableScope(variableReference)[variableReference] = operandDouble - 1;
                        return operandDouble - 1;
                    }
                    else 
                    {
                        throw new Exception("Unary operator - is only defined for numbers.");
                    }
                case TokenType.PlusOne:
                    if (operand is int operandDouble2)
                    {
                        if(FindVariableScope(variableReference) != null) FindVariableScope(variableReference)[variableReference] = operandDouble2 + 1;
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

        private object EvaluateBinaryExpressionNode(BinaryExpressionNode node)
        {
            object? left = Evaluate(node.Left);
            object? right = Evaluate(node.Right);

            GameObject card = null;
            object propertyName = "";
            
            if(node.Operator.type == TokenType.MinusEqual ||
               node.Operator.type == TokenType.PlusEqual  )
            {
                if(node.Left is PropertyCallNode propertyCall)
                {
                    card = Evaluate(propertyCall.Target) as GameObject;
                    propertyName = propertyCall.PropertyName is StringNode stringNode ? EvaluateString(stringNode) : throw new Exception("Property name must be a PropertyName.");
                }
            }

            switch(node.Operator.type)
            {
                case TokenType.Plus:
                    if (left is int leftDoublePlus && right is int rightDoublePlus)
                    {
                        return leftDoublePlus + rightDoublePlus;
                    }
                    else
                    {
                        throw new Exception("Binary operator + is only defined for numbers and strings.");
                    }
                case TokenType.Sub:
                    if (left is int leftDoubleSub && right is int rightDoubleSub)
                    {
                        return leftDoubleSub - rightDoubleSub;
                    }
                    else
                    {
                        throw new Exception("Binary operator - is only defined for numbers.");
                    }
                case TokenType.MinusEqual:
                    if(left is int leftDoubleMinusEqual && right is int rightDoubleMinusEqual)
                    {
                        if(propertyName as string == "Power" && card != null) card.GetComponent<Card>().attackPower = leftDoubleMinusEqual - rightDoubleMinusEqual;
                        return leftDoubleMinusEqual - rightDoubleMinusEqual;
                    }
                    else
                    {
                        throw new Exception("Binary operator -= is only defined for numbers.");
                    }
                case TokenType.PlusEqual:
                    if(left is int leftDoublePlusEqual && right is int rightDoublePlusEqual)
                    {
                        if(propertyName as string == "Power" && card != null) card.GetComponent<Card>().attackPower = leftDoublePlusEqual + rightDoublePlusEqual;
                        return leftDoublePlusEqual - rightDoublePlusEqual;
                    }
                    else
                    {
                        throw new Exception("Binary operator += is only defined for numbers.");
                    }
                case TokenType.Multiplication:
                    if (left is int leftDoubleMult && right is int rightDoubleMult)
                    {
                        return leftDoubleMult * rightDoubleMult;
                    }
                    else
                    {
                        throw new Exception("Binary operator * is only defined for numbers.");
                    }
                case TokenType.Div:
                    if (left is int leftDoubleDiv && right is int rightDoubleDiv)
                    {
                        return leftDoubleDiv / rightDoubleDiv;
                    }
                    else
                    {
                        throw new Exception("Binary operator / is only defined for numbers.");
                    }
                case TokenType.Pow:
                    if (left is int leftDoublePow && right is int rightDoublePow)
                    {
                        return Math.Pow(leftDoublePow, rightDoublePow);
                    }
                    else
                    {
                        throw new Exception("Binary operator ^ is only defined for numbers.");
                    }
                case TokenType.Equal:
                    if (left is int leftDouble && right is int rightDouble)
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
                    if (left is int leftDoubleNotEqual && right is int rightDoubleNotEqual)
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
                    if (left is int leftDoubleGreater && right is int rightDoubleGreater)
                    {
                        return leftDoubleGreater > rightDoubleGreater;
                    }
                    else
                    {
                        throw new Exception("Binary operator > is only defined for numbers.");
                    }
                case TokenType.GreatherOrEqual:
                    if (left is int leftDoubleGreaterEqual && right is int rightDoubleGreaterEqual)
                    {
                        return leftDoubleGreaterEqual >= rightDoubleGreaterEqual;
                    }
                    else
                    {
                        throw new Exception("Binary operator >= is only defined for numbers.");
                    }
                case TokenType.LessOrEqual:
                    if (left is int leftDoubleLessEqual && right is int rightDoubleLessEqual)
                    {
                        return leftDoubleLessEqual <= rightDoubleLessEqual;
                    }
                    else
                    {
                        throw new Exception("Binary operator <= is only defined for numbers.");
                    }
                case TokenType.LessThan:
                    if (left is int leftDoubleLess && right is int rightDoubleLess)
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
                    throw new Exception("Unknown binary operator");
                }
        }

        private void EvaluateBlock(List<ASTNode> statements)
        {
            foreach (ASTNode statement in statements)
            {
                Evaluate(statement);
            }
        }

        private object? EvaluatePropertyCallNode(PropertyCallNode node)
        {
            object obj = Evaluate(node.Target) is GameObject objectReferenceNode ? objectReferenceNode : throw new Exception("Target of property call must be a GameObject.");
            object propertyName = node.PropertyName is StringNode stringNode ? EvaluateString(stringNode) : throw new Exception("Property name must be a PropertyName.");

            if(objectReferenceNode.GetComponent<Card>() != null) //Entonces es una carta
            {
                switch(propertyName)
                {
                    case "Power": 
                        return objectReferenceNode.GetComponent<Card>().attackPower;
                    case "Faction":
                        return objectReferenceNode.GetComponent<Card>().cardFaction;
                    case "Owner":
                        if(objectReferenceNode.tag == "Card Player1")
                            return "player1";
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
                        if (GameObject.Find("Game Manager").GetComponent<GameManager>().state == gameTracker.Player1Turn)
                            return "player1";
                        else
                            return "player2";
                            
                    case "Hand":
                        if (GameObject.Find("Game Manager").GetComponent<GameManager>().state == gameTracker.Player1Turn)
                            return GetCardsInObject(GameObject.Find("Player1").GetComponent<PlayerManager>().hand);
                        else
                            return GetCardsInObject(GameObject.Find("Player2").GetComponent<PlayerManager>().hand);

                    case "Field":
                        if (GameObject.Find("Game Manager").GetComponent<GameManager>().state == gameTracker.Player1Turn)
                            return GetCardsInObject(GameObject.Find("Player1").GetComponent<PlayerManager>().field);
                        else
                            return GetCardsInObject(GameObject.Find("Player2").GetComponent<PlayerManager>().field);

                    case "Graveyard":
                        if (GameObject.Find("Game Manager").GetComponent<GameManager>().state == gameTracker.Player1Turn)
                            return GetCardsInObject(GameObject.Find("Player1").GetComponent<PlayerManager>().graveyard);
                        else
                            return GetCardsInObject(GameObject.Find("Player2").GetComponent<PlayerManager>().graveyard);
                    
                    case "Deck":
                        if (GameObject.Find("Game Manager").GetComponent<GameManager>().state == gameTracker.Player1Turn)
                            return GetCardsInObject(GameObject.Find("Player1").GetComponent<PlayerManager>().deck);
                        else
                            return GetCardsInObject(GameObject.Find("Player2").GetComponent<PlayerManager>().deck);

                    case "Board":
                        List<GameObject> cards = new List<GameObject>();

                        foreach (Transform child in GameObject.Find("Field p1").transform)
                        {
                            foreach (Transform child2 in child.gameObject.transform)
                            {
                            if(child2.GetComponent<Card>() != null) cards.Add(child2.gameObject);
                            }
                        }
                        foreach (Transform child in GameObject.Find("Field p2").transform)
                        {
                            foreach (Transform child2 in child.gameObject.transform)
                            {
                            if(child2.GetComponent<Card>() != null) cards.Add(child2.gameObject);
                            }
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

        private object? EvaluateMethodCallNode(MethodCallNode node)
        {
            object obj = Evaluate(node.Target);
            object MethodName = node.MethodName is StringNode stringNode ? EvaluateString(stringNode) : throw new Exception("Method name must be a PropertyName.");
            GameObject argument;

            if (node.Arguments != null && Evaluate(node.Arguments) is string argumentString)
            {
                argument = FindVariableScope(argumentString)[argumentString] as GameObject;
            }
            else if (node.Arguments != null && Evaluate(node.Arguments) is GameObject tempArgument)
            {
                argument = tempArgument;
            }
            else
            {
                argument = null;
            }
            
            if(obj is GameObject objectReferenceNode && objectReferenceNode.GetComponent<Context>() != null  )
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

                    case "DeckOfPlayer":
                        List<GameObject> cardsOnDeck = GetCardsInObject(argument.GetComponent<PlayerManager>().deck);
                        return cardsOnDeck;
                    default:
                        throw new Exception("Unknown Method name");
                }
            }

            else if (obj is List<GameObject> gameObjectListReference2 && argument == null)
            {
                Transform panelOfTheList = gameObjectListReference2[gameObjectListReference2.Count % 2].transform.parent;

                switch(MethodName)
                {
                    case "Pop":
                        while (GameObject.Find("Utility").transform.childCount > 0)
                        {
                            Destroy(panelOfTheList.transform.GetChild(0).gameObject);
                        }

                        gameObjectListReference2[0].transform.SetParent(GameObject.Find("Utility").transform);
                        return GameObject.Find("Utility").transform.GetChild(0).gameObject;

                    case "Shuffle":
                        
                        ShuffleCards(panelOfTheList.gameObject);
                        
                        return null;

                    default :
                        throw new Exception("Unknown Method name");
                }
            }

            else if(obj is List<GameObject> gameObjectListReference && argument.GetComponent<Card>() != null)
            {
                Transform panelOfTheList = gameObjectListReference[0].transform.parent;
                

                switch(MethodName)
                {
                    case "Push":
                        argument.transform.SetParent(panelOfTheList);
                        argument.transform.position = panelOfTheList.position;
                        argument.transform.SetAsFirstSibling();

                        return null;
                        
                    case "SendBottom":
                        
                        argument.transform.SetParent(panelOfTheList);
                        argument.transform.position = panelOfTheList.position;
                        argument.transform.SetAsLastSibling();
                        
                        return null;

                    case "Remove":

                        Destroy(argument);
                        return null;
                        
                    case "Add":

                        GameObject card = argument;
                        card.transform.SetParent(panelOfTheList);
                        argument.transform.position = panelOfTheList.position;
                        card.transform.SetAsLastSibling();
                        return null;

                    default:
                        throw new Exception("Unknown Method name");
                }
            }
            else if (obj is List<GameObject> gameObjectListReference3 && argument != null && node.Arguments is PredicateNode predicateNode)
            {
                switch(MethodName)
                {
                    case "Find":

                        List<GameObject> tempCards = new List<GameObject>();

                        string temp = EvaluateString(predicateNode.identifier.GameObject);
                        foreach (GameObject card in gameObjectListReference3)
                        {
                            predicateNode.identifier.GameObject.Value = card.gameObject.name;
                            var value = Evaluate(predicateNode.identifier);
                            if (value != null)
                            {
                                scopes.Peek().Add(temp, value);
                            }
                            else throw new Exception("Predicate evaluation returned null.");
                            
                            if (Evaluate(predicateNode.Condition) is bool condition && condition)
                            {
                                tempCards.Add(card);
                            }
                            scopes.Peek().Remove(temp);
                        }
                        return tempCards;

                    default:
                        throw new Exception("Unknown Method name");
                }
            }
            else 
            {
                throw new Exception("This GameObject cant be called with a Method");
            }
        }


        #region Utils
        private void ShuffleCards(GameObject panel)
        {
            int childCount = panel.transform.childCount;
            System.Random rand = new System.Random();
    
            for (int i = 0; i < childCount; i++)
            {
                int randomIndex = rand.Next(childCount);
                panel.transform.GetChild(i).SetSiblingIndex(randomIndex);
            }            
        }

        private List<GameObject> GetCardsInObject (GameObject panel)
        {
            List<GameObject> cards = new List<GameObject>();

            if(panel == GameObject.Find("Player2").GetComponent<PlayerManager>().field ||
               panel == GameObject.Find("Player1").GetComponent<PlayerManager>().field   )
            {
                foreach (Transform child in panel.transform)
                {
                    foreach (Transform child2 in child)
                    {
                        if(child2.GetComponent<Card>() != null) cards.Add(child2.gameObject);
                    }
                }
            }
            else
            {
                foreach (Transform transform in panel.transform)
                {
                    if (transform.GetComponent<Card>() != null) cards.Add(transform.gameObject);
                }
            }
            

            return cards;
        }

        private void EnterScope()
        {
            scopes.Push(new Dictionary<string, object>());
        }

        private void ExitScope()
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
            return null;
        }
        #endregion
    }
}