using Compiler;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardData cardData;

    public GameObject cardBack;

    public bool displayCardBack;

    [Header("Card Info")]
    //public TextMeshProUGUI carNametext;
    public TextMeshProUGUI cardDescText;
    public TextMeshProUGUI cardTypeText;
    public TextMeshProUGUI cardAttack;
    public TextMeshProUGUI cardPositionText;

    [Header("Card Data")]
    public int cardID;
    public string cardName;
    public string cardFaction;
    public string cardDescription;
    public OnActivationNode cardEffect;
    public bool isHero;
    public type cardType;
    public string[] cardSlot;
    
    public int attackPower;
    public Image art;

    private void Start()
    {
        CollectInfo();
    }

    private void CollectInfo()
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
        cardEffect = cardData.effect;
        isHero = cardData.isHero;
        cardSlot = cardData.slots;
        attackPower = cardData.attackPower;

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        cardDescText.text = cardDescription;
        cardAttack.text = attackPower.ToString();
        cardPositionText.text = ProcessCardSlot();
        cardTypeText.text = cardType.ToString();
        art.sprite = cardData.art;
    }

    private void DisplayCardBack()
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

    private string ProcessCardSlot()
    {
        string slot = "";

        foreach (string position in cardSlot)
        {
            if (position == "Melee")
            {
                slot += "M";
            }
            else if (position == "Range")
            {
                slot += "R";
            }
            else if (position == "Siege")
            {
                slot += "S";
            }
            else
            {
                slot += "X";
            }
        }

        return slot;
    }

    public void Update()
    {
        UpdateDisplay();
        DisplayCardBack();
    }
}
