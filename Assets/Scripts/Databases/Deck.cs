using System.Collections.Generic;
using Enums;
using UnityEngine;
using System.IO;
using System.Buffers;

public class Deck : MonoBehaviour
{
    public player owner;
    private string folderPath;
    //Coleccion de cartas
    public List<CardData> deck = new List<CardData>();
    public CardData leadCard;


    public void Awake()
    {
        folderPath = owner == player.Player1 ? "Assets/Resources/Decks/Player1" : "Assets/Resources/Decks/Player2";
        LoadLead();
        LoadDeck();
    }
    
    private void LoadDeck()
    {
        string[] files = Directory.GetFiles(folderPath, "*.txt");

        foreach (string file in files)
        {
            using (StreamReader sr = new StreamReader(file))
            {
                object returnedValue = Compiler.Compiler.ProcessInput(sr.ReadToEnd());
                if (returnedValue is CardData card)
                {
                    deck.Add(card);
                }
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


}