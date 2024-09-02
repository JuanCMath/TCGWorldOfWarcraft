using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Compiler;
using System.Data;
using System;

public class CreateCardMenu : MonoBehaviour
{
    public TextMeshProUGUI output;
    public bool canBeCompiled = false;
    ASTNode AST;

    public void ParseInput(string input)
    {
        try
        {
            List<Token> tokens = Lexer.Tokenize(input);

            Parse parsedExpresion = new Parse(tokens);
            AST = parsedExpresion.Parsing();

            // Code to execute if parsing is successful
            canBeCompiled = true;
            output.text = "Ready To Compile";
        }
        catch (SyntaxErrorException s)
        {
            output.text = $"Lexer Error: {s.Message}";
        }
        catch (Exception e)
        {
            output.text = $"Parsing Error: {e.Message}";
        }
    }

    public void SaveInput()
    {
        if (AST is CardDeclarationNode card)
        {
            string path = Application.persistentDataPath + $"/ {card.Name.Value}.txt";
            string input = output.text;
            System.IO.File.WriteAllText(path, input);
        }
        else if (AST is EffectDeclarationNode effect)
        {
            string path = Application.persistentDataPath + $"/ {effect.Name.Value}.txt";
            string input = output.text;
            System.IO.File.WriteAllText(path, input);
        }
    }
}
