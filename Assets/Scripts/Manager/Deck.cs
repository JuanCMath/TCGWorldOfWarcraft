using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using System.IO;
using System.Buffers;

public class Deck : MonoBehaviour
{
    public string faction;

    
    public List<CardData> deck = new List<CardData>();
    public CardData leadCard;


    public void Awake()
    {
        LoadLead();
        LoadDeck();
    }
    
    private void LoadDeck()
    {
        foreach (CardData card in Cards.availableCards)
        {
            if (card.cardFaction.Equals(faction, StringComparison.OrdinalIgnoreCase))
            {
                deck.Add(card);
            }
        }
    }

    private void LoadLead()
    {
        //TODO
    }

    public void PushCard(CardData card)
    {
        deck.Insert(0, card);
    }

    public CardData PopCard()
    {
        CardData card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    public void RemoveCard(CardData card)
    {
        deck.Remove(card);
    }

    public void SendBottom(CardData card)
    {
        deck.Insert(deck.Count, card);
    }

    public void ShuffleDeck()    
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

    /* private void OnGUI()
    {
        faction = GUI.TextField(new Rect(10, 10, 200, 20), faction);
    } */
}