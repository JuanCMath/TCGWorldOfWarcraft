using Compiler;
using Enums;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Create/NewCard")]
public class CardData : ScriptableObject
{   
    public type cardType;
    public bool isHero;
    public string cardName;
    public string cardFaction;
    public int attackPower;
    public string[] slots;
    public OnActivationNode effect;
    


    [Multiline] //adds more space to the string field
    public string cardDescription;
    
    public int cardID;
    public Sprite art;
}
