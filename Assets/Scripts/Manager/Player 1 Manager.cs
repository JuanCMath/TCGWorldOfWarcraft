using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Animations;
using System.Threading;

public class Player1Manager : MonoBehaviour
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

    //Robar Carta del Deck
    public void DrawCard(int amount)       
    {
        for (int i = 0; i < amount; i++)     
        {           
            //Instanciando la carta con el prefab y en la posicion de la mano
            GameObject g = Instantiate(cardPrefab1, handPlayer1.transform);                         
            //Dandole a cada prefab de carta los datos de los scriptable objects
            g.GetComponent<Card>().cardData = deckPlayer1.GetComponent<AspectosDeck>().aspectosDeck[i];
            //Eliminando la carta robada
            deckPlayer1.GetComponent<AspectosDeck>().aspectosDeck.RemoveAt(i);                          
            //Dandole un nombre a la carta en el inspector
            g.name = g.GetComponent<Card>().cardData.cardName;                         
        }
    }

    //Barajear el Deck
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

    //Contar la cantidad de ataque que existe en el campo
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
    }

    
    public void Update()
    {
       deckTextPlayer1.text = deckPlayer1.GetComponent<AspectosDeck>().aspectosDeck.Count.ToString();
    }
 }
