using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;

namespace Compiler
{
    public class Semantic
    {
        static List<string> acceptedTypesOfCards = new List<string> { "Oro", "Plata", "Despeje", "Señuelo", "Clima", "Lider", "Aumento"};
        static List<string> acceptedTypesOfRanges = new List<string> {"Melee", "Range", "Siege", "X"};
        public static List<string> acceptedTypesOfEffects = new List<string> {};

        public void CheckCemantic(ASTNode node)
        {
            switch(node)
            {
                case CardDeclarationNode cardNode:

                    foreach (StringNode rangeNode in  cardNode.Ranges)
                    {
                        if (!acceptedTypesOfRanges.Contains(rangeNode.Value))
                        {
                            throw new Exception("Unknown range type");
                        }    
                    }

                    if (!acceptedTypesOfCards.Contains(cardNode.Type.Value))
                    {
                        throw new Exception("Unknown card type");
                    }
                    else if (cardNode.Name.Value == "")
                    {
                        throw new Exception("Card name cannot be empty");
                    }
                    else if (cardNode.Faction.Value == "")
                    {
                        throw new Exception("Card faction cannot be empty");
                    }
                    else if (cardNode.Power.Value < 0)
                    {
                        throw new Exception("Card power cannot be negative");
                    }
                    else if (cardNode.OnActivation != null)
                    {
                        if(cardNode.OnActivation != null) CheckCemantic(cardNode.OnActivation);
                    }

                    break;
                case OnActivationNode effectNode:
                    
                    foreach (var effect in effectNode.effectActivations)
                    {
                        if (!acceptedTypesOfEffects.Contains(effect.Effect.Name.Value))
                        {
                            throw new Exception("Unknown effect type");
                        }

                        if (effect.PostAction != null)
                        {
                            if (!acceptedTypesOfEffects.Contains(effect.PostAction.parameters.Name.Value))
                            {
                                throw new Exception("Unknown effect type");
                            }
                        }
                    }
                    break;
                default:
                    throw new Exception($"Unknown node type");
            }
        }
    }
}