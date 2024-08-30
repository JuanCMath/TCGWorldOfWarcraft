using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

public class DeckSelectorMenu : MonoBehaviour
{
    public TextMeshProUGUI factionSelectorText;

    public int index = 0;
    public bool Choosed = false;
    public string ChoosedFaction = "";

    private static HashSet<string> factions = new HashSet<string>();
    private static List<string> listOfFactions = new List<string>();


    public static void LoadFactions()
    {
        foreach (CardData card in Cards.availableCards)
        {
            factions.Add(card.cardFaction);
        }

        listOfFactions = factions.ToList();
    }

    public void NextFaction()
    {
        if (index == listOfFactions.Count - 1)
        {
            index = 0;
        }
        else
        {
            index += 1;
        }
    }
 
    public void PreviousFaction()
    {
        if (index == 0)
        {
            index = listOfFactions.Count - 1;
        }
        else 
        {
            index -= 1;
        }
    }

    public void ChoosingFactionComplete()
    {
        Choosed = true;
        ChoosedFaction = listOfFactions[index];
        gameObject.SetActive(false);
    }

    void Update()
    {
        if(listOfFactions.Count != 0) factionSelectorText.text = listOfFactions[index];
    }


}
