using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Animations;

public class Player2CardManager : MonoBehaviour
{
    #region Variables
    public int powerPlayer2;

    public GameObject cardPrefab2;
    
    public GameObject deckPlayer2;
    public GameObject graveyardPlayer2;
    public GameObject handPlayer2;

    public GameObject meleeZonePlayer2;
    public GameObject rangeZonePlayer2;
    public GameObject siegeZonePlayer2;

    [Header ("Hand Counter")]
    public int startingHandSize = 10;
    public int maxHandSize = 10;

    [Header ("UI")]
    public TextMeshProUGUI deckTextPlayer2;
    public TextMeshProUGUI discardTextPlayer2;
    #endregion

    public void DrawCard(int amount)
    {
        for (int i = 0; i < amount; i++)
        {           
            GameObject g = Instantiate(cardPrefab2, handPlayer2.transform);
            //set the Card to the CardData or the cloned prefav
            g.GetComponent<Card>().cardData = deckPlayer2.GetComponent<ArthasDeck>().arthasDeck[i];
            deckPlayer2.GetComponent<ArthasDeck>().arthasDeck.RemoveAt(i);
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
        foreach (Transform child in meleeZonePlayer2.transform)
        {
            powerPlayer2 += child.GetComponent<Card>().attackPower;
        }
        foreach (Transform child in rangeZonePlayer2.transform)
        {
            powerPlayer2 += child.GetComponent<Card>().attackPower;
        }
        foreach (Transform child in siegeZonePlayer2.transform)
        {
            powerPlayer2 += child.GetComponent<Card>().attackPower;
        }
    }

     public void Start()
     {
        ShuffleDeck(deckPlayer2.GetComponent<ArthasDeck>().arthasDeck);
        DrawCard(10);
     }
     public void Update()
     {
        deckTextPlayer2.text = deckPlayer2.GetComponent<ArthasDeck>().arthasDeck.Count.ToString();
     }
 }