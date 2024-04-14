using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Animations;

public class Player1CardManager : MonoBehaviour
{
    #region Variables
    public int powerPlayer1;

    public GameObject cardPrefab1;
    
    public GameObject deckPlayer1;
    public GameObject graveyardPlayer1;
    public GameObject handPlayer1;

    public GameObject meleeZonePlayer1;
    public GameObject rangeZonePlayer1;
    public GameObject siegeZonePlayer1;

    [Header ("Hand Counter")]
    public int startingHandSize = 10;
    public int maxHandSize = 10;

    [Header ("UI")]
    public TextMeshProUGUI deckTextPlayer1;
    public TextMeshProUGUI discardTextPlayer1;
    #endregion

    public void DrawCard(int amount)
    {
        for (int i = 0; i < amount; i++)
        {           
            GameObject g = Instantiate(cardPrefab1, handPlayer1.transform);
            //set the Card to the CardData or the cloned prefav
            g.GetComponent<Card>().cardData = deckPlayer1.GetComponent<AspectosDeck>().aspectosDeck[i];
            deckPlayer1.GetComponent<AspectosDeck>().aspectosDeck.RemoveAt(i);
            //set the cards name in hierarchy
            g.name = g.GetComponent<Card>().cardData.cardName;            
        }
    }

    static void ShuffleDeck(List<CardData> deck)
    {
        System.Random rng = new System.Random();  
        int n = deck.Count;  
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            CardData value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
    }
    public void CountAttackOnField()
    {
        foreach (Transform child in meleeZonePlayer1.transform)
        {
            powerPlayer1 += child.GetComponent<Card>().attackPower;
        }
        foreach (Transform child in rangeZonePlayer1.transform)
        {
            powerPlayer1 += child.GetComponent<Card>().attackPower;
        }
        foreach (Transform child in siegeZonePlayer1.transform)
        {
            powerPlayer1 += child.GetComponent<Card>().attackPower;
        }
    }

     public void Start()
     {
        ShuffleDeck(deckPlayer1.GetComponent<AspectosDeck>().aspectosDeck);
        DrawCard(10);
     }
     public void Update()
     {
        deckTextPlayer1.text = deckPlayer1.GetComponent<AspectosDeck>().aspectosDeck.Count.ToString();
     }
 }
