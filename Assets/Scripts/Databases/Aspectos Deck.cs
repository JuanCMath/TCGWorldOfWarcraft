using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using System.Collections.ObjectModel;
using System;

public class AspectosDeck : MonoBehaviour
{
    //Coleccion de cartas
    public List<CardData> aspectosDeck = new List<CardData>();
    public CardData aspectosLeadCard;
    
    public void Awake()
    {
        LoadDeck();
        LoadLead();
    }
    private void LoadDeck()
    {
        CardData[] aspectosCards = Resources.LoadAll<CardData>("Scriptable Objects/Aspectos Deck");
        aspectosDeck.AddRange(aspectosCards);
    }
    private void LoadLead()
    {
        aspectosLeadCard = Resources.Load<CardData>("Scriptable Objects/Aspectos Lead/Deathwing" );
    }
}