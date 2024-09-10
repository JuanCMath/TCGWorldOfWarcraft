using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Compiler;
using System.Data;
using System;
using Unity.UI;
using UnityEngine.UI;
using UnityEditor.UI;

public class CreateCardMenu : MonoBehaviour
{
    public GameObject displayedCard;
    public GameObject prefab;
    public GameObject displayPanel;
    public TextMeshProUGUI output;
    public TMP_InputField inputField;

    private string tokenizableInput;
    public bool canBeCompiled = false;

    ASTNode AST;

    public void ResetValues()
    {   
        output.text = "";
        inputField.text = "";
        canBeCompiled = false;
    }

    public void DisplayCard()
    {
        if (canBeCompiled)
        {
            if (AST is CardDeclarationNode card)
            {
                CardData cardToDisplay = Compiler.Compiler.ProcessInputToDisplay(tokenizableInput) as CardData;

                displayedCard = Instantiate(prefab, displayPanel.transform);
                displayedCard.GetComponent<Card>().cardData = cardToDisplay;
                displayedCard.transform.localPosition = new Vector3(0,0,0);
                displayedCard.transform.localScale *= 5;
            }
        }
        else
        {
            foreach (Transform child in displayPanel.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void ParseInput(string input)
    {
        try
        {
            canBeCompiled = false;
            List<Token> tokens = Lexer.Tokenize(input);

            Parse parsedExpresion = new Parse(tokens);
            AST = parsedExpresion.Parsing();

            // Code to execute if parsing is successful
            canBeCompiled = true;
            tokenizableInput = input;
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
            System.IO.File.WriteAllText(path, tokenizableInput);
        }
        else if (AST is EffectDeclarationNode effect)
        {
            string path = Application.persistentDataPath + $"/ {effect.Name.Value}.txt";
            System.IO.File.WriteAllText(path, tokenizableInput);
        }
    }
}
