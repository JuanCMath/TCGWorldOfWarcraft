using Compiler;
using Enums;
using UnityEngine;

public class CardData : ScriptableObject
{   
    public type cardType;
    public bool isHero;
    public string cardName;
    public string cardFaction;
    public int attackPower;
    public string[] slots;
    public OnActivationNode effect;

    public string cardDescription;
    
    public int cardID;
    public Sprite art;
}
