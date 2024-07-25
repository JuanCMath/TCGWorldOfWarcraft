using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;

namespace Compiler
{
    public class Semantic
    {
        static List<string> acceptedTypesOfCards = new List<string> { "Oro", "Plata", "Despeje", "Señuelo", "Clima", "Lider"};
        static List<string> acceptedTypesOfEffects = new List<string> {};
        static List<string> acceptedTypesOfRanges = new List<string> {"Melee", "Range", "Sienge", "X"};
        static List<string> acceptedTypesOfFaction = new List<string> {"Arthas", "Aspectors"};


        public void EvaluateMain(MainProgramNode node)
        {
            CheckCemantic(node);
        }

        public void CheckCemantic(ASTNode node)
        {
            switch(node)
            {
                case MainProgramNode mainNode:
                    EvaluateBlock(mainNode.Body);
                    break;
                case CardDeclarationNode cardNode:
                    if (!acceptedTypesOfCards.Contains(cardNode.Type.Value))
                    {
                        throw new Exception("Invalid type");
                    }
                    else if (!acceptedTypesOfFaction.Contains(cardNode.Faction.Value))
                    {
                        throw new Exception("Invalid faction");
                    }
                    else if (cardNode.Name.Value == "")
                    {
                        throw new Exception("Invalid name");
                    }
                    
                    foreach (var effect in cardNode.OnActivation.effectActivations)
                    {
                        CheckCemantic(effect);
                    }
                    break;
                case EffectDeclarationNode effectNode:
                    if (!acceptedTypesOfEffects.Contains(effectNode.Name.Value))
                    {
                        throw new Exception("Invalid effect type");
                    }
                    break;
                case OnActivationNode onActivationNode:
                    foreach (EffectsToBeActivateNode effect in onActivationNode.effectActivations)
                    {
                        CheckCemantic(effect);
                    }
                    break;
                case EffectsToBeActivateNode effects:
                    if (!acceptedTypesOfEffects.Contains(effects.Effect.Name.Value))
                    {
                        throw new Exception("Invalid effect type");
                    }
                    break;
                default:
                    throw new Exception("Unknown node type");
            }
        }

        public void EvaluateBlock (List<ASTNode> statements)
        {
            foreach (var statement in statements)
            {
                CheckCemantic(statement);
            }
        }
    }
}