using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor.PackageManager;
using UnityEngine.EventSystems;
using UnityEditor.UI;
using System.Runtime.CompilerServices;
using System.Linq;
using Unity.VisualScripting;
using Enums;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using Unity.VisualScripting.Antlr3.Runtime;

public class EffectsManager : MonoBehaviour
{
    static GameObject selectedcard;
    static GameObject panel;

    public static void ClimateEffect(GameObject panel, int number)
    {   
        GameObject[] cardsP1 = GameObject.FindGameObjectsWithTag("Card Player1");
        GameObject[] cardsP2 = GameObject.FindGameObjectsWithTag("Card Player2");

        if (panel.transform.parent.name == GameObject.Find("Panels p1").transform.name)
            foreach (GameObject card in cardsP2)
            {   
                if (card.transform.parent.ToString() == "Hand p2") continue;
                else if (card.GetComponent<Card>().isHero == true) continue;
                else 
                {
                    card.GetComponent<Card>().attackPower -= number;
                    if (card.GetComponent<Card>().attackPower < 0) card.GetComponent<Card>().attackPower = 0;
                }
            }  

        else if (panel.transform.parent.name == GameObject.Find("Panels p2").transform.name)
            foreach (GameObject card in cardsP1)
            {
                if (card.transform.parent.ToString() == "Hand p1") continue;
                else if (card.GetComponent<Card>().isHero == true) continue;
                else 
                {
                    card.GetComponent<Card>().attackPower -= number;
                    if (card.GetComponent<Card>().attackPower < 0) card.GetComponent<Card>().attackPower = 0;
                }   
            }
    }

    public static void IncreaseEffect(GameObject ouputPanel, int number)
    {
        GameObject[] cardsP1 = GameObject.FindGameObjectsWithTag("Card Player1");
        GameObject[] cardsP2 = GameObject.FindGameObjectsWithTag("Card Player2");

        if (ouputPanel.transform.parent.name == GameObject.Find("Panels p1").transform.name)
        {
            foreach (GameObject card in cardsP1)
            {
                if (card.transform.IsChildOf(ouputPanel.transform))
                {
                    card.GetComponent<Card>().attackPower += number;
                }
            }
        }
        else if(ouputPanel.transform.parent.name == GameObject.Find("Panels p2").transform.name)
        {
            foreach (GameObject card in cardsP2)
            {
                if (card.transform.IsChildOf(ouputPanel.transform))
                {
                    card.GetComponent<Card>().attackPower += number;
                }
            }
        }
    }

    public void ClearanceEffect(GameObject panelOfTheDropedCard)
    {
        panel = panelOfTheDropedCard;
        Debug.Log("aplicando efecto señuelo");
        //Seleccionar una carta y devolverla a la mano
        EventManager.OnCardClicked += SelectCard;
        StartCoroutine(SendCardToHand());
        Debug.Log("listo para enviar el despeje al cementerio");
        
    }

    public void BaitEffect(GameObject panelOfTheDropedCard)
    {
        panel = panelOfTheDropedCard;
        Debug.Log("aplicando efecto señuelo");
        //Seleccionar una carta y devolverla a la mano
        EventManager.OnCardClicked += SelectCard;
        StartCoroutine(SendCardToHand());
    }

    public static void DrawACard()
    {
        if (GameManager.player1 == true)  GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().DrawCard(1);  
        else if (GameManager.player2 == true) GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().DrawCard(1);
    }

    public static void DestroyLowerPowerCardOnOponent()
    {
        GameObject[] cardsP1 = GameObject.FindGameObjectsWithTag("Card Player1");
        GameObject[] cardsP2 = GameObject.FindGameObjectsWithTag("Card Player2");

        GameObject cardToDestroy = null;
        int attackPowerOfCardToDestroy = 1000;

        int amountOfCardsInFieldP1 = 0;
        int amountOfCardsInFieldP2 = 0;

        foreach (GameObject card in cardsP1)
        {
            if (card.transform.parent.name == "Hand p1") continue;
            else amountOfCardsInFieldP1 += 1;
        }

        foreach (GameObject card in cardsP2)
        {
            if (card.transform.parent.name == "Hand p2") continue;
            else amountOfCardsInFieldP2 += 1;
        }
        if (GameManager.player1 == true)
        {
            if (amountOfCardsInFieldP2 != 0)
            {
                foreach (GameObject card in cardsP2)
                {
                    if (card.transform.parent.name == "Hand p2") continue;
                    else if (card.transform.parent.name == "Graveyard p2") continue;
                    else if (card.transform.parent.name == "Lider p2") continue;
                    else if (card.GetComponent<Card>().isHero == true) continue;
                    else if (card.GetComponent<Card>().cardType == type.Aumento) continue;
                    else if (card.GetComponent<Card>().cardType == type.Clima) continue;
                    else if (card.GetComponent<Card>().cardType == type.Señuelo) continue;
                    else if (card.GetComponent<Card>().cardType == type.Despeje) continue;
                    else
                    {
                        if (card.GetComponent<Card>().attackPower < attackPowerOfCardToDestroy)
                        {
                            attackPowerOfCardToDestroy = card.GetComponent<Card>().attackPower;
                            cardToDestroy = card;
                        }
                    }  
                }
                
                SendCardToGraveyard(cardToDestroy);
                
                selectedcard = null;
                panel = null;
            }    
        }
        else if (GameManager.player2 == true)
        {
            if (amountOfCardsInFieldP1 != 0)
            {
                foreach (GameObject card in cardsP1)
                {
                    if (card.transform.parent.name == "Hand p1") continue;
                    else if (card.transform.parent.name == "Graveyard p1") continue;
                    else if (card.transform.parent.name == "Lider p1") continue;
                    else if (card.GetComponent<Card>().isHero == true) continue;
                    else if (card.GetComponent<Card>().cardType == type.Aumento) continue;
                    else if (card.GetComponent<Card>().cardType == type.Clima) continue;
                    else if (card.GetComponent<Card>().cardType == type.Señuelo) continue;
                    else if (card.GetComponent<Card>().cardType == type.Despeje) continue;
                    else
                    {
                        if (card.GetComponent<Card>().attackPower < attackPowerOfCardToDestroy)
                        {
                            attackPowerOfCardToDestroy = card.GetComponent<Card>().attackPower;
                            cardToDestroy = card;
                        }
                    } 
                }
                SendCardToGraveyard(cardToDestroy);
                  
                selectedcard = null;
                panel = null;
            }    
        }
    }

    public static void DestroyHighestPowerCardOnField()
    {
        Card[] provisionalCards = FindObjectsOfType<Card>();

        // Convierte el array de Card a GameObject[]
        GameObject[] cards = new GameObject[provisionalCards.Length];

        for (int i = 0; i < provisionalCards.Length; i++)
        {
            cards[i] = provisionalCards[i].gameObject;
        }
        
        GameObject cardToDestroy = null;
        int attackPowerOfCardToDestroy = 0;

        foreach (GameObject card in cards)
        {
            if (card.transform.parent.name == "Hand p1" || card.transform.parent.name == "Hand p2") continue;
            else if (card.transform.parent.name == "Graveyard p1" || card.transform.parent.name == "Graveyard p2") continue;
            else if (card.transform.parent.name == "Lider p1" || card.transform.parent.name == "Lider p2") continue;
            else if (card.GetComponent<Card>().isHero == true) continue;
            else if (card.GetComponent<Card>().cardType == type.Aumento) continue;
            else if (card.GetComponent<Card>().cardType == type.Clima) continue;
            else if (card.GetComponent<Card>().cardType == type.Señuelo) continue;
            else if (card.GetComponent<Card>().cardType == type.Despeje) continue;
            else 
            {
                if (card.GetComponent<Card>().attackPower > attackPowerOfCardToDestroy)
                {
                    attackPowerOfCardToDestroy = card.GetComponent<Card>().attackPower;
                    cardToDestroy = card;
                }
            }
        }
        SendCardToGraveyard(cardToDestroy);
         
        selectedcard = null;
        panel = null;     
    }

    public static void MultAttackPower(GameObject dropedcard)
    {
        string multCardName = dropedcard.GetComponent<Card>().cardName;
        int amountOfCardsWithTheSameName = 0;

        GameObject[] cardsP1 = GameObject.FindGameObjectsWithTag("Card Player1");
        GameObject[] cardsP2 = GameObject.FindGameObjectsWithTag("Card Player2");

        if (GameManager.player1 == true)
        {
            foreach (GameObject card in cardsP1)
            {
                if (card.transform.parent.name == "Hand p1") continue;

                else if (card.GetComponent<Card>().cardName == multCardName)
                    {
                        amountOfCardsWithTheSameName += 1;
                    }
            }
            dropedcard.GetComponent<Card>().attackPower *= amountOfCardsWithTheSameName;     
        }
        else if (GameManager.player2 == true)
        {
            foreach (GameObject card in cardsP2)
            {
                if (card.transform.parent.name == "Hand p2") continue;

                else if (card.GetComponent<Card>().cardName == multCardName)
                {
                    amountOfCardsWithTheSameName += 1;
                }
            }
            dropedcard.GetComponent<Card>().attackPower *= amountOfCardsWithTheSameName;          
        }
    }

    public static void DestoyFieldSpotWithLowerAmountOfCards()
    {
        int amountOfCardsInPanelToDestroyedPanel = 0;
        GameObject panelToDestroyed = null;

        GameObject meleePlayer1 = GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().meleeZonePlayer1;
        GameObject rangePlayer1 = GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().rangeZonePlayer1;
        GameObject siegePlayer1 = GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().siegeZonePlayer1;
        
        GameObject meleePlayer2 = GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().meleeZonePlayer2;
        GameObject rangePlayer2 = GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().meleeZonePlayer2;
        GameObject siegePlayer2 = GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().meleeZonePlayer2;

        GameObject[] panels = new GameObject[] {meleePlayer1, rangePlayer1, siegePlayer1, meleePlayer2, rangePlayer2, siegePlayer2};

        foreach (GameObject panel in panels)
        {
            if (panel.transform.childCount > amountOfCardsInPanelToDestroyedPanel)
            {
                panelToDestroyed = panel;
                amountOfCardsInPanelToDestroyedPanel = panel.transform.childCount;
            }
        }
        for (int i = 0; i < amountOfCardsInPanelToDestroyedPanel; i++)
        {
            SendCardToGraveyard(panelToDestroyed.transform.GetChild(i).gameObject);
        }
    }

    public static void SetAttackPowerOfAllCardsToAverageAtackPower()
    {
        Card[] provisionalCards = FindObjectsOfType<Card>();

        // Convierte el array de Card a GameObject[]
        GameObject[] cards = new GameObject[provisionalCards.Length];

        for (int i = 0; i < provisionalCards.Length; i++)
        {
            cards[i] = provisionalCards[i].gameObject;
        }

        int amountOfCardsInField = 0;
        int sumOfAllAttackPower = 0;
        int averageAtackPower = 0;

        foreach (GameObject card in cards)
        {
            if (card.transform.parent.name == "Hand p1" || card.transform.parent.name == "Hand p2") continue;
            else if (card.transform.parent.name == "Graveyard p1" || card.transform.parent.name == "Graveyard p2") continue;
            else if (card.transform.parent.name == "Lider p1" || card.transform.parent.name == "Lider p2") continue;
            else if (card.GetComponent<Card>().isHero == true) continue;
            else if (card.GetComponent<Card>().cardType == type.Aumento) continue;
            else if (card.GetComponent<Card>().cardType == type.Clima) continue;
            else if (card.GetComponent<Card>().cardType == type.Señuelo) continue;
            else if (card.GetComponent<Card>().cardType == type.Despeje) continue;
            else
            {
                amountOfCardsInField += 1;
                sumOfAllAttackPower += card.GetComponent<Card>().attackPower;
            }
        }
        averageAtackPower = sumOfAllAttackPower % sumOfAllAttackPower;

        foreach (GameObject card in cards)
        {
            if (card.transform.parent.name == "Hand p1" || card.transform.parent.name == "Hand p2") continue;
            else if (card.transform.parent.name == "Graveyard p1" || card.transform.parent.name == "Graveyard p2") continue;
            else if (card.transform.parent.name == "Lider p1" || card.transform.parent.name == "Lider p2") continue;
            else if (card.GetComponent<Card>().isHero == true) continue;
            else if (card.GetComponent<Card>().cardType == type.Aumento) continue;
            else if (card.GetComponent<Card>().cardType == type.Clima) continue;
            else if (card.GetComponent<Card>().cardType == type.Señuelo) continue;
            else if (card.GetComponent<Card>().cardType == type.Despeje) continue;
            else
            {
                card.GetComponent<Card>().attackPower = averageAtackPower;
            }
        }


    }

    #region Utilidades
    public static void SelectCard(GameObject card)
    {
        if (card.transform.IsChildOf(panel.transform))
        {
            if (card.GetComponent<Card>().isHero == true)
            {
                Debug.Log("Cartas Heroes No son Afectadas");
            }
            else
            {
            selectedcard = card;
            }
        }
        else
        {
            Debug.Log("La carta debe estar en el mismo panel");
        }
    }

    IEnumerator SendCardToHand()
    {
        yield return new WaitUntil(() => selectedcard != null);

        ReturnCardToHand(selectedcard);

        EventManager.OnCardClicked -= SelectCard;
        selectedcard = null;
        panel = null;
    }

    public void ReturnCardToHand(GameObject card)
    {
        if (GameManager.player1 == true)
        {
            selectedcard.transform.SetParent(GameObject.Find("Game Manager").GetComponent<GameManager>().handP1.transform);
        }
        else if (GameManager.player2 == true)
        {
            selectedcard.transform.SetParent(GameObject.Find("Game Manager").GetComponent<GameManager>().handP2.transform);
        } 
    }

    public static void SendCardToGraveyard(GameObject card)
    {
        if (card != null)
        {
            if (card.transform.parent.parent.name == "Panels p1")
            {
                card.transform.SetParent(GameObject.Find("Player1 Manager").GetComponent<Player1Manager>().graveyardPlayer1.transform);
                card.transform.localPosition = new Vector3 (0,0,0);
            }
            else if (card.transform.parent.parent.name == "Panels p2")
            {
                card.transform.SetParent(GameObject.Find("Player2 Manager").GetComponent<Player2Manager>().graveyardPlayer2.transform);
                card.transform.localPosition = new Vector3 (0,0,0);
            }
        }
        else
        {
            return;
        }
        
    }
    #endregion
}