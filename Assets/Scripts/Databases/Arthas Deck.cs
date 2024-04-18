using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using System.Collections.ObjectModel;
using System;

public class ArthasDeck : MonoBehaviour
{
    //Coleccion de cartas
    public List<CardData> arthasDeck = new List<CardData>();
    public CardData arthasLeadCard;
    
    public void Awake()
    {
        LoadLead();
        LoadDeck();
    }
    private void LoadDeck()
    {
        CardData[] arthasCards = Resources.LoadAll<CardData>("Scriptable Objects/Arthas Deck");
        arthasDeck.AddRange(arthasCards);
    }
    private void LoadLead()
    {
        arthasLeadCard = Resources.Load<CardData>("Scriptable Objects/Arthas Lead/The Lick King" );
    }
}