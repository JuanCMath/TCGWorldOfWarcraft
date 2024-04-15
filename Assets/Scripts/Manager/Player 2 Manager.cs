using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Animations;
using System.Threading;

public class Player2Manager : MonoBehaviour
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

    public void CleanField()
    {
        GameObject[] cartas = GameObject.FindGameObjectsWithTag("Card Player2");

        foreach (GameObject carta in cartas)
        {
            //Si esta en la mano no la destruyas
            if (carta.transform.IsChildOf(handPlayer2.transform)) continue;

            //Si no esta en la mano destruyela, Aqui podriamos poner despues que se vayan al cementerio
            carta.transform.SetParent(graveyardPlayer2.transform);
            carta.transform.localPosition = new Vector3(0,0,0);
        }       
    }

    //Robar carta del Deck
    public void DrawCard(int amount)
    {
        for (int i = 0; i < amount; i++)     
        {   
            if (handPlayer2.transform.childCount < 10)
            {       
                //Instanciando la carta con el prefab y en la posicion de la mano
                GameObject g = Instantiate(cardPrefab2, handPlayer2.transform);                         
                //Dandole a cada prefab de carta los datos de los scriptable objects
                g.GetComponent<Card>().cardData = deckPlayer2.GetComponent<ArthasDeck>().arthasDeck[i];
                //Eliminando la carta robada
                deckPlayer2.GetComponent<ArthasDeck>().arthasDeck.RemoveAt(i);                          
                //Dandole un nombre a la carta en el inspector
                g.name = g.GetComponent<Card>().cardData.cardName;  
            }
            else
            {
                //Instanciando la carta con el prefab y en la posicion de la mano
                GameObject g = Instantiate(cardPrefab2, graveyardPlayer2.transform);
                g.transform.localPosition = new Vector3(0,0,0);                         
                //Dandole a cada prefab de carta los datos de los scriptable objects
                g.GetComponent<Card>().cardData = deckPlayer2.GetComponent<ArthasDeck>().arthasDeck[i];
                //Eliminando la carta robada
                deckPlayer2.GetComponent<ArthasDeck>().arthasDeck.RemoveAt(i);                          
                //Dandole un nombre a la carta en el inspector
                g.name = g.GetComponent<Card>().cardData.cardName;
            }                   
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
     }

     public void Update()
     {
        deckTextPlayer2.text = deckPlayer2.GetComponent<ArthasDeck>().arthasDeck.Count.ToString();
        discardTextPlayer2.text = (graveyardPlayer2.transform.childCount - 1).ToString();
     }
 }