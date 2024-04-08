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

    public void Awake()
    {
        LoadDeck();
    }
    public void LoadDeck()
    {
        CardData[] aspectosCards = Resources.LoadAll<CardData>("Scriptable Objects/Aspectos Deck");
        aspectosDeck.AddRange(aspectosCards);
        Debug.Log(aspectosCards);
    }
}