using System;
using System.Collections.Generic;
using Compiler;
using System.IO;
using UnityEngine;
using Unity.VisualScripting;

public class Effects : MonoBehaviour
{
    public static Dictionary<string, EffectDeclarationNode> availableEffects = new Dictionary<string, EffectDeclarationNode>();

    public void Awake()
    {
        LoadEffects();
    }

    private void LoadEffects()
    {
        string[] files = Directory.GetFiles("Assets/Resources/Effects", "*.txt");
        string[] persistentFiles = Directory.GetFiles(Application.persistentDataPath, "*.txt");

        if (persistentFiles != null) files.AddRange(persistentFiles);
        
        foreach (string file in files)
        {
            using (StreamReader sr = new StreamReader(file))
            {
                object returnedValue = Compiler.Compiler.ProcessInput(sr.ReadToEnd());
                if (returnedValue is EffectDeclarationNode effect)
                {
                    availableEffects.Add(effect.Name.Value, effect);
                }
            }
        }
    }
}