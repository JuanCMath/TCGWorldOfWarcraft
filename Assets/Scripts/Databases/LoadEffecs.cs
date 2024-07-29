using System;
using System.Collections.Generic;
using Compiler;
using System.IO;

public class Effects
{
    public static Dictionary<string, EffectDeclarationNode> availableEffects = new Dictionary<string, EffectDeclarationNode>();

    private string folderPath = "Assets/Resources/Effects";

    public void Awake()
    {
        LoadEffects();
    }

    private void LoadEffects()
    {
        string[] files = Directory.GetFiles(folderPath, "*.txt");

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