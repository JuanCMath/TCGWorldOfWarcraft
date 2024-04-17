using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor.PackageManager;
using UnityEngine.EventSystems;
using UnityEditor.UI;

public class EffectsManager : MonoBehaviour
{
    static GameObject selectedcard;
    static GameObject panel;

    public static void ClimaEffect(GameObject panel, int number)
    {   
        GameObject[] cardsP1 = GameObject.FindGameObjectsWithTag("Card Player1");
        GameObject[] cardsP2 = GameObject.FindGameObjectsWithTag("Card Player2");

        if (panel.transform.parent.name == GameObject.Find("Panels p1").transform.name)
            foreach (GameObject card in cardsP2)
            {   
                if (card.transform.parent.ToString() == "Hand p2") continue;
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
                else card.GetComponent<Card>().attackPower -= number;

                if (card.GetComponent<Card>().attackPower < 0) card.GetComponent<Card>().attackPower = 0;
            }
    }

    public static void AumentoEffec(GameObject ouputPanel, int number)
    {
        GameObject[] cardsP1 = GameObject.FindGameObjectsWithTag("Card Player1");
        GameObject[] cardsP2 = GameObject.FindGameObjectsWithTag("Card Player2");

        if (ouputPanel.transform.parent.name == GameObject.Find("Panels p1").transform.name)
        {
            foreach (GameObject card in cardsP1)
            {
                Debug.Log(card.transform.IsChildOf(ouputPanel.transform));
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
                Debug.Log(card.transform.IsChildOf(ouputPanel.transform));
                if (card.transform.IsChildOf(ouputPanel.transform))
                {
                    card.GetComponent<Card>().attackPower += number;
                }
            }
        }

    }

    public static void DespejeEffect()
    {
        
    }

    public void SeñueloEffect(GameObject panelOfTheDropedCard)
    {
        panel = panelOfTheDropedCard;
        Debug.Log("aplicando efecto");
        //Seleccionar una carta y devolverla a la mano
        EventManager.OnCardClicked += SelectCard;
        StartCoroutine(SendCardToHand());
    }
    public static void SelectCard(GameObject card)
    {
        if (card.transform.IsChildOf(panel.transform))
        {
            selectedcard = card;
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
    }

    public void ReturnCardToHand(GameObject card)
    {
        if (GameManager.player1 == true)
        {
            selectedcard.transform.SetParent(GameObject.Find("Game Manager").GetComponent<GameManager>().handp1.transform);
        }
        else if (GameManager.player2 == true)
        {
            selectedcard.transform.SetParent(GameObject.Find("Game Manager").GetComponent<GameManager>().handp2.transform);
        } 
    }
}