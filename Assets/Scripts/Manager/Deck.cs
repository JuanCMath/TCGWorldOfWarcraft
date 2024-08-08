using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public string faction;

    private List<CardData> deck = new List<CardData>();
    private CardData leadCard;
    

    public void LoadDeck()
    {
        foreach (CardData card in Cards.availableCards)
        {
            if (card.cardFaction == faction)
            {
                deck.Add(card);
            }
        }
        InstantiateDeck();
        LoadLead();
    }

    private void InstantiateDeck()
    {
        foreach (CardData card in deck)
        {
            GameObject cardObject = Instantiate(gameObject.transform.parent.GetComponent<PlayerManager>().cardPrefab, gameObject.transform);
            cardObject.GetComponent<Card>().cardData = card;
            cardObject.name = card.cardName;
            cardObject.transform.localPosition = new Vector3(515,0,0);
        }
    }

    private void LoadLead()
    {
        //TODO
    }

    public void PushCard(GameObject card)
    {
        card.transform.SetParent(gameObject.transform);
        card.transform.SetSiblingIndex(0);
    }

    public GameObject PopCard()
    {
        GameObject card = gameObject.transform.GetChild(0).gameObject;
        Destroy(gameObject.transform.GetChild(0).gameObject);
        return card;
    }

    public void RemoveCard(GameObject card)
    {
        Destroy(card);
    }

    public void SendBottom(GameObject card)
    {
        card.transform.SetParent(gameObject.transform);
        card.transform.SetAsLastSibling();
    }

    public void ShuffleDeck()    
    {
        int childCount = gameObject.transform.childCount;
        System.Random rand = new System.Random();
    
        for (int i = 0; i < childCount; i++)
        {
            int randomIndex = rand.Next(childCount);
            gameObject.transform.GetChild(i).SetSiblingIndex(randomIndex);
        } 
    }

    public void Clear()
    {
        foreach (Transform child in gameObject.transform)
        {
            if(child.gameObject.GetComponent<Card>() != null) Destroy(child);
        }
    }
}