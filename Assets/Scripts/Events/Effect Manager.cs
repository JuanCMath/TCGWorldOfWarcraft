using UnityEngine;
using System;

public class EffectsManager : MonoBehaviour
{
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

        Debug.Log("estoy aqui");
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

    public static void SeñueloEffect(Transform panel)
    {

    }

    public static void ClickCard(Transform panel)
    {
        
    }
    
}