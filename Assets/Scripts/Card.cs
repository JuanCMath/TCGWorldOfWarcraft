using System.Collections;
using System.Collections.Generic;
using Enums;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardData cardData;

    [Header("Card Info")]
    //public TextMeshProUGUI carNametext;
    public TextMeshProUGUI cardDescText;
    public TextMeshProUGUI cardTypeText;
    public TextMeshProUGUI cardAttack;

    [Header("Card Data")]
    public int cardID;
    public string cardName;
    public faction cardFaction;
    public string cardDescription;
    public bool isHero;
    public slot cardSlot;
    public type cardType;
    
    public int attackPower;

    private void Start()
    {
        CollectInfoFromSO();
    }

    private void CollectInfoFromSO()
    {
        if(cardData == null)
        {
            Destroy(this.gameObject);
            return;
        }

        cardID = cardData.cardID;
        cardName = cardData.cardName;
        cardFaction = cardData.cardFaction;
        cardDescription = cardData.cardDescription;
        isHero = cardData.isHero;
        cardSlot = cardData.cardSlot;
        cardType = cardData.cardType;
        attackPower = cardData.attackPower;   

        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        cardDescText.text = cardDescription;
        cardTypeText.text = cardType.ToString();
        cardAttack.text = attackPower.ToString();
    }
}
