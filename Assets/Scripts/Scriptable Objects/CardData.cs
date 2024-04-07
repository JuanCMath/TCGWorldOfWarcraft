using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Enums;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Create/NewCard")]
public class CardData : ScriptableObject
{   
    public int cardID;
    public string cardName;
    public faction cardFaction;

    [Multiline] //adds more space to the string field
    public string cardDescription;

    public bool isHero;
    public slot cardSlot;
    public type cardType;
    
    public int attackPower;
}
