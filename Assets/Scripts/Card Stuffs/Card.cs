using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Card : MonoBehaviour
{
    public CardData cardData;

    public GameObject cardBack;

    public bool displayCardBack;

    public int effectNumber;

    [Header("Card Info")]
    //public TextMeshProUGUI carNametext;
    public TextMeshProUGUI cardDescText;
    public TextMeshProUGUI cardTypeText;
    public TextMeshProUGUI cardAttack;
    public TextMeshProUGUI cardPositionText;

    [Header("Card Data")]
    public int cardID;
    public string cardName;
    public faction cardFaction;
    public string cardDescription;
    public bool isHero;
    public slot cardSlot;
    public type cardType;
    
    public int attackPower;
    public Image art;

    private void Start()
    {
        CollectInfoFromSO();
        ProcesDescription();
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
        cardPositionText.text = cardSlot.ToString();
        art.sprite = cardData.art;
    }
    public void ProcesDescription()
    {
        Regex regex = new Regex(@"\b\d\b");
        Match match = regex.Match(cardDescription);

        if (match.Success)
            effectNumber = int.Parse(match.Value);
    }
    public void DisplayCardBack()
    {
        if (displayCardBack == true)
        {
            cardBack.SetActive(true);
        }
        else
        {
            cardBack.SetActive(false);
        }
    }
    public void Update()
    {
        UpdateDisplay();
        DisplayCardBack();
    }
}
