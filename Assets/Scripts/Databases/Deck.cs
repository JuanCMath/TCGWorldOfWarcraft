using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    //Coleccion de cartas
    public List<CardData> deck = new List<CardData>();
    public CardData leadCard;
    
    public void Awake()
    {
        LoadLead();
        LoadDeck();
    }
    private void LoadDeck()
    {
    }
    private void LoadLead()
    {
    }
}