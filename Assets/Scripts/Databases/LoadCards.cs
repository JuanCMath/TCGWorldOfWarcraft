using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using System.Linq;

public class Cards : MonoBehaviour
{
    public static List<CardData> availableCards = new List<CardData>();

    void Awake()
    {
        LoadCards();
    }

    private void LoadCards()
    {
        string[] files = Directory.GetFiles(@"D:\Unity\TCGWorldOfWarcraft\Assets\Resources\Cards", "*.txt");
        string[] persistentFiles = Directory.GetFiles(Application.persistentDataPath, "*.txt");

        if (persistentFiles != null) files.AddRange(persistentFiles);
        
        foreach (string file in files)
        {
            using (StreamReader sr = new StreamReader(file))
            {
                object returnedValue = Compiler.Compiler.ProcessInput(sr.ReadToEnd());
                if (returnedValue is CardData card)
                {
                    availableCards.Add(card);
                }
            }
        }
    }
}