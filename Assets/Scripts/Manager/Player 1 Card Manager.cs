using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.UI;
using TMPro;

public class Player1CardManager : MonoBehaviour
{
    #region Variables
    public turn currentTurn;

    public GameObject newCardPrefab;
    
    public Transform deckPlayer1;
    public Transform graveyardPlayer1;
    public Transform handPlayer1;

    [Header ("Cards Player 1")]
    public List<CardData> currentPlayer1CardsInDeck = new List<CardData>();
    public List<CardData> totalPlayer1Cards = new List<CardData>();

    [Header ("Hand Counter")]
    public int startingHandSize = 10;
    public int maxHandSize = 10;

    public bool isDrawing = false;

    [Header ("UI")]
    public TextMeshProUGUI deckTextPlaye1;
    public TextMeshProUGUI discardTextPlayer1;
    #endregion

    public void Start()
    {
        isDrawing = true;
        LoadDeck();
    }
    private void LoadDeck()
    {
        for (int i = 0; i < currentPlayer1CardsInDeck.Count; i++)
        {
            GameObject g = Instantiate(newCardPrefab, handPlayer1);
            //set the Card to the CardData or the cloned prefav
            g.GetComponent<Card>().cardData = currentPlayer1CardsInDeck[i];
            g.name = g.GetComponent<Card>().cardData.cardName;
            //set the cards name in hierarchy
        }

        UpdateDisplay();

        InitialDrawForTurn();
    }

    public void UpdateDisplay()
    {
        deckTextPlaye1.text = deckPlayer1.childCount.ToString();
        discardTextPlayer1.text = graveyardPlayer1.childCount.ToString();

        for (int i = 0; i < handPlayer1.childCount; i++)
        {
                    //quitar lo mas seguro
        }
    }
    //first time we draw cards each turn
    public void InitialDrawForTurn()
    {
        currentTurn = turn.Player1;

        if (handPlayer1.childCount < startingHandSize)
        {
            DrawCard();
        }
        else
        {
            isDrawing = false;
        }
    }
    public void DrawCard()
    {
        if (deckPlayer1.childCount > 0)
        {
            int r = Random.Range(0, deckPlayer1.childCount);
            deckPlayer1.GetChild(r).transform.parent = handPlayer1;
        }
        else if (deckPlayer1.childCount <= 0)
        {
            //u lose, and change round
        }
    }
    public void ResetCardTransform(Transform card)
    {
        card.localPosition = Vector2.zero;
    }
}
